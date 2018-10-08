using System.Numerics;

namespace Xamla.Types.Simd
{
    public static class VectorPixelFormat
    {
        public static readonly PixelFormat RgbV3f = new PixelFormat(PixelType.F32C3, PixelChannels.Rgb, typeof(Vector3), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit }, ColorSpace.Srgb);

        public static readonly PixelFormat RgbaV4f = new PixelFormat(PixelType.F32C4, PixelChannels.Rgba, typeof(Vector4), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit, Range.Unit }, ColorSpace.Srgb);
        public static readonly PixelFormat RgbxV4f = new PixelFormat(PixelType.F32C4, PixelChannels.Rgbx, typeof(Vector4), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit, Range.Unit }, ColorSpace.Srgb);

        public static readonly PixelFormat LinearRgbV3f = new PixelFormat(PixelType.F32C3, PixelChannels.Rgb, typeof(Vector3), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit }, ColorSpace.LinearRgb);
        public static readonly PixelFormat LinearRgbaV4f = new PixelFormat(PixelType.F32C4, PixelChannels.Rgba, typeof(Vector4), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit, Range.Unit }, ColorSpace.LinearRgb);

        public static readonly PixelFormat YuvV3f = new PixelFormat(PixelType.F32C3, PixelChannels.Yuv, typeof(Vector3), new Range<double>[] { Range.Unit, new Range<double>(-0.436, 0.436), new Range<double>(-0.615, 0.615) }, ColorSpace.Yuv);
        public static readonly PixelFormat XyzV3f = new PixelFormat(PixelType.F32C3, PixelChannels.Xyz, typeof(Vector3), new Range<double>[] { new Range<double>(0, 0.9505), new Range<double>(0, 1.0), new Range<double>(0, 1.0890) }, ColorSpace.Xyz);
        public static readonly PixelFormat LabV3f = new PixelFormat(PixelType.F32C3, PixelChannels.Lab, typeof(Vector3), new Range<double>[] { new Range<double>(0, 100), new Range<double>(-150, 100), new Range<double>(-100, 150) }, ColorSpace.Lab);
        public static readonly PixelFormat LuvV3f = new PixelFormat(PixelType.F32C3, PixelChannels.Luv, typeof(Vector3), new Range<double>[] { new Range<double>(0, 100), new Range<double>(-100, 100), new Range<double>(-100, 100) }, ColorSpace.Luv);
        public static readonly PixelFormat HsvV3f = new PixelFormat(PixelType.F32C3, PixelChannels.Hsv, typeof(Vector3), new Range<double>[] { new Range<double>(0, 360), Range.Unit, Range.Unit }, ColorSpace.Hsv);
    }
}
