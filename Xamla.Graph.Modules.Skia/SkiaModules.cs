using System;
using System.IO;
using SkiaSharp;
using Xamla.Graph.MethodModule;
using Xamla.Types;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.Skia
{
    public static class SkiaModules
    {
        [StaticModule(ModuleType = "Skia.LoadBitmap", PreviewGenerator = typeof(SkiaPreviewGenerator))]
        public static SKBitmap LoadBitmap(
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path
        )
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File '{path}' does not exist.", path);

            return SKBitmap.Decode(path);
        }

        [StaticModule(ModuleType = "Skia.SaveBitmap")]
        public static void SaveBitmap(
            [InputPin(PropertyMode = PropertyMode.Never)] SKBitmap bitmap,
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path,
            [InputPin(PropertyMode = PropertyMode.Default)] SKEncodedImageFormat format = SKEncodedImageFormat.Png,
            [InputPin(PropertyMode = PropertyMode.Default)] int quality = 95
        )
        {
            using (var stream = new SKFileWStream(path))
            {
                SKPixmap.Encode(stream, bitmap, format, quality);
            }
        }

        [StaticModule(ModuleType = "Skia.DecodeBitmap")]
        public static SKBitmap DecodeBitmap(
            [InputPin(PropertyMode = PropertyMode.Never)] IReadable source
        )
        {
            using (var stream = source.Open())
            {
                return SKBitmap.Decode(stream);
            }
        }

        [StaticModule(ModuleType = "Skia.EncodeBitmap")]
        public static IWritable EncodeBitmap(
            [InputPin(PropertyMode = PropertyMode.Never)] SKBitmap bitmap,
            [InputPin(PropertyMode = PropertyMode.Default)] SKEncodedImageFormat format = SKEncodedImageFormat.Png,
            [InputPin(PropertyMode = PropertyMode.Default)] int quality = 95
        )
        {
            return Writable.Create(stream =>
            {
                using (var skStream = new SKManagedWStream(stream))
                {
                    SKPixmap.Encode(skStream, bitmap, format, quality);
                }
            });
        }

        [StaticModule(ModuleType = "Skia.ResizeBitmap", PreviewGenerator = typeof(SkiaPreviewGenerator))]
        public static SKBitmap ResizeBitmap(
            [InputPin(PropertyMode = PropertyMode.Never)] SKBitmap bitmap,
            [InputPin(PropertyMode = PropertyMode.Default)] int width = 256,
            [InputPin(PropertyMode = PropertyMode.Default)] int height = 256,
            [InputPin(PropertyMode = PropertyMode.Default)] SKBitmapResizeMethod method = SKBitmapResizeMethod.Lanczos3
        )
        {
            if (bitmap.ColorType != SKImageInfo.PlatformColorType)
            {
                using (var platformBitmap = new SKBitmap())
                {
                    if (!bitmap.CopyTo(platformBitmap, SKImageInfo.PlatformColorType))
                        throw new Exception($"Could not convert input image ({bitmap.ColorType}) to SKIA platform color type ({SKImageInfo.PlatformColorType}).");

                    var destinationInfo = platformBitmap.Info;
                    destinationInfo.Width = width;
                    destinationInfo.Height = height;
                    return platformBitmap.Resize(destinationInfo, method);
                }
            }
            else
            {
                SKBitmap result = new SKBitmap(width, height, bitmap.ColorType, bitmap.AlphaType);
                bitmap.Resize(result, method);
                return result;
            }
        }
    }
}
