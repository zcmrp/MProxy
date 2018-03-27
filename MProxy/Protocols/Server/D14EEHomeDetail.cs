using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class D14EEHomeDetail : ServerProtocol
    {
        public uint Localsid { get; private set; }
        public byte[] Unknown { get; private set; }
        public D14EEHomeDetail(Octets os, LinkProxy link) : base(0x14EE, os, link) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            Localsid = ReadUInt();
            SwitchOrder();
            Unknown = ReadToEnd();
            return this;
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            uint user_id = link.GetUserID(Localsid);
            if (user_id == UInt32.MaxValue)
                return;
            link.SendToUser(user_id, this.Marshal());
        }
    }
}
