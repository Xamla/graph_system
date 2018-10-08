using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Xamla.Utilities
{
    public class ReferenceComparer<T> : IEqualityComparer<T> where T : class
    {
        private static readonly ReferenceComparer<T> instance = new ReferenceComparer<T>();

        public static ReferenceComparer<T> Instance
        {
            get { return instance; }
        }

        public bool Equals(T x, T y)
        {
            return object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
