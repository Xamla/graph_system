using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class AnonymousIterator<T>
        : IIterator<T>
    {
        Func<CancellationToken, Task<bool>> next;
        Func<T> current;
        Action cancel;
        Func<IEnumerable<Task>> stop;
        object context;
        bool stopped;

        public AnonymousIterator(Func<CancellationToken, Task<bool>> next, Func<T> current, Action cancel, Func<IEnumerable<Task>> stop, object context)
        {
            this.next = next;
            this.current = current;
            this.cancel = cancel;
            this.stop = stop;
            this.context = context;
        }

        public T Current
        {
            get { return current(); }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            if (stopped)
                return TaskEx.Throw<bool>(new ObjectDisposedException("AnonymousIterator", "The sequence iterator has been stopped."));

            try
            {
                return next(cancel);
            }
            catch (Exception e)
            {
                Cancel();
                return TaskEx.Throw<bool>(e);
            }
        }

        public void Cancel()
        {
            stopped = true;
            cancel();
        }

        public IEnumerable<Task> Stop()
        {
            Cancel();
            return stop();
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
