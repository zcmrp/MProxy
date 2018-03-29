using MProxy.Data;
using MProxy.Net;
using MProxy.Net.Provider;
using MProxy.Net.Proxies;
using MProxy.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MProxy
{
    class ProxyControl
    {
        private ProxySetup Setup { get; set; }
        private NetIODeliveryServer Delivery { get; set; }
        private SortedDictionary<uint, UserProxy> Users;
        private SortedDictionary<uint, UserProxy> Roles;
        private List<NetIOLinkServer> Links;
        private List<NetIOProviderServer> Providers;
        public ProxyControl(ProxySetup setup)
        {
            Setup = setup;
            Links = new List<NetIOLinkServer>();
            Providers = new List<NetIOProviderServer>();
            Users = new SortedDictionary<uint, UserProxy>();
            Roles = new SortedDictionary<uint, UserProxy>();
        }
        public void Run()
        {
            Delivery = new NetIODeliveryServer(Setup.LinkProxyIP, Setup.LinkProxyPort, Setup.DeliveryServerHost, Setup.DeliveryServerPort);
            Delivery.OnDeliverySendUser += OnDeliverySendUser;
            Delivery.OnRequestRoleID += OnRequestRoleID;
            Delivery.OnUserSetLink += OnUserSetLink;
            Delivery.OnLinkDisconnect += OnLinkDisconnect;
            Delivery.OnUserLogout += OnUserLogout;
            Delivery.Start();
            foreach (Tuple<Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>> t in Setup.Proxies)
            {
                NetIOLinkServer link = new NetIOLinkServer(t.Item1.Item1, t.Item1.Item2, t.Item2.Item1, t.Item2.Item2);
                Links.Add(link);
                link.OnUserSetID += OnUserSetID;
                link.OnUserDisconnect += OnUserDisconnect;
                link.OnUserSetRoleID += OnUserSetRoleID;
                link.Start();
                NetIOProviderServer provider = new NetIOProviderServer(t.Item3.Item1, t.Item3.Item2, t.Item4.Item1, t.Item4.Item2);
                Providers.Add(provider);
                provider.OnGsSendUser += OnGsSendUser;
                provider.Start();
            }
        }
        private void OnUserSetRoleID(object sender, EventArgs e)
        {
            UserProxy user = (UserProxy)sender;
            lock (Roles)
                Roles[user.RoleID] = user;
        }
        private void OnUserSetID(object sender, EventArgs e)
        {
            UserProxy user = (UserProxy)sender;
            lock (Users)
                Users[user.ID] = user;
        }
        private void OnUserLogout(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint role_id = os.ReadUInt();
            UserProxy user;
            lock (Roles)
                if (!Roles.TryGetValue(role_id, out user))
                    return;
                else
                    Roles.Remove(role_id);
            user.SetRoleID(0);
        }
        private void OnRequestRoleID(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint localsid = os.ReadUInt();
            lock (Roles)
                foreach (KeyValuePair<uint, UserProxy> kvpuser in Roles)
                    if (kvpuser.Value.Localsid == localsid)
                    {
                        e.Res = new Octets().Write(kvpuser.Key);
                        return;
                    }
            e.Res = new Octets().Write(UInt32.MaxValue);
        }
        private void OnUserSetLink(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint user_id = os.ReadUInt();
            uint localsid = os.ReadUInt();
            LinkProxy link = (LinkProxy)sender;
            UserProxy user;
            lock (Users)
                if (!Users.TryGetValue(user_id, out user))
                    return;
            if (user.Link == null) {
            Console.WriteLine("User {0} logged in from Link {1}, IP {2}", user.ID, link.ID, user);
            user.SetLink(link);
                }
            link.AddUser(user);
        }
        private void OnUserDisconnect(object sender, EventArgs e)
        {
            UserProxy user = (UserProxy)sender;
            lock (Users)
                Users.Remove(user.ID);
            lock (Roles)
                Roles.Remove(user.RoleID);
            if (user.Link != null)
                user.Link.RemoveUser(user);
        }
        private void OnLinkDisconnect(object sender, EventArgs e)
        {
            LinkProxy link = (LinkProxy)sender;
            link.DisconnectAllUsers();
        }

        private void OnDeliverySendUser(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint role_id = os.ReadUInt();
            Octets data = os.ReadOctets();
            UserProxy user;
            lock (Roles)
                if (!Roles.TryGetValue(role_id, out user))
                    return;
            user.SendToClient(data);
        }
        private void OnGsSendUser(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint role_id = os.ReadUInt();
            Octets data = os.ReadOctets();
            UserProxy user;
            lock (Roles)
                if (!Roles.TryGetValue(role_id, out user))
                    return;
            user.SendToClient(data);
        }
        
    }
}
