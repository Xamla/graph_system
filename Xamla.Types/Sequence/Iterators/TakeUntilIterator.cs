using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Types.Sequence.Iterators
{
    internal class TakeUntilIterator<TSource>
        : IteratorBase<TSource, TSource>
    {
        IIterator trigger;
        Task<bool> triggerNext;

        public TakeUntilIterator(IIterator<TSource> source, IIterator trigger)
            : base(source)
        {
            this.trigger = trigger;
            this.triggerNext = trigger.MoveNext(cts.Token);
        }

        protected override async Task<bool> MoveNextInternal(CancellationToken cancel)
        {
            try
            {
                cancel.ThrowIfCancellationRequested();

                if (triggerNext == null)
                {
                    // no further trigger read operation pending (e.g. trigger sequence completed without producing a value)
                    return await source.MoveNext(cancel).ConfigureAwait(false);
                }
                else
                {
                    var sourceNext = source.MoveNext(cancel);

                    await Task.WhenAny<bool>(triggerNext, sourceNext).ConfigureAwait(false);

                    if (triggerNext.IsFaulted)
                        throw triggerNext.Exception;

                    if (sourceNext.IsFaulted)
                        throw sourceNext.Exception;

                    bool triggered = false;
                    if (triggerNext.IsCompleted)
                    {
                        triggered = triggerNext.Result;
                        triggerNext = null;
                        trigger.Dispose();
                    }

                    if (!triggered)
                    {
                        if (await sourceNext.ConfigureAwait(false))
                        {
                            return true;
                        }
                    }

                    trigger.Dispose();
                    source.Dispose();
                    return false;
                }
            }
            catch
            {
                trigger.Dispose();
                source.Dispose();
                throw;
            }
        }

        public override TSource Current
        {
            get { return source.Current; }
        }

        protected override IEnumerable<Task> StopInternal()
        {
            return trigger.Stop().Concat(base.StopInternal());
        }
    }
}
