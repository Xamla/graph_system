using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xamla.Types
{
    public enum ChannelType
    {
        U8 = 1 << 16,
        U16 = 2 << 16,
        F32 = 3 << 16,
        F64 = 4 << 16,
    }

    public enum PixelType
    {
        Unknown = 0,
        U8C1 = ChannelType.U8 | 1,          // 8 BPP, 8 bit per channel
        U8C3 = ChannelType.U8 | 3,          // 24 BPP, 8 bit per channel
        U8C4 = ChannelType.U8 | 4,          // 32 BPP, 8 bit per channel
        U16C1 = ChannelType.U16 | 1,        // 16 BPP, 16 bit per channel
        U16C3 = ChannelType.U16 | 3,        // 48 BPP, 16 bit per channel
        U16C4 = ChannelType.U16 | 4,        // 64 BPP, 16 bit per channel
        F32C1 = ChannelType.F32 | 1,        // 32 BPP, 32 bit per channel
        F32C3 = ChannelType.F32 | 3,        // 96 BPP, 32 bit per channel
        F32C4 = ChannelType.F32 | 4,        // 128 BPP, 32 bit per channel
        F64C1 = ChannelType.F64 | 1,        // 64 BPP, 64 bit per channel
        F64C3 = ChannelType.F64 | 3,        // 192 BPP, 64 bit per channel
        F64C4 = ChannelType.F64 | 4,        // 256 BPP, 64 bit per channel
        MaskChannelCount = 0xffff,
        MaskChannelType = ~0xffff
    }

    public enum PixelChannels
    {
        Unknown,
        Gray,
        Indexed,
        Rgb,
        Rgbx,           // last chanel is not used (e.g. 32bpp RGB)
        Rgba,
        Bgr,
        Bgra,
        Bgrx,           // last chanel is not used (e.g. 32bpp BGR)
        Yuv,
        Xyz,
        Lab,
        Luv,
        Hsv
        //Cmyk,
        //Lch
    }

    public enum GammaCorrection
    {
        Linear = 0,
        SRgb = 1,            // sRGB
        Rec709 = 2           // Rec. 709 (ITU-R Recommendation BT.709)
    }

    public enum ColorSpace
    {
        Unknown,
        LinearRgb,
        Srgb,               // sRGB IEC 61966-2-1 (standard RGB)
        Rec709,             // Rec. 709 (ITU-R Recommendation BT.709)
        Xyz,                // CIE 1931 XYZ
        Lab,                // CIE 1976 Lab (from XYZ derived color space, which also contains all visible colors)
        Hsv,                // Hue Saturation Value/Brightness
        Luv,                // CIE Luv
        Yuv,                // NTSC PAL Y'UV Luma + Chroma
    }

    public struct PixelFormat
    {
        public readonly PixelType PixelType;
        public readonly PixelChannels PixelChannels;
        public readonly Type ElementType;
        public readonly Range<double>[] ChannelRanges;
        public readonly ColorSpace ColorSpace;

        public PixelFormat(PixelType pixelType, PixelChannels pixelChannels, Type elementType, Range<double>[] channelRanges, ColorSpace colorspace)
        {
            this.PixelType = pixelType;
            this.PixelChannels = pixelChannels;
            this.ElementType = elementType;
            this.ChannelRanges = channelRanges;
            this.ColorSpace = colorspace;
        }

        public static readonly PixelFormat Unknown = new PixelFormat(PixelType.Unknown, PixelChannels.Unknown, typeof(byte), new Range<double>[0], ColorSpace.Unknown);
        public static readonly PixelFormat GrayU8 = new PixelFormat(PixelType.U8C1, PixelChannels.Gray, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat GrayU16 = new PixelFormat(PixelType.U16C1, PixelChannels.Gray, typeof(ushort), new Range<double>[] { new Range<double>(0, UInt16.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat GrayF32 = new PixelFormat(PixelType.F32C1, PixelChannels.Gray, typeof(float), new Range<double>[] { new Range<double>(0, float.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat GrayF64 = new PixelFormat(PixelType.F64C1, PixelChannels.Gray, typeof(double), new Range<double>[] { new Range<double>(0, double.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat Rgb24 = new PixelFormat(PixelType.U8C3, PixelChannels.Rgb, typeof(Rgb24), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat Rgba32 = new PixelFormat(PixelType.U8C4, PixelChannels.Rgba, typeof(Rgba32), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat Rgb48 = new PixelFormat(PixelType.U16C3, PixelChannels.Rgb, typeof(Rgb48), new Range<double>[] { new Range<double>(0, ushort.MaxValue), new Range<double>(0, ushort.MaxValue), new Range<double>(0, ushort.MaxValue) }, ColorSpace.Srgb);
        //public static readonly PixelFormat Rgba64 = new PixelFormat(PixelType.U16C4, PixelChannels.Rgba, typeof(Rgba64), new Range<double>[] { new Range<double>(0, UInt16.MaxValue), new Range<double>(0, UInt16.MaxValue), new Range<double>(0, UInt16.MaxValue), new Range<double>(0, UInt16.MaxValue) });

        public static readonly PixelFormat RgbU8 = new PixelFormat(PixelType.U8C3, PixelChannels.Rgb, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat RgbxU8 = new PixelFormat(PixelType.U8C4, PixelChannels.Rgbx, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, Byte.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat RgbaU8 = new PixelFormat(PixelType.U8C4, PixelChannels.Rgba, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, Byte.MaxValue) }, ColorSpace.Srgb);

        public static readonly PixelFormat BgrU8 = new PixelFormat(PixelType.U8C3, PixelChannels.Bgr, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat BgrxU8 = new PixelFormat(PixelType.U8C4, PixelChannels.Bgrx, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, Byte.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat BgraU8 = new PixelFormat(PixelType.U8C4, PixelChannels.Bgra, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, byte.MaxValue), new Range<double>(0, Byte.MaxValue) }, ColorSpace.Srgb);

        public static readonly PixelFormat RgbU16 = new PixelFormat(PixelType.U16C3, PixelChannels.Rgb, typeof(ushort), new Range<double>[] { new Range<double>(0, ushort.MaxValue), new Range<double>(0, ushort.MaxValue), new Range<double>(0, ushort.MaxValue) }, ColorSpace.Srgb);
        public static readonly PixelFormat RgbF32 = new PixelFormat(PixelType.F32C3, PixelChannels.Rgb, typeof(float), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit }, ColorSpace.Srgb);
        public static readonly PixelFormat RgbaF32 = new PixelFormat(PixelType.F32C4, PixelChannels.Rgba, typeof(float), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit, Range.Unit }, ColorSpace.Srgb);
        public static readonly PixelFormat RgbF64 = new PixelFormat(PixelType.F64C3, PixelChannels.Rgb, typeof(double), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit }, ColorSpace.Srgb);
        public static readonly PixelFormat RgbaF64 = new PixelFormat(PixelType.F64C4, PixelChannels.Rgba, typeof(double), new Range<double>[] { Range.Unit, Range.Unit, Range.Unit, Range.Unit }, ColorSpace.Srgb);
        public static readonly PixelFormat YuvF32 = new PixelFormat(PixelType.F32C3, PixelChannels.Yuv, typeof(float), new Range<double>[] { Range.Unit, new Range<double>(-0.436, 0.436), new Range<double>(-0.615, 0.615) }, ColorSpace.Yuv);
        public static readonly PixelFormat YuvF64 = new PixelFormat(PixelType.F64C3, PixelChannels.Yuv, typeof(double), new Range<double>[] { Range.Unit, new Range<double>(-0.436, 0.436), new Range<double>(-0.615, 0.615) }, ColorSpace.Yuv);
        public static readonly PixelFormat XyzF32 = new PixelFormat(PixelType.F32C3, PixelChannels.Xyz, typeof(float), new Range<double>[] { new Range<double>(0, 0.9505), new Range<double>(0, 1.0), new Range<double>(0, 1.0890) }, ColorSpace.Xyz);
        public static readonly PixelFormat XyzF64 = new PixelFormat(PixelType.F64C3, PixelChannels.Xyz, typeof(double), new Range<double>[] { new Range<double>(0, 0.9505), new Range<double>(0, 1.0), new Range<double>(0, 1.0890) }, ColorSpace.Xyz);
        public static readonly PixelFormat LabF32 = new PixelFormat(PixelType.F32C3, PixelChannels.Lab, typeof(float), new Range<double>[] { new Range<double>(0, 100), new Range<double>(-150, 100), new Range<double>(-100, 150) }, ColorSpace.Lab);
        public static readonly PixelFormat LabF64 = new PixelFormat(PixelType.F64C3, PixelChannels.Lab, typeof(double), new Range<double>[] { new Range<double>(0, 100), new Range<double>(-150, 100), new Range<double>(-100, 150) }, ColorSpace.Lab);
        public static readonly PixelFormat LuvF32 = new PixelFormat(PixelType.F32C3, PixelChannels.Luv, typeof(float), new Range<double>[] { new Range<double>(0, 100), new Range<double>(-100, 100), new Range<double>(-100, 100) }, ColorSpace.Luv);
        public static readonly PixelFormat LuvF64 = new PixelFormat(PixelType.F64C3, PixelChannels.Luv, typeof(double), new Range<double>[] { new Range<double>(0, 100), new Range<double>(-100, 100), new Range<double>(-100, 100) }, ColorSpace.Luv);
        public static readonly PixelFormat HsvF32 = new PixelFormat(PixelType.F32C3, PixelChannels.Hsv, typeof(float), new Range<double>[] { new Range<double>(0, 360), Range.Unit, Range.Unit }, ColorSpace.Hsv);
        public static readonly PixelFormat HsvF64 = new PixelFormat(PixelType.F64C3, PixelChannels.Hsv, typeof(double), new Range<double>[] { new Range<double>(0, 360), Range.Unit, Range.Unit }, ColorSpace.Hsv);

        public override bool Equals(object obj)
        {
            if (!(obj is PixelFormat))
                return false;

            var pfv = (PixelFormat)obj;

            return this.PixelType.Equals(pfv.PixelType)
                && this.PixelChannels.Equals(pfv.PixelChannels)
                && this.ElementType.Equals(pfv.ElementType)
                && this.ChannelRanges.SequenceEqual(pfv.ChannelRanges)
                && this.ColorSpace.Equals(pfv.ColorSpace);
        }

        public override int GetHashCode()
        {
            return this.PixelChannels.GetHashCode() + this.PixelChannels.GetHashCode() + this.ElementType.GetHashCode() + this.ChannelRanges.GetHashCode();
        }

        public int ChannelCount
        {
            get { return (PixelType != PixelType.Unknown) ? (int)(PixelType & PixelType.MaskChannelCount) : ChannelRanges.Length; }
        }

        public ChannelType ChannelType
        {
            get { return (ChannelType)(PixelType & PixelType.MaskChannelType); }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1} ({1}), {3}", PixelType, PixelChannels, ColorSpace, ElementType.Name);
        }
    }

    public struct Pixel
    {
        public double V0, V1, V2, V3;

        public Pixel(double v0, double v1 = 0, double v2 = 0, double v3 = 0)
        {
            V0 = v0; V1 = v1; V2 = v2; V3 = v3;
        }

        public V<double> ToV() => V.Create(V0, V1, V2, V3);
        public double[] ToArray() => new double[] { V0, V1, V2, V3 };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bgr24
    {
        public byte B, G, R;

        public Bgr24(byte b, byte g, byte r)
        {
            B = b; G = g; R = r;
        }

        public Pixel ToPixel() => new Pixel(B, G, R);
        public override string ToString() => $"B: {B}, G: {G}, R: {R}";
        public byte[] ToArray() => new byte[] { B, G, R };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bgra32
    {
        public byte B, G, R, A;

        public Bgra32(byte b, byte g, byte r, byte a)
        {
            B = b; G = g; R = r; A = a;
        }

        public Pixel ToPixel() => new Pixel(B, G, R, A);
        public override string ToString() => $"B: {B}, G: {G}, R: {R}, A: {A}";
        public byte[] ToArray() => new byte[] { B, G, R, A };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rgb24
    {
        public byte R, G, B;

        public Rgb24(byte r, byte g, byte b)
        {
            R = r; G = g; B = b;
        }

        public Pixel ToPixel() => new Pixel(R, G, B);
        public override string ToString() => $"R: {G}, G: {G}, B: {B}";
        public byte[] ToArray() => new byte[] { R, G, B };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rgba32
    {
        public byte R, G, B, A;

        public Rgba32(byte r, byte g, byte b, byte a = 255)
        {
            R = r; G = g; B = b; A = a;
        }

        public Pixel ToPixel() => new Pixel(R, G, B);
        public override string ToString() => $"R: {R}, G: {G}, B: {B}, A: {A}";
        public byte[] ToArray() => new byte[] { R, G, B, A };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Yuv24
    {
        public byte Y, U, V;

        public Yuv24(byte y, byte u, byte v)
        {
            Y = y; U = u; V = v;
        }

        public Pixel ToPixel() => new Pixel(Y, U, V);
        public override string ToString() => $"Y: {Y}, U: {U}, V: {V}";
        public byte[] ToArray() => new byte[] { Y, U, V };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rgb48
    {
        public ushort R, G, B;

        public Rgb48(ushort r, ushort g, ushort b)
        {
            R = r; G = g; B = b;
        }

        public Pixel ToPixel() => new Pixel(R, G, B);
        public override string ToString() => $"R: {R}, G: {G}, B: {B}";
        public ushort[] ToArray() => new ushort[] { R, G, B };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rbga64
    {
        public ushort R, B, G, A;

        public Rbga64(ushort r, ushort b, ushort g, ushort a)
        {
            R = r; B = b; G = g; A = a;
        }

        public Pixel ToPixel() => new Pixel(R, B, G, A);
        public override string ToString() => $"R: {R}, G: {G}, B: {B}, A: {A}";
        public ushort[] ToArray() => new ushort[] { R, B, G, A };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RgbaF32
    {
        public float R, G, B, A;

        public RgbaF32(float r, float g, float b, float a)
        {
            R = r; G = g; B = b; A = a;
        }

        public Pixel ToPixel() => new Pixel(R, B, G, A);
        public override string ToString() => $"R: {R}, G: {G}, B: {B}, A: {A}";
        public float[] ToArray() => new float[] { R, G, B, A };
    }

    public enum BorderMode
    {
        Zero,
        Replicate,
        Reflect
    }
}
