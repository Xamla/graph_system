using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Security.Cryptography;

namespace Xamla.Utilities
{
    public static class ShaExtensions
    {
        public static byte[] Sha256(this IEnumerable<ArraySegment<byte>> source)
        {
            using (var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                foreach (var s in source)
                    hash.AppendData(s.Array, s.Offset, s.Count);

                return hash.GetHashAndReset();
            }
        }

        public static byte[] Sha256(this IEnumerable<byte[]> source)
        {
            return source.Select(x => new ArraySegment<byte>(x)).Sha256();
        }

        public static IObservable<byte[]> Sha256(this IObservable<ArraySegment<byte>> source)
        {
            return Observable.Create<byte[]>(o =>
            {
                var disposable = new CompositeDisposable(2);
                var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
                disposable.Add(hash);

                var subscription = source.Subscribe(
                    onNext: x => hash.AppendData(x.Array, x.Offset, x.Count),
                    onError: o.OnError,
                    onCompleted: () =>
                    {
                        o.OnNext(hash.GetHashAndReset());
                        o.OnCompleted();
                    }
                );

                disposable.Add(subscription);
                return disposable;
            });
        }

        public static IObservable<byte[]> Sha256(this IObservable<byte[]> source)
        {
            return source.Select(x => new ArraySegment<byte>(x)).Sha256();
        }
    }
}
