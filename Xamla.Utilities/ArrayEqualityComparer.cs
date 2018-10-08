using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class ArrayEqualityComparer<T>
        : IEqualityComparer<T[]>
    {
        static volatile ArrayEqualityComparer<T> defaultComparer;

        public static ArrayEqualityComparer<T> Default
        {
            get
            {
                var comparer = defaultComparer;
                if (comparer == null)
                {
                    comparer = new ArrayEqualityComparer<T>();
                    defaultComparer = comparer;
                }
                return comparer;
            }
        }

        IEqualityComparer<T> elementComparer;

        public ArrayEqualityComparer()
            : this(EqualityComparer<T>.Default)
        {
        }

        public ArrayEqualityComparer(IEqualityComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        public bool Equals(T[] x, T[] y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null)
                return false;

            int length = x.Length;
            if (length != y.Length)
                return false;

            for (int i = 0; i < length; ++i)
            {
                if (!elementComparer.Equals(x[i], y[i]))
                    return false;
            }

            return true;
        }

        public int GetHashCode(T[] obj)
        {
            return HashHelper.GetHashCode(obj);
        }
    }

    public class ArrayEqualityComparer
        : IEqualityComparer
    {
        IEqualityComparer elementComparer;

        public ArrayEqualityComparer(IEqualityComparer elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
                return true;

            var a = x as Array;
            if (a == null)
                return false;

            var b = y as Array;

            int length = a.Length;
            if (length != b.Length)
                return false;

            for (int i = 0; i < length; ++i)
            {
                if (!elementComparer.Equals(a.GetValue(i), b.GetValue(i)))
                    return false;
            }

            return true;
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
                return 0;

            var array = (Array)obj;
            int length = array.Length;

            int hash = 17;
            for (int i = 0; i < length; ++i)
            {
                hash = hash.CombineHashCode(array.GetValue(i));
            }
            
            return hash;
        }
    }
}
