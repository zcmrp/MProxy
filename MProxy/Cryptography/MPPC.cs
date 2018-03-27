using MProxy.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Cryptography
{
    class MPPC
    {
        private MPPCPacker packer;
        private MPPCUnpacker unpacker;
        public MPPC()
        {
            packer = new MPPCPacker();
            unpacker = new MPPCUnpacker();
        }
        public Octets Pack(Octets data)
        {
            return new Octets(packer.Pack(data.RawData()));
        }
        public Octets Unpack(Octets data)
        {
            return new Octets(unpacker.Unpack(data.RawData()));
        }
    }
}
