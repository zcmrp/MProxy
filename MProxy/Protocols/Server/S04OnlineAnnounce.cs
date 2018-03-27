using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class S04OnlineAnnounce : ServerProtocol
    {
        public uint UserID { get; private set; }
        public uint Localsid { get; private set; }
        public byte[] Unknown { get; private set; }
        public S04OnlineAnnounce(Octets os, UserProxy user) : base(0x04, os, user) { }

        public override Protocol Unmarshal()
        {
            SwitchOrder();
            UserID = ReadUInt();
            Localsid = ReadUInt();
            SwitchOrder();
            Unknown = ReadToEnd();
            return this;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            user.SetLocalsid(Localsid);
            user.ChangeID(UserID);
            base.Process();
        }
    }
}
