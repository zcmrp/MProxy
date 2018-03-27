using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Cryptography
{
    class MPPCUnpacker
    {private int code1;
        private int code2;
        private int code3;
        private int code4;

        private byte packedoffset;

        private readonly List<byte> packedbytes;
        private readonly List<byte> unpackedbytes;
        private List<byte> unpackedchunk;

        public MPPCUnpacker()
        {
            code3 = 0;
            code4 = 0;

            packedbytes = new List<byte>();
            unpackedchunk = new List<byte>();
            unpackedbytes = new List<byte>(8 * 1024);
        }

        public void Unpack(byte packedByte, List<byte> unpackedChunk)
        {
            packedbytes.Add(packedByte);
            
            if (unpackedbytes.Count >= 10240)
                unpackedbytes.RemoveRange(0, 2048);

            for (; ;)
            {
                if (code3 == 0)
                {
                    if (HasBits(4))
                    {
                        if (GetPackedBits(1) == 0)
                        {
                            // 0-xxxxxxx
                            code1 = 1;
                            code3 = 1;
                        }
                        else
                        {
                            if (GetPackedBits(1) == 0)
                            {
                                // 10-xxxxxxx
                                code1 = 2;
                                code3 = 1;
                            }
                            else
                            {
                                if (GetPackedBits(1) == 0)
                                {
                                    // 110-xxxxxxxxxxxxx-*
                                    code1 = 3;
                                    code3 = 1;
                                }
                                else
                                {
                                    if (GetPackedBits(1) == 0)
                                    {
                                        // 1110-xxxxxxxx-*
                                        code1 = 4;
                                        code3 = 1;
                                    }
                                    else
                                    {
                                        // 1111-xxxxxx-*
                                        code1 = 5;
                                        code3 = 1;
                                    }
                                }
                            }
                        }
                    }
                    else
                        break;
                }
                else if (code3 == 1)
                {
                    if (code1 == 1)
                    {
                        if (HasBits(7))
                        {
                            var outB = (byte)GetPackedBits(7);
                            unpackedChunk.Add(outB);
                            unpackedbytes.Add(outB);
                            code3 = 0;
                        }
                        else
                            break;
                    }
                    else if (code1 == 2)
                    {
                        if (HasBits(7))
                        {
                            var outB = (byte)(GetPackedBits(7) | 0x80);
                            unpackedChunk.Add(outB);
                            unpackedbytes.Add(outB);
                            code3 = 0;
                        }
                        else
                            break;
                    }
                    else if (code1 == 3)
                    {
                        if (HasBits(13))
                        {
                            code4 = (int)GetPackedBits(13) + 0x140;
                            code3 = 2;
                        }
                        else
                            break;
                    }
                    else if (code1 == 4)
                    {
                        if (HasBits(8))
                        {
                            code4 = (int)GetPackedBits(8) + 0x40;
                            code3 = 2;
                        }
                        else
                            break;
                    }
                    else if (code1 == 5)
                    {
                        if (HasBits(6))
                        {
                            code4 = (int)GetPackedBits(6);
                            code3 = 2;
                        }
                        else
                            break;
                    }
                }
                else if (code3 == 2)
                {
                    if (code4 == 0)
                    {
                        // Guess !!!
                        if (packedoffset != 0)
                        {
                            packedoffset = 0;
                            packedbytes.RemoveAt(0);
                        }
                        code3 = 0;
                        continue;
                    }
                    code2 = 0;
                    code3 = 3;
                }
                else if (code3 == 3)
                {
                    if (HasBits(1))
                    {
                        if (GetPackedBits(1) == 0)
                        {
                            code3 = 4;
                        }
                        else
                        {
                            code2++;
                        }
                    }
                    else
                        break;
                }
                else if (code3 == 4)
                {
                    int copySize;

                    if (code2 == 0)
                    {
                        copySize = 3;
                    }
                    else
                    {
                        var size = code2 + 1;

                        if (HasBits(size))
                        {
                            copySize = (int)GetPackedBits(size) + (1 << size);
                        }
                        else
                            break;
                    }

                    Copy(code4, copySize, ref unpackedChunk);
                    code3 = 0;
                }
            }
        }
        public byte[] Unpack(byte packetByte)
        {
            unpackedchunk.Clear();
            Unpack(packetByte, unpackedchunk);
            return unpackedchunk.ToArray();
        }
        public byte[] Unpack(byte[] compressedBytes)
        {
            return Unpack(compressedBytes, 0, compressedBytes.Length);
        }
        public byte[] Unpack(byte[] compressedBytes, int offset, int count)
        {
            unpackedchunk.Clear();
            for(var i = 0; i < count; i++)
            {
                Unpack(compressedBytes[offset + i], unpackedchunk);
            }

            return unpackedchunk.ToArray();
        }

        private void Copy(int shift, int size, ref List<byte> unpackedChunkData)
        {
            for (var i = 0; i < size; i++)
            {
                var pIndex = unpackedbytes.Count - shift;

                if (pIndex < 0)
                    return;

                var b = unpackedbytes[pIndex];
                unpackedbytes.Add(b);
                unpackedChunkData.Add(b);
            }
        }

        private uint GetPackedBits(int bitCount)
        {
            if (bitCount > 16)
                return 0;

            if (!HasBits(bitCount))
                throw new Exception("Unpack bit stream overflow");

            var alBitCount = bitCount + packedoffset;
            var alByteCount = (alBitCount + 7) / 8;

            uint v = 0;

            for (var i = 0; i < alByteCount; i++)
            {
                v |= (uint)(packedbytes[i]) << (24 - i * 8);
            }

            v <<= packedoffset;
            v >>= 32 - bitCount;

            packedoffset += (byte)bitCount;
            var freeBytes = packedoffset / 8;

            if (freeBytes != 0)
                packedbytes.RemoveRange(0, freeBytes);

            packedoffset %= 8;
            return v;
        }

        private bool HasBits(int count)
        {
            return (packedbytes.Count * 8 - packedoffset) >= count;
        }
    }
}
