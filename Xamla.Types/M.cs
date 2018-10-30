using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types
{
    [DebuggerTypeProxy(typeof(M<>.DebugView))]
    public class M<T>
        : M
    {
        internal A<T> m;

        public M(int rows, int columns)
        {
            this.m = new A<T>(rows, columns);
        }

        public M(A<T> array)
        {
            if (array.Rank != 2)
                throw new ArgumentException("Source in not a 2D array.", "array");
            this.m = array;
        }

        public int Rows
        {
            get { return m.Dimension[0]; }
        }

        public int Columns
        {
            get { return m.Dimension[1]; }
        }

        public int Size
        {
            get { return m.Count; }
        }

        public M<TOut> Convert<TOut>()
        {
            return (M<TOut>)base.Convert(typeof(TOut));
        }

        public M<T> Transpose()
        {
            int r = this.Rows, c = this.Columns;
            var t = new A<T>(c, r);
            for (int i = 0; i < r; ++i)
            {
                for (int j = 0; j < c; ++j)
                {
                    t[j, i] = m[i, j];
                }
            }
            return new M<T>(t);
        }

        public V<T> GetColumn(int columnIndex)
        {
            return V.Generate(i => this[i, columnIndex], this.Rows);
        }

        public V<T> GetRow(int rowIndex)
        {
            return V.Generate(i => this[rowIndex, i], this.Columns);
        }

        public V<T> ToVector()
        {
            return new V<T>(this.UnderlyingArray.Buffer);
        }

        public T this[int row, int column]
        {
            get { return m[row, column]; }
            set { m[row, column] = value; }
        }

        public new A<T> UnderlyingArray
        {
            get { return m; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as M<T>;
            return other != null && other.m.Equals(this.m);
        }

        public override int GetHashCode()
        {
            return this.m.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < this.Rows; ++i)
            {
                for (int j = 0; j < this.Columns; ++j)
                {
                    if (j > 0)
                        sb.Append(", ");

                    var x = this[i, j];
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", x);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        protected override A GetUnderlyingArray()
        {
            return m;
        }

        public static implicit operator A<T>(M<T> m)
        {
            return m.UnderlyingArray;
        }

        public static M<T> operator *(M<T> a, M<T> b)
        {
            var ad = a as M<double>;
            if (ad != null)
            {
                return MExtensions.Multiply(ad, b as M<double>) as M<T>;
            }

            var af = a as M<float>;
            if (af != null)
            {
                return MExtensions.Multiply(af, b as M<float>) as M<T>;
            }

            throw new Exception($"Multiplication not supported for M<{typeof(T)}> type.");
        }

        [DebuggerNonUserCode]
        private sealed class DebugView
        {
            private readonly M<T> matrix;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public string[] Elements
            {
                get
                {
                    var array = new string[matrix.Rows];
                    for (int i = 0; i < matrix.Rows; i++)
                    {
                        array[i] = string.Format(CultureInfo.InvariantCulture, "{0}", matrix.GetRow(i).ToString());
                    }
                    return array;
                }
            }

            public DebugView(M<T> matrix)
            {
                this.matrix = matrix;
            }
        }
    }

    public abstract class M
    {
        public static M<T> Generate<T>(int rows, int columns, Func<int, int, T> generator)
        {
            var m = new M<T>(rows, columns);
            var buffer = m.UnderlyingArray.Buffer;
            for (int k = 0, i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                    buffer[k++] = generator(i, j);
            }
            return m;
        }

        protected abstract A GetUnderlyingArray();

        public A UnderlyingArray
        {
            get { return GetUnderlyingArray(); }
        }

        public M Convert(Type destinationElementType)
        {
            return Convert(this, destinationElementType);
        }

        public static M Convert(M source, Type destinationElementType)
        {
            var destinationArray = source.UnderlyingArray.Convert(destinationElementType);
            return FromArray(destinationArray);
        }

        public static M<double> Identity(int size)
        {
            return Generate<double>(size, size, (i, j) => i == j ? 1 : 0);
        }

        public static M FromArray(A array)
        {
            var matrixType = typeof(M<>).MakeGenericType(array.ElementType);
            return (M)Activator.CreateInstance(matrixType, array);
        }

        public static M<float> FromArray(float[,] array)
        {
            return new M<float>(A.FromArray(array));
        }

        public static M<double> FromArray(double[,] array)
        {
            return new M<double>(A.FromArray(array));
        }

        public static M<T> Column<T>(params T[] values)
        {
            return new M<T>(A.FromArray(values, values.Length, 1));
        }

        public static M<T> Row<T>(params T[] values)
        {
            return new M<T>(A.FromArray(values, 1, values.Length));
        }
    }

    public static class MExtensions
    {
        public static M<T> ToM<T>(this A<T> array)
        {
            return new M<T>(array);
        }

        public static M<double> Multiply(this M<double> a, M<double> b)
        {
            if (a.Columns != b.Rows)
                throw new Exception("Matrix size invalid for multiplication.");

            var r = new M<double>(a.Rows, b.Columns);

            for (int i = 0; i < r.Rows; ++i)
            {
                for (int j = 0; j < r.Columns; ++j)
                {
                    double sum = 0;
                    for (int k = 0; k < a.Columns; ++k)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    r[i, j] = sum;
                }
            }

            return r;
        }

        public static M<float> Multiply(this M<float> a, M<float> b)
        {
            if (a.Columns != b.Rows)
                throw new Exception("Matrix size invalid for multiplication.");

            var r = new M<float>(a.Rows, b.Columns);

            for (int i = 0; i < r.Rows; ++i)
            {
                for (int j = 0; j < r.Columns; ++j)
                {
                    float sum = 0;
                    for (int k = 0; k < a.Columns; ++k)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    r[i, j] = sum;
                }
            }

            return r;
        }

        public static M<R> Map<T, R>(this M<T> matrix, Func<T, R> lambda)
        {
            return M.Generate<R>(matrix.Rows, matrix.Columns, (i, j) => lambda(matrix[i, j]));
        }
    }
}
