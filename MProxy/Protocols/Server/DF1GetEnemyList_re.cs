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
    class DF1GetEnemyList_Re : ServerProtocol
    {
        private uint RoleID { get; set; }
        private uint Localsid { get; set; }
        public byte[] Unknown { get; private set; }
        public DF1GetEnemyList_Re(Octets os, LinkProxy link)
            : base(0xF1, os, link)
        {
            
        }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            RoleID = ReadUInt();
            Localsid = ReadUInt();
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
