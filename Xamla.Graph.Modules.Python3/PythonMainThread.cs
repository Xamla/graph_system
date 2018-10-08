using Python.Runtime;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.Python3
{
    public interface IPythonMainThread : IDisposable
    {
        Task<object> Enqueue(Func<CancellationToken, object> action, CancellationToken cancel = default(CancellationToken));
    }

    public static class PythonMainThreadExtensions
    {
        public static Task Enqueue(this IPythonMainThread thread, Action action, CancellationToken cancel = default(CancellationToken)) =>
            thread.Enqueue(_ => { action(); return null; }, cancel);

        public static void RunSync(this IPythonMainThread thread, Action action) =>
            thread.Enqueue(action).Wait();

        public static object EvalSync(this IPythonMainThread thread, Func<object> func) =>
            thread.Enqueue(_ => func()).Result;
    }

    public class PythonMainThread : IPythonMainThread
    {
        EventLoopScheduler scheduler;
        IntPtr pythonThreadState;

        public PythonMainThread()
        {
            scheduler = new EventLoopScheduler();
        }

        public Task<object> Enqueue(Func<CancellationToken, object> func, CancellationToken cancel = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            var finishedDisposable = new SingleAssignmentDisposable();
            bool running = false;
            var disposable = scheduler.Schedule(() =>
            {
                lock (tcs)
                {
                    if (tcs.Task.IsCanceled)
                        return;
                    running = true; // track that execution already began
                }

                using (finishedDisposable)
                {
                    try
                    {
                        if (pythonThreadState != IntPtr.Zero)
                        {
                            PythonEngine.EndAllowThreads(pythonThreadState);
                            pythonThreadState = IntPtr.Zero;
                        }

                        object result = null;
                        try
                        {
                            result = func(cancel);
                        }
                        finally
                        {
                            if (PythonEngine.IsInitialized)
                            {
                                pythonThreadState = PythonEngine.BeginAllowThreads();
                            }
                        }

                        tcs.TrySetResult(result);
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                }
            });
            finishedDisposable.Disposable = cancel.Register(() =>
            {
                disposable.Dispose();   // no longer schedule task

                lock (tcs)
                {
                    // check if execution already began
                    if (!running)
                    {
                        tcs.TrySetCanceled();
                    }
                }
            });

            return tcs.Task;
        }

        public void Dispose()
        {
            scheduler.Dispose();
        }
    }
}
