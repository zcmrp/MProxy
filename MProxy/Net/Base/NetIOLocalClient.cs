using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net.Base
{
    class NetIOLocalClient : NetIOClient
    {
        public EventHandler OnConnected;
        public EventHandler OnConnectionRefused;
        public NetIOLocalClient(IPEndPoint ipe) : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            IPE = ipe;
        }
        override public void Start()
        {
            Skt.BeginConnect(IPE, OnConnect, Skt);
        }

        protected virtual void OnConnect(IAsyncResult res)
        {
            OnConnected(this, new EventArgs());
        }
    }
}
