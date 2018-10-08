using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using Xamla.Types;
using Xamla.Types.Converters;

namespace Xamla.Graph.Modules.Skia
{
    public class SkiaConverter
        : ITypeConversionProvider
    {
        static IImageBuffer ToImageBuffer(SKBitmap bitmap)
        {
            var buffer = bitmap.Bytes;
            A<byte> data;
            if (bitmap.BytesPerPixel == 1)
            {
                data = new A<byte>(
                    buffer,
                    new int[] { bitmap.Height, bitmap.Width },
                    new int[] { bitmap.RowBytes, 1 }
                );
            }
            else
            {
                data = new A<byte>(
                    buffer,
                    new int[] { bitmap.Height, bitmap.Width, bitmap.BytesPerPixel },
                    new int[] { bitmap.RowBytes, bitmap.BytesPerPixel, 1 }
                );
            }

            PixelFormat format;
            if (bitmap.ColorType == SKColorType.Rgba8888)
            {
                if (bitmap.AlphaType == SKAlphaType.Opaque)
                    format = PixelFormat.RgbxU8;
                else
                    format = PixelFormat.RgbaU8;
            }
            else if (bitmap.ColorType == SKColorType.Bgra8888)
            {
                if (bitmap.AlphaType == SKAlphaType.Opaque)
                    format = PixelFormat.BgrxU8;
                else
                    format = PixelFormat.BgraU8;
            }
            else if (bitmap.ColorType == SKColorType.Gray8)
            {
                format = PixelFormat.GrayU8;
            }
            else
                throw new NotSupportedException("Input image format conversion not supported");

            return new I<byte>(format, data);
        }

        public IImageBuffer SKBitmapToImageBuffer(SKBitmap bitmap)
        {
            if (bitmap == null)
                return null;

            if (bitmap.ColorType != SKColorType.Rgba8888 && bitmap.ColorType != SKColorType.Bgra8888 && bitmap.ColorType != SKColorType.Gray8)
            {
                // try to convert image to 32BPP bitmap if source pixel format is unknown
                if (!bitmap.CanCopyTo(SKColorType.Rgba8888))
                    throw new NotSupportedException("SKImage Input pixel format not supported.");

                using (bitmap = bitmap.Copy(SKColorType.Rgba8888))
                {
                    return ToImageBuffer(bitmap);
                }
            }
            else
            {
                return ToImageBuffer(bitmap);
            }
        }

        public SKBitmap ImageBufferToSKBitmap(IImageBuffer imageBuffer)
        {
            if (imageBuffer == null)
                return null;

            imageBuffer = imageBuffer.ToU8();

            SKBitmap bitmap = null;
            SKColorSpace colorSpace = null;

            if (imageBuffer.Format.ColorSpace == ColorSpace.Srgb)
                colorSpace = SKColorSpace.CreateSrgb();
            else if (imageBuffer.Format.ColorSpace == ColorSpace.LinearRgb)
                colorSpace = SKColorSpace.CreateSrgbLinear();

            try
            {
                // for RGBA-images
                if (imageBuffer.Format.PixelType == PixelType.U8C3)
                {
                    // convert to 32bpp
                    var src = imageBuffer is I<byte> ? ((I<byte>)imageBuffer).Data.Buffer : imageBuffer.ToByteArray();
                    var dst = new byte[imageBuffer.Height * imageBuffer.Width * 4];
                    int i = 0, j = 0;
                    while (i < dst.Length)
                    {
                        dst[i+0] = src[j+0];
                        dst[i+1] = src[j+1];
                        dst[i+2] = src[j+2];
                        dst[i+3] = 0xff;
                        i += 4;
                        j += 3;
                    }

                    SKColorType colorType = SKColorType.Rgba8888;
                    switch (imageBuffer.Format.PixelChannels)
                    {
                        case PixelChannels.Rgb:
                            colorType = SKColorType.Rgba8888;
                            break;
                        case PixelChannels.Bgra:
                            colorType = SKColorType.Bgra8888;
                            break;
                    }

                    using (var pinnedImage = new Utilities.PinnedGCHandle(dst))
                    {
                        var sourceInfo = new SKImageInfo(imageBuffer.Width, imageBuffer.Height, colorType, SKAlphaType.Opaque);
                        if (colorSpace != null)
                        {
                            sourceInfo.ColorSpace = colorSpace;
                        }
                        bitmap = new SKBitmap();
                        Debug.Assert(sourceInfo.RowBytes == imageBuffer.Width * 4);
                        if (!bitmap.InstallPixels(sourceInfo, pinnedImage.Pointer))
                        {
                            throw new Exception("InstallPixels poperation of Skia bitmap failed.");
                        }
                    }
                }
                // for RGBA-images
                else if (imageBuffer.Format.PixelType == PixelType.U8C4)
                {
                    // try direct copy
                    using (var pinnedImage = imageBuffer.Pin())
                    {
                        SKAlphaType alphaType = SKAlphaType.Unknown;
                        SKColorType colorType = SKColorType.Rgba8888;
                        switch (imageBuffer.Format.PixelChannels)
                        {
                            case PixelChannels.Rgbx:
                                alphaType = SKAlphaType.Opaque;
                                colorType = SKColorType.Rgba8888;
                                break;
                            case PixelChannels.Rgba:
                                alphaType = SKAlphaType.Unpremul;
                                colorType = SKColorType.Rgba8888;
                                break;
                            case PixelChannels.Bgrx:
                                alphaType = SKAlphaType.Opaque;
                                colorType = SKColorType.Bgra8888;
                                break;
                            case PixelChannels.Bgra:
                                alphaType = SKAlphaType.Unpremul;
                                colorType = SKColorType.Bgra8888;
                                break;
                        }
                        var sourceInfo = new SKImageInfo(imageBuffer.Width, imageBuffer.Height, colorType, alphaType);
                        Debug.Assert(sourceInfo.RowBytes == imageBuffer.Width * 4);
                        if (colorSpace != null)
                        {
                            sourceInfo.ColorSpace = colorSpace;
                        }
                        bitmap = new SKBitmap();
                        if (!bitmap.InstallPixels(sourceInfo, pinnedImage.Pointer))
                        {
                            throw new Exception("InstallPixels poperation of Skia bitmap failed.");
                        }
                    }
                }
                // for gray-scale
                else if (imageBuffer.Format.PixelType == PixelType.U8C1)
                {
                    // try direct copy
                    using (var pinnedImage = imageBuffer.Pin())
                    {
                        var sourceInfo = new SKImageInfo(imageBuffer.Width, imageBuffer.Height, SKColorType.Gray8);
                        Debug.Assert(sourceInfo.RowBytes == imageBuffer.Width);
                        bitmap = new SKBitmap();
                        if (!bitmap.InstallPixels(sourceInfo, pinnedImage.Pointer))
                        {
                            throw new Exception("InstallPixels poperation of Skia bitmap failed.");
                        }
                    }
                }
                else
                {
                    throw new Exception("PixelFormat not yet implemented for preview image.");
                }

                SKBitmap result = bitmap;
                bitmap = null;
                colorSpace = null;
                return result;
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                    if (colorSpace != null)
                    {
                        colorSpace.Dispose();
                    }
                }
            }
        }

        public IEnumerable<ITypeConverter> GetConverters()
        {
            // conversion Bitmap <-> IImage
            return new[]
            {
                TypeConverter.Create<IImageBuffer, SKBitmap>(this.ImageBufferToSKBitmap),
                TypeConverter.Create<SKBitmap, IImageBuffer>(this.SKBitmapToImageBuffer)
            };
        }

        public Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> GetSerializers()
        {
            return customSerializedTypes;
        }

        static readonly Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> customSerializedTypes = new Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>>
        {
        };

        public IEnumerable<IDynamicTypeConverter> GetDynamicConverters()
        {
            return new IDynamicTypeConverter[] { };
        }
    }
}
