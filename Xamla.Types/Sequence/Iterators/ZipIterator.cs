using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Sequence.Iterators
{
    internal class ZipIterator<T>
        : IteratorBase<ISequence<T>, T[]>
    {
        readonly object gate = new object();
        List<IIterator<T>> iterators;
        volatile Task<bool> bindResult;
        volatile bool completed;
        Task<bool>[] moveNextTasks;
        T[] current;

        public ZipIterator(IIterator<ISequence<T>> source)
            : base(source)
        {
            this.iterators = new List<IIterator<T>>();
        }

        async Task<bool> MoveNextInternalAsync(CancellationToken cancel)
        {
            bool[] waitResult = await Task.WhenAll(moveNextTasks).ConfigureAwait(false);

            for (int i = 0; i < waitResult.Length; ++i)
            {
                if (!waitResult[i])
                {
                    // at least one iterator reached the end of its sequence, stop here
                    completed = true;
                    ClearCurrent();
                    CancelInternal();
                    return false;
                }

                current[i] = iterators[i].Current;
            }

            return true;
        }

        async Task<bool> ContinueBindAsync(Task<List<ISequence<T>>> bindOperation, CancellationToken cancel)
        {
            var sourceSequences = await bindOperation.ConfigureAwait(false);
            this.current = new T[sourceSequences.Count];
            this.moveNextTasks = new Task<bool>[sourceSequences.Count];

            lock (gate)
            {
                cts.Token.ThrowIfCancellationRequested();
                iterators = sourceSequences.Select(x => x.Start(source.Context)).ToList();
            }

            // start the MoveNext() operations
            for (int i = 0; i < iterators.Count; ++i)
            {
                moveNextTasks[i] = iterators[i].MoveNext(cancel);
            }

            return await MoveNextInternalAsync(cancel).ConfigureAwait(false);
        }

        protected override Task<bool> MoveNextInternal(CancellationToken cancel)
        {
            if (bindResult == null)      // double-checked locking
            {
                lock (gate)
                {
                    if (cts.IsCancellationRequested)
                        return TaskEx.Throw<bool>(new Exception("Iterator was stopped"));

                    if (bindResult == null)
                    {
                        try
                        {
                            // try to bind sources synchronously
                            var bindTask = source.ToListAsync(cancel);
                            if (!bindTask.IsCompleted)
                            {
                                bindResult = ContinueBindAsync(bindTask, cancel);
                                return bindResult;
                            }
                            else if (bindTask.IsFaulted)
                            {
                                return TaskEx.Throw<bool>(bindTask.Exception.InnerExceptions);
                            }
                            else if (bindTask.IsCanceled)
                            {
                                return TaskConstants.CanceledTask<bool>();
                            }

                            // we got the sequence list synchronously, now start the source sequences and pass our own context
                            var sourceSequences = bindTask.Result;
                            this.current = new T[sourceSequences.Count];
                            this.moveNextTasks = new Task<bool>[sourceSequences.Count];

                            if (cancel.IsCancellationRequested)
                                return TaskConstants.CanceledTask<bool>();

                            iterators = sourceSequences.Select(x => x.Start(source.Context)).ToList();
                            this.bindResult = TaskConstants.True;
                        }
                        catch (Exception e)
                        {
                            return TaskEx.Throw<bool>(e);
                        }
                    }
                }
            }

            if (!bindResult.IsCompleted)
                return TaskEx.Throw<bool>(new Exception("Previous MoveNext() has not been completed."));
            else if (bindResult.IsFaulted || bindResult.IsCanceled)
                return bindResult;

            if (completed)
                return TaskConstants.False;

            try
            {
                // start all move next operations
                bool allCompletedSynchronously = true;
                for (int i = 0; i < iterators.Count; ++i)
                {
                    moveNextTasks[i] = iterators[i].MoveNext(cancel);           // exceptions during MoveNext() should be communicated through the task object
                    if (!moveNextTasks[i].IsCompleted)
                        allCompletedSynchronously = false;
                }

                if (!allCompletedSynchronously)
                {
                    // fall back to asynchronous await Task.WhenAll(moveNextTasks) ...
                    return MoveNextInternalAsync(cancel);
                }

                for (int i = 0; i < iterators.Count; ++i)
                {
                    if (moveNextTasks[i].Result)
                    {
                        current[i] = iterators[i].Current;
                    }
                    else
                    {
                        // at least one iterator reached the end of its sequence, stop here
                        completed = true;
                        ClearCurrent();
                        CancelInternal();
                        return TaskConstants.False;
                    }
                }
            }
            catch (AggregateException ae)
            {
                ClearCurrent();

                if (moveNextTasks.Any(x => x.IsFaulted))
                {
                    // one or more move next operation sgenerated an error
                    var errorList = moveNextTasks.Where(x => x.IsFaulted).SelectMany(x => x.Exception.InnerExceptions).ToArray();
                    return TaskEx.Throw<bool>(errorList);
                }
                else if (ae.InnerException is OperationCanceledException || moveNextTasks.Any(x => x.IsCanceled))
                {
                    // one move next operation was canceled
                    return TaskConstants.CanceledTask<bool>();
                }
                else
                {
                    return TaskEx.Throw<bool>(ae);
                }
            }

            return TaskConstants.True;
        }

        protected override void ClearCurrent()
        {
            // clear current array just in case somebody would try to read old values,
            // also to avoid holding references to large objects (e.g. images) longer than necessary
            Array.Clear(current, 0, current.Length);
        }

        protected override void CancelInternal()
        {
            lock (gate)
            {
                for (int i = 0; i < iterators.Count; ++i)
                    iterators[i].Cancel();
            }
        }

        public override T[] Current
        {
            get { return current; }
        }

        protected override IEnumerable<Task> StopInternal()
        {
            List<Task> stops;
            lock (gate)
            {
                stops = iterators
                    .SelectMany(x => x.Stop())    // stop source iterators operations
                    .ToList();

                if (bindResult != null && !bindResult.IsCompleted)
                    stops.Add(bindResult);
            }
            return stops;
        }
    }
}
