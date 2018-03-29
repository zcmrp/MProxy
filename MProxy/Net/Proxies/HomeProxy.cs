using MProxy.Data;
using MProxy.Net.Base;
using MProxy.Net.Provider;
using MProxy.Protocols;
using MProxy.Protocols.Client;
using MProxy.Protocols.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net.Proxies
{
    class HomeProxy : Proxy
    {
        public HomeProxy(uint id, NetIORemoteClient link, NetIOProviderClient provider) : base(link, provider) { }
        public EventHandler<RecvEventArgs> OnGsSendUser;
        private static uint Counter = 0;

        public override ClientProtocol DecodeCli(Octets os)
        {
            CompactUInt type = new CompactUInt(os);
            if (type > 0x3FFF)
                throw new NotSupportedException();
            CompactUInt size = new CompactUInt(os);
            if (size + os.Position > os.Length)
                throw new ProtocolSizeExceedException();
            os.Undo();
            switch (type.Value)
            {
                case 0x14FF:
                    return new G14FFHomeSceneElements(os, this);
                default:
                    return new ClientProtocol(type, os, this);
            }
        }
        public override ServerProtocol DecodeSrv(Octets os)
        {
            CompactUInt type = new CompactUInt(os);
            if (type > 0x3FFF)
                throw new NotSupportedException();
            CompactUInt size = new CompactUInt(os);
            if (size + os.Position > os.Length)
                throw new ProtocolSizeExceedException();
            os.Undo();
            switch (type.Value)
            {
                default:
                    return new ServerProtocol(type, os, this);
            }
        }

        protected override void OnConnected(object sender, EventArgs e)
        {
            ID = ++Counter;
            Console.WriteLine("Gs {0} connected to Provider {1}", ID, Server);
            base.OnConnected(sender, e);
        }

        public void SendToUser(uint role_id, Octets data)
        {
            Octets os = new Octets().Write(role_id).Write(data);
            OnGsSendUser(this, new RecvEventArgs(os));
        }
    }
}
