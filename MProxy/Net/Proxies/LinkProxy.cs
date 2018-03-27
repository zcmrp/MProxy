using MProxy.Data;
using MProxy.Net.Base;
using MProxy.Net.Delivery;
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
    class LinkProxy : Proxy
    {
        public EventHandler<RecvEventArgs> UserLogin;
        public EventHandler<RecvEventArgs> OnDeliverySendUser;
        public EventHandler<RecvEventArgs> OnRequestUserID;
        public EventHandler<RecvEventArgs> OnUserSetLink;
        public LinkProxy(uint id, NetIORemoteClient remote, NetIODeliveryClient srv)
            : base(id, remote, srv)
        {

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
                case 0x52:
                    return new L52RoleList(os, this);
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
                case 0x42:
                    return new D42PlayerLogin_Re(os, this);
                case 0xEF:
                    return new DEFUpdateEnemyList_Re(os, this);
                case 0xF1:
                    return new DF1GetEnemyList_Re(os, this);
                case 0x14ED:
                    return new D14EDHomeBrief(os, this);
                case 0x14EE:
                    return new D14EEHomeDetail(os, this);
                case 0x1502:
                    return new D1502HomeSyncNoticeClient(os, this);
                case 0x150A:
                    return new D150AHomeFoundryFormulasSync(os, this);
                case 0x1516:
                    return new D1516HomeVisitorInfo(os, this);
                case 0x1521:
                    return new D1521HomeEditRes(os, this);
                case 0x1522:
                    return new D1522HomeBrowseInfoQuery_Re(os, this);
                default:
                    return new ServerProtocol(type, os, this);
            }
        }

        public void OnUserLogin(uint user_id, uint role_id)
        {
            Octets os = new Octets().Write(user_id).Write(role_id);
            UserLogin(this, new RecvEventArgs(os));
        }

        public void SendToUser(uint role_id, Octets data)
        {
            Octets os = new Octets().Write(role_id).Write(data);
            OnDeliverySendUser(this, new RecvEventArgs(os));
        }

        public uint GetUserID(uint localsid)
        {
            Octets os = new Octets().Write(localsid);
            RecvEventArgs arg = new RecvEventArgs(os);
            OnRequestUserID(this, arg);
            Octets res = arg.Res;
            return res.ReadUInt();
        }

        public void UserSetLink(uint user_id, uint localsid)
        {
            Octets os = new Octets().Write(user_id).Write(localsid);
            OnUserSetLink(this, new RecvEventArgs(os));
        }
    }
}
