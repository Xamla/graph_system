using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class AsyncManualResetEvent
    {
        object gate = new object();
        List<TaskCompletionSource<object>> list;
        bool signaled;

        public AsyncManualResetEvent(bool initialState = false)
        {
            this.signaled = initialState;
        }

        public bool IsSignaled
        {
            get { lock (gate) return signaled; }
        }

        public Task WaitAsync(CancellationToken cancel)
        {
            lock (gate)
            {
                if (signaled)
                    return TaskConstants.Completed;

                if (cancel.IsCancellationRequested)
                    return TaskConstants.Canceled;

                var tcs = new TaskCompletionSource<object>();

                if (list == null)
                    list = new List<TaskCompletionSource<object>>();

                list.Add(tcs);

                if (cancel.CanBeCanceled)
                {
                    var registration = cancel.Register(() =>
                    {
                        lock (gate)
                        {
                            if (list != null)
                                list.Remove(tcs);
                        }
                        tcs.TrySetCanceled();
                    });
                    tcs.Task.Finally(registration.Dispose);
                }

                return tcs.Task;
            }
        }

        public Task WaitAsync()
        {
            return WaitAsync(CancellationToken.None);
        }

        private static void Release(List<TaskCompletionSource<object>> release, bool useTaskPool)
        {
            if (release == null)
                return;

            if (useTaskPool)
            {
                Task.Factory.StartNew(s => ((List<TaskCompletionSource<object>>)s).ForEach(x => x.TrySetResult(null)),
                    release,
                    CancellationToken.None,
                    TaskCreationOptions.PreferFairness,
                    TaskScheduler.Default
                );
            }
            else
            {
                release.ForEach(x => x.TrySetResult(null));
            }
        }

        public void Set(bool useTaskPool = false)
        {
            List<TaskCompletionSource<object>> release;

            lock (gate)
            {
                signaled = true;
                release = list;
                list = null;
            }

            Release(release, useTaskPool);
        }

        public void Pulse(bool useTaskPool = false)
        {
            List<TaskCompletionSource<object>> release;

            lock (gate)
            {
                release = list;
                list = null;
            }

            Release(release, useTaskPool);
        }

        public void Reset()
        {
            lock (gate)
            {
                signaled = false;
            }
        }
    }
}
