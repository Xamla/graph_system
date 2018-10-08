using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Types.Sequence.Iterators;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    public class Result<T>
    {
        public T Value;
    }

    public static class Iterator
    {
        public static IIterator<T> Create<T>(Func<IObserver<T>, CancellationToken, Task> generator, object context)
        {
            return ObserverToIteratorAdapter<T>.Create(generator, context);
        }

        public static IIterator<T> Create<T>(Func<Result<T>, CancellationToken, Task<bool>> moveNext, object context, Action cancel = null)
        {
            return new SimpleIterator<T>(moveNext, context, cancel);
        }

        public static IIterator<T> Create<T>(Func<Result<T>, CancellationToken, Task<bool>> moveNext, object context, IDisposable cancel)
        {
            return new SimpleIterator<T>(moveNext, context, cancel != null ? cancel.Dispose : (Action)null);
        }

        public static IIterator<TResult> CreateOperator<TSource, TResult>(IIterator<TSource> source, Func<IIterator<TSource>, Result<TResult>, CancellationToken, Task<bool>> moveNext)
        {
            return new OperatorIterator<TSource, TResult>(source, moveNext);
        }

        public static IIterator<TResult> CreateSingleResultOperator<TSource, TResult>(IIterator<TSource> source, Func<IIterator<TSource>, CancellationToken, Task<TResult>> evaluate)
        {
            bool completed = false;
            return CreateOperator<TSource, TResult>(source, async (iter, result, cancel) =>
            {
                if (completed)
                    return false;

                result.Value = await evaluate(iter, cancel).ConfigureAwait(false);
                completed = true;
                iter.Dispose();
                return true;
            });
        }

        internal static IIterator<T> Create<T>(Func<CancellationToken, Task<bool>> moveNext, Func<T> current, Action cancel, Func<IEnumerable<Task>> stop, object context)
        {
            return new AnonymousIterator<T>(moveNext, current, cancel, stop, context);
        }

        internal static IIterator<T> Create<T>(Func<CancellationToken, Task<bool>> moveNext, Func<T> current, IDisposable cancel, object context)
        {
            return new AnonymousIterator<T>(moveNext, current, cancel.Dispose, Enumerable.Empty<Task>, context);
        }

        public static Task<bool> MoveNext<T>(this IIterator<T> source)
        {
            return source.MoveNext(CancellationToken.None);
        }

        internal static IIterator<T> ToIterator<T>(this AsyncQueue<T> queue, object context)
        {
            return new AnonymousIterator<T>(queue.MoveNext, () => queue.Current, queue.Dispose, Enumerable.Empty<Task>, context);
        }
    }
}
