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
        public Proxy(uint id, NetIORemoteClient remote, NetIOLocalClient srv)
        {
            this.ID = id;
            this.Client = remote;
            this.Server = srv;
            this.Client.OnRecv += OnClientRecv;
            this.Server.OnRecv += OnServerRecv;
            this.Client.OnDisconnect += OnClientDisconnect;
            this.Server.OnDisconnect += OnServerDisconnect;
            this.Server.OnConnected += OnConnected;
            Operative = new ProxyOperative(this);
        }

        public void Connect()
        {
            this.Server.Start();
            this.Client.Start();
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
        
        public void SetID(uint ID)
        {
            this.ID = ID;
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

        private void OnConnected(object sender, EventArgs e)
        {
            Connected = true;
            Operative.Run();
        }

        public void Disconnect()
        {
            Connected = false;
            Client.Dispose();
            Server.Dispose();
        }
        ~Proxy()
        {
            Operative.Abort();
        }
    }
}
