using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class EnumeratorIterator<T>
        : IIterator<T>
    {
        IEnumerator<T> source;
        object context;

        public EnumeratorIterator(IEnumerator<T> source, object context)
        {
            this.source = source;
            this.context = context;
        }

        public T Current
        {
            get { return source.Current; }
        }

        object IIterator.Current
        {
            get { return this.Current; }
        }

        public Task<bool> MoveNext(CancellationToken cancel)
        {
            cancel.ThrowIfCancellationRequested();

            return source.MoveNext() ? TaskConstants.True : TaskConstants.False;
        }

        public void Cancel()
        {
            source.Dispose();
        }

        public IEnumerable<Task> Stop()
        {
            return Enumerable.Empty<Task>();
        }

        public object Context
        {
            get { return context; }
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}
