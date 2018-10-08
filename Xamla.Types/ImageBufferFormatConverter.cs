using System;
using System.Runtime.InteropServices;

namespace Xamla.Types
{
    public static partial class I
    {
        public static readonly M<double> RgbToYuvMat = M.FromArray(new double[,] { { 0.299, 0.587, 0.114 }, { -0.14713, -0.28886, 0.436 }, { 0.615, -0.51499, -0.10001 } });
        public static readonly M<double> YuvToRgbMat = M.FromArray(new double[,] { { 1.000000000118, -0.0000117983843824, 1.13983457572 }, { 1.000003946461, -0.394646053262, -0.580594233834 }, { 0.999979678881, 2.03211193844, -0.0000151129806638 } });
        public static readonly M<double> RgbToXyzMat = M.FromArray(new double[,] { { 0.4124564, 0.3575761, 0.1804375 }, { 0.2126729, 0.7151522, 0.0721750 }, { 0.0193339, 0.1191920, 0.9503041 } });
        public static readonly M<double> XyzToRgbMat = M.FromArray(new double[,] { { 3.240454836, -1.5371388501, -0.49853154687 }, { -0.96926638988, 1.8760109288, 0.041556082347 }, { 0.055643419604, -0.20402585427, 1.0572251625 } });

        public static Type GetType(PixelFormat format)
        {
            return typeof(I<>).MakeGenericType(format.ElementType);
        }

        public static I<T> FromBytes<T>(byte[] source, int height, int width)
        {
            return FromBytes<T>(source, height, width, typeFormatMap[typeof(T)]);
        }

        public static I<T> FromBytes<T>(byte[] source, int height, int width, PixelFormat format)
        {
            var buffer = new A<T>(width, height);
            using (var p = buffer.Pin())
            {
                Marshal.Copy(source, 0, p.Pointer, buffer.SizeInBytes);
            }

            return new I<T>(format, buffer);
        }

        public static IImageBuffer Create(PixelFormat format, int height, int width)
        {
            return (IImageBuffer)Activator.CreateInstance(GetType(format), format, height, width);
        }

        public static IImageBuffer Create(PixelFormat format, A data)
        {
            return (IImageBuffer)Activator.CreateInstance(GetType(format), format, data);
        }

        public static I<float> CreateGrayF32(int height, int width)
        {
            return new I<float>(PixelFormat.GrayF32, height, width, 1);
        }

        public static I<double> CreateGrayF64(int height, int width)
        {
            return new I<double>(PixelFormat.GrayF64, height, width, 1);
        }

        public static I<float> CreateRgbF32(int height, int width)
        {
            return new I<float>(PixelFormat.RgbF32, height, width, 3);
        }

        public static I<double> CreateRgbF64(int height, int width)
        {
            return new I<double>(PixelFormat.RgbF64, height, width, 3);
        }

        public static I<float> CreateRgbaF32(int height, int width)
        {
            return new I<float>(PixelFormat.RgbaF32, height, width, 4);
        }

        public static I<double> CreateRgbaF64(int height, int width)
        {
            return new I<double>(PixelFormat.RgbaF64, height, width, 4);
        }

        public static I<float> CreateYuvF32(int height, int width)
        {
            return new I<float>(PixelFormat.YuvF32, height, width, 3);
        }

        public static I<double> CreateYuvF64(int height, int width)
        {
            return new I<double>(PixelFormat.YuvF64, height, width, 3);
        }

        public static I<float> CreateXyzF32(int height, int width)
        {
            return new I<float>(PixelFormat.XyzF32, height, width, 3);
        }

        public static I<double> CreateXyzF64(int height, int width)
        {
            return new I<double>(PixelFormat.XyzF64, height, width, 3);
        }

        public static I<float> CreateLabF32(int height, int width)
        {
            return new I<float>(PixelFormat.LabF32, height, width, 3);
        }

        public static I<double> CreateLabF64(int height, int width)
        {
            return new I<double>(PixelFormat.LabF64, height, width, 3);
        }

        public static I<float> CreateLuvF32(int height, int width)
        {
            return new I<float>(PixelFormat.LuvF32, height, width, 3);
        }

