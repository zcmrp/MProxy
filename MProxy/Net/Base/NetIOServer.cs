using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MProxy.Net.Base
{
    class NetIOServer
    {
        protected uint Counter { get; set; }
        protected IPEndPoint IPE { get; set; }
        protected IPEndPoint CIPE { get; set; }
        protected TcpListener TCP;
        protected int MaxClients { get; set; }
        public NetIOServer(IPAddress ip, ushort port, IPAddress cip, ushort cport)
        {
            this.IPE = new IPEndPoint(IPAddress.Any, port);
            this.CIPE = new IPEndPoint(cip, cport);
            this.TCP = new TcpListener(IPE);
            MaxClients = 0;
            Counter = UInt32.MaxValue;
        }
        public NetIOServer(ushort port, string chost, ushort cport) : this(IPAddress.Any, port, IPAddress.Parse(chost), cport) { }

        public void Start()
        {
            try
            {
                TCP.Start(MaxClients);
                Console.WriteLine("Now listening on port {0}.", IPE.Port);
                new Thread(() => OnListen()).Start();
            }
            catch (SocketException)
            {
                Console.WriteLine("Unable to listen for connections on port {0}, make sure the port is not being used.", IPE.Port);
            }
        }

        protected virtual void OnListen() { }
        protected virtual void OnClientDisconnect(object sender, EventArgs e) { }

        public void SetLimit(int max)
        {
            MaxClients = max;
        }
    }
}