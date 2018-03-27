using MProxy.Data;
using MProxy.Net.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class C03LogginAnnounce : ClientProtocol
    {
        public GString Login { get; private set; }
        public Octets Hash { get; private set; }
        public ushort Unk1 { get; private set; }
        public uint Unk2 { get; private set; }
        public C03LogginAnnounce(Octets os, UserProxy User)
            : base(0x03, os, User)
        {

        }
        public override Protocol Unmarshal()
        {
            Login = ReadString(Encoding.ASCII);
            Hash = ReadOctets();
            Unk1 = ReadShort();
            Unk2 = ReadUInt();
            return this;
        }
        public override void Process()
        {
            UserProxy user = (UserProxy)Session;
            user.SetHash(Hash);
            user.SetLogin(Login);
            base.Process();
        }
    }
}