        public static I<double> CreateLuvF64(int height, int width)
        {
            return new I<double>(PixelFormat.LuvF64, height, width, 3);
        }

        public static I<float> CreateHsvF32(int height, int width)
        {
            return new I<float>(PixelFormat.HsvF32, height, width, 3);
        }

        public static I<double> CreateHsvF64(int height, int width)
        {
            return new I<double>(PixelFormat.HsvF64, height, width, 3);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void Rec709ToLinearRgbInPlace(this I<float> image)
        {
            var pixelFormat = new PixelFormat(image.Format.PixelType, image.Format.PixelChannels, image.Format.ElementType, image.Format.ChannelRanges, ColorSpace.LinearRgb);
            image.format = pixelFormat;

            for (int z = 0; z < image.Channels; ++z)
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        if (image[y, x, z] <= 0.0)
                            image[y, x, z] = 0.0f;

                        else if (image[y, x, z] < 0.081)
                            image[y, x, z] /= 4.5f;

                        else if (image[y, x, z] < 1.0)
                            image[y, x, z] = (float)Math.Pow((image[y, x, z] + 0.099) / 1.099, (1.0 / 0.45));

                        else
                            image[y, x, z] = 1.0f;
                    }
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void LinearRgbToRec709InPlace(this I<float> image)
        {
            var pixelFormat = new PixelFormat(image.Format.PixelType, image.Format.PixelChannels, image.Format.ElementType, image.Format.ChannelRanges, ColorSpace.Rec709);
            image.format = pixelFormat;

            for (int z = 0; z < image.Channels; ++z)
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        if (image[y, x, z] <= 0.0)
                            image[y, x, z] = 0.0f;

                        else if (image[y, x, z] < 0.081)
                            image[y, x, z] *= 4.5f;

                        else if (image[y, x, z] < 1.0)
                            image[y, x, z] = 1.099f * (float)Math.Pow(image[y, x, z], 0.45) - 0.099f;

                        else
                            image[y, x, z] = 1.0f;
                    }
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void SRgbToLinearRgbInPlace(this I<float> image)
        {
            var pixelFormat = new PixelFormat(image.Format.PixelType, image.Format.PixelChannels, image.Format.ElementType, image.Format.ChannelRanges, ColorSpace.LinearRgb);
            image.format = pixelFormat;
            for (int z = 0; z < image.Channels; ++z)
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        if (image[y, x, z] <= 0.0)
                            image[y, x, z] = 0.0f;

                        else if (image[y, x, z] <= 0.04045)
                            image[y, x, z] /= 12.92f;

                        else if (image[y, x, z] < 1.0)
                            image[y, x, z] = (float)Math.Pow((image[y, x, z] + 0.055) / 1.055, 2.4);

                        else
                            image[y, x, z] = 1.0f;
                    }
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void LinearRgbToSRgbInPlace(this I<float> image)
        {
            var pixelFormat = new PixelFormat(image.Format.PixelType, image.Format.PixelChannels, image.Format.ElementType, image.Format.ChannelRanges, ColorSpace.Srgb);
            image.format = pixelFormat;
            for (int z = 0; z < image.Channels; ++z)
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        if (image[y, x, z] <= 0.0)
                            image[y, x, z] = 0.0f;

                        else if (image[y, x, z] <= 0.0031308)
                            image[y, x, z] *= 12.92f;

                        else if (image[y, x, z] < 1.0)
                            image[y, x, z] = 1.055f * (float)Math.Pow(image[y, x, z], (1.0 / 2.4)) - 0.055f;

                        else
                            image[y, x, z] = 1.0f;
                    }
                }
            }
        }

        public static I<float> Rec709ToLinearRgb(this I<float> image)
        {
            I<float> result = image.Clone();
            result.Rec709ToLinearRgbInPlace();
            return result;
        }

        public static I<float> LinearRgbToRec709(this I<float> image)
        {
            I<float> result = image.Clone();
            result.LinearRgbToRec709InPlace();
            return result;
        }

        public static I<float> SRgbToLinearRgb(this I<float> image)
        {
            I<float> result = image.Clone();
            result.SRgbToLinearRgbInPlace();
            return result;
        }

        public static I<float> LinearRgbToSRgb(this I<float> image)
        {
            I<float> result = image.Clone();
            result.LinearRgbToSRgbInPlace();
            return result;
        }

        public static I<float> ToF32(this I<byte> image)
        {
            var ranges = new Range<double>[image.Channels];
            for (int i = 0; i < image.Channels; ++i)
                ranges[i] = Range.Unit;

            var pixelFormat = new PixelFormat((PixelType)((int)ChannelType.F32 | image.Channels), image.Format.PixelChannels, typeof(float), ranges, image.Format.ColorSpace);
            var r = new I<float>(pixelFormat, image.Height, image.Width, image.Channels);
            var src = image.Data.Buffer;
            var dst = r.Data.Buffer;

            for (int i = 0; i < src.Length; i += 1)
                dst[i] = Range.Saturate(src[i] / 255.0f);

            return r;
        }

        private static I<T> BgrToRgbInternal<T>(this I<T> image, PixelFormat targetFormat)
        {
            if (image.Channels != 3)
                throw new ArgumentException("Source image must have 3 channels for BGR-to-RGB conversion.", "image");

            int h = image.Height, w = image.Width;
            var pixelFormat = new PixelFormat(targetFormat.PixelType, targetFormat.PixelChannels, targetFormat.ElementType, targetFormat.ChannelRanges, image.Format.ColorSpace);
            var result = new I<T>(pixelFormat, h, w);
            A<T> s = image.Data, d = result.Data;
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    d[y, x, 0] = s[y, x, 2];
                    d[y, x, 1] = s[y, x, 1];
                    d[y, x, 2] = s[y, x, 0];
                }
            }
            return result;
        }

        public static I<byte> BgrToRgb(this I<byte> image)
        {
            return BgrToRgbInternal(image, PixelFormat.RgbU8);
        }

        public static I<float> BgrToRgb(this I<float> image)
        {
            return BgrToRgbInternal(image, PixelFormat.RgbF32);
        }

        public static I<double> BgrToRgb(this I<double> image)
        {
            return BgrToRgbInternal(image, PixelFormat.RgbF64);
        }

        public static I<float> ToRgbF32(this I<Rgb24> image)
        {
            var pixelFormat = new PixelFormat(PixelFormat.RgbF32.PixelType, PixelFormat.RgbF32.PixelChannels, PixelFormat.RgbF32.ElementType, PixelFormat.RgbF32.ChannelRanges, image.Format.ColorSpace);
            var r = new I<float>(pixelFormat, image.Height, image.Width, 3);
            var src = image.Data.Buffer;
            var dst = r.Data.Buffer;

            for (int i = 0, j = 0; i < src.Length; i += 1, j += 3)
            {
                var p = src[i];
                dst[j + 0] = Range.Saturate(p.R / 255.0f);
                dst[j + 1] = Range.Saturate(p.G / 255.0f);
                dst[j + 2] = Range.Saturate(p.B / 255.0f);
            }

            return r;
        }

        public static I<Rgb24> RgbToRgb24(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Rgb))
                throw new ArgumentException("RGB input image required");

            var img8bpp = image.ToU8();
            var r = new I<Rgb24>(PixelFormat.Rgb24, image.Height, image.Width);

            using (var dst = r.Data.Pin())
            {
                Marshal.Copy(img8bpp.Data.Buffer, 0, dst.Pointer, img8bpp.Data.Count);
            }

            return r;
        }

        public static I<float> ToRgbaF32(this I<Rgba32> image)
        {
            var r = CreateRgbaF32(image.Height, image.Width);
            var src = image.Data.Buffer;
            var dst = r.Data.Buffer;

            for (int i = 0, j = 0; i < src.Length; i += 1, j += 4)
            {
                var p = src[i];
                dst[j + 0] = (float)(p.R / 255.0);
                dst[j + 1] = (float)(p.G / 255.0);
                dst[j + 2] = (float)(p.B / 255.0);
                dst[j + 3] = (float)(p.A / 255.0);
            }

            return r;
        }

        public static I<float> RgbToYuv(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Rgb))
                throw new ArgumentException("RGB input image required");

            if (image.Format.ColorSpace != ColorSpace.LinearRgb && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. SRgb expected.");

            return RgbToYuvMat.Multiply(image, PixelFormat.YuvF32);
        }

        public static I<float> YuvToRgb(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Yuv))
                throw new ArgumentException("YUV input image required");

            if (image.Format.ColorSpace != ColorSpace.Yuv && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. Yuv expected.");

            return YuvToRgbMat.Multiply(image, PixelFormat.RgbF32);
        }

        public static I<float> RgbToXyz(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Rgb))
                throw new ArgumentException("RGB input image required");

            if (image.Format.ColorSpace != ColorSpace.LinearRgb && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. LinearRgb expected.");

            return RgbToXyzMat.Multiply(image, PixelFormat.XyzF32);
        }

        public static I<float> XyzToRgb(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Xyz))
                throw new ArgumentException("XYZ input image required");

            if (image.Format.ColorSpace != ColorSpace.Xyz && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. Xyz expected.");

            return XyzToRgbMat.Multiply(image, PixelFormat.RgbF32);
        }

        private static double XyzToLabFunc(double t)
        {
            if (t > Math.Pow((6.0 / 29.0), 3.0))
                return Math.Pow(t, (1.0 / 3.0));
            else
                return (1.0 / 3.0) * Math.Pow((29.0 / 6.0), 2.0) * t + (4.0 / 29.0);
        }

        public static I<float> XyzToLab(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Xyz))
                throw new ArgumentException("XYZ input image required");

            if (image.Format.ColorSpace != ColorSpace.Xyz && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. Xyz expected.");

            var r = CreateLabF32(image.Height, image.Width);
            Range<double> r0 = r.Format.ChannelRanges[0], r1 = r.Format.ChannelRanges[1], r2 = r.Format.ChannelRanges[2];

            // For normalization of X,Y,Z to D65 white point
            var Xn = 0.95;
            var Yn = 1.0;
            var Zn = 1.09;

            for (int y = 0; y < r.Height; ++y)
            {
                for (int x = 0; x < r.Width; ++x)
                {
                    // Calculation of color channel 0 (L) via old channel 1 (Y)
                    r[y, x, 0] = (float)(r0.Clamp(116.0 * XyzToLabFunc(image[y, x, 1] / Yn) - 16.0));
                    // Calculation of color channel 1 (a) via old channels 0 (X) and 1 (Y)
                    r[y, x, 1] = (float)(r1.Clamp(500.0 * (XyzToLabFunc(image[y, x, 0] / Xn) - XyzToLabFunc(image[y, x, 1] / Yn))));
                    // Calculation of color channel 2 (b) via old channels 1 (Y) and 2 (Z)
                    r[y, x, 2] = (float)(r2.Clamp(200.0 * (XyzToLabFunc(image[y, x, 1] / Yn) - XyzToLabFunc(image[y, x, 2] / Zn))));
                }
            }
            return r;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static double LabToXyzFunc(double t)
        {
            if (t > (6.0 / 29.0))
                return Math.Pow(t, 3.0);
            else
                return 3.0 * Math.Pow((6.0 / 29.0), 2.0) * (t - (4.0 / 29.0));
        }

        public static I<float> LabToXyz(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Lab))
                throw new ArgumentException("LAB input image required");

            if (image.Format.ColorSpace != ColorSpace.Lab && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. Lab expected.");

            var r = CreateXyzF32(image.Height, image.Width);
            Range<double> r0 = r.Format.ChannelRanges[0], r1 = r.Format.ChannelRanges[1], r2 = r.Format.ChannelRanges[2];

            var Xn = 0.95;
            var Yn = 1.0;
            var Zn = 1.09;

            for (int y = 0; y < r.Height; ++y)
            {
                for (int x = 0; x < r.Width; ++x)
                {
                    var gyyn = (1.0 / 116.0) * (image[y, x, 0] + 16.0);
                    // Calculation of color channel 1 (Y) via old channel 0 (L)
                    r[y, x, 1] = (float)(r1.Clamp(Yn * LabToXyzFunc(gyyn)));
                    // Calculation of color channel 0 (X) via old channels 0 (L) and 1 (a)
                    r[y, x, 0] = (float)(r0.Clamp(Xn * LabToXyzFunc(gyyn + image[y, x, 1] / 500.0)));
                    // Calculation of color channel 2 (Z) via old channels 0 (L) and 2 (b)
                    r[y, x, 2] = (float)(r2.Clamp(Zn * LabToXyzFunc(gyyn - image[y, x, 2] / 200.0)));
                }
            }
            return r;
        }

        public static I<float> XyzToLuv(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Xyz))
                throw new ArgumentException("XYZ input image required");

            if (image.Format.ColorSpace != ColorSpace.Xyz && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. Xyz expected.");

            var r = CreateLuvF32(image.Height, image.Width);
            Range<double> r0 = r.Format.ChannelRanges[0], r1 = r.Format.ChannelRanges[1], r2 = r.Format.ChannelRanges[2];

            var Yn = 1.0;
            var Un = 0.1977;
            var Vn = 0.4683;
            var denominator = 0.0;
            var s = 0.0;
            var t = 0.0;

            for (int y = 0; y < r.Height; ++y)
            {
                for (int x = 0; x < r.Width; ++x)
                {
                    // Calculation of color channel 0 (L) via old channel 1 (Y)
                    if ((image[y, x, 1] / Yn) > Math.Pow((6.0 / 29.0), 3.0))
                        r[y, x, 0] = (float)(r0.Clamp(116.0 * Math.Pow((image[y, x, 1] / Yn), (1.0 / 3.0)) - 16.0));
                    else
                        r[y, x, 0] = (float)(r0.Clamp(Math.Pow((29.0 / 3.0), 3.0) * (image[y, x, 1] / Yn)));
                    // Calculation of auxiliary variables s and t via old channels 0 (X), 1 (Y) and 2 (Z)
                    denominator = image[y, x, 0] + 15.0 * image[y, x, 1] + 3.0 * image[y, x, 2];
                    s = (4.0 * image[y, x, 0]) / denominator;
                    t = (9.0 * image[y, x, 1]) / denominator;
                    // Calculation of color channel 1 (u) via new channel 0 (L) and auxiliary variable s
                    r[y, x, 1] = (float)(r1.Clamp(13.0 * r[y, x, 0] * (s - Un)));
                    // Calculation of color channel 2 (v) via new channel 0 (L) and auxiliary variable t
                    r[y, x, 2] = (float)(r2.Clamp(13.0 * r[y, x, 0] * (t - Vn)));
                }
            }
            return r;
        }

        public static I<float> LuvToXyz(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Luv))
                throw new ArgumentException("LUV input image required");

            if (image.Format.ColorSpace != ColorSpace.Luv && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. Luv expected.");

            var r = CreateXyzF32(image.Height, image.Width);
            Range<double> r0 = r.Format.ChannelRanges[0], r1 = r.Format.ChannelRanges[1], r2 = r.Format.ChannelRanges[2];

            var Yn = 1.0;
            var Un = 0.1977;
            var Vn = 0.4683;
            var s = 0.0;
            var t = 0.0;

            for (int y = 0; y < r.Height; ++y)
            {
                for (int x = 0; x < r.Width; ++x)
                {
                    // Calculation of color channel 1 (Y) via old channel 0 (L)
                    if (image[y, x, 0] > 8.0)
                        r[y, x, 1] = (float)(r1.Clamp(Yn * Math.Pow(((image[y, x, 0] + 16.0) / 116.0), 3.0)));
                    else
                        r[y, x, 1] = (float)(r1.Clamp(Yn * image[y, x, 0] * Math.Pow((3.0 / 29.0), 3.0)));
                    // Calculation of auxiliary variables s and t via old channels 0 (L), 1 (u) and 2 (v)
                    s = image[y, x, 1] / (13.0 * image[y, x, 0]) + Un;
                    t = image[y, x, 2] / (13.0 * image[y, x, 0]) + Vn;
                    // Calculation of color channel 0 (X) via new channel 1 (Y) and auxiliary variables s and t
                    r[y, x, 0] = (float)(r0.Clamp(r[y, x, 1] * ((9.0 * s) / (4.0 * t))));
                    // Calculation of color channel 2 (Z) via new channel 1 (Y) and auxiliary variables s and t
                    r[y, x, 2] = (float)(r2.Clamp(r[y, x, 1] * ((12.0 - 3.0 * s - 20.0 * t) / (4.0 * t))));
                }
            }
            return r;
        }

        public static I<float> RgbToHsv(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Rgb))
                throw new ArgumentException("RGB input image required");

            if (image.Format.ColorSpace != ColorSpace.Srgb && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. SRgb expected.");

            var r = CreateHsvF32(image.Height, image.Width);
            Range<double> r0 = r.Format.ChannelRanges[0], r1 = r.Format.ChannelRanges[1], r2 = r.Format.ChannelRanges[2];

            var max = 0.0;
            var min = 0.0;
            var diff = 0.0;

            for (int y = 0; y < r.Height; ++y)
            {
                for (int x = 0; x < r.Width; ++x)
                {
                    // Calculation of max(R,G,B), min(R,G,B) and max(R,G,B) - min(R,G,B)
                    max = Math.Max(Math.Max(image[y, x, 0], image[y, x, 1]), image[y, x, 2]);
                    min = Math.Min(Math.Min(image[y, x, 0], image[y, x, 1]), image[y, x, 2]);
                    diff = max - min;
                    // Calculation of color channel 0 (H)
                    if (max == min)
                        r[y, x, 0] = 0;
                    else if (max == image[y, x, 0])
                        r[y, x, 0] = (float)(60.0 * (image[y, x, 1] - image[y, x, 2]) / diff);
                    else if (max == image[y, x, 1])
                        r[y, x, 0] = (float)(60.0 * (2.0 + (image[y, x, 2] - image[y, x, 0]) / diff));
                    else if (max == image[y, x, 2])
                        r[y, x, 0] = (float)(60.0 * (4.0 + (image[y, x, 0] - image[y, x, 1]) / diff));
                    if (r[y, x, 0] < 0)
                        r[y, x, 0] = r[y, x, 0] + 360;
                    r[y, x, 0] = (float)r0.Clamp(r[y, x, 0]);
                    // Calculation of color channel 1 (S)
                    if (max == 0)
                        r[y, x, 1] = 0;
                    else
                        r[y, x, 1] = (float)r1.Clamp(diff / max);
                    // Calculation of color channel 2 (V)
                    r[y, x, 2] = (float)r2.Clamp(max);
                }
            }
            return r;
        }

        public static I<float> HsvToRgb(this I<float> image)
        {
            if (image.Channels != 3 || (image.Format.PixelChannels != PixelChannels.Unknown && image.Format.PixelChannels != PixelChannels.Hsv))
                throw new ArgumentException("HSV input image required");

            if (image.Format.ColorSpace != ColorSpace.Hsv && image.Format.ColorSpace != ColorSpace.Unknown)
                throw new Exception("Wrong color-space of input image. Hsv expected.");

            var r = CreateRgbF32(image.Height, image.Width);
            Range<double> r0 = r.Format.ChannelRanges[0], r1 = r.Format.ChannelRanges[1], r2 = r.Format.ChannelRanges[2];

            int h = 0;
            var f = 0.0;
            var p = 0.0;
            var q = 0.0;
            var t = 0.0;

            for (int y = 0; y < r.Height; ++y)
            {
                for (int x = 0; x < r.Width; ++x)
                {
                    // Calculation of auxiliary variables h, f, p, q and t
                    h = (int)(image[y, x, 0] / 60.0);
                    f = image[y, x, 0] / 60.0 - h;
                    p = image[y, x, 2] * (1 - image[y, x, 1]);
                    q = image[y, x, 2] * (1 - image[y, x, 1] * f);
                    t = image[y, x, 2] * (1 - image[y, x, 1] * (1 - f));
                    // Calculation of color channel 0 (R), 1 (G) and 2 (B)
                    if (h == 0 || h == 6)
                    {
                        r[y, x, 0] = (float)r0.Clamp(image[y, x, 2]);
                        r[y, x, 1] = (float)r1.Clamp(t);
                        r[y, x, 2] = (float)r2.Clamp(p);
                    }
                    else if (h == 1)
                    {
                        r[y, x, 0] = (float)r0.Clamp(q);
                        r[y, x, 1] = (float)r1.Clamp(image[y, x, 2]);
                        r[y, x, 2] = (float)r2.Clamp(p);
                    }
                    else if (h == 2)
                    {
                        r[y, x, 0] = (float)r0.Clamp(p);
                        r[y, x, 1] = (float)r1.Clamp(image[y, x, 2]);
                        r[y, x, 2] = (float)r2.Clamp(t);
                    }
                    else if (h == 3)
                    {
                        r[y, x, 0] = (float)r0.Clamp(p);
                        r[y, x, 1] = (float)r1.Clamp(q);
                        r[y, x, 2] = (float)r2.Clamp(image[y, x, 2]);
                    }
                    else if (h == 4)
                    {
                        r[y, x, 0] = (float)r0.Clamp(t);
                        r[y, x, 1] = (float)r1.Clamp(p);
                        r[y, x, 2] = (float)r2.Clamp(image[y, x, 2]);
                    }
                    else if (h == 5)
                    {
                        r[y, x, 0] = (float)r0.Clamp(image[y, x, 2]);
                        r[y, x, 1] = (float)r1.Clamp(p);
                        r[y, x, 2] = (float)r2.Clamp(q);
                    }
                }
            }
            return r;
        }

        public static I<float> CastFormat(this I<byte> image, PixelFormat targetFormat, bool rescaleChannels)
        {
            var imageFloat = image.ToF32();
            return CastFormat(imageFloat, targetFormat, rescaleChannels);
        }

        public static I<float> CastFormat(this I<float> image, PixelFormat targetFormat, bool rescaleChannels)
        {
            if (image.Format.Equals(targetFormat))
                return image;

            var dstImage = new I<float>(targetFormat, image.Height, image.Width, targetFormat.ChannelCount);
            var inRanges = image.Format.ChannelRanges;
            var outRanges = targetFormat.ChannelRanges;
            var channels = Math.Min(image.Channels, targetFormat.ChannelCount);

            if (rescaleChannels)
            {
                var offsets = new double[channels];
                var scales = new double[channels];

                for (int i = 0; i < channels; ++i)
                {
                    offsets[i] = inRanges[i].Low - outRanges[i].Low;
                    scales[i] = (outRanges[i].High - outRanges[i].Low) / (inRanges[i].High - inRanges[i].Low);
                }

                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        for (int c = 0; c < channels; ++c)
                        {
                            dstImage[y, x, c] = (float)outRanges[c].Clamp((image[y, x, c] - offsets[c]) * scales[c]);
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        for (int c = 0; c < channels; ++c)
                        {
                            dstImage[y, x, c] = (float)outRanges[c].Clamp(image[y, x, c]);
                        }
                    }
                }
            }

            return dstImage;
        }

        public static I<byte> ToU8(this I<float> image)
        {
            PixelFormat targetFormat;
            if (image.Format.ChannelCount == 1)
            {
                targetFormat = new PixelFormat(PixelType.U8C1, image.Format.PixelChannels, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue) }, image.Format.ColorSpace);
            }
            else if (image.Format.ChannelCount == 3)
            {
                targetFormat = new PixelFormat(PixelType.U8C3, image.Format.PixelChannels, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, Byte.MaxValue), new Range<double>(0, Byte.MaxValue) }, image.Format.ColorSpace);
            }
            else if (image.Format.ChannelCount == 4)
            {
                targetFormat = new PixelFormat(PixelType.U8C4, image.Format.PixelChannels, typeof(byte), new Range<double>[] { new Range<double>(0, Byte.MaxValue), new Range<double>(0, Byte.MaxValue), new Range<double>(0, Byte.MaxValue), new Range<double>(0, Byte.MaxValue) }, image.Format.ColorSpace);
            }
            else
            {
                throw new Exception("Source pixel fomat not supported");
            }

            var r = new I<byte>(targetFormat, image.Height, image.Width, image.Channels);
            float[] src = image.Data.Buffer;
            byte[] dst = r.Data.Buffer;

            for (int i = 0; i < dst.Length; ++i)
                dst[i] = (byte)(Range.Saturate(src[i]) * 255);

            return r;
        }
    }
}
