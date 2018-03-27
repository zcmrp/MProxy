using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class L52RoleList : ClientProtocol
    {
        public uint UserID { get; private set; }
        public uint Localsid { get; private set; }
        public int Handle { get; private set; }
        public L52RoleList(Octets os, LinkProxy link) : base(0x52, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            UserID = ReadUInt();
            Localsid = ReadUInt();
            Handle = (int)ReadUInt();
            SwitchOrder();
            return this;
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            if (Handle == -1)
                link.UserSetLink(UserID, Localsid);
            base.Process();
        }
    }
}
