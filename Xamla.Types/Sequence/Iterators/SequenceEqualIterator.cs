using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence.Iterators
{
    internal class SequenceEqualIterator<T>
        : IIterator<bool>
    {
        readonly object context;
        readonly IEqualityComparer<T> comparer;
        volatile bool completed;
        ISequence<T> first;
        ISequence<T> second;
        IIterator<T> firstIter;
        IIterator<T> secondIter;
        bool current;       // result of the sequence equal operation

        public SequenceEqualIterator(ISequence<T> first, ISequence<T> second, object context, IEqualityComparer<T> comparer)
        {
            this.first = first;
            this.second = second;
            this.context = context;
            this.comparer = comparer;
        }

        object IIterator.Current =>
            this.Current;

        public bool Current =>
            current;

        public async Task<bool> MoveNext(CancellationToken cancel)
        {
            if (completed)
                return false;

            // handle null cases
            if (Object.ReferenceEquals(first, second))
            {
                current = true;
                completed = true;
                return true;
            }
            else if (first == null || second == null)
            {
                current = false;
                completed = true;
                return true;
            }

            if (firstIter == null)
                firstIter = first.Start(context);
            if (secondIter == null)
                secondIter = second.Start(context);

            if (firstIter == null)
                throw new NullReferenceException("The first sequence returned a null iterator reference upon Start().");
            if (secondIter == null)
                throw new NullReferenceException("The second sequence returned a null iterator reference upon Start().");

            while (true)
            {
                var t1 = firstIter.MoveNext(cancel);
                var t2 = secondIter.MoveNext(cancel);

                bool[] waitResult = await Task.WhenAll(t1, t2).ConfigureAwait(false);

                if (waitResult[0] == waitResult[1])
                {
                    if (waitResult[0])
                    {
                        // both sequences generated a value, check equality
                        if (!comparer.Equals(firstIter.Current, secondIter.Current))
                            break;
                    }
                    else
                    {
                        // both iterators reachd the end of the sequence
                        current = true;
                        break;
                    }
                }
                else
                {
                    // only one iterator reached the end of its sequence
                    current = false;
                    break;
                }
            }

            completed = true;
            return true;
        }

        public void Cancel()
        {
            firstIter?.Cancel();
            secondIter?.Cancel();
        }

        public IEnumerable<Task> Stop()
        {
            Cancel();
            IEnumerable<Task> firstStop = firstIter?.Stop() ?? Enumerable.Empty<Task>();
            IEnumerable<Task> secondStop = secondIter?.Stop() ?? Enumerable.Empty<Task>();
            return Enumerable.Concat(firstStop, secondStop);
        }

        public object Context =>
            context;

        void IDisposable.Dispose() =>
            Cancel();
    }
}
