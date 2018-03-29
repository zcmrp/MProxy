using MProxy.Net.Base;
using MProxy.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net.Provider
{
    class NetIOProviderClient : NetIOLocalClient
    {
        public NetIOProviderClient(IPEndPoint ipe) : base(ipe) { }

        protected override void OnConnect(IAsyncResult res)
        {
            Socket s = (Socket)res.AsyncState;
            if (!s.Connected)
            {
                Console.WriteLine("Unable to connect to Provider server {0}, check if it's running and that the ports match", this);
                OnConnectionRefused(this, new EventArgs());
            }
            else
            {
                base.OnConnect(res);
                Skt.BeginReceive(Buffer, 0, (int)Constants.BUF_ALLOC_SIZE, SocketFlags.None, OnRecvBegin, Skt);
            }
        }
    }
}
