using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence.Iterators
{
    internal class RepeatIterator<TSource>
        : IteratorBase<TSource, TSource>
    {
        ISequence<TSource> sourceSequence;
        int? count;

        public RepeatIterator(ISequence<TSource> sourceSequence, object context, int? count = null)
            : base(null, context)
        {
            this.sourceSequence = sourceSequence;
            this.count = count;
        }

        protected override async Task<bool> MoveNextInternal(CancellationToken cancel)
        {
            if (sourceSequence == null)
                return false;

            while (!count.HasValue || count.Value > 0)
            {
                if (source == null)
                    source = sourceSequence.Start(context);

                if (await source.MoveNext(cancel).ConfigureAwait(false))
                {
                    return true;
                }
                else
                {
                    source = null;
                    if (count.HasValue)
                        count -= 1;
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
