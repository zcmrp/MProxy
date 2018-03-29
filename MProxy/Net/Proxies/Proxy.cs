using MProxy.Data;
using MProxy.Net.Base;
using MProxy.Net.Proxies;
using MProxy.Protocols;
using MProxy.Protocols.Client;
using MProxy.Protocols.Server;
using MProxy.Setup;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MProxy.Net
{
    abstract class Proxy
    {
        protected NetIORemoteClient Client;
        protected NetIOLocalClient Server;

        private ProxyOperative Operative;

        private bool Connected = false;

        public EventHandler OnDisconnect;
        public uint ID { get; protected set; }
        public Proxy(NetIORemoteClient remote, NetIOLocalClient srv)
        {
            ID = 0;
            Client = remote;
            Server = srv;
            Client.OnRecv += OnClientRecv;
            Server.OnRecv += OnServerRecv;
            Client.OnDisconnect += OnClientDisconnect;
            Server.OnDisconnect += OnServerDisconnect;
            Server.OnConnected += OnConnected;
            Server.OnConnectionRefused += OnConnectionRefused;
            Operative = new ProxyOperative(this);
        }

        public void Connect()
        {
            this.Server.Start();
        }
        
        protected void OnClientRecv(object sender, RecvEventArgs e)
        {
            Octets os = e.Data.Clone();
            os.Reset();
            os = UpdateRecvCli(os);
            Operative.EnqueueCli(os);
        }
        protected void OnServerRecv(object sender, RecvEventArgs e)
        {
            Octets os = e.Data.Clone();
            os.Reset();
            os = UpdateRecvSrv(os);
            Operative.EnqueueSrv(os);
        }

        protected void OnClientDisconnect(object sender, EventArgs e)
        {
            if (Connected)
            {
                Connected = false;
                OnDisconnect(this, e);
            }
            this.Server.Dispose();
        }
        protected void OnServerDisconnect(object sender, EventArgs e)
        {
            if (Connected)
            {
                Connected = false;
                OnDisconnect(this, e);
            }
            this.Client.Dispose();
        }

        public void SendToServer(Octets data)
        {
            if (!Connected) return;
            data = UpdateSendSrv(data);
            Server.Send(data);
        }
        public void SendToClient(Octets data)
        {
            if (!Connected) return;
            data = UpdateSendCli(data);
            Client.Send(data);
        }
        protected virtual Octets UpdateRecvCli(Octets os)
        {
            return os;
        }
        protected virtual Octets UpdateRecvSrv(Octets os)
        {
            return os;
        }
        protected virtual Octets UpdateSendCli(Octets os)
        {
            return os;
        }
        protected virtual Octets UpdateSendSrv(Octets os)
        {
            return os;
        }
        public abstract ServerProtocol DecodeSrv(Octets os);
        public abstract ClientProtocol DecodeCli(Octets os);

        protected virtual void OnConnected(object sender, EventArgs e)
        {
            Connected = true;
            Client.Start();
            Operative.Run();
        }
        private void OnConnectionRefused(object sender, EventArgs e)
        {
            Client.Dispose();
            OnDisconnect(this, new EventArgs());
        }

        public void Disconnect()
        {
            Connected = false;
            Client.Dispose();
            Server.Dispose();
            OnDisconnect(this, new EventArgs());
        }
        public override string ToString()
        {
            return Client.ToString();
        }
        ~Proxy()
        {
            Operative.Abort();
        }
    }
}
