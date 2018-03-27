using MProxy.Data;
using MProxy.Net.Proxies;
using MProxy.Protocols.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols.Client
{
    class G14FFHomeSceneElements : ClientProtocol
    {
        public uint RoleID { get; private set; }
        public uint HomeID { get; private set; }
        public uint Localsid { get; private set; }
        public uint Worldtag { get; private set; }
        public byte Reason { get; private set; }
        public uint Mask { get; private set; }
        public byte[] Unknown { get; private set; }
        public G14FFHomeSceneElements(Octets os, HomeProxy home) : base(0x14ff, os, home) 
        {
        }
        public override Protocol Unmarshal()
        {
            SwitchOrder();
            RoleID = ReadUInt();
            HomeID = ReadUInt();
            Localsid = ReadUInt();
            Worldtag = ReadUInt();
            Reason = ReadByte();
            SwitchOrder();
            Unknown = ReadToEnd();
            return this;
        }

        public override void Process()
        {
            HomeProxy home = (HomeProxy)Session;
            home.SendToUser(RoleID, this.Marshal());
        }
    }
}
