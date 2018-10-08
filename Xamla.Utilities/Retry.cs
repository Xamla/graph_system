using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public delegate TimeSpan? RetryPolicy(Exception error, int retryCount);

    public static class Retry
    {
        static bool IsCriticalException(Exception error)
        {
            return error is OutOfMemoryException
                || error is NotImplementedException
                || error is InvalidCastException
                || error is NullReferenceException
                || error is ArgumentException
                || error is OperationCanceledException
                || error is ObjectDisposedException;
        }

        static TimeSpan? QuadraticBackOff(Exception error, int retryCount, Func<Exception, bool> exceptionFilter, int? maxRetries, int baseWaitMilliseconds, int maxWaitMilliseconds)
        {
            if (IsCriticalException(error))
                return null;

            if ((maxRetries.HasValue && retryCount > maxRetries.Value) || (exceptionFilter != null && !exceptionFilter(error)))
                return null;

            return TimeSpan.FromMilliseconds(Math.Min((retryCount * retryCount) * baseWaitMilliseconds, maxWaitMilliseconds));
        }

        public static RetryPolicy CreateQuadraticBackOffPolicy(Func<Exception, bool> exceptionFilter = null, int? maxRetries = 5, int baseWaitMilliseconds = 100, int maxWaitMilliseconds = 30 * 1000)
        {
            return (error, retryCount) => QuadraticBackOff(error, retryCount, exceptionFilter, maxRetries, baseWaitMilliseconds, maxWaitMilliseconds);
        }

        public static T WithPolicy<T>(Func<T> block, RetryPolicy policy)
        {
            for (int retry = 0; ; ++retry)
            {
                try
                {
                    return block();
                }
                catch (Exception e)
                {
                    var delay = policy(e, retry);
                    if (!delay.HasValue)
                        throw;

                    Thread.Sleep(delay.Value);
                }
            }
        }

        public static void WithPolicy(Action block, RetryPolicy policy)
        {
            WithPolicy<int>(() => { block(); return 0; }, policy);
        }

        public static async Task<T> WithPolicyAsync<T>(Func<CancellationToken, Task<T>> block, RetryPolicy policy, CancellationToken cancel = default(CancellationToken))
        {
            for (int retry = 0; ; ++retry)
            {
                cancel.ThrowIfCancellationRequested();

                TimeSpan? delay;
                try
                {
                    return await block(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    delay = policy(e, retry);
                    if (!delay.HasValue)
                        throw;
                }

                await Task.Delay(delay.Value, cancel);
            }
        }

        public static Task<T> WithPolicyAsync<T>(Func<Task<T>> block, RetryPolicy policy)
        {
            return WithPolicyAsync(c => block(), policy);
        }

        public static Task WithPolicyAsync(Func<CancellationToken, Task> block, RetryPolicy policy, CancellationToken cancel)
        {
            return WithPolicyAsync<int>(async c => { await block(c); return 0; }, policy, cancel);
        }

        public static Task WithPolicyAsync(Func<Task> block, RetryPolicy policy)
        {
            return WithPolicyAsync<int>(async c => { await block(); return 0; }, policy);
        }
    }
}
