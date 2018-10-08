using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public static class ObservableExtensions
    {
        public static IObservable<Unit> AsUnits<TSource>(this IObservable<TSource> source)
        {
            return source.Select(x => Unit.Default);
        }

        public static IObservable<object> AsObjects<TSource>(this IObservable<TSource> source)
        {
            return source.Select(x => (object)x);
        }

        public static IObservable<Unit> Completed<TSource>(this IObservable<TSource> source)
        {
            return source.All(x => true).Select(x => Unit.Default);
        }

        public static IObservable<T> ToObservable<T>(this CancellationToken cancel)
        {
            return cancel.CanBeCanceled ? Observable.Create<T>(o => cancel.Register(() => o.OnError(new OperationCanceledException()))) : Observable.Never<T>();
        }

        public static IObservable<T> ThrottleResponsive<T>(this IObservable<T> source, TimeSpan minInterval)
        {
            return Observable.Create<T>(o =>
            {
                object gate = new Object();
                Notification<T> last = null, lastNonTerminal = null;
                DateTime referenceTime = DateTime.UtcNow - minInterval;

                var delayedReplay = new SerialDisposable();
                return new CompositeDisposable(source.Materialize().Subscribe(x =>
                {
                    lock (gate)
                    {
                        var elapsed = DateTime.UtcNow - referenceTime;
                        if (elapsed >= minInterval && delayedReplay.Disposable == null)
                        {
                            referenceTime = DateTime.UtcNow;
                            x.Accept(o);
                        }
                        else
                        {
                            if (x.Kind == NotificationKind.OnNext)
                                lastNonTerminal = x;
                            last = x;
                            if (delayedReplay.Disposable == null)
                            {
                                delayedReplay.Disposable = Scheduler.Default.Schedule(minInterval - elapsed, () =>
                                {
                                    lock (gate)
                                    {
                                        referenceTime = DateTime.UtcNow;
                                        if (lastNonTerminal != null && lastNonTerminal != last)
                                            lastNonTerminal.Accept(o);
                                        last.Accept(o);
                                        last = lastNonTerminal = null;
                                        delayedReplay.Disposable = null;
                                    }
                                });
                            }
                        }
                    }
                }), delayedReplay);
            });
        }

        public static IObservable<T2> SelectManySequential<T1, T2>(this IObservable<T1> source, Func<T1, IObservable<T2>> selector)
        {
            return source
                .Select(x => Observable.Defer<T2>(() => selector(x)))
                .Concat();
        }

        public static IObservable<T2> SelectManySequential<T1, T2>(this IObservable<T1> source, Func<T1, Task<IObservable<T2>>> selector)
        {
            return source
                .Select(x => Observable.Defer<T2>(() => selector(x)))
                .Concat();
        }

        public static IObservable<TSource> RepeatUntilEmpty<TSource>(this IObservable<TSource> source)
        {
            return Observable.Create<TSource>(o =>
            {
                var subscription = new SerialDisposable();
                Action loop = null;
                loop = () =>
                {
                    bool empty = true;
                    subscription.Disposable = source.Subscribe(
                        onNext: n => { empty = false; o.OnNext(n); },
                        onError: o.OnError,
                        onCompleted: () => { if (!empty) loop(); else o.OnCompleted(); }
                    );
                };
                loop();
                return subscription;
            });
        }

        public static IObservable<TSource> CatchAndRetry<TSource, TException>(
            this IObservable<TSource> source,
            Func<TException /* error */, IObservable<TSource> /* originalSource */, int /* failureCount */, IObservable<TSource>> errorHandler,
            int maxRetries,
            IScheduler scheduler = null
        )
            where TException : Exception
        {
            return source.CatchAndRetry<TSource, TException>((e, s, i) => i < maxRetries ? s : null, scheduler);
        }


        public static IObservable<TSource> CatchAndRetry<TSource, TException>(
               this IObservable<TSource> source,
               Func<TException /* error */, IObservable<TSource> /* originalSource */, int /* failureCount */, IObservable<TSource>> delayFactory,
               IScheduler scheduler = null
           )
           where TException : Exception
        {
            return Observable.Create<TSource>(observer =>
            {
                scheduler = scheduler ?? Scheduler.CurrentThread;
                var originalScoure = source;
                var sourceDisposable = new SerialDisposable();
                int retryCount = 0;

                var scheduleDisposable = scheduler.Schedule(
                self =>
                {
                    var oldSubscription = sourceDisposable.Disposable;
                    var newSubscription = source.Subscribe(
                        onNext: x => { observer.OnNext(x); if (retryCount > 0) retryCount = 0; },
                        onError: ex =>
                        {
                            var typedException = ex as TException;
                            if (typedException != null)
                            {
                                source = delayFactory(typedException, originalScoure, ++retryCount);
                                if (source != null)
                                {
                                    self();
                                }
                                else
                                {
                                    observer.OnError(ex);
                                }
                            }
                            else
                            {
                                observer.OnError(ex);
                            }
                        },
                        onCompleted: observer.OnCompleted
                    );

                    if (sourceDisposable.Disposable == oldSubscription)
                        sourceDisposable.Disposable = newSubscription;
                });

                return new CompositeDisposable(scheduleDisposable, sourceDisposable);
            });
        }

        public static IObservable<TSource> RetryWithBackoff<TSource, TException>(
            this IObservable<TSource> source,
            Func<TException, int, TimeSpan?> backOffStrategy,
            IScheduler scheduler = null
        )
        where TException : Exception
        {
            scheduler = scheduler ?? Scheduler.Default;
            return source.CatchAndRetry<TSource, TException>(
                (error, originalSource, failureCount) =>
                {
                    TimeSpan? delay = backOffStrategy(error, failureCount);
                    if (!delay.HasValue)
                        return null;

                    return Observable.Timer(delay.Value, scheduler).SelectMany(x => originalSource);
                },
                scheduler
            );
        }

        public static IObservable<TSource> DefaultIfEmpty<TSource>(this IObservable<TSource> source, IObservable<TSource> defaultSource)
        {
            return Observable.Create<TSource>(o =>
            {
                var subscription = new SingleAssignmentDisposable();
                bool empty = true;
                subscription.Disposable = source.Subscribe(
                    onNext: n => { empty = false; o.OnNext(n); },
                    onError: o.OnError,
                    onCompleted: () =>
                    {
                        if (!empty)
                            o.OnCompleted();
                        else
                            subscription.Disposable = defaultSource.Subscribe(o);
                    }
                );
                return subscription;
            });
        }


        public static IObservable<T> StartWithSyncSubscribe<T>(this IObservable<T> eventSource, IList<T> startBuffer, object gate)
        {
            return Observable.Create<T>(o =>
            {
                lock (gate)
                {
                    foreach (var x in startBuffer)
                        o.OnNext(x);
                    return eventSource.Subscribe(o);
                }
            });
        }

        public static IObservable<T> StartWithSyncSubscribe<T>(this IObservable<T> eventSource, Func<T> syncValueGetter, object gate)
        {
            return Observable.Create<T>(o =>
            {
                lock (gate)
                {
                    var value = syncValueGetter();
                    o.OnNext(value);
                    return eventSource.Subscribe(o);
                }
            });
        }
    }
}
