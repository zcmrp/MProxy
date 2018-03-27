using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class C1510HomeFoundryQuery : ClientProtocol
    {
        public uint RoleID { get; private set; }
        public uint HomeID { get; private set; }
        public byte QueryType { get; private set; }
        public uint CurrentRid { get; private set; }
        public C1510HomeFoundryQuery(Octets os, UserProxy user) : base(0x1510, os, user) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            HomeID = ReadUInt();
            RoleID = ReadUInt();
            QueryType = ReadByte();
            CurrentRid = ReadUInt();
            SwitchOrder();
            return this;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            if (this.RoleID == user.ID)
                user.SendToDelivery(this.Marshal());
        }
    }
}
