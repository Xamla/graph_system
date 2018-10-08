using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence
{
    public interface IIterator
        : IDisposable
    {
        object Current { get; }
        Task<bool> MoveNext(CancellationToken cancel);
        void Cancel();
        IEnumerable<Task> Stop();
        object Context { get; }
    }

    public interface IIterator<out T>
        : IIterator
    {
        new T Current { get; }
    }
}
