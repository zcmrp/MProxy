using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Data
{
    class Octets
    {
        private static int ALLOC_SIZE = 128;
        private enum ByteOrder { LittleEndian = 1, BigEndian = 2 };
        private byte[] buffer;
        public uint Position { get; private set; }

        private uint EXLen { get; set; }
        private uint EXPos { get; set; }
        public CompactUInt Length { get; private set; }
        public uint Size { get { return (uint)buffer.Length; } }
        private ByteOrder Order { get; set; }

        public Octets()
        {
            this.Order = BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            this.buffer = new byte[ALLOC_SIZE];
            Length = 0;
        }
        public Octets(byte[] buf)
            : this(buf, (uint)buf.Length)
        {

        }
        public Octets(byte[] buf, uint size)
        {
            this.Order = BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            this.buffer = new byte[size];
            Length = size;
            Array.Copy(buf, 0, this.buffer, 0, size);
        }
        public Octets(Octets os)
        {
            this.Order = BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            Length = new CompactUInt(os);
            buffer = os.ReadBytes(Length);
        }

        public Octets Reset()
        {
            Position = 0;
            return this;
        }
        public Octets SwitchOrder()
        {
            if (this.Order == ByteOrder.LittleEndian)
                this.Order = ByteOrder.BigEndian;
            else
                this.Order = ByteOrder.LittleEndian;
            return this;
        }

        public Octets Write(object data)
        {
            byte[] conv;
            if (data is byte[])
            {
                byte[] write = (byte[])data;
                this.Extend((uint)write.Length);
                write.CopyTo(this.buffer, this.EXLen);
                return this;
            } 
            else if (data is bool)
            {
                return Write((byte)((bool)data ? 1 : 0));
            }
            else if (data is byte)
            {
                this.Extend(1).buffer[this.EXLen] = (byte)data;
                return this;
            }
            else if (data is short || data is ushort)
            {
                conv = BitConverter.GetBytes((ushort)data);
                if (this.Order == ByteOrder.BigEndian)
                    conv = conv.Reverse().ToArray();
                return Write(conv);
            }
            else if (data is uint || data is int)
            {
                conv = BitConverter.GetBytes((uint)data);
                if (this.Order == ByteOrder.BigEndian)
                    conv = conv.Reverse().ToArray();
                return Write(conv);
            }
            else if (data is ulong || data is long)
            {
                conv = BitConverter.GetBytes((ulong)data);
                if (this.Order == ByteOrder.BigEndian)
                    conv = conv.Reverse().ToArray();
                return Write(conv);
            }
            else if (data is Octets)
            {
                Octets src = (Octets)data;
                return Write(src.Length).Write(src.RawData());
            }
            else if (data is CompactUInt)
            {
                CompactUInt cu = (CompactUInt)data;
                return Write(cu.RawData());
            }
            else if (data is GString)
            {
                GString gstr = (GString)data;
                return Write(gstr.Length).Write(gstr.RawData());
            }
            return this;
        }
        public Octets Clone()
        {
            return new Octets(buffer, Length);
        }

        public byte[] ReadToEnd()
        {
            uint len = Length - Position;
            if (len == 0) return null;
            return ReadBytes(len);
        }
        public byte[] ReadBytes(uint len)
        {
            Skip(len);
            byte[] ret = new byte[len];
            Array.Copy(buffer, EXPos, ret, 0, len);
            return ret;
        }
        public byte ReadByte()
        {
            return Skip(1).buffer[EXPos];
        }
        public ushort ReadShort()
        {
            byte[] ret = ReadBytes(2);
            if (Order == ByteOrder.BigEndian)
                ret = ret.Reverse().ToArray();
            return BitConverter.ToUInt16(ret, 0);
        }
        public uint ReadUInt()
        {
            byte[] ret = ReadBytes(4);
            if (Order == ByteOrder.BigEndian)
                ret = ret.Reverse().ToArray();
            return BitConverter.ToUInt32(ret, 0);
        }
        public ulong ReadULong()
        {
            byte[] ret = ReadBytes(8);
            if (Order == ByteOrder.BigEndian)
                ret = ret.Reverse().ToArray();
            return BitConverter.ToUInt64(ret, 0);
        }
        public GString ReadString(Encoding enc)
        {
            return new GString(this, enc);
        }
        public Octets ReadOctets()
        {
            return new Octets(this);
        }
        public bool ReadBoolean()
        {
            return ReadByte() > 0;
        }
        public CompactUInt ReadCompactUInt()
        {
            return new CompactUInt(this);
        }

        public Octets Extend(uint val)
        {
            EXLen = Length;
            while (Length + val > Size)
                Realloc();
            Length += val;
            return this;
        }
        public Octets Skip(uint val)
        {
            if (Position + val > Length)
                throw new IndexOutOfRangeException("Error. Tried reading beyond the array range.");
            EXPos = Position;
            Position += val;
            return this;
        }
        public Octets Undo()
        {
            Position = EXPos;
            return this;
        }
        public byte[] RawData()
        {
            byte[] ret = new byte[Length];
            Array.Copy(buffer, 0, ret, 0, Length);
            return ret;
        }
        public Octets Realloc()
        {
            byte[] temp = new byte[Size];
            buffer.CopyTo(temp, 0);
            buffer = new byte[Size * 2];
            temp.CopyTo(buffer, 0);
            return this;
        }
        public bool CanRead()
        {
            return Position < Length;
        }

        public override string ToString()
        {
            return BitConverter.ToString(RawData());
        }
    }
}
