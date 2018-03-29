using MProxy.Cryptography;
using MProxy.Data;
using MProxy.Net;
using MProxy.Net.Base;
using MProxy.Protocols;
using MProxy.Protocols.Client;
using MProxy.Protocols.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net.Proxies
{
    class UserProxy : Proxy
    {

        private uint State = 0;
        private MD5H MD5;
        public uint Localsid { get; private set; }
        public LinkProxy Link { get; private set; }
        public uint RoleID { get; private set; }

        public EventHandler OnSetID;
        public EventHandler OnSetRoleID;
        public UserProxy(uint id, NetIORemoteClient remote, NetIOLinkClient srv) : base(remote, srv)
        {
            MD5 = new MD5H();
            Link = null;
        }

        public void InitS2C(byte[] key)
        {
            State++;
            Client.S2C.InitEnc(MD5.GetKey(key));
            Server.S2C.InitDec(MD5.GetKey(key));
        }
        public void InitC2S(byte[] key)
        {
            State++;
            Client.C2S.InitEnc(MD5.GetKey(key));
            Server.C2S.InitDec(MD5.GetKey(key));
        }
        public void SetHash(Octets os)
        {
            MD5.SetHash(os);
        }
        public void SetLogin(GString login)
        {
            MD5.SetLogin(login);
        }

        public void SetID(uint id)
        {
            ID = id;
            OnSetID(this, new EventArgs());
        }
        public void SetLocalsid(uint localsid)
        {
            Localsid = localsid;
        }
        public void SetRoleID(uint role_id)
        {
            RoleID = role_id;
        }
        public void SetLink(LinkProxy link)
        {
            Link = link;
        }
        public void ProcessRoleID()
        {
            OnSetRoleID(this, new EventArgs());
        }
        protected override Octets UpdateSendCli(Octets os)
        {
            if (State > 1)
                return Client.S2C.Encrypt(os);
            return os;
        }
        protected override Octets UpdateSendSrv(Octets os)
        {
            if (State > 0)
                return Client.C2S.Encrypt(os);
            return os;

        }
        protected override Octets UpdateRecvCli(Octets os)
        {
            if (State > 0)
                return Server.C2S.Decrypt(os);
            return os;
        }
        protected override Octets UpdateRecvSrv(Octets os)
        {
            if (State > 1)
                return Server.S2C.Decrypt(os);
            return os;
        }

        public void SendToDelivery(Octets os)
        {
            if (Link == null)
                Disconnect();
            else
                Link.SendToServer(os);
        }

        public override ClientProtocol DecodeCli(Octets os)
        {
            CompactUInt type = new CompactUInt(os);
            if (type > 0x3FFF)
                throw new NotSupportedException();
            CompactUInt size = new CompactUInt(os);
            if (size + os.Position > os.Length)
                throw new ProtocolSizeExceedException();
            os.Undo();
            switch (type.Value)
            {
                case 0x02:
                    return new C02CMKey(os, this);
                case 0x03:
                    return new C03LogginAnnounce(os, this);
                case 0x46:
                    return new C46SelectRole(os, this);
                case 0xF0:
                    return new CF0GetEnemyList(os, this);
                case 0x14EC:
                    return new C14ECHomeQuery(os, this);
                case 0x14F1:
                    return new C14F1EditHomeEnd(os, this);
                case 0x1510:
                    return new C1510HomeFoundryQuery(os, this);
                case 0x1515:
                    return new C1515HomeVisitorQuery(os, this);
                case 0x1517:
                    return new C1517HomeBrowseInfoQuery(os, this);
                default:
                    return new ClientProtocol(type, os, this);
            }
        }
        public override ServerProtocol DecodeSrv(Octets os)
        {
            CompactUInt type = new CompactUInt(os);
            if (type > 0x3FFF)
                throw new NotSupportedException();
            CompactUInt size = new CompactUInt(os);
            if (size + os.Position > os.Length)
                throw new ProtocolSizeExceedException();
            os.Undo();
            switch (type.Value)
            {
                case 0x02:
                    return new S02SMKey(os, this);
                case 0x04:
                    return new S04OnlineAnnounce(os, this);
                case 0x47:
                    return new S47SelectRole_Re(os, this);
                default:
                    return new ServerProtocol(type, os, this);
            }
        }
    }
}
