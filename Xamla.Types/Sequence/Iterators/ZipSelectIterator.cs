using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class ZipSelectIterator<T1, T2, R>
        : ZipSelectIteratoBase<R>
    {
        IIterator<T1> iterator1;
        IIterator<T2> iterator2;
        Func<T1, T2, R> selector;

        public ZipSelectIterator(object context, IIterator<T1> iterator1, IIterator<T2> iterator2, Func<T1, T2, R> selector)
            : base(context, 2)
        {
            this.iterator1 = iterator1;
            this.iterator2 = iterator2;
            this.selector = selector;
        }

        protected override void StartMoveNextOperations(CancellationToken cancel)
        {
            moveNextTasks[0] = iterator1.MoveNext(cancel);
            moveNextTasks[1] = iterator2.MoveNext(cancel);
        }

        protected override R Select()
        {
            return selector(iterator1.Current, iterator2.Current);
        }

        public override void Cancel()
        {
            base.Cancel();
            iterator1.Cancel();
            iterator2.Cancel();
        }

        public override IEnumerable<Task> Stop()
        {
            Cancel();
            return iterator1.Stop()
                .Concat(iterator2.Stop());
        }
    }

    internal class ZipSelectIterator<T1, T2, T3, R>
       : ZipSelectIteratoBase<R>
    {
        IIterator<T1> iterator1;
        IIterator<T2> iterator2;
        IIterator<T3> iterator3;
        Func<T1, T2, T3, R> selector;

        public ZipSelectIterator(object context, IIterator<T1> iterator1, IIterator<T2> iterator2, IIterator<T3> iterator3, Func<T1, T2, T3, R> selector)
            : base(context, 3)
        {
            this.iterator1 = iterator1;
            this.iterator2 = iterator2;
            this.iterator3 = iterator3;
            this.selector = selector;
        }

        protected override void StartMoveNextOperations(CancellationToken cancel)
        {
            moveNextTasks[0] = iterator1.MoveNext(cancel);
            moveNextTasks[1] = iterator2.MoveNext(cancel);
            moveNextTasks[2] = iterator3.MoveNext(cancel);
        }

        protected override R Select()
        {
            return selector(iterator1.Current, iterator2.Current, iterator3.Current);
        }

        public override void Cancel()
        {
            base.Cancel();
            iterator1.Cancel();
            iterator2.Cancel();
            iterator3.Cancel();
        }

        public override IEnumerable<Task> Stop()
        {
            Cancel();
            return iterator1.Stop()
                .Concat(iterator2.Stop())
                .Concat(iterator3.Stop());
        }
    }

    internal class ZipSelectIterator<T1, T2, T3, T4, R>
       : ZipSelectIteratoBase<R>
    {
        IIterator<T1> iterator1;
        IIterator<T2> iterator2;
        IIterator<T3> iterator3;
        IIterator<T4> iterator4;
        Func<T1, T2, T3, T4, R> selector;

        public ZipSelectIterator(object context, IIterator<T1> iterator1, IIterator<T2> iterator2, IIterator<T3> iterator3, IIterator<T4> iterator4, Func<T1, T2, T3, T4, R> selector)
            : base(context, 4)
        {
            this.iterator1 = iterator1;
            this.iterator2 = iterator2;
            this.iterator3 = iterator3;
            this.iterator4 = iterator4;
            this.selector = selector;
        }

        protected override void StartMoveNextOperations(CancellationToken cancel)
        {
            moveNextTasks[0] = iterator1.MoveNext(cancel);
            moveNextTasks[1] = iterator2.MoveNext(cancel);
            moveNextTasks[2] = iterator3.MoveNext(cancel);
            moveNextTasks[3] = iterator4.MoveNext(cancel);
        }

        protected override R Select()
        {
            return selector(iterator1.Current, iterator2.Current, iterator3.Current, iterator4.Current);
        }

        public override void Cancel()
        {
            base.Cancel();
            iterator1.Cancel();
            iterator2.Cancel();
            iterator3.Cancel();
            iterator4.Cancel();
        }

        public override IEnumerable<Task> Stop()
        {
            Cancel();
            return iterator1.Stop()
                .Concat(iterator2.Stop())
                .Concat(iterator3.Stop())
                .Concat(iterator4.Stop());
        }
    }

    internal class ZipSelectIterator<T1, T2, T3, T4, T5, R>
       : ZipSelectIteratoBase<R>
    {
        IIterator<T1> iterator1;
        IIterator<T2> iterator2;
        IIterator<T3> iterator3;
        IIterator<T4> iterator4;
        IIterator<T5> iterator5;
        Func<T1, T2, T3, T4, T5, R> selector;

        public ZipSelectIterator(object context, IIterator<T1> iterator1, IIterator<T2> iterator2, IIterator<T3> iterator3, IIterator<T4> iterator4, IIterator<T5> iterator5, Func<T1, T2, T3, T4, T5, R> selector)
            : base(context, 5)
        {
            this.iterator1 = iterator1;
            this.iterator2 = iterator2;
            this.iterator3 = iterator3;
            this.iterator4 = iterator4;
            this.iterator5 = iterator5;
            this.selector = selector;
        }

        protected override void StartMoveNextOperations(CancellationToken cancel)
        {
            moveNextTasks[0] = iterator1.MoveNext(cancel);
            moveNextTasks[1] = iterator2.MoveNext(cancel);
            moveNextTasks[2] = iterator3.MoveNext(cancel);
            moveNextTasks[3] = iterator4.MoveNext(cancel);
            moveNextTasks[4] = iterator5.MoveNext(cancel);
        }

        protected override R Select()
        {
            return selector(iterator1.Current, iterator2.Current, iterator3.Current, iterator4.Current, iterator5.Current);
        }

        public override void Cancel()
        {
            base.Cancel();
            iterator1.Cancel();
            iterator2.Cancel();
            iterator3.Cancel();
            iterator4.Cancel();
            iterator5.Cancel();
        }

        public override IEnumerable<Task> Stop()
        {
            Cancel();
            return iterator1.Stop()
                .Concat(iterator2.Stop())
                .Concat(iterator3.Stop())
                .Concat(iterator4.Stop())
                .Concat(iterator5.Stop());
        }
    }

    internal abstract class ZipSelectIteratoBase<T>
        : IIterator<T>
    {
        protected object gate = new object();
        CancellationTokenSource cts;
        object context;
        volatile bool completed;
        protected Task<bool>[] moveNextTasks;
        protected T current;

        public ZipSelectIteratoBase(object context, int count)
        {
            this.context = context;
            this.moveNextTasks = new Task<bool>[count];
            this.cts = new CancellationTokenSource();
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
                }

                cts.Token.ThrowIfCancellationRequested();
                current = Select();

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
            current = default(T);
            Cancel();
        }

        protected abstract void StartMoveNextOperations(CancellationToken cancel);
        protected abstract T Select();

        public T Current
        {
            get { return current; }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        bool AllMoveNextCallsCompleted()
        {
            for (int i = 0; i < moveNextTasks.Length; ++i)
            {
                if (moveNextTasks[i] != null && !moveNextTasks[i].IsCompleted)
                    return false;
            }
            return true;
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            lock (gate)
            {
                if (completed)
                    return TaskConstants.False;

                if (!AllMoveNextCallsCompleted())
                    return TaskEx.Throw<bool>(new OverlappingMoveNextException());

                current = default(T);

                if (cts.IsCancellationRequested || cancel.IsCancellationRequested)
                    return TaskConstants.CanceledTask<bool>();

                try
                {
                    // start all move next operations
                    StartMoveNextOperations(cancel);

                    // check if all MoveNext() operations executed synchronously
                    if (!AllMoveNextCallsCompleted())
                    {
                        // fall back to asynchronous await Task.WhenAll(moveNextTasks) ...
                        return MoveNextInternalAsync(cancel);
                    }

                    for (int i = 0; i < moveNextTasks.Length; ++i)
                    {
                        if (!moveNextTasks[i].Result)
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

                try
                {
                    if (cts.IsCancellationRequested)
                        return TaskConstants.CanceledTask<bool>();

                    current = Select();
                }
                catch (Exception error)
                {
                    return TaskEx.Throw<bool>(error);
                }

                return TaskConstants.True;
            }
        }

        public virtual void Cancel()
        {
            this.cts.Cancel();
        }

        public abstract IEnumerable<Task> Stop();
        
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
