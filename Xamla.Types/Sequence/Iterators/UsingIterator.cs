using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence.Iterators
{
    public class UsingIterator<T>
            : IIterator<T>
    {
        IIterator<T> source;
        IDisposable resource;

        public UsingIterator(IIterator<T> source, IDisposable resource)
        {
            this.source = source;
            this.resource = resource;
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
            return source.MoveNext(cancel);
        }

        public void Cancel()
        {
            source.Cancel();
        }

        public IEnumerable<Task> Stop()
        {
            return source.Stop();
        }

        public object Context
        {
            get { return source.Context; }
        }

        public void Dispose()
        {
            resource.Dispose();
        }
    }
}
