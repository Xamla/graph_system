using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence.Iterators
{
    internal class SimpleIterator<T>
        : IteratorBase<T, T>
    {
        readonly Func<Result<T>, CancellationToken, Task<bool>> moveNext;
        readonly Result<T> result = new Result<T>();
        readonly Action cancel;

        public SimpleIterator(Func<Result<T>, CancellationToken, Task<bool>> moveNext, object context, Action cancel = null)
            : base(null, context)
        {
            this.moveNext = moveNext;
            this.cancel = cancel;
        }

        protected override Task<bool> MoveNextInternal(CancellationToken cancel) =>
            moveNext(result, cancel);

        public override T Current =>
            result.Value;

        protected override void CancelInternal()
        {
            cancel?.Invoke();
        }
    }

    internal class OperatorIterator<TSource, TResult>
        : IteratorBase<TSource, TResult>
    {
        readonly Func<IIterator<TSource>, Result<TResult>, CancellationToken, Task<bool>> moveNext;
        readonly Result<TResult> result = new Result<TResult>();

        public OperatorIterator(IIterator<TSource> source, Func<IIterator<TSource>, Result<TResult>, CancellationToken, Task<bool>> moveNext)
            : base(source)
        {
            this.moveNext = moveNext;
        }

        protected override Task<bool> MoveNextInternal(CancellationToken cancel) =>
            moveNext(this.source, result, cancel);

        public override TResult Current =>
            result.Value;
    }
}
