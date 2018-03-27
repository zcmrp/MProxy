using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class D150AHomeFoundryFormulasSync : ServerProtocol
    {
        public uint Localsid { get; private set; }
        public uint HomeID { get; private set; }

        public byte[] Unknown { get; private set; }
        public D150AHomeFoundryFormulasSync(Octets os, LinkProxy link) : base(0x150A, os, link) { }

        public override Octets Marshal()
        {
            SwitchOrder();
            Localsid = ReadUInt();
            HomeID = ReadUInt();
            SwitchOrder();
            Unknown = ReadToEnd();
            return this;
        }

        public override void Process()
        {
            LinkProxy link = (LinkProxy)Session;
            link.SendToUser(HomeID, this.Marshal());
        }
    }
}
