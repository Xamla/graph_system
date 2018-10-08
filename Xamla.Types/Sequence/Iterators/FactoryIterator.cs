using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    class FactoryIterator<T>
        : IIterator<T>
    {
        Func<object, CancellationToken, Task<T>> factory;
        Task<T> producerTask;
        object context;
        int remaining;

        public FactoryIterator(Func<T> factory, object context, int count = 1)
            : this((ctx, cancel) => Task.FromResult(factory()), context, count)
        {
        }

        public FactoryIterator(Func<Task<T>> factory, object context, int count = 1)
            : this((ctx, cancel) => factory(), context, count)
        {
        }

        public FactoryIterator(Func<object, CancellationToken, Task<T>> factory, object context, int count = 1)
        {
            this.factory = factory;
            this.context = context;
            this.remaining = count;
        }

        public T Current
        {
            get { return producerTask.Result; }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        async Task<bool> WaitResultAsync()
        {
            await producerTask.ConfigureAwait(false);
            return true;
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            lock (factory)
            {
                if (producerTask != null && !producerTask.IsCompleted)
                    return TaskEx.Throw<bool>(new OverlappingMoveNextException());

                if (remaining <= 0)
                {
                    producerTask = null;
                    return TaskConstants.False;
                }

                --remaining;
                producerTask = factory(context, cancel);
                if (producerTask.IsCompleted && !producerTask.IsFaulted && !producerTask.IsCanceled)
                    return TaskConstants.True;

                return WaitResultAsync();
            }
        }

        public void Cancel()
        {
        }

        public IEnumerable<Task> Stop()
        {
            lock (factory)
            {
                if (producerTask != null && !producerTask.IsCompleted)
                    yield return producerTask;
            }
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
