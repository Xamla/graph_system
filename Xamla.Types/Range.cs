using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types
{
    //[Serializable]
    public struct Range<T>
        where T : struct
    {
        static readonly Comparer<T> comparer = Comparer<T>.Default;

        public T Low;
        public T High;

        public Range(T low, T high)
        {
            this.Low = low;
            this.High = high;
        }

        public Range<T> Normalize()
        {
            return comparer.Compare(this.Low, this.High) < 0 ? this : new Range<T>(this.High, this.Low);
        }

        public bool Contains(T value)
        {
            return comparer.Compare(this.Low, value) <= 0 && comparer.Compare(this.High, value) >= 0;
        }

        public bool Contains(Range<T> other)
        {
            return comparer.Compare(this.Low, other.Low) <= 0 && comparer.Compare(this.High, other.High) >= 0;
        }

        public bool Overlaps(Range<T> other)
        {
            return !(comparer.Compare(this.High, other.Low) < 0 || comparer.Compare(this.Low, other.High) > 0);
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Low, High);
        }
    }

    public static class Range
    {
        public static readonly Range<double> Unit = new Range<double>(0, 1);

        public static readonly Range<byte> Byte = new Range<byte>(byte.MinValue, byte.MaxValue);
        public static readonly Range<short> Int16 = new Range<short>(short.MinValue, short.MaxValue);
        public static readonly Range<int> Int32 = new Range<int>(int.MinValue, int.MaxValue);
        public static readonly Range<long> Int64 = new Range<long>(long.MinValue, long.MaxValue);
        public static readonly Range<float> Float32 = new Range<float>(float.MinValue, float.MaxValue);
        public static readonly Range<double> Float64 = new Range<double>(double.MinValue, double.MaxValue);

        public static Range<T> Create<T>(T low, T high)
            where T : struct
        {
            return new Range<T>(low, high);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this Range<double> range)
        {
            return range.Low > range.High;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int x, int min, int max)
        {
            if (x < min)
                return min;
            if (x > max)
                return max;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long Clamp(long x, long min, long max)
        {
            if (x < min)
                return min;
            if (x > max)
                return max;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float x, float min, float max)
        {
            if (x < min)
                return min;
            if (x > max)
                return max;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double x, double min, double max)
        {
            if (x < min)
                return min;
            if (x > max)
                return max;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this Range<int> range, int x)
        {
            if (x < range.Low)
                return range.Low;
            if (x > range.High)
                return range.High;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long Clamp(this Range<long> range, long x)
        {
            if (x < range.Low)
                return range.Low;
            if (x > range.High)
                return range.High;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this Range<float> range, float x)
        {
            if (x < range.Low)
                return range.Low;
            if (x > range.High)
                return range.High;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this Range<double> range, double x)
        {
            if (x < range.Low)
                return range.Low;
            if (x > range.High)
                return range.High;
            return x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Saturate(float x)
        {
            return Clamp(x, 0, 1);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Saturate(double x)
        {
            return Clamp(x, 0, 1);
        }
    }
}
