using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class WriteLimitExceededException
        : Exception
    {
        public WriteLimitExceededException()
            : base("Stream write limit exceeded.")
        {
        }
    }

    public class WriteLimitedStream
        : Stream
    {
        Stream baseStream;
        int writeLimit;
        int remainingBytes;

        public WriteLimitedStream(Stream baseStream, int writeLimit = 0)
        {
            this.baseStream = baseStream;
            this.WriteLimit = writeLimit;
        }

        public int WriteLimit
        {
            get { return writeLimit; }
            set
            {
                writeLimit = value;
                ResetWriteLimit();
            }
        }

        public void ResetWriteLimit()
        {
            this.remainingBytes = writeLimit;
        }

        void CheckWrite(int count)
        {
            remainingBytes -= count;
            if (remainingBytes <= 0)
                throw new WriteLimitExceededException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckWrite(count);
            baseStream.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            CheckWrite(count);
            return baseStream.WriteAsync(buffer, offset, count, cancellationToken);
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
        public override int Read(byte[] buffer, int offset, int count) { return baseStream.Read(buffer, offset, count); }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) { return baseStream.ReadAsync(buffer, offset, count, cancellationToken); }
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
    }
}
