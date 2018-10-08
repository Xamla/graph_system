using System;

namespace Xamla.Utilities
{
    public static class HexString
    {
        public static byte[] Parse(string hexString)
        {
            if (hexString == null)
                throw new ArgumentNullException("token");

            if ((hexString.Length % 2) != 0)
                throw new ArgumentException("Input must have even number of characters");

            if (hexString.StartsWith("0x"))
                hexString = hexString.Substring(2);

            byte[] ret = new byte[hexString.Length / 2];
            for (int i = 0; i < ret.Length; i++)
            {
                int high = ParseNibble(hexString[i * 2]);
                int low = ParseNibble(hexString[i * 2 + 1]);
                ret[i] = (byte)((high << 4) | low);
            }

            return ret;
        }

        private static int ParseNibble(char c)
        {
            // http://stackoverflow.com/a/14335076/387870
            unchecked
            {
                uint i = (uint)(c - '0');
                if (i < 10)
                    return (int)i;
                i = ((uint)c & ~0x20u) - 'A';       // folded tests for upper- and lowercase
                if (i < 6)
                    return (int)i + 10;
                throw new ArgumentException("Invalid nibble: " + c);
            }
        }

        public static string ToHexString(this byte[] buffer)
        {
            var newArray = new string[buffer.Length];
            for (int i = 0; i < buffer.Length; ++i)
                newArray[i] = buffer[i].ToString("X2");
            return string.Concat(newArray);
        }
    }
}
