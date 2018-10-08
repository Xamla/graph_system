using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence.Iterators
{
    internal class RepeatUntilEmptyIterator<TSource>
        : IteratorBase<TSource, TSource>
    {
        ISequence<TSource> sourceSequence;
        bool empty;

        public RepeatUntilEmptyIterator(ISequence<TSource> sourceSequence, object context)
            : base(null, context)
        {
            this.sourceSequence = sourceSequence;
        }

        protected override async Task<bool> MoveNextInternal(CancellationToken cancel)
        {
            if (sourceSequence == null)
                return false;

            while (!empty)
            {
                if (source == null)
                {
                    empty = true;
                    source = sourceSequence.Start(context);
                }

                if (await source.MoveNext(cancel).ConfigureAwait(false))
                {
                    empty = false;
                    return true;
                }
                else
                {
                    source = null;
                }
            }

            sourceSequence = null;
            return false;
        }

        public override TSource Current
        {
            get { return source.Current; }
        }
    }
}
