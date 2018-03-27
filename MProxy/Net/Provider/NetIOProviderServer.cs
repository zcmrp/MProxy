using MProxy.Data;
using MProxy.Net.Base;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net.Provider
{
    class NetIOProviderServer : NetIOServer
    {
        public NetIOProviderServer(IPAddress ip, ushort port, IPAddress cip, ushort cport) : base(ip, port, cip, cport) { }
        public EventHandler<RecvEventArgs> OnGsSendUser;
        protected override void OnListen()
        {
            while (true)
            {
                Socket rcli = TCP.AcceptSocket();
                NetIORemoteClient remote = new NetIORemoteClient(rcli);
                Console.WriteLine("Link connected from {0}", remote);
                NetIOProviderClient provider = new NetIOProviderClient(CIPE);
                HomeProxy home = new HomeProxy(0, remote, provider);
                home.OnDisconnect += OnClientDisconnect;
                home.OnGsSendUser += OnGsSendUser;
                home.Connect();
            }
        }
        protected override void OnClientDisconnect(object sender, EventArgs e)
        {
            HomeProxy home = (HomeProxy)sender;
            Console.WriteLine("Link {0} disconnect from delivery", home.ID);
        }
    }
}
