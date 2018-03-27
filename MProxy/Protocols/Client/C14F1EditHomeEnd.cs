using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class C14F1EditHomeEnd : ClientProtocol
    {
        public uint HomeID { get; private set; }
        public byte[] Unknown { get; private set; }
        public C14F1EditHomeEnd(Octets os, UserProxy user) : base(0x14F1, os, user) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            HomeID = ReadUInt();
            SwitchOrder();
            Unknown = ReadToEnd();
            return this;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            if (HomeID == user.ID)
                user.SendToDelivery(this.Marshal());
        }

    }
}
