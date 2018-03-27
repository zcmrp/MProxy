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

        public EventHandler<RecvEventArgs> OnUserLogin;
        public EventHandler<RecvEventArgs> OnDeliverySendUser;
        public EventHandler<RecvEventArgs> OnRequestUserID;
        public EventHandler<RecvEventArgs> OnUserSetLink;

        protected override void OnListen()
        {
            while (true)
            {
                Socket rcli = TCP.AcceptSocket();
                NetIORemoteClient remote = new NetIORemoteClient(rcli);
                Console.WriteLine("Link connected from {0}", remote);
                NetIODeliveryClient delivery = new NetIODeliveryClient(CIPE);
                LinkProxy link = new LinkProxy(0, remote, delivery);
                link.OnDisconnect += OnClientDisconnect;
                link.UserLogin += OnUserLogin;
                link.OnDeliverySendUser += OnDeliverySendUser;
                link.OnRequestUserID += OnRequestUserID;
                link.OnUserSetLink += OnUserSetLink;
                link.Connect();
            }
        }
        protected override void OnClientDisconnect(object sender, EventArgs e)
        {
            LinkProxy link = (LinkProxy)sender;
            Console.WriteLine("Link {0} disconnect from delivery", link.ID);
        }
    }
}
