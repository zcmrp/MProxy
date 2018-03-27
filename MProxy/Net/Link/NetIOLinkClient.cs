using MProxy.Data;
using MProxy.Net.Base;
using MProxy.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net
{
    class NetIOLinkClient : NetIOLocalClient
    {
        public NetIOLinkClient(IPEndPoint ipe) : base(ipe) { }

        protected override void OnConnect(IAsyncResult res)
        {
            Socket s = (Socket)res.AsyncState;
            if (!s.Connected)
                Console.WriteLine("Unable to connect to Linkserver, check if it's running and that the ports match.");
            else
            {
                base.OnConnect(res);
                Console.WriteLine("Connected to Link {0}.", this);
                sock.BeginReceive(buffer, 0, (int)Constants.BUF_ALLOC_SIZE, SocketFlags.None, OnRecvBegin, sock);
            }
        }
    }
}
