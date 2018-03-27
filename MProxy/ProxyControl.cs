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
        private List<NetIOLinkServer> Links;
        private List<NetIOProviderServer> Providers;
        public ProxyControl(ProxySetup setup)
        {
            Setup = setup;
            Links = new List<NetIOLinkServer>();
            Providers = new List<NetIOProviderServer>();
            Users = new SortedDictionary<uint, UserProxy>();
        }
        public void Run()
        {
            Delivery = new NetIODeliveryServer(Setup.LinkProxyIP, Setup.LinkProxyPort, Setup.DeliveryServerHost, Setup.DeliveryServerPort);
            Delivery.OnUserLogin += OnUserLogin;
            Delivery.OnDeliverySendUser += OnDeliverySendUser;
            Delivery.OnRequestUserID += OnRequestUserID;
            Delivery.OnUserSetLink += OnUserSetLink;
            Delivery.Start();
            foreach (Tuple<Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>> t in Setup.Proxies)
            {
                NetIOLinkServer link = new NetIOLinkServer(t.Item1.Item1, t.Item1.Item2, t.Item2.Item1, t.Item2.Item2);
                Links.Add(link);
                link.OnUserChangeID += OnUserChangeID;
                link.OnUserDisconnect += OnUserDisconnect;
                link.Start();
                NetIOProviderServer provider = new NetIOProviderServer(t.Item3.Item1, t.Item3.Item2, t.Item4.Item1, t.Item4.Item2);
                Providers.Add(provider);
                provider.OnGsSendUser += OnGsSendUser;
                provider.Start();
            }
        }

        private void OnUserLogin(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint user_id = os.ReadUInt();
            uint role_id = os.ReadUInt();
            UserProxy user;
            lock (Users)
                if (!Users.TryGetValue(user_id, out user))
                    return;
            user.ChangeID(role_id);
        }
        private void OnRequestUserID(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint localsid = os.ReadUInt();
            lock (Users)
                foreach (KeyValuePair<uint, UserProxy> kvpuser in Users)
                    if (kvpuser.Value.Localsid == localsid)
                    {
                        e.Res = new Octets().Write(kvpuser.Key);
                        return;
                    }
            e.Res = new Octets().Write(UInt32.MaxValue);
        }
        private void OnUserChangeID(object sender, uint new_id)
        {
            UserProxy user = (UserProxy)sender;
            lock (Users)
                Users.Remove(user.ID);
            user.SetID(new_id);
            lock (Users)
                Users[user.ID] = user;
        }
        private void OnUserSetLink(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint user_id = os.ReadUInt();
            uint localsid = os.ReadUInt();
            LinkProxy link = (LinkProxy)sender;
            UserProxy user = null;
            lock (Users)
                foreach (KeyValuePair<uint, UserProxy> kvpuser in Users)
                    if (kvpuser.Value.Localsid == localsid)
                        user = kvpuser.Value;
            if (user == null)
                return;
            user.SetLink(link);
            user.ChangeID(user_id);

        }
        private void OnUserDisconnect(object sender, EventArgs e)
        {
            UserProxy user = (UserProxy)sender;
            lock (Users)
                Users.Remove(user.ID);
        }
        private void OnDeliverySendUser(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint user_id = os.ReadUInt();
            Octets data = os.ReadOctets();
            UserProxy user;
            lock (Users)
                if (!Users.TryGetValue(user_id, out user))
                    return;
            user.SendToClient(data);
        }
        private void OnGsSendUser(object sender, RecvEventArgs e)
        {
            Octets os = e.Data;
            uint user_id = os.ReadUInt();
            Octets data = os.ReadOctets();
            UserProxy user;
            lock (Users)
                if (!Users.TryGetValue(user_id, out user))
                    return;
            user.SendToClient(data);
        }
        
    }
}
