using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class CF0GetEnemyList : ClientProtocol
    {
        public uint RoleID { get; private set; }
        private uint Localsid { get; set; }
        public CF0GetEnemyList(Octets os, UserProxy User)
            : base(0xF0, os, User)
        {

        }

        public override Octets Marshal()
        {
            return new Octets().Write(Type).Write(Len).SwitchOrder().Write(RoleID).Write(Localsid);
        }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            RoleID = ReadUInt();
            Localsid = ReadUInt();
            SwitchOrder();
            return this;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            if (user.ID != RoleID)
                return;
            Localsid = user.Localsid;
            user.SendToDelivery(this.Marshal());
        }
    }
}
