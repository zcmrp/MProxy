using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Data
{
    class GString
    {
        private string str;
        public Encoding GEncoding { get; private set; }
        public CompactUInt Length { get { return new CompactUInt((uint)((str.Length + 1) * (GEncoding == Encoding.ASCII ? 1 : 2))); } }
        
        public GString(string str) : this(str, Encoding.ASCII)
        {
        }
        public GString(string str, Encoding enc)
        {
            this.str = str;
            this.GEncoding = enc;
        }
        public GString(Octets os) : this(os, Encoding.ASCII)
        {

        }
        public GString(Octets os, Encoding enc)
        {
            CompactUInt len = os.ReadCompactUInt();
            byte[] bstr = os.ReadBytes(len);
            this.str = enc.GetString(bstr);
            this.GEncoding = enc;
        }

        public override string ToString()
        {
            return str;
        }

        public byte[] RawData()
        {
            return GEncoding.GetBytes(str);
        }
    }
}
