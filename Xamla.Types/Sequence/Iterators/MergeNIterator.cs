using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class MergeNIterator<T>
        : IIterator<T>
    {
        IIterator<T>[] readers;
        Task<bool>[] pendingReads;
        int activeReaderCount;
        CancellationTokenSource cts;
        List<Task> pendingStops;

        Queue<T> available;
        T current;

        Task<bool> completedResult;
        object context;

        public MergeNIterator(IEnumerable<IIterator<T>> sources, object context)
        {
            this.readers = sources.ToArray();
            this.available = new Queue<T>(this.readers.Length);
            this.pendingReads = new Task<bool>[this.readers.Length];
            this.activeReaderCount = readers.Length;
            this.context = context;
            this.cts = new CancellationTokenSource();
            this.pendingStops = new List<Task>();
        }

        public T Current
        {
            get { return current; }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        void RemoveReader(int index)
        {
            // replace reader to remove with last element end pop tail (by setting it null)
            StopReader(readers[index]);
            --activeReaderCount;
            if (activeReaderCount > 0)
            {
                readers[index] = readers[activeReaderCount];
                pendingReads[index] = pendingReads[activeReaderCount];
            }
            readers[activeReaderCount] = null;
            pendingReads[activeReaderCount] = null;
        }

        void StopReader(IIterator<T> iterator)
        {
            pendingStops.AddRange(iterator.Stop());
            pendingStops.RemoveAll(x => x.IsCompleted && !x.IsFaulted);
        }

        bool ProcessReadResults()
        {
            var cancel = cts.Token;

            lock (readers)
            {
                List<Exception> errors = null;

                int i = 0;
                while (i < activeReaderCount && !cancel.IsCancellationRequested)
                {
                    if (pendingReads[i] == null)
                        pendingReads[i] = readers[i].MoveNext(cancel);

                    var read = pendingReads[i];
                    if (!read.IsCompleted)                              // read result not yet available
                    {
                        ++i;
                    }
                    else if (read.IsFaulted)                            // source faulted
                    {
                        errors = read.Exception.InnerExceptions.ToList();
                        RemoveReader(i);
                        break;
                    }
                    else if (read.IsCanceled || !read.Result)           // source has no more data or read was canceled
                    {
                        RemoveReader(i);
                    }
                    else
                    {
                        available.Enqueue(readers[i].Current);

                        pendingReads[i] = readers[i].MoveNext(cancel);
                        ++i;
                    }
                }

                if (errors != null || cancel.IsCancellationRequested)
                {
                    Cancel();
                    available.Clear();
                    current = default(T);
                    completedResult = errors != null ? TaskEx.Throw<bool>(errors) : TaskConstants.CanceledTask<bool>();
                    return false;
                }

                return true;
            }
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            if (completedResult != null)
                return completedResult;

            if (available.Count == 0 && activeReaderCount > 0)
            {
                // underflow, try to fetch some input elements...
                if (!ProcessReadResults())
                {
                    return completedResult;
                }
            }

            if (available.Count > 0)
            {
                current = available.Dequeue();
                return TaskConstants.True;
            }

            // no more results synchronously available... check if any active readers are left
            if (activeReaderCount == 0)
            {
                completedResult = TaskConstants.False;
                return completedResult;
            }

            return Task.WhenAny(pendingReads.Take(activeReaderCount))
                .ContinueWith<Task<bool>>((t, o) => ((MergeNIterator<T>)o).MoveNext(cancel), this, cancel, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current)
                .Unwrap();
        }

        public void Cancel()
        {
            cts.Cancel();

            lock (readers)
            {
                completedResult = TaskConstants.CanceledTask<bool>();

                for (int i = 0; i < activeReaderCount; ++i)
                    readers[i].Cancel();
            }
        }

        public IEnumerable<Task> Stop()
        {
            Cancel();
            current = default(T);
            var stopTasks = new List<Task>();

            lock (readers)
            {
                stopTasks.AddRange(pendingStops.Where(x => !x.IsCompleted || x.IsFaulted));
                pendingStops.Clear();
                for (int i = 0; i < activeReaderCount; ++i)
                {
                    if (pendingReads[i] != null && !pendingReads[i].IsCompleted)
                        stopTasks.Add(pendingReads[i]);         // ensure all pending reads return
                    stopTasks.AddRange(readers[i].Stop());
                }
            }

            return stopTasks;
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
