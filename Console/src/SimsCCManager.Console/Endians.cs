using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SimsCCManager.Decryption.EndianDecoding
{
    public enum EndianType {
            Little,
            Big
        }    
    public static class Endian
    {
        public static uint ReadValueU32(this BinaryReader stream, EndianType endian){
            var data = stream.ReadBytes(4);
            var value = BitConverter.ToUInt32(data, 0);
            if (ShouldSwap(endian) == true)
            {
                value = value.Swap();
            }
            return value;
        }

        public static uint ReadValueU32(this BinaryReader stream)
        {
            return stream.ReadValueU32(EndianType.Little);
        }

        public static int ReadValueS32(this BinaryReader stream, EndianType endian)
        {
            return (int)stream.ReadValueU32(endian);
        }
        public static int ReadValueS32(this BinaryReader stream)
        {
            return (int)stream.ReadValueU32();
        }

        internal static bool ShouldSwap(EndianType endian)
        {
            switch (endian)
            {
                case EndianType.Little: return BitConverter.IsLittleEndian == false;
                case EndianType.Big: return BitConverter.IsLittleEndian == true;
                default: throw new ArgumentException("unsupported endianness", "endian");
            }
        }

        public static ulong ReadValueU64(this BinaryReader stream, EndianType endian)
        {
            var data = stream.ReadBytes(8);
            var value = BitConverter.ToUInt64(data, 0);
            if (ShouldSwap(endian) == true)
            {
                value = value.Swap();
            }
            return value;
        }
        public static ulong ReadValueU64(this BinaryReader stream)
        {
            return stream.ReadValueU64(EndianType.Little);
        }

        public static ushort ReadValueU16(this BinaryReader stream, EndianType endian)
        {
            var data = stream.ReadBytes(2);
            var value = BitConverter.ToUInt16(data, 0);
            if (ShouldSwap(endian) == true)
            {
                value = value.Swap();
            }
            return value;
        }
        public static ushort ReadValueU16(this BinaryReader stream)
        {
            return stream.ReadValueU16(EndianType.Little);
        }






        public static short Swap(this short value)
        {
            return (short)((ushort)value).Swap();
        }
        public static ushort Swap(this ushort value)
        {
            return (ushort)((0x00FFu) & (value >> 8) |
                            (0xFF00u) & (value << 8));
        }
        public static int Swap(this int value)
        {
            return (int)((uint)value).Swap();
        }
        public static uint Swap(this uint value)
        {
            return ((0x000000FFu) & (value >> 24) |
                    (0x0000FF00u) & (value >> 8) |
                    (0x00FF0000u) & (value << 8) |
                    (0xFF000000u) & (value << 24));
        }
        public static long Swap(this long value)
        {
            return (long)((ulong)value).Swap();
        }
        public static ulong Swap(this ulong value)
        {
            return ((0x00000000000000FFu) & (value >> 56) |
                    (0x000000000000FF00u) & (value >> 40) |
                    (0x0000000000FF0000u) & (value >> 24) |
                    (0x00000000FF000000u) & (value >> 8) |
                    (0x000000FF00000000u) & (value << 8) |
                    (0x0000FF0000000000u) & (value << 24) |
                    (0x00FF000000000000u) & (value << 40) |
                    (0xFF00000000000000u) & (value << 56));
        }
        public static float Swap(this float value)
        {
            var overlap = new OverlapSingle(value);
            overlap.AsU = overlap.AsU.Swap();
            return overlap.AsF;
        }
        public static double Swap(this double value)
        {
            var overlap = new OverlapDouble(value);
            overlap.AsU = overlap.AsU.Swap();
            return overlap.AsD;
        }
    }

        [StructLayout(LayoutKind.Explicit)]
    internal struct OverlapSingle
    {
        [FieldOffset(0)]
        private float F;
        [FieldOffset(0)]
        private uint U;
        public OverlapSingle(float f)
        {
            this.U = default(uint);
            this.F = f;
        }
        public OverlapSingle(uint u)
        {
            this.F = default(float);
            this.U = u;
        }
        public float AsF
        {
            get { return this.F; }
            set { this.F = value; }
        }
        public uint AsU
        {
            get { return this.U; }
            set { this.U = value; }
        }
    }

        [StructLayout(LayoutKind.Explicit)]
    internal struct OverlapDouble
    {
        [FieldOffset(0)]
        private double D;
        [FieldOffset(0)]
        private ulong U;
        public OverlapDouble(double d)
        {
            this.U = default(ulong);
            this.D = d;
        }
        public OverlapDouble(ulong u)
        {
            this.D = default(double);
            this.U = u;
        }
        public double AsD
        {
            get { return this.D; }
            set { this.D = value; }
        }
        public ulong AsU
        {
            get { return this.U; }
            set { this.U = value; }
        }
    }    
}