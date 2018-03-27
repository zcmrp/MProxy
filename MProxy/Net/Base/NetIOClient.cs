using MProxy.Cryptography;
using MProxy.Data;
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
    abstract class NetIOClient : IDisposable
    {
        protected IPEndPoint IPE { get; set; }

        protected Socket sock;

        protected byte[] buffer;

        public EventHandler<RecvEventArgs> OnRecv;
        public EventHandler OnDisconnect;

        public C2SSecurity c2s;
        public S2CSecurity s2c;

        public NetIOClient(Socket sock)
        {
            this.sock = sock;
            this.IPE = (IPEndPoint)sock.RemoteEndPoint;
            buffer = new byte[Constants.BUF_ALLOC_SIZE];
            c2s = new C2SSecurity();
            s2c = new S2CSecurity();
        }

        public abstract void Start();

        public void OnRecvBegin(IAsyncResult res)
        {
            try
            {
                Socket s = (Socket)res.AsyncState;
                if (!s.Connected)
                {
                    Disconnect();
                    return;
                }
                uint len = (uint)s.EndReceive(res);
                if (len > 0)
                {
                    RecvEventArgs re = new RecvEventArgs(new Octets(buffer, len));
                    OnRecv(this, re);
                    sock.BeginReceive(buffer, 0, (int)Constants.BUF_ALLOC_SIZE, SocketFlags.None, OnRecvBegin, sock);
                }
                else
                    Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Disconnect();
            }
        }

        public void Send(Octets os)
        {
            try
            {
                sock.BeginSend(os.RawData(), 0, os.Length, SocketFlags.None, OnSend, sock);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Disconnect();
            }
        }

        public void OnSend(IAsyncResult res)
        {
            try
            {
                Socket s = (Socket)res.AsyncState;
                if (!s.Connected)
                    Disconnect();
                else
                    s.EndSend(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Disconnect();
            }
        }

        public override string ToString()
        {
            return IPE.Address + ":" + IPE.Port;
        }

        private void Disconnect()
        {
            OnDisconnect(this, new EventArgs());
            Dispose();
        }

        public void Dispose()
        {
            this.sock.Close();
        }
    }
}
