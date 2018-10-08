using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Types.Sequence.Iterators;
using Xamla.Utilities;

namespace Xamla.Types.Sequence
{
    class BroadcastHandler<T>
    {
        class BroadcastSequence
            : ISequence<T>
        {
            BroadcastHandler<T> parent;
            int queueIndex;

            public BroadcastSequence(BroadcastHandler<T> parent, int queueIndex)
            {
                this.parent = parent;
                this.queueIndex = queueIndex;    
            }

            public IIterator<T> Start(object context)
            {
                return parent.GetQueuesForContext(context).Start(queueIndex);
            }

            IIterator ISequence.Start(object context)
            {
                return this.Start(context);
            }
        }

        class BroadcastQueues
        {
            object gate = new object();
            ISequence<T> source;
            object context;
            CancellationTokenSource cts;
            CancellationToken cancel;
            Task readTask;
            IIterator<T> sourceIterator;
            AsyncQueue<T>[] queues;
            IIterator<T>[] iterators;
            int maxQueueLength;

            public BroadcastQueues(ISequence<T> source, object context, int count, int maxQueueLength)
            {
                this.source = source;
                this.context = context;
                
                this.maxQueueLength = maxQueueLength; 
                this.cts = new CancellationTokenSource();
                this.cancel = this.cts.Token;

                this.iterators = new IIterator<T>[count];
                this.queues = new AsyncQueue<T>[count];
                for (int i = 0; i < count; ++i)
                    this.queues[i] = new AsyncQueue<T>(maxQueueLength);         // initially a queue is created without an active iterator
            }

            public IIterator<T> Start(int queueIndex)
            {
                EnsureReading();

                lock (gate)
                {
                    var queue = queues[queueIndex];
                    if (queue == null)
                    {
                        if (readTask != null && readTask.IsCompleted)
                        {
                            if (readTask.IsFaulted)
                                return Sequence.Throw<T>(readTask.Exception).Start(context);
                            else if (readTask.IsCanceled)
                                return Sequence.Throw<T>(new OperationCanceledException()).Start(context);
                            else
                                return Sequence.Empty<T>().Start(context);
                        }

                        queue = new AsyncQueue<T>(maxQueueLength);
                        queues[queueIndex] = queue;
                    }

                    if (iterators[queueIndex] != null)
                        throw new InvalidOperationException("The broadcast target already has an active iterator.");

                    var iterator = Iterator.Create(
                        moveNext: queue.MoveNext,
                        current: () => queue.Current,
                        cancel: () =>
                        {
                            lock (gate)
                            {
                                iterators[queueIndex] = null;
                                if (queue != null)
                                {
                                    queue.Dispose();
                                    queues[queueIndex] = null;
                                }
                            }
                        },
                        stop: Stop,
                        context: context
                    );

                    iterators[queueIndex] = iterator;
                    return iterator;
                }
            }

            void EnsureReading()
            {
                lock (queues)
                {
                    if (this.readTask != null)
                        return;

                    this.readTask = Read();      // context of first started client is passed to source sequence
                }

                this.readTask.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        OnError(t.Exception);
                    }
                    else if (t.IsCompleted)
                    {
                        OnCompleted();
                    }
                    else if (t.IsCanceled)
                    {
                        OnError(new OperationCanceledException());
                    }
                });
            }

            Task ReadTask
            {
                get
                {
                    lock (queues)
                    {
                        return readTask;
                    }
                }
            }

            IEnumerable<Task> Stop()
            {
                cts.Cancel();

                var stops = new List<Task>();
                lock (queues)
                {
                    foreach (var i in iterators)
                    {
                        if (i != null)
                            i.Cancel();
                    }

                    if (sourceIterator != null)
                        stops.AddRange(sourceIterator.Stop());
                    if (readTask != null)
                        stops.Add(readTask.WhenCompleted());
                }
                return stops;
            }

            async Task Read()
            {
                try
                {
                    lock (queues)
                    {
                        sourceIterator = source.Start(context);
                    }

                    while (await sourceIterator.MoveNext(cancel))
                    {
                        try
                        {
                            await OnNext(sourceIterator.Current, cancel);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                }
                catch
                {
                    if (sourceIterator != null)
                    {
                        sourceIterator.Cancel();
                    }
                    throw;
                }
            }

            Task OnNext(T value, CancellationToken cancel)
            {
                var tasks = new Task[queues.Length];
                lock (gate)
                {
                    for (int i = 0; i < queues.Length; ++i)
                    {

                        tasks[i] = queues[i] != null ? queues[i].OnNext(value, cancel) : TaskConstants.Completed;
                    }
                }
                return Task.WhenAll(tasks);
            }

            void OnError(Exception error)
            {
                lock (gate)
                {
                    this.queues.Where(x => x != null).ForEach(x => x.OnError(error));
                }
            }

            void OnCompleted()
            {
                lock (gate)
                {
                    this.queues.Where(x => x != null).ForEach(x => x.OnCompleted());
                }
            }
        }

        static object nullContextKey = new object();

        ISequence<T> source;
        BroadcastSequence[] targets;
        int maxQueueLength;
        Dictionary<object, WeakReference<BroadcastQueues>> queuesByContext;

        public BroadcastHandler(ISequence<T> source, int count, int maxQueueLength = 1)
        {
            this.source = source;
            this.maxQueueLength = maxQueueLength;
            this.targets = new BroadcastSequence[count];
            for (int i = 0; i < count; ++i)
                this.targets[i] = new BroadcastSequence(this, i);
            this.queuesByContext = new Dictionary<object, WeakReference<BroadcastQueues>>(ReferenceComparer<object>.Instance);
        }

        public ISequence<T>[] Targets
        {
            get { return targets; }
        }

        BroadcastQueues GetQueuesForContext(object context)
        {
            BroadcastQueues broadcastSource;
            WeakReference<BroadcastQueues> weakReference;
            if (!queuesByContext.TryGetValue(context ?? nullContextKey, out weakReference) || !weakReference.TryGetTarget(out broadcastSource))
            {
                // remove all garbage collected sequences ...
                foreach (var key in queuesByContext.Where(x => { BroadcastQueues s; return !x.Value.TryGetTarget(out s); }).Select(x => x.Key).ToList())
                    queuesByContext.Remove(key);

                broadcastSource = new BroadcastQueues(source, context, this.targets.Length, maxQueueLength);
                queuesByContext.Add(context ?? nullContextKey, new WeakReference<BroadcastQueues>(broadcastSource));
            }

            return broadcastSource;
        }
    }
}
