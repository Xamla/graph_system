using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence
{
    public interface ISequence
    {
        IIterator Start(object context);
    }

    public interface ISequence<out T>
        : ISequence
    {
        new IIterator<T> Start(object context);
    }
}
