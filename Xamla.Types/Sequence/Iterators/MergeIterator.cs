using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class MergeIterator<T>
        : IteratorBase<ISequence<T>, T>
    {
        AsyncQueue<T> queue;
        Task bindOperation;     // bind to sources
        List<Tuple<IIterator<T>, Task>> readers;
        SemaphoreSlim bindSemaphore;
        List<Task> pendingStops;

        public MergeIterator(IIterator<ISequence<T>> source, int? maxConcurrent = null, int queueLimit = 16 * 1024)
            : base(source)
        {
            this.queue = new AsyncQueue<T>(queueLimit);
            this.readers = new List<Tuple<IIterator<T>, Task>>();
            this.bindSemaphore = (maxConcurrent != null) ? new SemaphoreSlim(maxConcurrent.Value) : null;
            this.bindOperation = BindToSources(source, cts.Token);
            this.pendingStops = new List<Task>();
        }

        Task Enter(CancellationToken cancel)
        {
            return (this.bindSemaphore != null) ? this.bindSemaphore.WaitAsync(cancel) : TaskConstants.Completed;
        }

        void Leave()
        {
            if (this.bindSemaphore != null)
                this.bindSemaphore.Release();
        }

        void StopReader(IIterator<T> iterator)
        {
            pendingStops.AddRange(iterator.Stop());
            pendingStops.RemoveAll(x => x.IsCompleted && !x.IsFaulted);
        }

        async Task BindToSources(IIterator<ISequence<T>> source, CancellationToken cancel)
        {
            try
            {
                while (await source.MoveNext(cancel).ConfigureAwait(false))
                {
                    await Enter(cancel).ConfigureAwait(false);

                    Tuple<IIterator<T>, Task> entry;

                    var iterator = source.Current.Start(source.Context);
                    var readTask = Read(iterator, cancel);

                    lock (readers)
                    {
                        entry = Tuple.Create(iterator, readTask);
                        this.readers.Add(entry);
                    }

                    var t = readTask.Finally(() =>
                    {
                        lock (readers)
                        {
                            StopReader(entry.Item1);
                            readers.Remove(entry);
                        }
                        Leave();
                    });
                }

                await Task.WhenAll(this.ReaderTasks);
                queue.OnCompleted();
            }
            catch (Exception e)
            {
                queue.OnError(e);       // pass exception on to outer iterator
                throw;
            }
        }

        IEnumerable<Task> ReaderTasks
        {
            get 
            {
                lock (readers)
                {
                    return readers.Select(x => x.Item2).ToList();
                }
            }
        }

        async Task Read(IIterator<T> iterator, CancellationToken cancel)
        {
            try
            {
                while (await iterator.MoveNext(cancel).ConfigureAwait(false))
                {
                    await queue.OnNext(iterator.Current, cancel).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                queue.OnError(e);
                iterator.Cancel();
            }
        }

        protected override Task<bool> MoveNextInternal(CancellationToken cancel)
        {
            return queue.MoveNext(cancel);
        }

        public override T Current
        {
            get { return queue.Current; }
        }

        protected override IEnumerable<Task> StopInternal()
        {
            List<Task> stops;
            lock (readers)
            {
                stops = pendingStops.Where(x => !x.IsCompleted || x.IsFaulted)          // pending stops
                    .Concat(this.readers.SelectMany(x => x.Item1.Stop()))               // stop source iterators operations
                    .Concat(this.readers.Select(x => x.Item2.WhenCompleted()))          // wait for completion of read operations
                    .ToList();
                stops.Add(bindOperation);
                pendingStops.Clear();
            }
            return stops;
        }
    }
}
