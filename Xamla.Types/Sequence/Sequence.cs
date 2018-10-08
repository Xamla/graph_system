using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence.Iterators;
using Xamla.Utilities;

namespace Xamla.Types.Sequence
{
    public static class Sequence
    {
        internal class AnonymousSequence<T>
            : ISequence<T>
        {
            Func<object, IIterator<T>> start;

            public AnonymousSequence(Func<object, IIterator<T>> start)
            {
                this.start = start;
            }

            public IIterator<T> Start(object context)
            {
                try
                {
                    return start(context);
                }
                catch (Exception e)
                {
                    return Sequence.Throw<T>(e).Start(context);
                }
            }

            IIterator ISequence.Start(object context)
            {
                return this.Start(context);
            }
        }

        public static ISequence<T> Create<T>(Func<object, IIterator<T>> start)
        {
            return new AnonymousSequence<T>(start);
        }

        public static ISequence<T> Create<T>(Func<IObserver<T>, CancellationToken, Task> start)
        {
            return Create(context => Iterator.Create(start, context));
        }

        public static ISequence<T> CreateProducer<T>(Func<IAsyncObserver<T>, CancellationToken, Task> producer, int queueLimit)
        {
            return Create(context =>
            {
                var cts = new CancellationTokenSource();
                var queue = new AsyncQueue<T>(queueLimit);

                var task = producer(queue, cts.Token)
                    .ContinueWith(t => queue.OnError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

                return Iterator.Create(
                    moveNext: queue.MoveNext,
                    current: () => queue.Current,
                    cancel: cts.Cancel,
                    stop: () => EnumerableEx.Return(task),
                    context: context
                );
            });
        }

        public static ISequence<T> Produce<T>(Func<T> factory, int count = 1)
        {
            return Create(context => new FactoryIterator<T>(factory, context, count));
        }

        public static ISequence<T> Produce<T>(Func<Task<T>> factory, int count = 1)
        {
            return Create(context => new FactoryIterator<T>(factory, context, count));
        }

        public static ISequence<T> Produce<T>(Func<object, CancellationToken, Task<T>> factory, int count = 1)
        {
            return Create(context => new FactoryIterator<T>(factory, context, count));
        }

        public static ISequence<TResult> CreateOperator<TSource, TResult>(
            ISequence<TSource> source,
            Func<IIterator<TSource>, Result<TResult>, CancellationToken, Task<bool>> moveNext
        )
        {
            return Create(context => Iterator.CreateOperator<TSource, TResult>(source.Start(context), moveNext));
        }

        public static ISequence<TResult> CreateSingleResultOperator<TSource, TResult>(
            ISequence<TSource> source,
            Func<IIterator<TSource>, CancellationToken, Task<TResult>> evaluate
        )
        {
            return Create(context => Iterator.CreateSingleResultOperator<TSource, TResult>(source.Start(context), evaluate));
        }

        public static ISequence<T> ToSequence<T>(this IEnumerable<T> source)
        {
            return Create(context => new EnumeratorIterator<T>(source.GetEnumerator(), context));
        }

        public static ISequence ToSequence(this IEnumerable source)
        {
            return Create(context => new EnumeratorIterator<object>(source.OfType<object>().GetEnumerator(), context));
        }

        //public static ISequence<T> ToSequence<T>(this IAsyncEnumerable<T> source)
        //{
        //    return Create<T>(context =>
        //    {
        //        var asyncEnum = source.GetEnumerator();
        //        return Iterator.Create(asyncEnum.MoveNext, () => asyncEnum.Current, asyncEnum, context);
        //    });
        //}

        public static ISequence<T> ToSequence<T>(this IObservable<T> source)
        {
            return Create<T>(context =>
                ObserverToIteratorAdapter<T>.Create(
                    (adapter, cancel) =>
                    {
                        var subscription = new SingleAssignmentDisposable();
                        cancel.Register(subscription.Dispose);
                        subscription.Disposable = source.SubscribeSafe(adapter);
                        return TaskConstants.Completed;
                    },
                    context
                )
            );
        }

        public static IObservable<T> ToObservable<T>(this ISequence<T> source, object context = null)
        {
            return Observable.Create<T>(async (o, c) =>
            {
                var iterator = source.Start(context);
                while (await iterator.MoveNext(c).ConfigureAwait(false))
                    o.OnNext(iterator.Current);
                o.OnCompleted();
            });
        }

        public static IEnumerable<T> ToEnumerable<T>(this ISequence<T> source, object context = null)
        {
            using (var i = source.Start(context))
            {
                while (i.MoveNext(CancellationToken.None).Result)
                    yield return i.Current;
            }
        }

        public static System.Collections.IEnumerable ToEnumerable(this ISequence source, object context = null)
        {
            using (var i = source.Start(context))
            {
                while (i.MoveNext(CancellationToken.None).Result)
                    yield return i.Current;
            }
        }

        public static List<T> ToList<T>(this ISequence<T> source, object context = null)
        {
            return source.ToEnumerable(context).ToList();
        }

        public static List<object> ToList(this ISequence source, object context = null)
        {
            return source.ToEnumerable(context).OfType<object>().ToList();
        }

        public static async Task<List<T>> ToListAsync<T>(this IIterator<T> iterator, CancellationToken cancel = default(CancellationToken))
        {
            var list = new List<T>();

            for (;;)
            {
                var moveNextTask = iterator.MoveNext(cancel);
                if (!moveNextTask.IsCompleted)
                {
                    if (!await moveNextTask.ConfigureAwait(false))
                        break;
                }
                else if (!moveNextTask.Result)
                    break;

                list.Add(iterator.Current);
            }

            return list;
        }

        public static async Task<List<T>> ToListAsync<T>(this ISequence<T> source, object context = null, CancellationToken cancel = default(CancellationToken))
        {
            using (var iterator = source.Start(context))
            {
                return await ToListAsync(iterator, cancel).ConfigureAwait(false);
            }
        }

        public static ISequence<object> AsObjects(this ISequence source)
        {
            return source as ISequence<object> ?? Create<object>(context =>
            {
                var iterator = source.Start(context);
                return Iterator.Create(iterator.MoveNext, () => iterator.Current, iterator.Dispose, iterator.Stop, context);
            });
        }

        public static ISequence<T> Using<T, TResource>(Func<TResource> resourceFactory, Func<TResource, ISequence<T>> sequenceBuilder)
            where TResource : IDisposable
        {
            return Create<T>(context =>
                {
                    var resource = resourceFactory();
                    return new UsingIterator<T>(sequenceBuilder(resource).Start(context), resource);
                });
        }

        public static ISequence<Unit> AsUnits<T>(this ISequence<T> source)
        {
            return source.Select(x => Unit.Default);
        }

        public static ISequence<Unit> Completed<T>(this ISequence<T> source)
        {
            return CreateSingleResultOperator<T, Unit>(source, async (iterator, cancel) =>
            {
                while (true)
                {
                    if (!await iterator.MoveNext(cancel).ConfigureAwait(false))
                        return Unit.Default;
                }
            });
        }

        public static ISequence<TSource> Distinct<TSource, TKey>(this ISequence<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Sequence.Defer(() =>
            {
                var lookup = new HashSet<TKey>();
                return source.Where(x => { lock (lookup) { return lookup.Add(keySelector(x)); } });
            });
        }

        public static ISequence<TSource> Distinct<TSource>(this ISequence<TSource> source)
        {
            return source.Distinct(x => x);
        }

        public static ISequence<T> Return<T>(T value)
        {
            return new T[] { value }.ToSequence();
        }

        public static ISequence<T> Empty<T>()
        {
            return Enumerable.Empty<T>().ToSequence();
        }

        public static ISequence<T> Never<T>()
        {
            return Sequence.Create<T>(context => Iterator.Create<T>(
                moveNext: c => c.AsTask().Then(() => TaskConstants.CanceledTask<bool>()),
                current: () => { throw new InvalidOperationException(); },
                cancel: Disposable.Empty,
                context: context)
            );
        }

        public static ISequence<T> Defer<T>(Func<object, ISequence<T>> factory)
        {
            return Create(context => factory(context).Start(context));
        }

        public static ISequence<T> Defer<T>(Func<ISequence<T>> factory)
        {
            return Create(context => factory().Start(context));
        }

        public static ISequence<T> ToSequence<T>(this Task<T> task)
        {
            return Sequence.Produce(() => task, 1);
        }

        public static ISequence<TResult> Select<TSource, TResult>(this ISequence<TSource> source, Func<TSource, TResult> selector)
        {
            return Create<TResult>(context => new SelectIterator<TSource, TResult>(source.Start(context), selector));
        }

        public static ISequence<TResult> SelectAsync<TSource, TResult>(this ISequence<TSource> source, Func<TSource, CancellationToken, Task<TResult>> selector)
        {
            return CreateOperator<TSource, TResult>(source, async (iter, result, cancel) =>
            {
                while (await iter.MoveNext(cancel).ConfigureAwait(false))
                {
                    result.Value = await selector(iter.Current, cancel).ConfigureAwait(false);
                    return true;
                }
                return false;
            });
        }

        public static ISequence<TResult> SelectManyAsync<TSource, TResult>(this ISequence<TSource> source, Func<TSource, CancellationToken, Task<TResult>> selector)
        {
            return CreateOperator<TSource, Task<TResult>>(source, async (iter, result, cancel) =>
                {
                    while (await iter.MoveNext(cancel).ConfigureAwait(false))
                    {
                        result.Value = selector(iter.Current, cancel);
                        return true;
                    }
                    return false;
                })
                .Select(x => x.ToSequence())
                .Merge();
        }

        public static ISequence<TResult> SelectManySequential<TSource, TResult>(this ISequence<TSource> source, Func<TSource, ISequence<TResult>> selector)
        {
            return source.Select(selector).Concat();
        }

        public static ISequence<TResult> SelectMany<TSource, TResult>(this ISequence<TSource> source, Func<TSource, ISequence<TResult>> selector)
        {
            return source.Select(selector).Merge();
        }

        public static ISequence<TResult> SelectMany<TSource, TResult>(this ISequence<TSource> source, Func<TSource, ISequence<TResult>> selector, int maxConcurrent)
        {
            return source.Select(selector).Merge(maxConcurrent);
        }

        public static ISequence<T> Concat<T>(this ISequence<ISequence<T>> source)
        {
            return Create(context => new ConcatIterator<T>(source.Start(context)));
        }

        public static ISequence<T> Concat<T>(this ISequence<T> self, ISequence<T> other)
        {
            return new ISequence<T>[] { self, other }.ToSequence().Concat();
        }

        public static ISequence<T> Concat<T>(this ISequence<T> source, ISequence<ISequence<T>> others)
        {
            return Sequence.Return(source).Concat(others).Concat();
        }

        public static ISequence<T> Merge<T>(this ISequence<ISequence<T>> source)
        {
            return Create(context => new MergeIterator<T>(source.Start(context)));
        }

        public static ISequence<T> Merge<T>(this ISequence<ISequence<T>> source, int maxConcurrent)
        {
            return Create(context => new MergeIterator<T>(source.Start(context), maxConcurrent));
        }

        public static ISequence<T> MergeN<T>(this IEnumerable<ISequence<T>> sources)
        {
            return Create(context => new MergeNIterator<T>(sources.Select(x => x.Start(context)), context));
        }

        public static ISequence<T> MergeN<T>(params ISequence<T>[] sources)
        {
            return MergeN((IEnumerable<ISequence<T>>)sources);
        }

        public static ISequence<T> Merge<T>(this ISequence<T> source, ISequence<T> other)
        {
            return MergeN(source, other);
        }

        public static ISequence<T> Merge<T>(this ISequence<T> source, ISequence<ISequence<T>> others)
        {
            return Sequence.Return(source).Concat(others).Merge();
        }

        public static ISequence<TSource> Repeat<TSource>(this ISequence<TSource> source)
        {
            return Sequence.Create<TSource>(context => new RepeatIterator<TSource>(source, context));
        }

        public static ISequence<TSource> Repeat<TSource>(this ISequence<TSource> source, int count)
        {
            if (count < 0)
                throw new ArgumentException("Repeat count must be a positive integer.", "count");

            return Sequence.Create<TSource>(context => new RepeatIterator<TSource>(source, context, count));
        }

        public static ISequence<TSource> RepeatUntilEmpty<TSource>(this ISequence<TSource> source)
        {
            return Sequence.Create<TSource>(context => new RepeatUntilEmptyIterator<TSource>(source, context));
        }

        public static ISequence<T> Where<T>(this ISequence<T> source, Func<T, bool> predicate)
        {
            return Sequence.CreateOperator<T, T>(source, async (iter, result, cancel) =>
            {
                while (await iter.MoveNext(cancel).ConfigureAwait(false))
                {
                    if (predicate(iter.Current))
                    {
                        result.Value = iter.Current;
                        return true;
                    }
                }
                iter.Dispose();
                return false;
            });
        }

        public static ISequence<T> WhereAsync<T>(this ISequence<T> source, Func<T, CancellationToken, Task<bool>> predicate)
        {
            return Sequence.CreateOperator<T, T>(source, async (iter, result, cancel) =>
            {
                while (await iter.MoveNext(cancel).ConfigureAwait(false))
                {
                    if (await predicate(iter.Current, cancel).ConfigureAwait(false))
                    {
                        result.Value = iter.Current;
                        return true;
                    }
                }
                iter.Dispose();
                return false;
            });
        }

        public static ISequence<bool> Any<T>(this ISequence<T> source, Func<T, bool> predicate)
        {
            return CreateSingleResultOperator<T, bool>(source, async (iterator, cancel) =>
            {
                while (await iterator.MoveNext(cancel).ConfigureAwait(false))
                {
                    if (predicate(iterator.Current))
                        return true;
                }

                return false;
            });
        }

        public static ISequence<bool> Any<T>(this ISequence<T> source)
        {
            return source.Any(x => true);
        }

        public static ISequence<bool> All<T>(this ISequence<T> source, Func<T, bool> predicate)
        {
            return source.Any(x => !predicate(x)).Select(x => !x);
        }

        public static ISequence<bool> SequenceEqual<T>(this ISequence<T> first, ISequence<T> second)
        {
            return Create(context => new SequenceEqualIterator<T>(first, second, context, EqualityComparer<T>.Default));
        }

        public static ISequence<bool> SequenceEqual<T, TResult>(
            this ISequence<T> first,
            ISequence<T> second,
            IEqualityComparer<T> comparer
        )
        {
            return Create(context => new SequenceEqualIterator<T>(first, second, context, comparer));
        }

        public static ISequence<int> Count<T>(this ISequence<T> source, Func<T, bool> predicate)
        {
            return CreateSingleResultOperator<T, int>(source, async (iterator, cancel) =>
            {
                int i = 0;
                for (;;)
                {
                    var moveNextTask = iterator.MoveNext(cancel);
                    if (!moveNextTask.IsCompleted)
                    {
                        if (!await moveNextTask.ConfigureAwait(false))
                            return i;
                    }
                    else if (!moveNextTask.Result)
                        return i;

                    if (predicate == null || predicate(iterator.Current))
                        i++;
                }
            });
        }

        public static ISequence<int> Count<T>(this ISequence<T> source)
        {
            return source.Count(null);
        }

        public static ISequence<int> Count(this ISequence source)
        {
            return source.AsObjects().Count(null);
        }

        private static async Task<bool> CopyToQueue<T>(this IIterator<T> source, AsyncQueue<T> destination, int count, CancellationToken cancel, bool complete = false)
        {
            try
            {
                for (; count > 0; --count)
                {
                    if (!await source.MoveNext(cancel).ConfigureAwait(false))
                    {
                        if (complete)
                            destination.OnCompleted();
                        return false;
                    }
                    await destination.OnNext(source.Current, cancel).ConfigureAwait(false);
                }

                if (complete)
                    destination.OnCompleted();
            }
            catch (Exception e)
            {
                destination.OnError(e);
                throw;
            }

            return true;
        }

        public static ISequence<ISequence<T>> Window<T>(this ISequence<T> source, int windowSize, int queueLimit = 1)
        {
            if (queueLimit < 1)
                throw new ArgumentOutOfRangeException("queueLimit must be greater that one.");

            return Create<ISequence<T>>(context =>
            {
                Task<bool> lastCopyToResult = null;
                var i = source.Start(context);
                var subscription = new RefCountDisposable(i);
                return Iterator.Create<ISequence<T>>(async (r, c) =>
                {
                    if (lastCopyToResult != null && await lastCopyToResult.ConfigureAwait(false) == false)
                        return false;

                    if (!await i.MoveNext(c).ConfigureAwait(false))
                        return false;

                    var queue = new AsyncQueue<T>(queueLimit);
                    await queue.OnNext(i.Current, c).ConfigureAwait(false);

                    r.Value = Create(context2 => new AnonymousIterator<T>(queue.MoveNext, () => queue.Current, queue.Dispose, Enumerable.Empty<Task>, context2));
                    var keepAlive = subscription.GetDisposable();
                    lastCopyToResult = CopyToQueue(i, queue, windowSize - 1, CancellationToken.None, true).Finally(keepAlive.Dispose);
                    return true;
                }, context, subscription);
            });
        }

        public static ISequence<IList<T>> Buffer<T>(this ISequence<T> source)
        {
            return CreateOperator<T, IList<T>>(source, async (i, r, c) =>
            {
                if (!await i.MoveNext(c).ConfigureAwait(false))
                    return false;

                r.Value = new List<T>() { i.Current };
                while (await i.MoveNext(c).ConfigureAwait(false))
                    r.Value.Add(i.Current);
                return true;
            });
        }

        public static ISequence<IList<T>> Buffer<T>(this ISequence<T> source, int count)
        {
            if (count < 1)
                throw new ArgumentException("Buffer size must be greater than zero.", "count");

            return CreateOperator<T, IList<T>>(source, async (i, r, c) =>
            {
                if (!await i.MoveNext(c).ConfigureAwait(false))
                    return false;

                r.Value = new List<T>(count) { i.Current };
                for (int j = 1; j < count; ++j)
                {
                    if (!await i.MoveNext(c).ConfigureAwait(false))
                        break;
                    r.Value.Add(i.Current);
                }

                return true;
            });
        }

        public static ISequence<T> First<T>(this ISequence<T> source)
        {
            return CreateSingleResultOperator<T, T>(source, async (iter, cancel) =>
            {
                if (!await iter.MoveNext(cancel).ConfigureAwait(false))
                    throw new Exception("Sequence is empty.");
                return iter.Current;
            });
        }

        public static ISequence<T> FirstOrDefault<T>(this ISequence<T> source)
        {
            return Sequence.Create(context => Iterator.CreateSingleResultOperator<T, T>(source.Start(context), async (iter, cancel) =>
                await iter.MoveNext(cancel).ConfigureAwait(false) ? iter.Current : default(T)
            ));
        }

        public static ISequence<T> Single<T>(this ISequence<T> source)
        {
            return CreateSingleResultOperator<T, T>(source, async (iter, cancel) =>
            {
                if (!await iter.MoveNext(cancel).ConfigureAwait(false))
                    throw new Exception("Sequence is empty.");
                var result = iter.Current;
                if (await iter.MoveNext(cancel).ConfigureAwait(false))
                    throw new Exception("Sequence contains more than one element.");
                return result;
            });
        }

        public static ISequence<TAccumulate> Aggregate<T, TAccumulate>(this ISequence<T> source, TAccumulate seed, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            return CreateSingleResultOperator<T, TAccumulate>(source, async (iterator, cancel) =>
            {
                TAccumulate value = seed;
                while (true)
                {
                    var moveNextTask = iterator.MoveNext(cancel);
                    if (!moveNextTask.IsCompleted)
                    {
                        if (!await moveNextTask.ConfigureAwait(false))
                            return value;
                    }
                    else if (!moveNextTask.Result)
                        return value;
                    value = accumulator(value, iterator.Current);
                }
            });
        }

        public static ISequence<T> Aggregate<T>(this ISequence<T> source, Func<T, T, T> accumulator)
        {
            return CreateSingleResultOperator<T, T>(source, async (iterator, cancel) =>
            {
                if (!await iterator.MoveNext(cancel).ConfigureAwait(false))
                    throw new Exception("Sequence is empty.");

                T value = iterator.Current;
                while (true)
                {
                    var moveNextTask = iterator.MoveNext(cancel);
                    if (!moveNextTask.IsCompleted)
                    {
                        if (!await moveNextTask.ConfigureAwait(false))
                            return value;
                    }
                    else if (!moveNextTask.Result)
                        return value;
                    value = accumulator(value, iterator.Current);
                }
            });
        }

        public static ISequence<T> Scan<T>(this ISequence<T> source, Func<T, T, T> accumulator)
        {
            return Create(context =>
            {
                T current = default(T);
                bool accumulate = false;
                return Iterator.CreateOperator<T, T>(source.Start(context), async (iterator, result, cancel) =>
                {
                    if (!await iterator.MoveNext(cancel).ConfigureAwait(false))
                        return false;

                    if (accumulate)
                    {
                        current = accumulator(current, iterator.Current);
                    }
                    else
                    {
                        current = iterator.Current;
                        accumulate = true;
                    }

                    result.Value = current;
                    return true;
                });
            });
        }

        public static ISequence<TAccumulate> Scan<TSource, TAccumulate>(this ISequence<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            return Create(context =>
            {
                var current = seed;
                return Iterator.CreateOperator<TSource, TAccumulate>(source.Start(context), async (iterator, result, cancel) =>
                {
                    if (!await iterator.MoveNext(cancel).ConfigureAwait(false))
                        return false;
                    current = accumulator(current, iterator.Current);
                    result.Value = current;
                    return true;
                });
            });
        }

        public static ISequence<TAccumulate> ScanAsync<TSource, TAccumulate>(this ISequence<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, Task<TAccumulate>> accumulator)
        {
            return Create(context =>
            {
                var current = seed;
                return Iterator.CreateOperator<TSource, TAccumulate>(source.Start(context), async (iterator, result, cancel) =>
                {
                    if (!await iterator.MoveNext(cancel).ConfigureAwait(false))
                        return false;
                    current = await accumulator(current, iterator.Current);
                    result.Value = current;
                    return true;
                });
            });
        }

        public static ISequence<T> Min<T>(this ISequence<T> source, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            return source.Aggregate((a,b) => comparer.Compare(a, b) < 0 ? a : b);
        }

        public static ISequence<int> Min(this ISequence<int> source) { return source.Aggregate(Math.Min); }
        public static ISequence<long> Min(this ISequence<long> source) { return source.Aggregate(Math.Min); }
        public static ISequence<float> Min(this ISequence<float> source) { return source.Aggregate(Math.Min); }
        public static ISequence<double> Min(this ISequence<double> source) { return source.Aggregate(Math.Min); }
        public static ISequence<decimal> Min(this ISequence<decimal> source) { return source.Aggregate(Math.Min); }

        public static ISequence<T> Max<T>(this ISequence<T> source, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            return source.Aggregate((a, b) => comparer.Compare(a, b) > 0 ? a : b);
        }

        public static ISequence<int> Max(this ISequence<int> source) { return source.Aggregate(Math.Max); }
        public static ISequence<long> Max(this ISequence<long> source) { return source.Aggregate(Math.Max); }
        public static ISequence<float> Max(this ISequence<float> source) { return source.Aggregate(Math.Max); }
        public static ISequence<double> Max(this ISequence<double> source) { return source.Aggregate(Math.Max); }
        public static ISequence<decimal> Max(this ISequence<decimal> source) { return source.Aggregate(Math.Max); }

        public static ISequence<int> Sum(this ISequence<int> source) { return source.Aggregate(0, (a, b) => a + b); }
        public static ISequence<long> Sum(this ISequence<long> source) { return source.Aggregate(0L, (a, b) => a + b); }
        public static ISequence<float> Sum(this ISequence<float> source) { return source.Aggregate(.0f, (a, b) => a + b); }
        public static ISequence<double> Sum(this ISequence<double> source) { return source.Aggregate(.0, (a, b) => a + b); }
        public static ISequence<decimal> Sum(this ISequence<decimal> source) { return source.Aggregate(0M, (a, b) => a + b); }

        class _Avg<T>
        {
            public T Sum;
            public int Count;
        }

        public static ISequence<int> Avg(this ISequence<int> source) { return source.Aggregate(new _Avg<int>(), (a, v) => { a.Sum += v; a.Count++; return a; }).Select(x => x.Sum / x.Count); }
        public static ISequence<long> Avg(this ISequence<long> source) { return source.Aggregate(new _Avg<long>(), (a, v) => { a.Sum += v; a.Count++; return a; }).Select(x => x.Sum / x.Count); }
        public static ISequence<float> Avg(this ISequence<float> source) { return source.Aggregate(new _Avg<float>(), (a, v) => { a.Sum += v; a.Count++; return a; }).Select(x => x.Sum / x.Count); }
        public static ISequence<double> Avg(this ISequence<double> source) { return source.Aggregate(new _Avg<double>(), (a, v) => { a.Sum += v; a.Count++; return a; }).Select(x => x.Sum / x.Count); }
        public static ISequence<decimal> Avg(this ISequence<decimal> source) { return source.Aggregate(new _Avg<decimal>(), (a, v) => { a.Sum += v; a.Count++; return a; }).Select(x => x.Sum / x.Count); }

        public static ISequence<T> Skip<T>(this ISequence<T> source, int count)
        {
            return Create(context =>
            {
                int skip = count;
                return Iterator.CreateOperator<T, T>(source.Start(context), async (iterator, result, cancel) =>
                {
                    while (await iterator.MoveNext(cancel).ConfigureAwait(false))
                    {
                        if (skip <= 0)
                        {
                            result.Value = iterator.Current;
                            return true;
                        }

                        --skip;
                    }

                    return false;
                });
            });
        }

        public static ISequence<T> Take<T>(this ISequence<T> source, int count)
        {
            if (count < 0)
                throw new ArgumentException("Count must be greater or equal than 0.", "count");

            return Create(context =>
            {
                int remaining = count;
                return Iterator.CreateOperator<T, T>(source.Start(context), async (iterator, result, cancel) =>
                {
                    if (remaining > 0 && await iterator.MoveNext(cancel).ConfigureAwait(false))
                    {
                        result.Value = iterator.Current;
                        --remaining;
                        return true;
                    }

                    iterator.Dispose();
                    return false;
                });
            });
        }

        public static ISequence<T> TakeUntil<T>(this ISequence<T> source, ISequence other)
        {
            return Create(context => new TakeUntilIterator<T>(source.Start(context), other.Start(context)));
        }

        //static async Task FillQueue<T>(IIterator<T> source, AsyncProducerConsumerQueue<T> queue, CancellationToken cancel)
        //{
        //    while (await source.MoveNext(cancel).ConfigureAwait(false))
        //        await queue.EnqueueAsync(source.Current, cancel);
        //}

        public static ISequence<T> ReadAhead<T>(this ISequence<T> source, int bufferSize)
        {
            return Create(context => new MergeIterator<T>(Sequence.Return(source).Start(context), null, bufferSize));
        }

        public static ISequence<int> Range(int start, int count)
        {
            return Enumerable.Range(start, count).ToSequence();
        }

        public static ISequence<T> StartWith<T>(this ISequence<T> source, params T[] values)
        {
            return values.ToSequence().Concat(source);
        }

        public static ISequence<T> Throw<T>(Exception exception)
        {
            return Observable.Throw<T>(exception).ToSequence();
        }

        public static ISequence<T> Delay<T>(this ISequence<T> source, TimeSpan delay)
        {
            return source.SelectAsync(async (x, cancel) =>
            {
                if (delay.TotalMilliseconds < 1)
                {
                    if (delay.Ticks > 0)
                        await Task.Yield();     // Task.Delay() returns synchronously for all values < 1ms, ensure Delay() always decouples if delay.Ticks > 0
                }
                else
                {
                    await Task.Delay(delay, cancel).ConfigureAwait(false);
                }
                return x;
            });
        }

        //public static ISequence<R> Cast<T, R>(this ISequence<T> source)
        //    where T : R
        //{
        //    return source as ISequence<R> ?? Create<R>(context =>
        //    {
        //        var iterator = source.Start(context);
        //        return Iterator.Create(iterator.MoveNext, () => (R)iterator.Current, iterator.Dispose, iterator.Stop, context);
        //    });;
        //}

        public static ISequence<T> Cast<T>(this ISequence source)
        {
            return source as ISequence<T> ?? Create<T>(context =>
            {
                var iterator = source.Start(context);
                return Iterator.Create(iterator.MoveNext, () => { return (T)iterator.Current; }, iterator.Dispose, iterator.Stop, context);
            });
        }

        public static ISequence<T> OfType<T>(this ISequence source)
        {
            return source.AsObjects().Where(x => x is T).Select(x => (T)x);
        }

        public static ISequence<T>[] Broadcast<T>(this ISequence<T> source, int count)
        {
            var handler = new BroadcastHandler<T>(source, count);
            return handler.Targets;
        }

        public static ISequence<T> Share<T>(this ISequence<T> source, int maxReadAhead = 1)
        {
            if (maxReadAhead < 1)
                throw new ArgumentException("maxReadAhead must be greater or equal to 1.", "maxReadAhead");
            return new ShareSequence<T>(source, maxReadAhead);
        }

        public static ISequence<T[]> Zip<T>(this ISequence<ISequence<T>> sources)
        {
            return Create(context => new ZipIterator<T>(sources.Start(context)));
        }

        public static ISequence<T[]> ZipN<T>(params ISequence<T>[] sources)
        {
            return Create(context => new ZipNIterator<T>(sources.Select(x => x.Start(context)), context));
        }

        public static ISequence<Tuple<T1, T2>> Zip<T1, T2>(this ISequence<T1> s1, ISequence<T2> s2)
        {
            return Create(context => new ZipSelectIterator<T1, T2, Tuple<T1, T2>>(context, s1.Start(context), s2.Start(context), Tuple.Create<T1, T2>));
        }

        public static ISequence<Tuple<T1, T2, T3>> Zip<T1, T2, T3>(this ISequence<T1> s1, ISequence<T2> s2, ISequence<T3> s3)
        {
            return Create(context => new ZipSelectIterator<T1, T2, T3, Tuple<T1, T2, T3>>(context, s1.Start(context), s2.Start(context), s3.Start(context), Tuple.Create<T1, T2, T3>));
        }

        public static ISequence<Tuple<T1, T2, T3, T4>> Zip<T1, T2, T3, T4>(this ISequence<T1> s1, ISequence<T2> s2, ISequence<T3> s3, ISequence<T4> s4)
        {
            return Create(context => new ZipSelectIterator<T1, T2, T3, T4, Tuple<T1, T2, T3, T4>>(context, s1.Start(context), s2.Start(context), s3.Start(context), s4.Start(context), Tuple.Create<T1, T2, T3, T4>));
        }

        public static ISequence<Tuple<T1, T2, T3, T4, T5>> Zip<T1, T2, T3, T4, T5>(this ISequence<T1> s1, ISequence<T2> s2, ISequence<T3> s3, ISequence<T4> s4, ISequence<T5> s5)
        {
            return Create(context => new ZipSelectIterator<T1, T2, T3, T4, T5, Tuple<T1, T2, T3, T4, T5>>(context, s1.Start(context), s2.Start(context), s3.Start(context), s4.Start(context), s5.Start(context), Tuple.Create<T1, T2, T3, T4, T5>));
        }

        public static ISequence<R> Zip<T1, T2, R>(this ISequence<T1> s1, ISequence<T2> s2, Func<T1, T2, R> selector)
        {
            return Create(context => new ZipSelectIterator<T1, T2, R>(context, s1.Start(context), s2.Start(context), selector));
        }

        public static ISequence<R> Zip<T1, T2, T3, R>(this ISequence<T1> s1, ISequence<T2> s2, ISequence<T3> s3, Func<T1, T2, T3, R> selector)
        {
            return Create(context => new ZipSelectIterator<T1, T2, T3, R>(context, s1.Start(context), s2.Start(context), s3.Start(context), selector));
        }

        public static ISequence<R> Zip<T1, T2, T3, T4, R>(this ISequence<T1> s1, ISequence<T2> s2, ISequence<T3> s3, ISequence<T4> s4, Func<T1, T2, T3, T4, R> selector)
        {
            return Create(context => new ZipSelectIterator<T1, T2, T3, T4, R>(context, s1.Start(context), s2.Start(context), s3.Start(context), s4.Start(context), selector));
        }

        public static ISequence<R> Zip<T1, T2, T3, T4, T5, R>(this ISequence<T1> s1, ISequence<T2> s2, ISequence<T3> s3, ISequence<T4> s4, ISequence<T5> s5, Func<T1, T2, T3, T4, T5, R> selector)
        {
            return Create(context => new ZipSelectIterator<T1, T2, T3, T4, T5, R>(context, s1.Start(context), s2.Start(context), s3.Start(context), s4.Start(context), s5.Start(context), selector));
        }

        public static ISequence<R> ZipAsync<T1, T2, R>(this ISequence<T1> left, ISequence<T2> right, Func<T1, T2, CancellationToken, Task<R>> selector)
        {
            return Sequence.Zip(left, right)
                .SelectAsync((x, cancel) => selector(x.Item1, x.Item2, cancel));
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this ISequence<T> source, object context = null, CancellationToken cancel = default(CancellationToken))
        {
            using (var iter = source.Start(context))
            {
                if (!await iter.MoveNext(cancel).ConfigureAwait(false))
                    return default(T);
                return iter.Current;
            }
        }

        public static async Task<T> FirstAsync<T>(this ISequence<T> source, object context = null, CancellationToken cancel = default(CancellationToken))
        {
            using (var iter = source.Start(context))
            {
                if (!await iter.MoveNext(cancel).ConfigureAwait(false))
                    throw new Exception("Sequence is empty.");

                return iter.Current;
            }
        }

        public static async Task<T> LastOrDefaultAsync<T>(this ISequence<T> source, object context = null, CancellationToken cancel = default(CancellationToken))
        {
            using (var iter = source.Start(context))
            {
                if (!await iter.MoveNext().ConfigureAwait(false))
                    return default(T);

                T last = iter.Current;
                while (await iter.MoveNext(cancel).ConfigureAwait(false))
                    last = iter.Current;

                return last;
            }
        }

        public static async Task<T> LastAsync<T>(this ISequence<T> source, object context = null, CancellationToken cancel = default(CancellationToken))
        {
            using (var iter = source.Start(null))
            {
                if (!await iter.MoveNext(cancel).ConfigureAwait(false))
                    throw new Exception("Sequence is empty.");

                T last = iter.Current;
                while (await iter.MoveNext(cancel).ConfigureAwait(false))
                    last = iter.Current;

                return last;
            }
        }
    }
}
