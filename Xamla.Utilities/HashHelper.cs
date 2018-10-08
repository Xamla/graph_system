using System;
using System.Collections.Generic;

namespace Xamla.Utilities
{
    // Source: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
    public static class HashHelper
    {
        public const int Start = 17;

        /// <summary>
        /// a fluent interface like this:<br />
        /// return HashHelper.Start.CombineHashCode(field1).CombineHashCode(field2).
        ///     CombineHashCode(field3);
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCode<T>(this int hashCode, T arg)
        {
            var h = arg?.GetHashCode() ?? 0;
            return unchecked((hashCode * 29) + h);
        }

        public static int GetHashCode<T1, T2>(T1 arg1, T2 arg2)
        {
            return Start.CombineHashCode(arg1).CombineHashCode(arg2);
        }

        public static int GetHashCode<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            return Start.CombineHashCode(arg1).CombineHashCode(arg2).CombineHashCode(arg3);
        }

        public static int GetHashCode<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return Start.CombineHashCode(arg1).CombineHashCode(arg2).CombineHashCode(arg3).CombineHashCode(4);
        }

        public static int GetHashCode<T>(IEnumerable<T> list)
        {
            unchecked
            {
                if (list == null)
                    return 0;

                int hash = Start;
                foreach (var item in list)
                    hash = hash.CombineHashCode(item);
                return hash;
            }
        }

        public static int GetHashCode(params object[] list)
        {
            return GetHashCode<object>(list);
        }

        /// <summary>
        /// Gets a hashcode for a collection for that the order of items
        /// does not matter.
        /// So {1, 2, 3} and {3, 2, 1} will get same hash code.
        /// </summary>
        public static int GetHashCodeForOrderNoMatterCollection<T>(IEnumerable<T> list)
        {
            unchecked
            {
                int hash = 17;
                int count = 0;
                foreach (var item in list)
                {
                    hash += item.GetHashCode();
                    count++;
                }
                return 29 * hash + count.GetHashCode();
            }
        }
    }
}
