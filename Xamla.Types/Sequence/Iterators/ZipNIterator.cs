using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class ZipNIterator<T>
        : IIterator<T[]>
    {
        readonly object context;
        IIterator<T>[] iterators;
        volatile bool completed;
        Task<bool>[] moveNextTasks;
        T[] current;

        public ZipNIterator(IEnumerable<IIterator<T>> iterators, object context)
        {
            this.context = context;
            this.iterators = iterators.ToArray();
            this.moveNextTasks = new Task<bool>[this.iterators.Length];
            this.current = new T[this.iterators.Length];
        }

        async Task<bool> MoveNextInternalAsync(CancellationToken cancel)
        {
            try
            {
                bool[] waitResult = await Task.WhenAll(moveNextTasks).ConfigureAwait(false);

                for (int i = 0; i < waitResult.Length; ++i)
                {
                    if (!waitResult[i])
                    {
                        // at least one iterator reached the end of its sequence, stop here
                        completed = true;
                        ClearCurrentAndStop();
                        return false;
                    }

                    current[i] = iterators[i].Current;
                }

                return true;
            }
            catch
            {
                ClearCurrentAndStop();
                throw;
            }
        }

        void ClearCurrentAndStop()
        {
            // clear current array just in case somebody would try to read old values,
            // also to avoid holding references to large objects (e.g. images) longer than necessary
            Array.Clear(current, 0, current.Length);
            Cancel();
        }

        public T[] Current
        {
            get { return current; }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            if (completed)
                return TaskConstants.False;

            try
            {
                // start all move next operations
                bool allCompletedSynchronously = true;
                for (int i = 0; i < iterators.Length; ++i)
                {
                    moveNextTasks[i] = iterators[i].MoveNext(cancel);           // exceptions during MoveNext() should be communicated through the task object
                    if (!moveNextTasks[i].IsCompleted)
                        allCompletedSynchronously = false;
                }

                if (!allCompletedSynchronously)
                {
                    // fall back to asynchronous await Task.WhenAll(moveNextTasks) ...
                    return MoveNextInternalAsync(cancel);
                }

                for (int i = 0; i < iterators.Length; ++i)
                {
                    if (moveNextTasks[i].Result)
                    {
                        current[i] = iterators[i].Current;
                    }
                    else
                    {
                        // at least one iterator reached the end of its sequence, stop here
                        completed = true;
                        ClearCurrentAndStop();
                        return TaskConstants.False;
                    }
                }
            }
            catch (AggregateException ae)
            {
                ClearCurrentAndStop();

                if (moveNextTasks.Any(x => x.IsFaulted))
                {
                    // one or more move next operation sgenerated an error
                    var errorList = moveNextTasks.Where(x => x.IsFaulted).SelectMany(x => x.Exception.InnerExceptions).ToArray();
                    return TaskEx.Throw<bool>(errorList);
                }
                else if (ae.InnerException is OperationCanceledException || moveNextTasks.Any(x => x.IsCanceled))
                {
                    // one move next operation was canceled
                    return TaskConstants.CanceledTask<bool>();
                }
                else
                {
                    return TaskEx.Throw<bool>(ae);
                }
            }

            return TaskConstants.True;
        }

        public void Cancel()
        {
            for (int i = 0; i < iterators.Length; ++i)
                iterators[i].Cancel();
        }

        public IEnumerable<Task> Stop()
        {
            Cancel();
            return iterators.SelectMany(x => x.Stop()).ToList();
        }

        public object Context
        {
            get { return context; }
        }

        void IDisposable.Dispose()
        {
            Cancel();
        }
    }
}
