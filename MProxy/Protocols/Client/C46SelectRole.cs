using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class C46SelectRole : ClientProtocol
    {
        public uint RoleID { get; private set; }
        public byte Flag { get; private set; }
        public C46SelectRole(Octets os, UserProxy user)
            : base(0x46, os, user)
        {

        }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            RoleID = ReadUInt();
            SwitchOrder();
            Flag = ReadByte();
            return this;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            user.SetRoleID(RoleID);
            base.Process();
        }
    }
}
