using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public static class MemoryPressure
    {
        const int DELTA_THRESHOLD = 128 * 1024 * 1024;
        const int MIN_SIZE = 85 * 1000;

        static readonly Object lockObj = new Object();

        static long lastTotalMemory;
        static long totalMemory;

        public static int ForceCollectionCount;

        public static void Add(long bytesAllocated)
        {
            if (bytesAllocated < 0)
                throw new ArgumentOutOfRangeException("bytesAllocated");

            if (bytesAllocated < MIN_SIZE)
                return;

            bool shouldCollect = false;

            lock (lockObj)
            {
                totalMemory += bytesAllocated;

                if (totalMemory < lastTotalMemory)
                {
                    lastTotalMemory = totalMemory;
                }
                else if (totalMemory - lastTotalMemory >= DELTA_THRESHOLD)
                {
                    lastTotalMemory = totalMemory;
                    shouldCollect = true;
                    ForceCollectionCount++;
                }
            }

            if (shouldCollect)
            {
                GC.Collect(2);
            }
        }

        public static void Remove(long bytesRemoved)
        {
            Debug.Assert(bytesRemoved >= 0);

            if (bytesRemoved < MIN_SIZE)
                return;

            lock (lockObj)
            {
                totalMemory -= bytesRemoved;
                Debug.Assert(totalMemory >= 0);
            }
        }
    }
}
