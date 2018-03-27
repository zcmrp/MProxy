using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class D1521HomeEditRes : ServerProtocol
    {
        public uint HomeID { get; private set; }
        public uint Mask { get; private set; }
        public uint Res { get; private set; }
        public uint Localsid { get; private set; }
        public D1521HomeEditRes(Octets os, LinkProxy link) : base(0x1521, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            HomeID = ReadUInt();
            Mask = ReadUInt();
            Res = ReadUInt();
            Localsid = ReadUInt();
            SwitchOrder();
            return this;
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            link.SendToUser(HomeID, this.Marshal());
        }
    }
}
