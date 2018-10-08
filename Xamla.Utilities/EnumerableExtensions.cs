using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public static class EnumerableEx
    {
        public static IEnumerable<T> Return<T>(T value)
        {
            return new[] { value };
        }

        public static bool IsEmpty<T>(IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static IEnumerable<T> SkipLast<T>(this IList<T> list, int count)
        {
            var end = list.Count - count;
            for (int i = 0; i < end; ++i)
                yield return list[i];
        }

        static IEnumerable<T> SkipLastImpl<T>(IEnumerable<T> source, int count)
        {
            var q = new Xamla.Utilities.Collections.Deque<T>(count);
            foreach (var x in source)
            {
                if (q.Count >= count)
                {
                    yield return q.Front;
                    q.PopFront();
                }

                q.PushBack(x);
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            if (count <= 0)
                return source;

            var list = source as IList<T>;
            if (list != null)
                return SkipLast(list, count);

            return SkipLastImpl(source, count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            var l = source.ToArray();
            for (int i = l.Length - 1; i >= 0; --i)
            {
                var r = rng.Next(i + 1);
                var tmp = l[i];
                l[i] = l[r];
                l[r] = tmp;
                yield return l[i];
            }
        }

        public static IEnumerable<T> StartWith<T>(this IEnumerable<T> source, params T[] values)
        {
            return values.Concat(source);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> scope)
        {
            foreach (var x in source)
                scope(x);
        }

        static IEnumerable<List<T>> BufferImpl<T>(IEnumerable<T> source, int count)
        {
            var buffer = new List<T>(count);
            foreach (var x in source)
            {
                buffer.Add(x);
                if (buffer.Count >= count)
                {
                    yield return buffer;
                    buffer = new List<T>(count);
                }
            }

            if (buffer.Count > 0)
                yield return buffer;
        }

        public static IEnumerable<List<T>> Buffer<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (count < 1)
                throw new ArgumentException("Count must be greater than 0.", "count");

            return BufferImpl(source, count);
        }

        public static IEnumerable<TAccum> Scan<T, TAccum>(this IEnumerable<T> seq, TAccum init, Func<TAccum, T, TAccum> op)
        {
            TAccum current = init;
            foreach (T item in seq)
            {
                current = op(current, item);
                yield return current;
            }
        }

        public static IEnumerable<T> Generate<T>(int count, Func<int, T> generator)
        {
            if (generator == null)
                throw new ArgumentNullException("generator");

            for (int i = 0; i < count; i++)
                yield return generator(i);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            int i = 0;
            foreach (var x in source)
            {
                if (object.Equals(x, value))
                    return i;
                ++i;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var x in source)
            {
                if (predicate(x))
                    return i;
                ++i;
            }

            return -1;
        }

        public static (T, int) FirstOrDefaultIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var x in source)
            {
                if (predicate(x))
                    return (x, i);
            }

            return (default(T), -1);
        }

        public static T[] ToArray<T>(this T[] array)
        {
            var clone = new T[array.Length];
            array.CopyTo(clone, 0);
            return clone;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
            => new Dictionary<TKey, TValue>(source);

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, bool ignoreDuplicateKeys)
        {
            var d = new Dictionary<TKey, TValue>();
            if (ignoreDuplicateKeys)
            {
                foreach (var x in source)
                {
                    var key = keySelector(x);
                    if (!d.ContainsKey(key))
                        d.Add(key, x);
                }
            }
            else
            {
                foreach (var x in source)
                    d.Add(keySelector(x), x);
            }
            return d;
        }

        public static T MaxBy<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector)
        {
            return MaxBy(source, valueSelector, Comparer<C>.Default);
        }

        public static T MaxBy<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector, IComparer<C> comparer)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    throw new Exception("Source sequence is empty.");

                var best = e.Current;
                var bestValue = valueSelector(best);
                while (e.MoveNext())
                {
                    var value = valueSelector(e.Current);
                    if (comparer.Compare(value, bestValue) > 0)
                    {
                        best = e.Current;
                        bestValue = value;
                    }
                }

                return best;
            }
        }

        public static T MinBy<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector)
        {
            return MinBy(source, valueSelector, Comparer<C>.Default);
        }

        public static T MinBy<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector, IComparer<C> comparer)
        {
            return MaxBy(source, valueSelector, comparer.Invert());
        }

        public static T MaxByOrDefault<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector)
        {
            return MaxByOrDefault(source, valueSelector, Comparer<C>.Default);
        }

        public static T MaxByOrDefault<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector, IComparer<C> comparer)
        {
            var best = default(T);
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    return best;

                best = e.Current;
                var bestValue = valueSelector(best);
                while (e.MoveNext())
                {
                    var value = valueSelector(e.Current);
                    if (comparer.Compare(value, bestValue) > 0)
                    {
                        best = e.Current;
                        bestValue = value;
                    }
                }
            }

            return best;
        }

        public static T MinByOrDefault<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector)
        {
            return MinByOrDefault(source, valueSelector, Comparer<C>.Default);
        }

        public static T MinByOrDefault<T, C>(this IEnumerable<T> source, Func<T, C> valueSelector, IComparer<C> comparer)
        {
            return MaxByOrDefault(source, valueSelector, comparer.Invert());
        }

        public static T SoftMax<T>(this IEnumerable<T> source, Func<T, double> valueSelector, double tau, Random rng)
        {
            var arr = source.ToArray();

            if (arr.Length == 0)
                return default(T);

            var sum = source.Sum(x => Math.Exp(valueSelector(x) / tau));

            if (sum < 1E-10)
            {
                return arr[rng.Next(arr.Length)];
            }
            else
            {
                var scan = arr.Scan(0.0, (acc, x) => acc + Math.Exp(valueSelector(x) / tau)).ToArray();
                var r = rng.NextDouble() * scan[scan.Length - 1];

                int i = scan.Where(a => a <= r).Count() - 1;
                return arr[Math.Max(Math.Min(i, arr.Length - 1), 0)];
            }
        }

        public static IEnumerable<T> Mid<T>(this T[] source, int index)
        {
            for (int i = index; i < source.Length; ++i)
            {
                yield return source[i];
            }
        }

        public static IEnumerable<T> Mid<T>(this T[] source, int index, int count)
        {
            for (int i = index, j = 0; j < count && i < source.Length; ++i, ++j)
            {
                yield return source[i];
            }
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        private static void Sort<T, TCompare>(ref T a, ref T b, Func<T, TCompare> selector, IComparer<TCompare> cmp)
        {
            if (cmp.Compare(selector(a), selector(b)) > 0)
            {
                Swap(ref a, ref b);
            }
        }

        private static TSource MedianOfFive<TSource, TCompare>(TSource a, TSource b, TSource c, TSource d, TSource e, Func<TSource, TCompare> selector, IComparer<TCompare> cmp)
        {
            Sort(ref a, ref b, selector, cmp);
            Sort(ref c, ref d, selector, cmp);

            if (cmp.Compare(selector(c), selector(a)) < 0)
            {
                Swap(ref b, ref d);
                c = a;
            }
            a = e;
            Sort(ref a, ref b, selector, cmp);

            if (cmp.Compare(selector(a), selector(c)) < 0)
            {
                Swap(ref b, ref d);
                a = c;
            }

            return (cmp.Compare(selector(d), selector(a)) < 0 ? d : a);
        }

        public static T NthElement<T>(this IEnumerable<T> source, int rank) where T : IComparable<T>
        {
            return source.NthElement(rank, x => x);
        }

        public static TSource NthElement<TSource, TCompare>(this IEnumerable<TSource> source, int rank, Func<TSource, TCompare> selector) where TCompare : IComparable<TCompare>
        {
            return source.NthElement(rank, selector, Comparer<TCompare>.Default);
        }

        public static TSource NthElement<TSource, TCompare>(this IEnumerable<TSource> source, int rank, Func<TSource, TCompare> selector, IComparer<TCompare> comparer)
        {
            var a = source.ToArray();
            return NthElementInplace(a, 0, a.Length, rank, selector, comparer);
        }

        class InvertComparer<T>
            : IComparer<T>
        {
            IComparer<T> baseComparer;

            public InvertComparer(IComparer<T> baseComparer)
            {
                this.baseComparer = baseComparer;
            }

            public int Compare(T x, T y)
            {
                return -baseComparer.Compare(x, y);
            }
        }

        public static IComparer<T> Invert<T>(this IComparer<T> comparer)
        {
            return new InvertComparer<T>(comparer);
        }

        public static IEnumerable<TSource> TopNWithInsertionSort<TSource, TCompare>(this IEnumerable<TSource> source, int n, Func<TSource, TCompare> selector, IComparer<TCompare> comparer)
        {
            if (n <= 0)
                throw new ArgumentOutOfRangeException("n");

            var best = new List<TSource>(n);

            using (var e = source.GetEnumerator())
            {
                while (best.Count < n && e.MoveNext())
                {
                    var x = e.Current;

                    int i = best.Count;
                    while (i > 0 && comparer.Compare(selector(x), selector(best[i - 1])) < 0)
                        --i;

                    best.Insert(i, x);
                }

                while (e.MoveNext())
                {
                    var x = e.Current;

                    if (comparer.Compare(selector(x), selector(best[best.Count - 1])) >= 0)
                        continue;

                    int i = best.Count - 1;
                    while (i > 0 && comparer.Compare(selector(x), selector(best[i - 1])) < 0)
                    {
                        best[i] = best[i - 1];
                        --i;
                    }

                    best[i] = x;
                }
            }

            return best;
        }

        public static TSource NthElementWithInsertionSort<TSource, TCompare>(TSource[] source, int start, int end, int rank, Func<TSource, TCompare> selector, IComparer<TCompare> comparer)
        {
            int length = end - start;
            if (length <= 0)
                throw new ArgumentException();

            int pos = start;
            int c = 0;
            for (; pos < end && c <= rank; ++c, ++pos)
            {
                var x = source[pos];
                int i = pos;
                while (i > start && comparer.Compare(selector(x), selector(source[i - 1])) < 0)
                {
                    source[i] = source[i - 1];
                    --i;
                }

                source[i] = x;
            }

            if (pos >= end)
            {
                return source[pos - 1];
                // throw new ArgumentOutOfRangeException("rank");
            }

            for (; pos < end; ++pos)
            {
                var x = source[pos];
                if (comparer.Compare(selector(x), selector(source[start + rank])) >= 0)
                    continue;

                int i = start + rank;
                var tmp = source[i];

                while (i > start && comparer.Compare(selector(x), selector(source[i - 1])) < 0)
                {
                    source[i] = source[i - 1];
                    --i;
                }

                source[i] = x;
                source[pos] = tmp;
            }

            return source[start + rank];
        }

        private static void PartitionInplace<TSource, TCompare>(TSource[] source, int start, int end, TSource pivot, Func<TSource, TCompare> selector, IComparer<TCompare> comparer, out int center, out int right)
        {
            center = right = end;

            var pivotValue = selector(pivot);
            for (int i = start; i < center; )
            {
                int cmp = comparer.Compare(selector(source[i]), pivotValue);
                if (cmp < 0)
                {
                    ++i;
                    continue;
                }

                while (center > i + 1 && comparer.Compare(selector(source[center - 1]), pivotValue) == 0)
                    center--;

                if (cmp == 0)
                {
                    Swap(ref source[--center], ref source[i]);
                    cmp = comparer.Compare(selector(source[i]), pivotValue);
                    if (cmp <= 0)
                    {
                        ++i;
                        continue;
                    }
                }

                --center;
                if (right > center + 1)
                {
                    Swap(ref source[center], ref source[right - 1]);
                }

                cmp = comparer.Compare(selector(source[right - 1]), pivotValue);
                if (cmp > 0)
                {
                    --right;
                }
                else if (cmp < 0)
                {
                    Swap(ref source[i], ref source[--right]);
                    ++i;
                    continue;
                }
                //else if (cmp == 0)
                //{
                //    new pivot element found
                //}
            }
        }

        public static TSource NthElementInplace<TSource, TCompare>(this TSource[] source, int rank, Func<TSource, TCompare> selector, IComparer<TCompare> comparer)
        {
            return source.NthElementInplace(0, source.Length, rank, selector, comparer);
        }

        public static TSource NthElementInplace<TSource, TCompare>(this TSource[] source, int start, int end, int rank, Func<TSource, TCompare> selector, IComparer<TCompare> comparer)
        {
            int length = end - start;
            if (length <= 0)
                throw new ArgumentException();

            if (rank >= length)
                rank = length - 1;

            if (rank < 15)
                return NthElementWithInsertionSort(source, start, end, rank, selector, comparer);

            TSource m = MedianOfFive(
                source[start],
                source[start + length * 1 / 4],
                source[start + length * 2 / 4],
                source[start + length * 3 / 4],
                source[start + length - 1],
                selector,
                comparer
            );

            int center, right;
            PartitionInplace(source, start, end, m, selector, comparer, out center, out right);

            if (rank < center - start)
                return NthElementInplace(source, start, center, rank, selector, comparer);

            if (rank < (right - start))
                return m;

            return NthElementInplace(source, right, end, rank - (right - start), selector, comparer);
        }

        /// <summary>
        /// Convert an IEnumerable<> sequence into an IObservable<> source, iterating the
        /// source waiting after each element for an element-specific duration that is
        /// calculated by evaluationg delaySelector.
        /// </summary>
        public static IObservable<T> ToObservableDelay<T>(
            this IEnumerable<T> source,
            Func<T, TimeSpan> delaySelector,
            IScheduler scheduler = null
        )
        {
            scheduler = scheduler ?? DefaultScheduler.Instance;
            return Observable.Create<T>(o =>
                scheduler.ScheduleAsync(async (s, c) =>
                {
                    try
                    {
                        foreach (var x in source)
                        {
                            await Task.Delay(delaySelector(x), c).ConfigureAwait(false);
                            o.OnNext(x);
                        }
                        o.OnCompleted();
                    }
                    catch (OperationCanceledException) { /* ignore */ }
                    catch (Exception e)
                    {
                        o.OnError(e);
                    }
                })
            );
        }

        class UsingEnumerable<TSource, TResource>
            : IEnumerable<TSource>
            where TResource : IDisposable
        {
            class Enumerator<T>
                : IEnumerator<T>
            {
                IEnumerator<T> source;
                IDisposable resource;

                public Enumerator(IEnumerator<T> source, IDisposable resource)
                {
                    this.source = source;
                    this.resource = resource;
                }

                public T Current
                {
                    get { return source.Current; }
                }

                public void Dispose()
                {
                    try
                    {
                        source.Dispose();
                    }
                    finally
                    {
                        resource.Dispose();
                    }
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return Current; }
                }

                public bool MoveNext()
                {
                    return source.MoveNext();
                }

                public void Reset()
                {
                    source.Reset();
                }
            }

            Func<TResource> resourceFactory;
            Func<TResource, IEnumerable<TSource>> sourceFactory;

            public UsingEnumerable(Func<TResource> resourceFactory, Func<TResource, IEnumerable<TSource>> sourceFactory)
            {
                this.resourceFactory = resourceFactory;
                this.sourceFactory = sourceFactory;
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                var resource = resourceFactory();

                try
                {
                    var source = sourceFactory(resource).GetEnumerator();
                    return new Enumerator<TSource>(source, resource);
                }
                catch
                {
                    resource.Dispose();
                    throw;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public static IEnumerable<TSource> Using<TSource, TResource>(Func<TResource> resourceFactory, Func<TResource, IEnumerable<TSource>> sourceFactory)
            where TResource : IDisposable
        {
            return new UsingEnumerable<TSource, TResource>(resourceFactory, sourceFactory);
        }
    }
}
