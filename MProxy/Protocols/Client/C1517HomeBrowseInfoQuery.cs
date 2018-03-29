using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class C1517HomeBrowseInfoQuery : ClientProtocol
    {
        public uint HomeID { get; private set; }
        public uint RoleID { get; private set; }
        public C1517HomeBrowseInfoQuery(Octets os, UserProxy user)
            : base(0x1517, os, user)
        {

        }
        public override Protocol Unmarshal()
        {
            SwitchOrder();
            HomeID = ReadUInt();
            RoleID = ReadUInt();
            SwitchOrder();
            return this;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            if (user.RoleID == RoleID)
                user.SendToDelivery(this.Marshal());
        }
    }
}
