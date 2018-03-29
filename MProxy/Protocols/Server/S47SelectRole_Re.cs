using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Server
{
    class S47SelectRole_Re : ServerProtocol
    {
        public byte[] Unknown { get; private set; }
        public S47SelectRole_Re(Octets os, UserProxy user)
            : base(0x47, os, user)
        {

        }

        public override Protocol Unmarshal()
        {
            Unknown = ReadToEnd();
            return this;
        }

        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            user.ProcessRoleID();
            base.Process();
        }
    }
}
