using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class D42PlayerLogin_Re : ServerProtocol
    {
        public int Result { get; private set; }
        public uint RoleID { get; private set; }
        public uint SrcProviderID { get; private set; }
        public uint Localsid { get; private set; }
        public byte Flag { get; private set; }
        public D42PlayerLogin_Re(Octets os, LinkProxy link) : base(0x42, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            Result = (int)ReadUInt();
            RoleID = ReadUInt();
            SrcProviderID = ReadUInt();
            Localsid = ReadUInt();
            SwitchOrder();
            Flag = ReadByte();
            return base.Unmarshal();
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            uint user_id = link.GetUserID(Localsid);
            if (user_id == UInt32.MaxValue)
                return;
            link.OnUserLogin(user_id, RoleID);
            base.Process();
        }
    }
}
