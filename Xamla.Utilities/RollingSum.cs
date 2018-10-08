using System;

namespace Xamla.Utilities
{
    public class RollingSum
    {
        const int ROLLSUM_CHAR_OFFSET = 31;
        const int DEFAULT_WINDOW_BITS = 6;
        const int DEFAULT_BLOBBITS = 18;

        byte[] buffer;
        int s1, s2;
        int offset;
        int windowSize;
        int blobBits;
        int blobSize;

        public RollingSum(int windowBits = DEFAULT_WINDOW_BITS, int blobBits = DEFAULT_BLOBBITS)
        {
            if (windowBits <= 0)
                throw new ArgumentException("windowBits must be positive", "windowBits");

            if (blobBits <= 0)
                throw new ArgumentException("blobBits must be positive", "blobBits");

            this.blobBits = blobBits;
            this.blobSize = (1 << blobBits);
            this.windowSize = (1 << windowBits);
            this.buffer = new byte[windowSize];
            
            Reset();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void Reset()
        {
            unchecked
            {
                this.s1 = windowSize * ROLLSUM_CHAR_OFFSET;
                this.s2 = windowSize * (windowSize - 1) * ROLLSUM_CHAR_OFFSET;
            }

            this.offset = 0;
            Array.Clear(buffer, 0, buffer.Length);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        bool Add(byte add)
        {
            unchecked
            {
                int remove = buffer[offset];
                buffer[offset] = add;
                offset = (offset + 1) % windowSize;
                s1 += add - remove;
                s2 += s1 - (windowSize * remove * ROLLSUM_CHAR_OFFSET);
                return (s2 & (blobSize - 1)) == blobSize - 1;
            }
        }

        public int Digest
        {
            get { return unchecked((s1 << 16) | (s2 & 0xffff)); }
        }

        public int FindNextSplitOffset(byte[] buffer, int offset, int count)
        {
            int end = offset + count;
            for (int i = offset; i < end; ++i)
            {
                if (Add(buffer[i]))     // check if we are on a split point
                {
                    Reset();
                    return i + 1;
                }
            }

            return -1;
        }

        public int FindNextSplitOffset(byte[] buffer, int offset, int count, out int bitCount)
        {
            int end = offset + count;
            for (int i = offset; i < end; ++i)
            {
                if (Add(buffer[i]))     // check if we are on a split point
                {
                    int sum = this.Digest >> blobBits;
                    bitCount = 0; ;
                    while (sum != 0)
                    {
                        ++bitCount;
                        sum >>= 1;
                    }
                    Reset();
                    return i + 1;
                }
            }

            bitCount = 0;
            return -1;
        }
    }
}
