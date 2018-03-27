using MProxy.Data;
using MProxy.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net.Base
{
    class NetIORemoteClient : NetIOClient
    {
        public NetIORemoteClient(Socket sock) : base(sock)
        {

        }
        override public void Start()
        {
            sock.BeginReceive(buffer, 0, (int)Constants.BUF_ALLOC_SIZE, SocketFlags.None, OnRecvBegin, sock);
        }
    }
}
