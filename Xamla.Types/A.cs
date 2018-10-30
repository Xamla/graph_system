using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamla.Utilities;
using System.Runtime.Serialization;
using System.Collections;
using System.Reflection;
using System.Numerics;

namespace Xamla.Types
{
    //[Serializable]
    public class A<T>
        : A
        , IEnumerable<T>
    //    , ISerializable
    {
        public static readonly A<T> Null = default(A<T>);

        public new T[] Buffer { get; private set; }

        public A(params int[] dimension)
        {
            this.Dimension = dimension;
            this.Stride = A.BuildStride(dimension);

            int length = dimension.Length > 0 ? this.Stride[0] * dimension[0] : 0;
            this.Buffer = new T[length];
            this.count = length;

            this.memoryPressure = this.SizeInBytes;
            MemoryPressure.Add(this.memoryPressure);
        }

        public A(T[] buffer, params int[] dimension)
            : this(buffer, dimension, A.BuildStride(dimension))
        {
        }

        public A(T[] buffer, int[] dimension, int[] stride)
        {
            this.Buffer = buffer;
            this.Dimension = dimension;
            this.Stride = stride;
            this.count = buffer.Length;
        }

        //private A(SerializationInfo info, StreamingContext context)
        //{
        //    this.Buffer = (T[])info.GetValue("buffer", typeof(T[]));
        //    this.Dimension = (int[])info.GetValue("dimension", typeof(int[]));
        //    this.Stride = A.BuildStride(this.Dimension);
        //    this.count = this.Buffer.Length;
        //}

        ~A()
        {
            MemoryPressure.Remove(this.memoryPressure);
        }

        protected override Array GetArrayBuffer()
        {
            return Buffer;
        }

        public A<TOut> Convert<TOut>()
        {
            return (A<TOut>)base.Convert(typeof(TOut));
        }

        public T this[int index]
        {
            get { return Buffer[index]; }
            set { Buffer[index] = value; }
        }

        public T this[int index0, int index1]
        {
            get { return Buffer[index0 * Stride[0] + index1]; }
            set { Buffer[index0 * Stride[0] + index1] = value; }
        }

        public T this[Int2 index]
        {
            get { return this[index.Y, index.X]; }
            set { this[index.Y, index.X] = value; }
        }

        public T this[int index0, int index1, int index2]
        {
            get { return Buffer[index0 * Stride[0] + index1 * Stride[1] + index2]; }
            set { Buffer[index0 * Stride[0] + index1 * Stride[1] + index2] = value; }
        }

        public T this[Int3 index]
        {
            get { return this[index.Z, index.Y, index.X]; }
            set { this[index.Z, index.Y, index.X] = value; }
        }

        public T this[params int[] indicies]
        {
            get { return Buffer[IndiciesToOffset(indicies)]; }
            set { Buffer[IndiciesToOffset(indicies)] = value; }
        }

        /// <summary>
        /// Creates an array with all elements of the current instance but with all dimensions of size 1 removed.
        /// Only for single element arrays a remaining dimension of size 1 is kept.
        /// </summary>
        /// <returns>The simplified array.</returns>
        public new A<T> Simplify()
        {
            return (A<T>)base.Simplify();
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Buffer).GetEnumerator();
        }

        public IEnumerable<T> GetSliceValues(int[] start, int[] dimension)
        {
            if (start.Length != dimension.Length)
                throw new Exception("Start index array and dimension array have different lengths.");

            var remaining = dimension.ToArray();
            var current = new int[start.Length];
            int offset = 0;
            for (int i = 0; i < current.Length; ++i)
            {
                offset += Stride[i] * start[i];
                current[i] = offset;
            }

            int last = start.Length - 1;
            for (;;)
            {
                if (remaining[last] > 0)
                    yield return Buffer[offset++];

                int i = last;
                for (; i >= 0; --i)
                {
                    if (--remaining[i] > 0)
                    {
                        current[i] += Stride[i];
                        break;
                    }

                    remaining[i] = dimension[i];
                }

                if (i == last)
                    continue;

                if (i < 0)
                    yield break;

                offset = current[i++];
                for (; i < start.Length; ++i)
                {
                    offset += start[i] * Stride[i];
                    current[i] = offset;
                }
            }
        }

