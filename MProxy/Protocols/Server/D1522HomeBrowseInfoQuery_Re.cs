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
    class D1522HomeBrowseInfoQuery_Re : ServerProtocol
    {
        public uint HomeID { get; private set; }
        public uint RoleID { get; private set; }
        public uint Localsid { get; private set; }
        public uint Retcode { get; private set; }
        public byte[] Unknown { get; private set; }
        public D1522HomeBrowseInfoQuery_Re(Octets os, LinkProxy link) : base(0x1522, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            HomeID = ReadUInt();
            RoleID = ReadUInt();
            Localsid = ReadUInt();
            Retcode = ReadUInt();
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
