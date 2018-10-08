using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class SelectIterator<TSource, TResult>
       : IIterator<TResult>
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        IIterator<TSource> source;
        Func<TSource, TResult> selector;
        TResult current;
        Task<bool> moveNextTask;

        public SelectIterator(IIterator<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException("buffer");

            this.source = source;
            this.selector = selector;
        }

        public TResult Current
        {
            get { return current; }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        async Task<bool> AwaitMoveNext()
        {
            try
            {
                if (!await moveNextTask.ConfigureAwait(false))
                    return false;

                cts.Token.ThrowIfCancellationRequested();
                current = selector(source.Current);
            }
            catch
            {
                Cancel();
                throw;
            }

            return true;
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            lock (selector)
            {
                if (moveNextTask != null && !moveNextTask.IsCompleted)
                    return TaskEx.Throw<bool>(new OverlappingMoveNextException());

                if (cts.IsCancellationRequested)
                    return TaskConstants.CanceledTask<bool>();

                ClearCurrent();
                moveNextTask = source.MoveNext(cancel);
                if (!moveNextTask.IsCompleted)
                    return AwaitMoveNext();

                if (moveNextTask.IsFaulted)
                {
                    Cancel();
                    return moveNextTask;
                }

                if (moveNextTask.Result)
                {
                    try
                    {
                        current = selector(source.Current);
                        return TaskConstants.True;
                    }
                    catch (Exception e)
                    {
                        return TaskEx.Throw<bool>(e);
                    }
                }
                else
                {
                    return TaskConstants.False;
                }
            }
        }

        public void Cancel()
        {
            cts.Cancel();
            source.Cancel();
        }

        public IEnumerable<Task> Stop()
        {
            Cancel();
            return source.Stop();
        }

        public object Context
        {
            get { return source.Context; }
        }

        public void Dispose()
        {
            this.Cancel();
        }

        void ClearCurrent()
        {
            current = default(TResult);
        }
    }
}
