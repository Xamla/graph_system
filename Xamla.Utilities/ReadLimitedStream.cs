using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class ReadLimitExceededException
        : Exception
    {
        public ReadLimitExceededException()
            : base("Stream read limit exceeded.")
        {
        }
    }

    public class ReadLimitedStream
        : Stream
    {
        Stream baseStream;
        int readLimit;
        int remainingBytes;

        public ReadLimitedStream(Stream baseStream, int readLimit = 0)
        {
            this.baseStream = baseStream;
            this.ReadLimit = readLimit;
        }

        public int ReadLimit
        {
            get { return readLimit; }
            set
            {
                readLimit = value;
                ResetReadLimit();
            }
        }

        public void ResetReadLimit()
        {
            remainingBytes = readLimit;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return ReadInternal(buffer, offset, count, cancellationToken, true);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadInternal(buffer, offset, count, CancellationToken.None, false).Result;
        }

        public override bool CanSeek { get { return false; } }
        public override long Position
        {
            get { return baseStream.Position; }
            set { throw new InvalidOperationException(); }
        }

        public override long Seek(long offset, SeekOrigin origin) { throw new InvalidOperationException(); }
        public override void SetLength(long value) { throw new InvalidOperationException(); }

        public override bool CanRead { get { return baseStream.CanRead; } }
        public override bool CanWrite { get { return baseStream.CanWrite; } }
        public override long Length { get { return baseStream.Length; } }
        public override void Write(byte[] buffer, int offset, int count) { baseStream.Write(buffer, offset, count); }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) { return baseStream.WriteAsync(buffer, offset, count, cancellationToken); }
        public override void Flush() { baseStream.Flush(); }
        public override Task FlushAsync(CancellationToken cancellationToken) { return baseStream.FlushAsync(cancellationToken); }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                baseStream.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task<int> ReadInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
        {
            if (remainingBytes <= 0)
                throw new ReadLimitExceededException();

            count = Math.Min(remainingBytes, count);

            int read;
            if (async)
            {
                read = await baseStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                read = baseStream.Read(buffer, offset, count);
            }

            if (read >= 0)
            {
                remainingBytes -= read;
            }

            return read;
        }
    }
}
