using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types
{
    public interface IWritable
        : IDisposable
    {
        Task WriteTo(Stream destination, CancellationToken cancel);
    }

    public static class Writable
    {
        public static IWritable Create(Func<Stream, CancellationToken, Task> writeToFunc, IDisposable resource = null)
        {
            return new AnonymousWritable(writeToFunc, resource);
        }

        public static IWritable Create(Action<Stream> writeToFunc, IDisposable resource = null)
        {
            return Create((stream, cancel) =>
            {
                try
                {
                    writeToFunc(stream);
                    return TaskConstants.Completed;
                }
                catch (Exception ex)
                {
                    return TaskEx.Throw(ex);
                } 
            }, resource);
        }

        public static IWritable Create(Func<Stream> buildStream, IDisposable resource = null)
        {
            return Create(async (stream, cancel) =>
            {
                var source = buildStream();
                if (source != null)
                    await source.CopyToAsync(stream, 16 * 1024, cancel);
            }, resource);
        }
    }

    internal class AnonymousWritable
        : IWritable
    {
        Func<Stream, CancellationToken, Task> writeToFunc;
        IDisposable resource;

        public AnonymousWritable(Func<Stream, CancellationToken, Task> writeToFunc, IDisposable resource)
        {
            this.writeToFunc = writeToFunc;
            this.resource = resource;
        }

        public Task WriteTo(Stream destination, CancellationToken cancel)
        {
            return writeToFunc(destination, cancel);
        }
    
        public void Dispose()
        {
            if (resource != null)
                resource.Dispose();
        }
    }

    public class ReadableWritableAdapter : IWritable
    {
        IReadable readable;

        public ReadableWritableAdapter(IReadable readable)
        {
            this.readable = readable;
        }

        public async Task WriteTo(Stream destination, CancellationToken cancel)
        {
            using (var source = readable.Open())
            {
                await source.CopyToAsync(destination, 32 * 1024, cancel).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
        }
    }
}
