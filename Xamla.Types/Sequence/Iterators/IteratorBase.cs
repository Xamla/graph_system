using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    public abstract class IteratorBase<TSource, TResult>
        : IIterator<TResult>
    {
        protected CancellationTokenSource cts = new CancellationTokenSource();
        protected IIterator<TSource> source;
        protected object context;

        protected IteratorBase(IIterator<TSource> source)
            : this(source, source?.Context)
        {
        }

        protected IteratorBase(IIterator<TSource> source, object context)
        {
            this.source = source;
            this.context = context;
        }

        public abstract TResult Current
        {
            get;
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        async Task<bool> AwaitMoveNextInternal(Task<bool> moveNextTask)
        {
            try
            {
                if (await moveNextTask.ConfigureAwait(false))
                    return true;
            }
            catch
            {
                Cancel();
                throw;
            }

            return false;
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            if (cts.IsCancellationRequested)
                return TaskConstants.CanceledTask<bool>();

            var moveNextTask = MoveNextInternal(cancel);
            if (!moveNextTask.IsCompleted)
                return AwaitMoveNextInternal(moveNextTask);

            if (moveNextTask.IsFaulted)
            {
                ClearCurrent();
                Cancel();
            }

            return moveNextTask;
        }

        protected abstract Task<bool> MoveNextInternal(CancellationToken cancel);
        protected virtual void CancelInternal() {}
        protected virtual void ClearCurrent() {}
        protected virtual IEnumerable<Task> StopInternal() { return Enumerable.Empty<Task>(); }

        public void Cancel()
        {
            cts.Cancel();
            if (source != null)
                source.Cancel();
            CancelInternal();       // cancel producer tasks
        }

        public IEnumerable<Task> Stop()
        {
            Cancel();
            if (source == null)
                return StopInternal();

            return source.Stop().Concat(StopInternal());
        }

        public object Context
        {
            get { return context; }
        }

        void IDisposable.Dispose()
        {
            this.Cancel();
        }
    }
}
