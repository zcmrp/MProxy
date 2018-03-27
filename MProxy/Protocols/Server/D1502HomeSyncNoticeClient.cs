using MProxy.Data;
using MProxy.Net;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class D1502HomeSyncNoticeClient : ServerProtocol
    {
        public uint Localsid { get; private set; }
        public uint OptType { get; private set; }

        public uint RetCode { get; private set; }
        public uint RoleID { get; private set; }

        public byte[] Unknown { get; private set; }
        public D1502HomeSyncNoticeClient(Octets os, LinkProxy link) : base(0x1502, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            Localsid = ReadUInt();
            OptType = ReadUInt();
            RetCode = ReadUInt();
            RoleID = ReadUInt();
            SwitchOrder();
            Unknown = ReadToEnd();
            return this;
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            link.SendToUser(RoleID, this.Marshal());
        }
    }
}
