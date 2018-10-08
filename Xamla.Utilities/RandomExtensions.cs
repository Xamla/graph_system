using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public static class RandomExtensions
    {
        public static IEnumerable<int> Int32Sequence(this Random rng)
        {
            for (;;)
            {
                yield return rng.Next();
            }
        }

        public static IEnumerable<int> Int32Sequence(this Random rng, int maxValue)
        {
            for (;;)
            {
                yield return rng.Next(maxValue);
            }
        }

        public static IEnumerable<int> SignedInt32Sequence(this Random rng)
        {
            var bits = new byte[4];
            for (;;)
            {
                rng.NextBytes(bits);
                yield return BitConverter.ToInt32(bits, 0);
            }
        }

        public static IEnumerable<int> SignedInt32Sequence(this Random rng, int min, int max)
        {
            if (min > max)
                throw new ArgumentException();

            long r = max - min;
            return rng.SignedInt64Sequence().Select(x =>
            {
                if (x < 0)
                    x = -x;
                return (int)((x % r) + min);
            });
        }

        public static IEnumerable<long> SignedInt64Sequence(this Random rng)
        {
            var bits = new byte[8];
            for (;;)
            {
                rng.NextBytes(bits);
                yield return BitConverter.ToInt64(bits, 0);
            }
        }

        public static IEnumerable<float> FloatSequence(this Random rng)
        {
            for (; ; )
            {
                yield return (float)rng.NextDouble();
            }
        }

        public static IEnumerable<double> DoubleSequence(this Random rng)
        {
            for (;;)
            {
                yield return rng.NextDouble();
            }
        }

        public static IEnumerable<double> Normal(this Random rng, double mean, double sigma)
        {
            return Normal(rng).Select(x => x * sigma + mean);
        }

        public static IEnumerable<double> Normal(this Random rng)
        {
            // polar method, see: http://en.wikipedia.org/wiki/Marsaglia_polar_method
            double u1, u2, q;
            for (;;)
            {
                do
                {
                    u1 = rng.NextDouble() * 2 - 1;
                    u2 = rng.NextDouble() * 2 - 1;
                    q = u1 * u1 + u2 * u2;
                } while (q > 1);

                var p = Math.Sqrt(-2 * Math.Log(q) / q);

                yield return u1 * p;
                yield return u2 * p;
            }
        }

        public static string AlphaString(this Random rng, int size)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return RandomString(rng, size, chars);
        }

        public static string AlphaNumericString(this Random rng, int size)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return RandomString(rng, size, chars);
        }

        private static string RandomString(Random rng, int size, string chars)
        {
            char[] buffer = new char[size];
            for (int i = 0; i < size; i++)
                buffer[i] = chars[rng.Next(chars.Length)];
        
            return new string(buffer);
        }
    }
}
