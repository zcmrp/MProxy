using MProxy.Data;
using MProxy.Net.Base;
using MProxy.Net.Delivery;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MProxy.Net
{
    class NetIODeliveryServer : NetIOServer
    {
        public NetIODeliveryServer(ushort port, string chost, ushort cport) : base(port, chost, cport) { }
        public NetIODeliveryServer(IPAddress ip, ushort port, IPAddress cip, ushort cport) : base(ip, port, cip, cport) { }

        public EventHandler<RecvEventArgs> OnDeliverySendUser;
        public EventHandler<RecvEventArgs> OnRequestRoleID;
        public EventHandler<RecvEventArgs> OnUserSetLink;
        public EventHandler<RecvEventArgs> OnUserLogout;
        public EventHandler OnLinkDisconnect;

        protected override void OnListen()
        {
            while (true)
            {
                Socket rcli = TCP.AcceptSocket();
                NetIORemoteClient remote = new NetIORemoteClient(rcli);
                NetIODeliveryClient delivery = new NetIODeliveryClient(CIPE);
                ++Counter;
                LinkProxy link = new LinkProxy(Counter, remote, delivery);
                link.OnDisconnect += OnClientDisconnect;
                link.OnDeliverySendUser += OnDeliverySendUser;
                link.OnRequestRoleID += OnRequestRoleID;
                link.OnUserSetLink += OnUserSetLink;
                link.OnUserLogout += OnUserLogout;
                link.Connect();
            }
        }
        protected override void OnClientDisconnect(object sender, EventArgs e)
        {
            LinkProxy link = (LinkProxy)sender;
            if (link.ID != 0)
                Console.WriteLine("Link {0} disconnect from delivery, drop all players", link.ID);
            OnLinkDisconnect(sender, e);
        }
    }
}
