using MProxy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Cryptography
{
    class C2SSecurity : Security
    {
        public C2SSecurity()
            : base()
        {

        }
        public override Octets Decrypt(Data.Octets data)
        {
            byte[] dat = data.RawData();
            dec.Decrypt(dat);
            return new Octets(dat);
        }
        public override Octets Encrypt(Data.Octets data)
        {
            byte[] dat = data.RawData();
            enc.Encrypt(dat);
            return new Octets(dat);
        }
    }
}
