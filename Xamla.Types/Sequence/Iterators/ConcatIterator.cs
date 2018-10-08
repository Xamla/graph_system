using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class ConcatIterator<T>
        : IteratorBase<ISequence<T>, T>
    {
        object gate = new object();
        IIterator<T> inner;
        T current;

        public ConcatIterator(IIterator<ISequence<T>> source)
            : base(source)
        {
        }

        async Task<bool> AwaitMoveNextOuterAsync(Task<bool> moveNextTask, CancellationToken cancel)
        {
            if (!await moveNextTask.ConfigureAwait(false))
                return false;

            lock (gate)
            {
                cancel.ThrowIfCancellationRequested();
                inner = source.Current.Start(source.Context);
            }

            return await AwaitMoveNextInnerAsync(inner.MoveNext(cancel), cancel).ConfigureAwait(false);
        }

        async Task<bool> AwaitMoveNextInnerAsync(Task<bool> moveNextTask, CancellationToken cancel)
        {
            for (;;)
            {
                if (!moveNextTask.IsCompleted)
                {
                    if (!await moveNextTask.ConfigureAwait(false))
                        inner = null;
                }
                else if (!moveNextTask.Result)
                    inner = null;

                if (inner != null)
                {
                    current = inner.Current;
                    return true;
                }

                if (inner == null)
                {
                    moveNextTask = source.MoveNext(cancel);
                    if (!moveNextTask.IsCompleted)
                    {
                        if (!await moveNextTask.ConfigureAwait(false))
                            return false;
                    }
                    else if (!moveNextTask.Result)
                        return false;

                    lock (gate)
                    {
                        cancel.ThrowIfCancellationRequested();
                        inner = source.Current.Start(source.Context);
                    }
                }

                moveNextTask = inner.MoveNext(cancel);
            }
        }

        protected override Task<bool> MoveNextInternal(CancellationToken cancel)
        {
            Task<bool> moveNextTask;
            for (;;)
            {
                if (inner == null)
                {
                    moveNextTask = source.MoveNext(cancel);
                    if (!moveNextTask.IsCompleted)
                        return AwaitMoveNextOuterAsync(moveNextTask, cancel);
                    else if (moveNextTask.IsFaulted || moveNextTask.IsCanceled)
                        return moveNextTask;
                    else if (!moveNextTask.Result)
                        return TaskConstants.False;

                    lock (gate)
                    {
                        if (cancel.IsCancellationRequested)
                            return TaskConstants.CanceledTask<bool>();

                        inner = source.Current.Start(source.Context);
                    }
                }

                moveNextTask = inner.MoveNext(cancel);
                if (!moveNextTask.IsCompleted)
                    return AwaitMoveNextInnerAsync(moveNextTask, cancel);
                else if (moveNextTask.IsFaulted || moveNextTask.IsCanceled)
                    return moveNextTask;

                if (moveNextTask.Result)
                {
                    current = inner.Current;
                    return TaskConstants.True;
                }
                else
                {
                    inner = null;
                }
            }
        }

        public override T Current
        {
            get { return current; }
        }

        protected override void CancelInternal()
        {
            lock (gate)
            {
                if (inner != null)
                    inner.Cancel();
            }
        }

        protected override IEnumerable<Task> StopInternal()
        {
            lock (gate)
            {
                if (inner != null)
                    return inner.Stop();
            }

            return Enumerable.Empty<Task>();
        }
    }
}
