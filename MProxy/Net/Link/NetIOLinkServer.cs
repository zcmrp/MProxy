using MProxy.Data;
using MProxy.Net.Base;
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
    class NetIOLinkServer : NetIOServer
    {
        public NetIOLinkServer(ushort port, string chost, ushort cport) : base(port, chost, cport) { }
        public NetIOLinkServer(IPAddress ip, ushort port, IPAddress cip, ushort cport) : base(ip, port, cip, cport) { }
        public EventHandler OnUserSetID;
        public EventHandler OnUserSetRoleID;
        public EventHandler OnUserDisconnect;

        protected override void OnListen()
        {
            while (true)
            {
                Socket cli = TCP.AcceptSocket();
                NetIORemoteClient remote = new NetIORemoteClient(cli);
                NetIOLinkClient link = new NetIOLinkClient(CIPE);
                UserProxy User = new UserProxy(0, remote, link);
                User.OnDisconnect += OnClientDisconnect;
                User.OnSetID += OnUserSetID;
                User.OnSetRoleID += OnUserSetRoleID;
                User.Connect();
            }
        }

        protected override void OnClientDisconnect(object sender, EventArgs e)
        {
            UserProxy user = (UserProxy)sender;
            OnUserDisconnect(sender, e);
            if (user.ID != 0)
                Console.WriteLine("User {0} disconnect from server", user.ID);
        }
    }
}
