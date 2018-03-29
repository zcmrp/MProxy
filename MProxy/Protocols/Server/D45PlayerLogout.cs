using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class D45PlayerLogout : ServerProtocol
    {
        public uint Result { get; private set; }
        public uint RoleID { get; private set; }
        public uint ProviderLinkID { get; private set; }
        public uint Localsid { get; private set; }
        public D45PlayerLogout(Octets os, LinkProxy link)
            : base(0x45, os, link)
        {

        }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            Result = ReadUInt();
            RoleID = ReadUInt();
            ProviderLinkID = ReadUInt();
            Localsid = ReadUInt();
            SwitchOrder();
            return this;
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            if (Result == 1)
                link.LogoutUser(RoleID);
            base.Process();
        }
    }
}
