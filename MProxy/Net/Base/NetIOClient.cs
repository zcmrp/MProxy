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

        protected Socket Skt;

        protected byte[] Buffer;

        public EventHandler<RecvEventArgs> OnRecv;
        public EventHandler OnDisconnect;

        public C2SSecurity C2S;
        public S2CSecurity S2C;

        public NetIOClient(Socket sock)
        {
            this.Skt = sock;
            this.IPE = (IPEndPoint)sock.RemoteEndPoint;
            Buffer = new byte[Constants.BUF_ALLOC_SIZE];
            C2S = new C2SSecurity();
            S2C = new S2CSecurity();
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
                    RecvEventArgs re = new RecvEventArgs(new Octets(Buffer, len));
                    OnRecv(this, re);
                    Skt.BeginReceive(Buffer, 0, (int)Constants.BUF_ALLOC_SIZE, SocketFlags.None, OnRecvBegin, Skt);
                }
                else
                    Disconnect();
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        public void Send(Octets os)
        {
            try
            {
                Skt.BeginSend(os.RawData(), 0, os.Length, SocketFlags.None, OnSend, Skt);
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            this.Skt.Close();
        }
    }
}
