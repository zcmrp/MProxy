using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class D1516HomeVisitorInfo : ServerProtocol
    {
        public uint Localsid { get; private set; }
        public uint HomeID { get; private set; }
        public uint RoleID { get; private set; }
        public D1516HomeVisitorInfo(Octets os, LinkProxy link) : base(0x1516, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            Localsid = ReadUInt();
            HomeID = ReadUInt();
            RoleID = ReadUInt();
            SwitchOrder();
            return this;
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            link.SendToUser(RoleID, this.Marshal());
        }
    }
}
