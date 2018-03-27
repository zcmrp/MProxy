using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class DEFUpdateEnemyList_Re : ServerProtocol
    {
        public uint Retcode { get; private set; }
        public byte OpType { get; private set; }
        public uint RoleID { get; private set; }
        public byte[] Unknown { get; private set; }
        public DEFUpdateEnemyList_Re(Octets os, LinkProxy link) : base(0xEF, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            Retcode = ReadUInt();
            OpType = ReadByte();
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
