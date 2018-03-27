using MProxy.Data;
using MProxy.Net;
using MProxy.Protocols.Client;
using MProxy.Protocols.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols
{
    class Protocol : Octets, IProtocol<Protocol>
    {
        public CompactUInt Type { get; private set; }
        public CompactUInt Len { get; private set; }
        public bool Incomplete { get; private set; }
        public Protocol(CompactUInt type) : base()
        {
            this.Type = type;
            this.Incomplete = false;
        }
        public Protocol(CompactUInt type, Octets os) : base()
        {
            this.Type = type;
            this.Len = os.ReadCompactUInt();
            if (!SizePolicy())
                throw new NotSupportedException("Err: size policy wrong, size = " + this.Len);
            os.Undo();
            Write(os.ReadOctets().RawData());
            Unmarshal();
        }
        public virtual Octets Marshal()
        {
            return new Octets().Write(Type).Write(this);
        }
        public virtual Protocol Unmarshal()
        {
            return this;
        }
        public virtual bool SizePolicy()
        {
            return this.Len <= 0x10000;
        }

        public virtual void Process() { }


        public new Protocol Clone()
        {
            throw new NotImplementedException();
        }
    }
}
