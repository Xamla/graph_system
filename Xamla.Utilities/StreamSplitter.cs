using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    /// <summary>
    /// Acts as a proxy stream that breaks the input one or more smaller chunks.
    /// The split points are determined with a rolling checksum algorithms <see cref="RollingSum" />.
    /// </summary>
    public class StreamSplitter
        : Stream
    {
        public interface IDestinationStreamProvider
        {
            Task<Stream> GetStream(long position, CancellationToken cancel);
            Task Completed(Stream stream, long offset, long length, byte[] sha256hash, CancellationToken cancel);
        }

        RollingSum sum;
        IDestinationStreamProvider streamProvider;
        Stream stream;
        IncrementalHash hash;
        long chunkOffset;
        long chunkLength;
        long position;

        public StreamSplitter(IDestinationStreamProvider destinationProvider)
        {
            this.streamProvider = destinationProvider;
            this.sum = new RollingSum();
        }

        public async override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            do      // allow 0 writes (e.g. empty files)
            {
                if (stream == null)
                {
                    stream = await streamProvider.GetStream(position, cancellationToken).ConfigureAwait(false);
                    hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
                    chunkLength = 0;
                    chunkOffset = position;
                }

                int splitPoint = sum.FindNextSplitOffset(buffer, offset, count);
                if (splitPoint < 0)
                {
                    hash.AppendData(buffer, offset, count);
                    await stream.WriteAsync(buffer, offset, count).ConfigureAwait(false);
                    chunkLength += count;
                    position += count;
                    return;
                }

                int segmentLength = (splitPoint - offset);

                hash.AppendData(buffer, offset, segmentLength);
                await stream.WriteAsync(buffer, offset, segmentLength).ConfigureAwait(false);
                chunkLength += segmentLength;

                await CompleteCurrent(cancellationToken).ConfigureAwait(false);

                offset = splitPoint;
                count -= segmentLength;
                position += segmentLength;
            }
            while (count > 0);
        }

        async Task CompleteCurrent(CancellationToken cancel)
        {
            if (stream == null)
                return;

            var hashResult = hash.GetHashAndReset();
            stream.Dispose();
            await streamProvider.Completed(stream, chunkOffset, chunkLength, hashResult, cancel).ConfigureAwait(false);
            chunkLength = 0;
            chunkOffset = position;
            stream = null;
            hash = null;
        }

        protected override void Dispose(bool disposing)
        {
            CompleteCurrent(CancellationToken.None).Wait();
            base.Dispose(disposing);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (stream == null)
                return TaskConstants.Completed;
            return stream.FlushAsync(cancellationToken);
        }

        #region legacy methods Write, BeginWrite, EndWrite, Flush

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).Wait();
        }

        public override void Flush()
        {
            if (stream != null)
                stream.Flush();
        }

        #endregion

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        #region Not implemented stream members

        public override long Length
        {
            get { throw new InvalidOperationException(); }
        }

        public override long Position
        {
            get { return position; }
            set { throw new InvalidOperationException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
