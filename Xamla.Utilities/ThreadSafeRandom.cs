using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    // The Random class may use the same seed value for multiple instances that are created nearly simultaneously 
    // on different threads (the default seed is time based). To ensure different random sequences we use a thread
    // local Random instance that is initialized from a global seed generator.
    public static class ThreadSafeRandom
    {
        static Random seedGenerator = new Random();

        [ThreadStatic]
        static Random threadLocalDefaultRng;

        public static Random Generator
        {
            get
            {
                if (threadLocalDefaultRng == null)
                {
                    int seed;
                    lock (seedGenerator)
                    {
                        seed = seedGenerator.Next();
                    }

                    threadLocalDefaultRng = new Random(seed);
                }

                return threadLocalDefaultRng;
            }
        }
    }
}
