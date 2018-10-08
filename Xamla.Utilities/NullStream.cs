using System;
using System.IO;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class NullStream : Stream
    {
        long position;
        long length;

        public NullStream()
        {
        }

        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override bool CanSeek { get { return true; } }

        public override void Flush() { }

        public override long Length { get { return length; } }
        public override long Position
        {
            get { return position; }
            set
            {
                position = value;
                if (position > length)
                    length = position;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition = Position;

            switch (origin)
            {
                default:
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = Position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = Length + offset;
                    break;
            }

            if (newPosition < 0)
                throw new ArgumentOutOfRangeException("offset", "Attempt to seek before start of stream.");

            Position = newPosition;
            return newPosition;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotSupportedException("This stream doesn't support reading.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("This stream doesn't support reading.");
        }

        public override void SetLength(long value)
        {
            length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Seek(count, SeekOrigin.Current);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            Seek(count, SeekOrigin.Current);
            return TaskConstants.Completed;
        }
    }
}
