using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types
{
    public class V<T>
        : V
        , IEnumerable<T>
    {
        internal A<T> v;

        public V(int rows)
        {
            this.v = new A<T>(rows);
        }

        public V(A<T> array)
        {
            if (array.Rank != 1)
                throw new ArgumentException("Source in not a 1D array.", "array");
            this.v = array;
        }

        public V(T[] source)
        {
            this.v = A.FromArray(source);
        }

        public V(IEnumerable<T> source)
            : this(source.ToArray())
        {
        }

        public int Rows
        {
            get { return v.Count; }
        }

        public T this[int index]
        {
            get { return v.Buffer[index]; }
            set { v.Buffer[index] = value; }
        }

        public M<T> Transpose()
        {
            return new M<T>(new A<T>(v.Buffer, 1, v.Dimension[0]));
        }

        public M<T> ToMatrix()
        {
            return new M<T>(new A<T>(v.Buffer, v.Dimension[0], 1));
        }

        public new A<T> UnderlyingArray
        {
            get { return v; }
        }

        protected override A GetUnderlyingArray()
        {
            return v;
        }

        public override bool Equals(object obj)
        {
            var other = obj as V<T>;
            return other != null && other.v.Equals(v);
        }

        public override int GetHashCode()
        {
            return v.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var x in v.Buffer)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", x);
            }

            return sb.ToString();
        }

        public static implicit operator A<T>(V<T> v)
        {
            return v.UnderlyingArray;
        }

        public int IndexOf(T element)
        {
            return Array.IndexOf(v.Buffer, element);
        }

        public int LastIndexOf(T element)
        {
            return Array.LastIndexOf(v.Buffer, element);
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return this.UnderlyingArray.GetEnumerator();
        }
    }

    public abstract class V
        : IEnumerable
    {
        public static V<T> Generate<T>(Func<int, T> generator, int rows)
        {
            return A.Generate(generator, rows).ToV();
        }

        protected abstract A GetUnderlyingArray();

        public A UnderlyingArray
        {
            get { return GetUnderlyingArray(); }
        }

        public V Convert(Type destinationElementType)
        {
            return Convert(this, destinationElementType);
        }

        public static V Convert(V source, Type destinationELementType)
        {
            var destinationArray = source.UnderlyingArray.Convert(destinationELementType);
            return FromArray(destinationArray);
        }

        public static V FromArray(A array)
        {
            var vectorType = typeof(V<>).MakeGenericType(array.ElementType);
            return (V)Activator.CreateInstance(vectorType, array);
        }

        public static V FromArray(Array array)
        {
            return FromArray(A.FromArray(array));
        }

        public static V<T> FromArray<T>(T[] array)
        {
            return new V<T>(A.FromArray(array));
        }

        public static V<T> Create<T>(params T[] values)
        {
            return new V<T>(values);
        }

        public static V<double> Zeros(int rows)
        {
            return new V<double>(rows);
        }

        public static V<double> Ones(int rows)
        {
            return A.Fill<double>(1, rows).ToV();
        }

        public IEnumerator GetEnumerator()
        {
            return this.UnderlyingArray.GetEnumerator();
        }
    }

    public static class VExtenstions
    {
        public static V<T> ToV<T>(this A<T> array)
        {
            return new V<T>(array.Buffer);
        }

        static V<double> Multiply(this V<double> v, double scalar)
        {
            return v.v.Multiply(scalar).ToV();
        }

        static V<double> Add(this V<double> a, V<double> b)
        {
            return a.v.Add(b.v).ToV();
        }

        static V<double> Subtract(this V<double> a, V<double> b)
        {
            return a.v.Subtract(b.v).ToV();
        }

        static V<double> Negate(this V<double> v)
        {
            return v.UnderlyingArray.Negate().ToV();
        }

        public static double Dot(this V<double> a, V<double> b)
        {
            return a.UnderlyingArray.Dot(b.UnderlyingArray);
        }

        public static double Norm(this V<double> a, double p = 2.0)
        {
            return a.UnderlyingArray.Norm(p);
        }

        public static double SumOfSquares(this V<double> a)
        {
            double sum = 0;
            foreach (var x in a.UnderlyingArray.Buffer)
                sum += x * x;
            return sum;
        }

        public static double Length(this V<double> a)
        {
            return Math.Sqrt(a.SumOfSquares());
        }

        public static double Distance(this V<double> a, V<double> b)
        {
            return a.Subtract(b).Length();
        }

        public static V<double> Normalize(this V<double> v)
        {
            return v.Multiply(1.0 / v.Length());
        }

        public static V<double> ToVector(this Float2 p)
        {
            return new V<double>(new double[] { p.X, p.Y });
        }

        public static V<double> ToVector(this Float3 p)
        {
            return new V<double>(new double[] { p.X, p.Y, p.Z });
        }

        public static V<int> ToVector(this Int2 p)
        {
            return new V<int>(new int[] { p.X, p.Y });
        }

        public static V<int> ToVector(this Int3 p)
        {
            return new V<int>(new int[] { p.X, p.Y, p.Z });
        }

        public static V<T> Cycle<T>(this V<T> v, int offset)
        {
            return V.Generate(i => v[(i + offset) % v.Rows], v.Rows);
        }
    }
}
