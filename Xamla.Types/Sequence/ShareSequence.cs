using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Types.Sequence.Iterators;
using Xamla.Utilities;

namespace Xamla.Types.Sequence
{
    class ShareSequence<T>
        : ISequence<T>
    {
        class ShareSource
        {
            public IIterator<T> SourceIterator;
            public AsyncQueue<T> Queue;
            public Task readTask;

            public ShareSource(IIterator<T> sourceIterator, int maxQueueLength)
            {
                this.SourceIterator = sourceIterator;
                this.Queue = new AsyncQueue<T>(maxQueueLength);
                this.readTask = Read();
                readTask.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Queue.OnError(t.Exception);
                    }
                    else if (t.IsCompleted)
                    {
                        Queue.OnCompleted();
                    }
                    else if (t.IsCanceled)
                    {
                        Queue.Dispose();
                    }
                });
            }

            public IIterator<T> Start()
            {
                return new ShareIterator(this);
            }

            async Task Read()
            {
                try
                {
                    while (await SourceIterator.MoveNext(CancellationToken.None).ConfigureAwait(false))
                    {
                        try
                        {
                            await Queue.OnNext(SourceIterator.Current, CancellationToken.None).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                }
                catch
                {
                    if (SourceIterator != null)
                    {
                        SourceIterator.Cancel();
                    }
                    throw;
                }
            }
        }

        class ShareIterator
            : IteratorBase<T, T>
        {
            ShareSource shareSource;

            public ShareIterator(ShareSource shareSource)
                : base(shareSource.SourceIterator)
            {
                this.shareSource = shareSource;
            }

            protected override Task<bool> MoveNextInternal(CancellationToken cancel)
            {
                return shareSource.Queue.MoveNext(cancel);
            }

            public override T Current
            {
                get { return shareSource.Queue.Current; }
            }

            protected override IEnumerable<Task> StopInternal()
            {
                shareSource.Queue.Dispose();
                return EnumerableEx.Return(TaskConstants.Completed);
            }
        }

        static object nullContextKey = new object();

        ISequence<T> source;
        int maxQueueLength;
        Dictionary<object, WeakReference<ShareSource>> shareByContext;
        
        public ShareSequence(ISequence<T> source, int maxQueueLength = 1)
        {
            this.source = source;
            this.maxQueueLength = maxQueueLength;
            this.shareByContext = new Dictionary<object, WeakReference<ShareSource>>(ReferenceComparer<object>.Instance);
        }

        public IIterator<T> Start(object context)
        {
            lock (shareByContext)
            {
                ShareSource shareSource;
                WeakReference<ShareSource> weakReference;
                if (!shareByContext.TryGetValue(context ?? nullContextKey, out weakReference) || !weakReference.TryGetTarget(out shareSource))
                {
                    // remove all garbage collected sequences ...
                    foreach (var key in shareByContext.Where(x => { ShareSource s; return !x.Value.TryGetTarget(out s); }).Select(x => x.Key).ToList())
                        shareByContext.Remove(key);

                    shareSource = new ShareSource(source.Start(context), maxQueueLength);
                    shareByContext.Add(context ?? nullContextKey, new WeakReference<ShareSource>(shareSource));
                }

                Debug.Assert(shareSource.SourceIterator.Context == context);

                return shareSource.Start();
            }
        }

        IIterator ISequence.Start(object context)
        {
            return this.Start(context);
        }
    }
}
