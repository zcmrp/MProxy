using MProxy.Data;
using MProxy.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class ClientProtocol : Protocol
    {

        protected Proxy Session { get; private set; }
        public ClientProtocol(CompactUInt type) : base(type)
        {
        }
        public ClientProtocol(CompactUInt type, Octets os, Proxy session)
            : base(type, os)
        {
            this.Session = session;
        }
        public override void Process()
        {
            Session.SendToServer(this.Marshal());
        }
    }
}
