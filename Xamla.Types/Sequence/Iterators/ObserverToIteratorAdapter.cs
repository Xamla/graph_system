using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class ObserverToIteratorAdapter<T>
        : IIterator<T>
        , IObserver<T>
    {
        ConcurrentQueue<T> queue;
        volatile bool completed;
        T current;
        object context;
        Exception error;
        AsyncAutoResetEvent next;
        CancellationTokenSource cancelSource;
        AsyncAutoResetEvent done;

        private ObserverToIteratorAdapter(object context)
        {
            this.context = context;
            this.queue = new ConcurrentQueue<T>();
            this.next = new AsyncAutoResetEvent(false);
            this.cancelSource = new CancellationTokenSource();
            this.done = new AsyncAutoResetEvent();
        }

        public static ObserverToIteratorAdapter<T> Create(Func<IObserver<T>, CancellationToken, Task> generator, object context)
        {
            var adapter = new ObserverToIteratorAdapter<T>(context);
            adapter.BindToGenerator(generator(adapter, adapter.CancellationToken));
            return adapter;
        }

        public void OnCompleted()
        {
            lock (queue)
            {
                completed = true;
            }
            next.Set();
        }

        public void OnError(Exception error)
        {
            lock (queue)
            {
                completed = true;
                this.error = error;
            }
            next.Set();
        }

        public void OnNext(T value)
        {
            if (completed)
            {
                System.Diagnostics.Debug.Assert(false);
                return;
            }

            queue.Enqueue(value);
            next.Set(true);
        }

        public T Current
        {
            get { return current; }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        public async Task<bool> MoveNext(CancellationToken cancel)
        {
            for (;;)
            {
                if (queue.TryDequeue(out current))
                    return true;

                lock (queue)
                {
                    if (queue.Count > 0)
                        continue;

                    if (completed)
                    {
                        if (error != null)
                            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
                        return false;
                    }
                }

                await next.WaitAsync(cancel).ConfigureAwait(false);
            }
        }

        public CancellationToken CancellationToken
        {
            get { return cancelSource.Token; }
        }

        public void BindToGenerator(Task generator)
        {
            generator.ContinueWith(t => done.Set(true), TaskContinuationOptions.ExecuteSynchronously);
        }

        public void Cancel()
        {
            cancelSource.Cancel();
        }

        public IEnumerable<Task> Stop()
        {
            Cancel();
            yield return done.WaitAsync();
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
