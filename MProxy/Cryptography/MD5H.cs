using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MProxy.Data;

namespace MProxy.Cryptography
{
    class MD5H
    {
        private byte[] Hash = new byte[16];
        private byte[] Login = new byte[16];

        public MD5H()
        {

        }
        public byte[] GetKey(byte[] key)
        {
            byte[] nhash = new byte[key.Length + Hash.Length];
            Array.Copy(Hash, nhash, Hash.Length);
            Array.Copy(key, 0, nhash, Hash.Length, key.Length);

            return new HMACMD5(Login).ComputeHash(nhash);
        }

        public void SetHash(Octets hash)
        {
            Hash = hash.RawData();
        }

        public void SetLogin(GString login)
        {
            Login = login.RawData();
        }
    }
}
