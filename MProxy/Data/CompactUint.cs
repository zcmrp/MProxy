using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Data
{
    class CompactUInt
    {
        public uint Value { get; private set; }
        public CompactUInt(uint value)
        {
            this.Value = value;
        }

        public CompactUInt(Octets os)
        {
            os.SwitchOrder();
            byte first = os.ReadByte();
            switch (first & 0xE0)
            {
                case 0xE0:
                    Value = BitConverter.ToUInt32(os.ReadBytes(4), 0);
                    break;
                case 0xC0:
                    os.Undo();
                    Value = os.ReadUInt() & 0x1FFFFFFF;
                    break;
                case 0x80:
                case 0xA0:
                    os.Undo();
                    Value = (uint)os.ReadShort() & 0x3FFF;
                    break;
                default:
                    Value = first;
                    break;
            }
            os.SwitchOrder();
        }

        public static implicit operator uint(CompactUInt cu)
        {
            return cu.Value;
        }
        public static implicit operator CompactUInt(uint val)
        {
            return new CompactUInt(val);
        }
        public static implicit operator int(CompactUInt cu)
        {
            return (int)cu.Value;
        }
        public static implicit operator CompactUInt(int val)
        {
            return new CompactUInt((uint)val);
        }

        public byte[] RawData()
        {
            if (Value <= 0x7F)
                return new byte[] { (byte)Value };
            if (Value <= 0x3FFF)
                return BitConverter.GetBytes((ushort)(Value + 0x8000)).Reverse().ToArray();
            if (Value <= 0x1FFFFFFF)
                return BitConverter.GetBytes((uint)(Value + 0xC0000000)).Reverse().ToArray();
            List<byte> ret = BitConverter.GetBytes((char)0xE0).ToList();
            ret.AddRange(BitConverter.GetBytes(Value).Reverse().ToArray());
            return ret.ToArray();
        }
    }
}