        public void CopyTo(int[] start, int[] dimension, T[] destination)
        {
            if (start.Length != dimension.Length)
                throw new Exception("Start index array and dimension array have different lengths.");

            // check if a linear memory segment is addressed
            if (start.Length == this.Rank)
            {
                var end = new int[this.Rank];
                int i = dimension.Length - 1;
                for (; i > 0; --i)
                {
                    if (start[i] != 0 || this.Dimension[i] != dimension[i])
                        break;
                }

                end[i] = start[i] + dimension[i];
                --i;        // a single dimension that is partially copied is allowed

                for (; i >= 0; --i)
                {
                    if (dimension[i] != 1)
                        break;
                    end[i] = start[i];
                }

                if (i < 0)      // linear copy possible
                {
                    int startOffest = IndiciesToOffset(start);
                    int endOffest = IndiciesToOffset(end);

                    if (typeof(T).GetTypeInfo().IsPrimitive)
                    {
                        int elementSizeInBytes = Marshal.SizeOf<T>();
                        System.Buffer.BlockCopy(this.Buffer, startOffest * elementSizeInBytes, destination, 0, (endOffest - startOffest) * elementSizeInBytes);
                    }
                    else
                    {
                        Array.Copy(this.Buffer, startOffest, destination, 0, endOffest - startOffest);
                    }

                    return;
                }
            }

            var remaining = dimension.ToArray();
            var current = new int[start.Length];
            int offset = 0;
            for (int i = 0; i < current.Length; ++i)
            {
                offset += Stride[i] * start[i];
                current[i] = offset;
            }

            int j = 0;
            for (;;)
            {
                if (remaining[start.Length - 1] > 0)
                    destination[j++] = Buffer[offset++];

                int i = start.Length - 1;
                for (; i >= 0; --i)
                {
                    if (--remaining[i] > 0)
                    {
                        current[i] += Stride[i];
                        break;
                    }

                    remaining[i] = dimension[i];
                }

                if (i == start.Length - 1)
                    continue;

                if (i < 0)
                    return;

                offset = current[i++];
                for (; i < start.Length; ++i)
                {
                    offset += start[i] * Stride[i];
                    current[i] = offset;
                }
            }
        }

        public A<T> Slice(int[] start, int[] dimension)
        {
            if (start.Length != dimension.Length)
                throw new Exception("Start index array and dimension array have different lengths.");

            var slice = new A<T>(dimension);
            CopyTo(start, dimension, slice.Buffer);
            return slice;
        }

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("buffer", this.Buffer, typeof(T[]));
        //    info.AddValue("dimension", this.Dimension, typeof(int[]));
        //}

        public static explicit operator T[](A<T> a)
        {
            return a.Buffer;
        }

        public A<T> Clone()
        {
            if (typeof(T).GetTypeInfo().IsPrimitive)
            {
                var b = new T[Buffer.Length];
                System.Buffer.BlockCopy(Buffer, 0, b, 0, System.Buffer.ByteLength(Buffer));
                return new A<T>(b, this.Dimension, this.Stride);
            }
            else
            {
                return new A<T>((T[])this.Buffer.Clone(), this.Dimension, this.Stride);
            }
        }

        public new int SizeInBytes
        {
            get { return this.Count * Marshal.SizeOf<T>(); }
        }

