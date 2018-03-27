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
    class UserProxy : Proxy, IComparable
    {

        private uint State = 0;
        private MD5H MD5;
        public uint Localsid { get; private set; }
        public LinkProxy Link { get; private set; }

        public EventHandler<uint> OnChangeID;
        public UserProxy(uint id, NetIORemoteClient remote, NetIOLinkClient srv) : base(id, remote, srv)
        {
            MD5 = new MD5H();
        }

        public void InitS2C(byte[] key)
        {
            State++;
            Client.s2c.InitEnc(MD5.GetKey(key));
            Server.s2c.InitDec(MD5.GetKey(key));
        }
        public void InitC2S(byte[] key)
        {
            State++;
            Client.c2s.InitEnc(MD5.GetKey(key));
            Server.c2s.InitDec(MD5.GetKey(key));
        }
        public void SetHash(Octets os)
        {
            MD5.SetHash(os);
        }
        public void SetLogin(GString login)
        {
            MD5.SetLogin(login);
        }

        public void ChangeID(uint UID)
        {
            OnChangeID(this, UID);
        }
        public void SetLocalsid(uint localsid)
        {
            Localsid = localsid;
        }

        public void SetLink(LinkProxy link)
        {
            Link = link;
        }

        protected override Octets UpdateSendCli(Octets os)
        {
            if (State > 1)
                return Client.s2c.Encrypt(os);
            return os;
        }
        protected override Octets UpdateSendSrv(Octets os)
        {
            if (State > 0)
                return Client.c2s.Encrypt(os);
            return os;

        }
        protected override Octets UpdateRecvCli(Octets os)
        {
            if (State > 0)
                return Server.c2s.Decrypt(os);
            return os;
        }
        protected override Octets UpdateRecvSrv(Octets os)
        {
            if (State > 1)
                return Server.s2c.Decrypt(os);
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
                default:
                    return new ServerProtocol(type, os, this);
            }
        }

        public int CompareTo(object obj)
        {
            UserProxy user2 = (UserProxy)obj;
            if (this.ID == user2.ID) return 0;
            return this.ID < user2.ID ? -1 : 1;
        }
    }
}
