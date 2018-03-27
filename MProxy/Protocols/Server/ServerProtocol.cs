using MProxy.Data;
using MProxy.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class ServerProtocol : Protocol
    {
        protected Proxy Session { get; private set; }
        public ServerProtocol(CompactUInt type)
            : base(type)
        {
        }
        public ServerProtocol(CompactUInt type, Octets os, Proxy session)
            : base(type, os)
        {
            this.Session = session;
        }
        public override void Process()
        {
            Session.SendToClient(this.Marshal());
        }
    }
}