        public byte[] ToBytes()
        {
            byte[] buffer = new byte[this.SizeInBytes];
            using (var p = this.Pin())
            {
                Marshal.Copy(p.Pointer, buffer, 0, this.Count * Marshal.SizeOf<T>());
            }
            return buffer;
        }
    }

    public abstract class A
        : System.Collections.IEnumerable
    {
        protected int memoryPressure;
        protected int count;

        public int[] Dimension { get; protected set; }
        public int[] Stride { get; protected set; }

        protected abstract Array GetArrayBuffer();

        public Type ElementType
        {
            get { return this.GetArrayBuffer().GetType().GetElementType(); }
        }

        public Array Buffer
        {
            get { return GetArrayBuffer(); }
        }

        public int Rank
        {
            get { return Dimension.Length; }
        }

        public int Count
        {
            get { return count; }
        }

        public A Simplify()
        {
            if (Buffer == null || this.Count == 0)
                return A.Create(this.ElementType, 0);

            var d = Dimension.Where(x => x > 1).DefaultIfEmpty(1).ToArray();
            var s = A.BuildStride(d);
            Debug.Assert((d.Length > 0 ? s[0] * d[0] : 0) >= Buffer.Length);
            return A.FromArray(Buffer, d, s);
        }

        public int IndiciesToOffset(params int[] indicies)
        {
            int offset = 0;
            for (int i = 0; i < indicies.Length; ++i)
                offset += indicies[i] * Stride[i];
            return offset;
        }

        public int[] OffsetToIndicies(int offset)
        {
            var indicies = new int[this.Rank];
            for (int i = 0; i < indicies.Length; ++i)
            {
                indicies[i] = offset / Stride[i];
                offset -= indicies[i] * Stride[i];
            }
            return indicies;
        }

        public object GetValue(int index)
        {
            return this.GetArrayBuffer().GetValue(index);
        }

        public void SetValue(object value, int index)
        {
            this.GetArrayBuffer().SetValue(value, index);
        }

        public object GetValue(int index0, int index1)
        {
            return this.GetArrayBuffer().GetValue(index0 * Stride[0] + index1);
        }

        public void SetValue(object value, int index0, int index1)
        {
            this.GetArrayBuffer().SetValue(value, index0 * Stride[0] + index1);
        }

        public object GetValue(int index0, int index1, int index2)
        {
            return this.GetArrayBuffer().GetValue(index0 * Stride[0] + index1 * Stride[1] + index2);
        }

        public void SetValue(object value, int index0, int index1, int index2)
        {
            this.GetArrayBuffer().SetValue(value, index0 * Stride[0] + index1 * Stride[1] + index2);
        }

        public object GetValue(params int[] indicies)
        {
            return this.GetArrayBuffer().GetValue(IndiciesToOffset(indicies));
        }

        public void SetValue(object value, params int[] indicies)
        {
            this.GetArrayBuffer().SetValue(value, IndiciesToOffset(indicies));
        }

        public PinnedGCHandle Pin()
        {
            return new PinnedGCHandle(this.GetArrayBuffer());
        }

        public int SizeInBytes
        {
            get { return this.Count * Marshal.SizeOf(this.ElementType); }
        }

        public bool Equals(A other)
        {
            Array a = this.GetArrayBuffer(), b = other.GetArrayBuffer();
            if (a.Length != b.Length)
                return false;

            return ((IStructuralEquatable)a).Equals(b, StructuralComparisons.StructuralEqualityComparer);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is A))
                return false;
            return Equals((A)obj);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.GetArrayBuffer()).CombineHashCode(HashHelper.GetHashCode(this.Dimension));
        }

        public static A Create(Type elementType, params int[] dimension)
        {
            var arrayType = typeof(A<>).MakeGenericType(elementType);
            return (A)Activator.CreateInstance(arrayType, dimension);
        }

        public static A Convert(A source, Type destinationElementType)
        {
            var sourceBuffer = source.GetArrayBuffer();
            var sourceElementType = sourceBuffer.GetType().GetElementType();
            var converter = ArrayExtensions.GetPrimitiveConverter(sourceElementType, destinationElementType);
            var destinationBuffer = converter(sourceBuffer);
            return A.FromArray(destinationBuffer, source.Dimension, source.Stride);
        }

        public A Convert(Type destinationElementType)
        {
            return Convert(this, destinationElementType);
        }

        public static A<T> Generate<T>(Func<T> generator, params int[] dimension)
        {
            var a = new A<T>(dimension);
            for (int i = 0; i < a.Buffer.Length; ++i)
                a.Buffer[i] = generator();
            return a;
        }

        public static A<T> Generate<T>(Func<int, T> generator, int length)
        {
            var a = new A<T>(length);
            for (int i = 0; i < a.Buffer.Length; ++i)
                a.Buffer[i] = generator(i);
            return a;
        }

        public static A<T> Generate<T>(Func<int[], T> generator, params int[] dimension)
        {
            var a = new A<T>(dimension);
            int i = 0;
            foreach (var indicies in StepIndices(new int[a.Rank], dimension))
                a.Buffer[i++] = generator(indicies);
            return a;
        }

        public static A<T> Fill<T>(T value, params int[] dimension)
        {
            var a = new A<T>(dimension);
            for (int i = 0; i < a.Buffer.Length; ++i)
                a.Buffer[i] = value;
            return a;
        }

        public static A<T> FromArray<T>(T[] buffer)
        {
            return new A<T>(buffer, buffer.Length);
        }

        public static A FromArray(Array source)
        {
            var d = new int[source.Rank];
            for (int i = 0; i < source.Rank; i++)
                d[i] = source.GetLength(i);
            return FromArray(source, d);
        }

        public static A FromArray(Array buffer, params int[] dimension)
        {
            var elementType = buffer.GetType().GetElementType();
            var arrayType = typeof(A<>).MakeGenericType(elementType);
            return (A)Activator.CreateInstance(arrayType, buffer, dimension);
        }

        public static A FromArray(Array buffer, int[] dimension, int[] stride)
        {
            var elementType = buffer.GetType().GetElementType();
            var arrayType = typeof(A<>).MakeGenericType(elementType);
            return (A)Activator.CreateInstance(arrayType, buffer, dimension, stride);
        }

        public static A<float> FromArray(float[,] source)
        {
            var destination = new A<float>(source.GetLength(0), source.GetLength(1));
            using (var handle = new PinnedGCHandle(source))
            {
                Marshal.Copy(handle.Pointer, destination.Buffer, 0, destination.Buffer.Length);
            }
            return destination;
        }

        public static A<double> FromArray(double[,] source)
        {
            var destination = new A<double>(source.GetLength(0), source.GetLength(1));
            using (var handle = new PinnedGCHandle(source))
            {
                Marshal.Copy(handle.Pointer, destination.Buffer, 0, destination.Buffer.Length);
            }
            return destination;
        }

        public static A<float> FromArray<T>(float[, ,] source)
        {
            var destination = new A<float>(source.GetLength(0), source.GetLength(1), source.GetLength(2));
            var s = GCHandle.Alloc(source, GCHandleType.Pinned);
            using (var handle = new PinnedGCHandle(source))
            {
                Marshal.Copy(handle.Pointer, destination.Buffer, 0, destination.Buffer.Length);
            }
            return destination;
        }

        public static A<double> FromArray<T>(double[, ,] source)
        {
            var destination = new A<double>(source.GetLength(0), source.GetLength(1), source.GetLength(2));
            var s = GCHandle.Alloc(source, GCHandleType.Pinned);
            using (var handle = new PinnedGCHandle(source))
            {
                Marshal.Copy(handle.Pointer, destination.Buffer, 0, destination.Buffer.Length);
            }
            return destination;
        }

        public static A<T> FromArray<T>(T[] source, params int[] dimension)
        {
            var stride = BuildStride(dimension);
            return new A<T>(source, dimension, stride);
        }

        public static IEnumerable<int[]> StepIndices(int[] start, int[] dimension)
        {
            var current = start.ToArray();
            var remaining = dimension.ToArray();

            int last = start.Length - 1;
            for (; ; )
            {
                if (remaining[last] > 0)
                    yield return current;

                for (int i = last; i >= 0; --i)
                {
                    if (--remaining[i] > 0)
                    {
                        ++current[i];
                        break;
                    }

                    if (i == 0)
                        yield break;

                    current[i] = start[i];
                    remaining[i] = dimension[i];
                }
            }
        }

        internal static int[] BuildStride(int[] dimension)
        {
            var stride = new int[dimension.Length];
            for (int l = 1, i = dimension.Length - 1; i >= 0; l *= dimension[i--])
            {
                if (dimension[i] < 0)
                    throw new RankException("The size of a matrix dimension must be positive.");
                stride[i] = l;
            }
            return stride;
        }

        internal static int LengthFromDimension(int[] dimension)
        {
            if (dimension.Length == 0)
                return 0;

            int l = 1;
            foreach (var d in dimension)
                l *= d;
            return l;
        }

        public static A<T> Zero<T>(params int[] dimension)
        {
            return new A<T>(dimension);
        }

        public static A<T> Const<T>(T value, params int[] dimension)
        {
            return A.Generate<T>(x => value, dimension);
        }

        public static A<T> One<T>(params int[] dimension)
            where T : struct
        {
            dynamic x = 1;
            return A.Const<T>((T)x, dimension);
        }

        static System.Reflection.MethodInfo fromBytesMethod = typeof(A).GetTypeInfo().GetMethod("FromBytes", new Type[] { typeof(byte[]), typeof(int[]) });

        public static object FromBytes(Type elementType, byte[] source, int[] dimension)
        {
            return fromBytesMethod.MakeGenericMethod(elementType).Invoke(null, new object[] { source, dimension });
        }

        public static A<T> FromBytes<T>(byte[] source, int[] dimension)
        {
            var buffer = new A<T>(dimension);
            using (var p = buffer.Pin())
            {
                Marshal.Copy(source, 0, p.Pointer, buffer.Count * Marshal.SizeOf<T>());
            }

            return buffer;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.GetArrayBuffer().GetEnumerator();
        }
    }

    public static class AExtensions
    {
        static float Saturate(float x)
        {
            if (x < 0)
                return 0;
            if (x > 1)
                return 1;
            return x;
        }

        public static A<T> ToA<T>(this IEnumerable<T> source, params int[] dimension)
        {
            return new A<T>(source.Take(A.LengthFromDimension(dimension)).ToArray(), dimension);
        }

        public static A<TResult> Map<TSource, TResult>(this A<TSource> source, Func<TSource, TResult> selector)
        {
            var r = new A<TResult>(source.Dimension);
            TSource[] xs = source.Buffer;
            TResult[] ys = r.Buffer;
            for (int i = 0; i < xs.Length; ++i)
                ys[i] = selector(xs[i]);
            return r;
        }

        public static A<TResult> Zip<T1, T2, TResult>(this A<T1> a, A<T2> b, Func<T1, T2, TResult> combine)
        {
            if (a.Count != b.Count)
                throw new ArgumentException("Element count of operands does not match.");

            var result = new A<TResult>(a.Dimension);
            var xs = a.Buffer;
            var ys = b.Buffer;
            var zs = result.Buffer;

            for (int i = 0; i < zs.Length; ++i)
                zs[i] = combine(xs[i], ys[i]);

            return result;
        }


        public static A<float> Saturate(this A<float> a)
        {
            return a.Map(Saturate);
        }

        public static A<float> UniformFloat(this Random rng, params int[] dimension)
        {
            return rng.FloatSequence().ToA(dimension);
        }

        public static A<double> UniformDouble(this Random rng, params int[] dimension)
        {
            return rng.DoubleSequence().ToA(dimension);
        }

        // Binary Ops

        public static A<float> Add(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => x + y);
        }

        public static A<double> Add(this A<double> a, A<double> b)
        {
            return Zip(a, b, (x, y) => x + y);
        }

        public static A<float> Subtract(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => x - y);
        }

        public static A<double> Subtract(this A<double> a, A<double> b)
        {
            return Zip(a, b, (x, y) => x - y);
        }

        public static float Dot(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => x * y).Sum();
        }

        public static double Dot(this A<double> a, A<double> b)
        {
            return Zip(a, b, (x, y) => x * y).Sum();
        }

        public static float LNorm(this A<float> a, float l)
        {
            return (l == 1) ? a.Sum() : (float)Math.Pow(a.Sum(x => Math.Pow(x, l)), 1 / l);
        }

        public static double LNorm(this A<double> a, double l)
        {
            return (l == 1) ? a.Sum() : Math.Pow(a.Sum(x => Math.Pow(x, l)), 1 / l);
        }

        public static A<float> Multiply(this A<float> a, float scalar)
        {
            return a.Map(x => x * scalar);
        }

        public static A<double> Multiply(this A<double> a, double scalar)
        {
            return a.Map(x => x * scalar);
        }

        public static A<float> Multiply(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => x * y);
        }

        public static A<double> Multiply(this A<double> a, A<double> b)
        {
            return Zip(a, b, (x, y) => x * y);
        }

        public static A<float> Divide(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => x / y);
        }

        public static A<double> Divide(this A<double> a, A<double> b)
        {
            return Zip(a, b, (x, y) => x / y);
        }

        public static A<float> Min(this A<float> a, A<float> b)
        {
            return Zip(a, b, Math.Min);
        }

        public static A<double> Min(this A<double> a, A<double> b)
        {
            return Zip(a, b, Math.Min);
        }

        public static A<float> Max(this A<float> a, A<float> b)
        {
            return Zip(a, b, Math.Max);
        }

        public static A<double> Max(this A<double> a, A<double> b)
        {
            return Zip(a, b, Math.Max);
        }

        public static A<float> Pow(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => (float)Math.Pow(x, y));
        }

        public static A<double> Pow(this A<double> a, A<double> b)
        {
            return Zip(a, b, Math.Pow);
        }

        public static A<float> Log(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => (float)Math.Log(x, y));
        }

        public static A<double> Log(this A<double> a, A<double> b)
        {
            return Zip(a, b, Math.Log);
        }

        public static A<float> Atan2(this A<float> a, A<float> b)
        {
            return Zip(a, b, (x, y) => (float)Math.Atan2(x, y));
        }

        public static A<double> Atan2(this A<double> a, A<double> b)
        {
            return Zip(a, b, Math.Atan2);
        }

        public static A<double> Saturate(this A<double> a)
        {
            return a.Map(Saturate);
        }

        static double Saturate(double x)
        {
            if (x < 0)
                return 0;
            if (x > 1)
                return 1;
            return x;
        }

        public static A<float> Abs(this A<float> a)
        {
            return a.Map(Math.Abs);
        }

        public static A<double> Abs(this A<double> a)
        {
            return a.Map(Math.Abs);
        }

        public static A<float> Acos(this A<float> a)
        {
            return a.Map(x => (float)Math.Acos(x));
        }

        public static A<double> Acos(this A<double> a)
        {
            return a.Map(Math.Acos);
        }

        public static A<float> Asin(this A<float> a)
        {
            return a.Map(x => (float)Math.Asin(x));
        }

        public static A<double> Asin(this A<double> a)
        {
            return a.Map(Math.Asin);
        }

        public static A<float> Atan(this A<float> a)
        {
            return a.Map(x => (float)Math.Atan(x));
        }

        public static A<double> Atan(this A<double> a)
        {
            return a.Map(Math.Atan);
        }

        public static A<float> Ceiling(this A<float> a)
        {
            return a.Map(x => (float)Math.Ceiling(x));
        }

        public static A<double> Ceiling(this A<double> a)
        {
            return a.Map(Math.Ceiling);
        }

        public static A<float> Cos(this A<float> a)
        {
            return a.Map(x => (float)Math.Cos(x));
        }

        public static A<double> Cos(this A<double> a)
        {
            return a.Map(Math.Cos);
        }

        public static A<float> Cosh(this A<float> a)
        {
            return a.Map(x => (float)Math.Cosh(x));
        }

        public static A<double> Cosh(this A<double> a)
        {
            return a.Map(Math.Cosh);
        }

        public static A<float> Floor(this A<float> a)
        {
            return a.Map(x => (float)Math.Floor(x));
        }

        public static A<double> Floor(this A<double> a)
        {
            return a.Map(Math.Floor);
        }

        public static A<float> Exp(this A<float> a)
        {
            return a.Map(x => (float)Math.Exp(x));
        }

        public static A<double> Exp(this A<double> a)
        {
            return a.Map(Math.Exp);
        }

        public static A<float> Log_e(this A<float> a)
        {
            return a.Map(x => (float)Math.Log(x));
        }

        public static A<double> Log_e(this A<double> a)
        {
            return a.Map(Math.Log);
        }

        public static A<float> Log_10(this A<float> a)
        {
            return a.Map(x => (float)Math.Log10(x));
        }

        public static A<double> Log_10(this A<double> a)
        {
            return a.Map(Math.Log10);
        }

        public static A<float> Round(this A<float> a)
        {
            return a.Map(x => (float)Math.Round(x));
        }

        public static A<double> Round(this A<double> a)
        {
            return a.Map(Math.Round);
        }

        public static A<float> Sign(this A<float> a)
        {
            return a.Map(x => (float)Math.Sign(x));
        }

        public static A<double> Sign(this A<double> a)
        {
            return a.Map(x => (double)Math.Sign(x));
        }

        public static A<double> Sinh(this A<double> a)
        {
            return a.Map(Math.Sinh);
        }

        public static A<float> Sqrt(this A<float> a)
        {
            return a.Map(x => (float)Math.Sqrt(x));
        }

        public static A<double> Sqrt(this A<double> a)
        {
            return a.Map(Math.Sqrt);
        }

        public static A<float> Tan(this A<float> a)
        {
            return a.Map(x => (float)Math.Tan(x));
        }

        public static A<double> Tan(this A<double> a)
        {
            return a.Map(Math.Tan);
        }

        public static A<float> Tanh(this A<float> a)
        {
            return a.Map(x => (float)Math.Tanh(x));
        }

        public static A<double> Tanh(this A<double> a)
        {
            return a.Map(Math.Tanh);
        }

        public static A<float> Square(this A<float> a)
        {
            return a.Map(x => (float)Math.Pow(x, 2.0));
        }

        public static A<double> Square(this A<double> a)
        {
            return a.Map(x => Math.Pow(x, 2.0));
        }

        public static A<float> Reciprocal(this A<float> a)
        {
            return a.Map(x => 1.0f / x);
        }

        public static A<double> Reciprocal(this A<double> a)
        {
            return a.Map(x => 1.0 / x);
        }

        public static A<float> Negate(this A<float> a)
        {
            return a.Map(x => -x);
        }

        public static A<double> Negate(this A<double> a)
        {
            return a.Map(x => -x);
        }

        public static float[] ToRowMajorArray(this Matrix4x4 m) =>
            new float[] {
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            };

        public static float[] ToClumnMajorArray(this Matrix4x4 m) =>
            new float[] {
                m.M11, m.M21, m.M31, m.M41,
                m.M12, m.M22, m.M32, m.M42,
                m.M13, m.M23, m.M33, m.M43,
                m.M14, m.M24, m.M34, m.M44
            };

        public static A<float> ToA(this Matrix4x4 m) =>
            new A<float>(m.ToRowMajorArray(), 4, 4);

        public static M<float> ToM(this Matrix4x4 m) =>
            new M<float>(m.ToA());
    }
}
