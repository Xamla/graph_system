using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;
using Xamla.Types;

namespace Xamla.Graph.Modules.Skia
{
    public class SkiaPreviewGenerator
        : PreviewGeneratorBase
    {
        object gate = new object();
        SKBitmap previewImage;

        public SkiaPreviewGenerator(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        protected override object PreviewImage => previewImage;

        protected override void SetOutputsInternal(params object[] outputs)
        {
            var supportedOutput = outputs
                .OfType<SKBitmap>()
                .SingleOrDefault();

            lock (gate)
            {
                if (supportedOutput == null)
                    this.previewImage = null;
                else
                {
                    if (previewImage != null)
                    {
                        previewImage.Dispose();
                    }

                    this.previewImage = this.RenderThumbnailInternal(supportedOutput);
                }
            }

            this.IsDirty = true;
        }

        public override void WritePreviewTo(Stream destination)
        {
            lock (gate)
            {
                if (previewImage == null)
                    throw new Exception("Preview unavailable.");

                using (var skStream = new SKManagedWStream(destination))
                {
                    previewImage.Encode(skStream, SKEncodedImageFormat.Jpeg, 90);
                }
            }
        }

        SKBitmap RenderThumbnailInternal(SKBitmap bitmap)
        {
            SKBitmap resizedBitmap = null;

            if (base.CalculateNewSize(new Int2(bitmap.Width, bitmap.Height), this.DesiredSize, out Int2 size, out IntRect cropArea))
            {
                // generate new cropped bitmap
                using (var surface = SKSurface.Create(new SKImageInfo(size.X, size.Y, SKColorType.Bgra8888)))
                {
                    // the the canvas and properties
                    var canvas = surface.Canvas;

                    // make sure the canvas is blank
                    canvas.Clear(SKColors.White);

                    canvas.DrawBitmap(
                        bitmap,
                        new SKRect(cropArea.Left, cropArea.Top, cropArea.Width, cropArea.Height),
                        new SKRect(0, 0, size.X, size.Y),
                        new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true }
                    );

                    using (var image = surface.Snapshot())
                    {
                        resizedBitmap = SKBitmap.FromImage(image);
                    }
                }
            }
            else
            {
                resizedBitmap = new SKBitmap(size.X, size.Y, bitmap.ColorType, bitmap.AlphaType);
                bitmap.Resize(resizedBitmap, SKBitmapResizeMethod.Mitchell);
            }

            return resizedBitmap;
        }
    }
}
