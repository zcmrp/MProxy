using MProxy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Cryptography
{
    class Security
    {
        protected MPPC mppc;
        protected RC4 enc;
        protected RC4 dec;
        public Security()
        {
            mppc = new MPPC();
        }
        public void InitDec(byte[] key)
        {
            dec = new RC4(key);
        }
        public void InitEnc(byte[] key)
        {
            enc = new RC4(key);
        }
        public virtual Octets Encrypt(Octets data)
        {
            return data;
        }
        public virtual Octets Decrypt(Octets data)
        {
            return data;
        }
    }
}
