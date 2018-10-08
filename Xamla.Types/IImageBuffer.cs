using System;
using Xamla.Utilities;

namespace Xamla.Types
{
    public interface IImageBuffer
        : IDisposable
    {
        A Data { get; }
        PixelFormat Format { get; }
        int Height { get; }
        int Width { get; }
        int Channels { get; }
        byte[] ToByteArray();
        PinnedGCHandle Pin();
        long SizeInBytes { get; }
        IImageBuffer Clone();
    }
}
