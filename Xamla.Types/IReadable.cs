using System;
using System.IO;

namespace Xamla.Types
{
    public interface IReadable
    {
        Stream Open();
    }

    public static class Readable
    {
        public static IReadable Create(Func<Stream> openFunc)
        {
            return new AnonymousReadable(openFunc);
        }
    }

    internal class AnonymousReadable
        : IReadable
    {
        Func<Stream> openFunc;

        public AnonymousReadable(Func<Stream> openFunc)
        {
            this.openFunc = openFunc;
        }

        public Stream Open()
        {
            return openFunc();
        }
    }
}
