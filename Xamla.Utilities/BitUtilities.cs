using System;

namespace Xamla.Utilities
{
    public static class BitUtilities
    {
        public static UInt32 NextPowerOfTwo32(UInt32 value)
        {
            // round up to next power of two
            // http://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            ++value;
            return value;
        }

        public static bool IsPowerOfTwo32(UInt32 value)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#DetermineIfPowerOf2 
            return (value & (value - 1)) == 0 && value != 0;
        }

        public static UInt16 SwapEndianness16(UInt16 value)
        {
            int x = (value >> 8) & 0xff;
            int y = (value << 8) & 0xff00;
            return (UInt16)(x | y);
        }

        public static UInt32 SwapEndianness32(UInt32 value)
        {
            return ((UInt32)SwapEndianness16((UInt16)value) << 16) | (UInt32)SwapEndianness16((UInt16)(value >> 16));
        }

        public static UInt64 SwapEndianness64(UInt64 value)
        {
            return ((UInt64)SwapEndianness32((UInt32)value) << 32) | (UInt64)SwapEndianness32((UInt32)(value >> 32));
        }

        public static int SearchLowestBit32(UInt32 value)
        {
            int bit = 0;

            if ((value & 0x0000ffffU) == 0)
            {
                bit += 16;
                value >>= 16;
            }

            if ((value & 0x00ffU) == 0)
            {
                bit += 8;
                value >>= 8;
            }

            if ((value & 0x0fU) == 0)
            {
                bit += 4;
                value >>= 4;
            }

            if ((value & 0x3U) == 0)
            {
                bit += 2;
                value >>= 2;
            }

            if ((value & 0x1U) == 0)
            {
                bit += 1;
                value >>= 1;
            }

            if ((value & 0x1U) == 0)
                return -1;

            return bit;
        }

        public static int SearchLowestBit64(UInt64 value)
        {
            int bit = 0;

            if ((value & 0x00000000ffffffffUL) == 0)
            {
                bit += 32;
                value >>= 32;
            }

            if ((value & 0x0000ffffUL) == 0)
            {
                bit += 16;
                value >>= 16;
            }

            if ((value & 0x00ffUL) == 0)
            {
                bit += 8;
                value >>= 8;
            }

            if ((value & 0x0fUL) == 0)
            {
                bit += 4;
                value >>= 4;
            }

            if ((value & 0x3UL) == 0)
            {
                bit += 2;
                value >>= 2;
            }

            if ((value & 0x1UL) == 0)
            {
                bit += 1;
                value >>= 1;
            }

            if ((value & 0x1UL) == 0)
                return -1;

            return bit;
        }

        public static int SearchHighestBit32(UInt32 value)
        {
            int bit = 31;

            if ((value & 0xffff0000U) == 0)
                bit -= 16;
            else
                value >>= 16;

            if ((value & 0xff00U) == 0)
                bit -= 8;
            else
                value >>= 8;

            if ((value & 0xf0U) == 0)
                bit -= 4;
            else
                value >>= 4;

            if ((value & 0xcU) == 0)
                bit -= 2;
            else
                value >>= 2;

            if ((value & 0x2U) == 0)
                bit -= 1;
            else
                value >>= 1;

            if ((value & 0x1U) == 0)
                return -1;

            return bit;
        }

        public static int SearchHighestBit64(UInt64 value)
        {
            int bit = 63;

            if ((value & 0xffffffff00000000UL) == 0)
                bit -= 32;
            else
                value >>= 32;

            if ((value & 0xffff0000UL) == 0)
                bit -= 16;
            else
                value >>= 16;

            if ((value & 0xff00UL) == 0)
                bit -= 8;
            else
                value >>= 8;

            if ((value & 0xf0UL) == 0)
                bit -= 4;
            else
                value >>= 4;

            if ((value & 0xcUL) == 0)
                bit -= 2;
            else
                value >>= 2;

            if ((value & 0x2UL) == 0)
                bit -= 1;
            else
                value >>= 1;

            if ((value & 0x1UL) == 0)
                return -1;

            return bit;
        }

        public static uint ToGrayCode(uint x)
        {
            return x ^ (x >> 1);
        }

        public static uint FromGrayCode(uint x)
        {
            uint bit = 1u << 31;
            while (bit > 0)
            {
                x ^= ((x & bit) >> 1);
                bit >>= 1;
            }
            return x;
        }

        public static void Write(this System.IO.Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
