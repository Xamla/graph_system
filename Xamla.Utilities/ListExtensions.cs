using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public static class ListExtensions
    {
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (predicate(list[i]))
                    return i;
            }

            return -1;
        }

        public static IEnumerable<T> Mid<T>(this IList<T> source, int index)
        {
            for (int i = index; i < source.Count; ++i)
            {
                yield return source[i];
            }
        }

        public static IEnumerable<T> Mid<T>(this IList<T> source, int index, int count)
        {
            for (int i = index, j = 0; j < count && i < source.Count; ++i, ++j)
            {
                yield return source[i];
            }
        }

        public static IEnumerable<T> SampleReplacing<T>(this IList<T> source, Random rng)
        {
            for (;;)
                yield return source[rng.Next(source.Count)];
        }

        public static IEnumerable<T> SampleReplacing<T>(this IList<T> source, Random rng, int count)
        {
            return SampleReplacing(source, rng).Take(count);
        }

        /// <summary>
        /// Returns elements of the source list in random order.
        /// The Source collection is modified (selected elements are moved to the back of the list).
        /// </summary>
        public static IEnumerable<T> SampleWithoutReplacing<T>(this IList<T> source, Random rng)
        {
            int end = source.Count;
            while (end > 0)
            {
                var i = rng.Next(end);      // pick one of the remaining elements (that have not been selected before)

                // swap selected element with item at back of list (avoid reselection)
                --end;
                T t = source[i];
                source[i] = source[end];
                source[end] = t;
                yield return t;
            }
        }

        public static IEnumerable<T> SampleWithoutReplacing<T>(this IList<T> source, Random rng, int count)
        {
            return SampleWithoutReplacing(source, rng).Take(count);
        }
    }
}
