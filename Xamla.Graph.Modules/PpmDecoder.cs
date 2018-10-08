using System;
using System.Globalization;
using System.IO;
using System.Text;
using Xamla.Types;

namespace Xamla
{
    public class PpmDecoder
    {
        struct Header
        {
            public int Height;
            public int Width;
            public int Maxval;

            public int BytesPerSample
            {
                get { return Maxval > 256 ? 2 : 1; }
            }
        }

        char Next(Stream input)
        {
            int v = input.ReadByte();
            if (v == -1)
                throw new EndOfStreamException();
            return (char)v;
        }

        void SkipComment(Stream input)
        {
            for (; ; )
            {
                char c = Next(input);
                if (c == '\n' || c == '\r')
                    break;
            }
        }

        string ReadToken(Stream input, int? maxLength = null, Func<char, bool> validator = null)
        {
            var sb = new StringBuilder();

            char c = '\0';
            bool skipWhiteSpaces = true;
            do
            {
                if (skipWhiteSpaces)
                {
                    // skip leading white-space characters
                    do
                    {
                        c = Next(input);
                    } while (char.IsWhiteSpace(c));
                    skipWhiteSpaces = false;
                }

                // read token
                if (c == '#')       // start of comment
                {
                    SkipComment(input);
                    skipWhiteSpaces = true;
                }
                else
                {
                    if (validator != null && !validator(c))
                        throw new InvalidDataException("Unexpected character read.");

                    sb.Append(c);       // append character

                    if (maxLength.HasValue && sb.Length > maxLength)
                        throw new InvalidDataException("More characters read than expected.");
                }

                c = Next(input);
            } while (!char.IsWhiteSpace(c));

            return sb.ToString();
        }

        int ReadInt32Token(Stream reader)
        {
            return int.Parse(ReadToken(reader, 10, char.IsDigit), NumberFormatInfo.InvariantInfo);
        }

        Header ReadHeader(Stream input)
        {
            var magic = ReadToken(input, 2);
            if (magic != "P6")
                throw new Exception("Magic number 'P6' expected.");

            var h = new Header
            {
                Height = ReadInt32Token(input),
                Width = ReadInt32Token(input),
                Maxval = ReadInt32Token(input)
            };

            // validate header value ranges

            if (h.Height < 0)
                throw new Exception("Height must be positive.");

            if (h.Width < 0)
                throw new Exception("Width must be positive.");

            if (h.Maxval < 0 || h.Maxval > 0xffff)
                throw new Exception("Maxval must be positve and less or equal to 0xFFFF (65535).");

            return h;
        }

        public A<byte> Decode(string path)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                return Decode(file);
            }
        }

        public A<byte> Decode(Stream input)
        {
            var h = ReadHeader(input);

            var mem = new MemoryStream(h.Height * h.Width * h.BytesPerSample * 3);
            input.CopyTo(mem);

            if (h.BytesPerSample == 1)
            {
                return new A<byte>(mem.ToArray(), h.Height, h.Width, 3);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
