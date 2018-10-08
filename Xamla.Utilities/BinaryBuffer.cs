using System;
using System.Text;

namespace Xamla.Xmpp
{ 
    public class BinaryBuffer
    {
        public static BinaryBuffer FromExistingBuffer(byte[] existingBuffer)
        {
            return new BinaryBuffer(existingBuffer, 0, existingBuffer.Length);
        }

        byte[] data;

        public BinaryBuffer(int size)
            : this(new byte[size], 0, 0)
        {
        }

        public int BehindLast { get; set; }
        public int Offset { get; set; }

        public BinaryBuffer(byte[] existingBuffer, int offset, int count)
        {
            this.data = existingBuffer;
            this.Offset = offset;
            this.BehindLast = offset + count;
        }

        public byte[] Data
        {
            get { return data; }
        }

        public int Available
        {
            get { return this.BehindLast - this.Offset; }
        }

        public int BufferSpaceRemaining
        {
            get { return this.Data.Length - BehindLast; }
        }

        public void Append(byte[] elements)
        {
            Append(elements, 0, elements.Length);
        }

        public void Append(byte[] elements, int offset, int count)
        {
            Buffer.BlockCopy(elements, offset, this.Data, this.BehindLast, count);
            BehindLast += count;
        }

        public int Read(byte[] destination, int offset, int maxCount)
        {
            int readCount = Math.Min(maxCount, this.Available);
            Buffer.BlockCopy(this.Data, this.Offset, destination, offset, readCount);
            this.Offset += readCount;
            return readCount;
        }

        /// <summary>
        /// Reads a single element from the buffer.
        /// </summary>
        /// <returns>The element at the current buffer offset.</returns>
        public byte Read()
        {
            return data[Offset++];
        }

        public byte Current
        {
            get { return data[Offset]; }
        }

        /// <summary>
        /// Moves all available elements to the beginning of the buffer to maximize the remaining space of the buffer.
        /// </summary>
        public void Reorganize()
        {
            if (this.Available > 0 && Offset > 0)
            {
                Buffer.BlockCopy(this.Data, this.Offset, this.Data, 0, this.Available);
                BehindLast = this.Available;
                Offset = 0;
            }
        }

        public void Recycle()
        {
            BehindLast = 0;
            Offset = 0;
        }

        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(this.Data, this.Offset, this.Available);
        }

        public byte[] ToArray()
        {
            var array = new byte[this.Available];
            Buffer.BlockCopy(this.Data, this.Offset, array, 0, this.Available);
            return array;
        }

        public string ConvertToString()
        {
            return ConvertToString(Encoding.UTF8);
        }

        public string ConvertToString(Encoding encoding)
        {
            return encoding.GetString(this.Data, this.Offset, this.Available);
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(this.Data, this.Offset, this.Available);
        }
    }
}
