using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class C02CMKey : ClientProtocol
    {
        public Octets ClientKey { get; private set; }
        public bool Force { get; private set; }
        public C02CMKey(Octets os, UserProxy User) : base(0x02, os, User)
        {

        }
        public override Protocol Unmarshal()
        {
            ClientKey = ReadOctets();
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
            user.InitS2C(ClientKey.RawData());
            base.Process();
        }
    }
}
