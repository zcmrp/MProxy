using MProxy.Data;
using MProxy.Net.Proxies;
using MProxy.Protocols.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class S02SMKey : ServerProtocol
    {
        public Octets ServerKey { get; private set; }
        public bool Force { get; private set; }
        public S02SMKey(Octets os, UserProxy user)
            : base(0x02, os, user)
        {

        }
        public override Protocol Unmarshal()
        {
            ServerKey = ReadOctets();
            Force = ReadBoolean();
            return this;
        }

        public override bool SizePolicy()
        {
            return Length <= 0x20;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            user.InitC2S(ServerKey.RawData());
            base.Process();
        }
    }
}
