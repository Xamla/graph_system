using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    public class ImageBufferPreviewGenerator
        : PreviewGeneratorBase
    {
        object gate = new object();
        IImageBuffer previewImage;

        public ImageBufferPreviewGenerator(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        internal bool IgnoreEmptyOutputs { get; set; }

        protected override object PreviewImage =>
            previewImage;

        protected override void SetOutputsInternal(params object[] outputs)
        {
            var supportedOutput = outputs
                .OfType<IImageBuffer>()
                .FirstOrDefault();

            if (this.IgnoreEmptyOutputs && supportedOutput == null)
                return;

            lock (gate)
            {
                if (this.previewImage != null)
                {
                    this.previewImage.Dispose();
                    this.previewImage = null;
                }

                if (supportedOutput != null)
                {
                    this.previewImage = this.RenderThumbnail(supportedOutput);
                }
            }

            IsDirty = true;
        }

        public override void WritePreviewTo(Stream destination)
        {
            lock (gate)
            {
                if (this.previewImage == null)
                    throw new Exception("Preview unavailable.");

                var converter = this.Runtime.TypeConverters.TryGetConverter(typeof(IImageBuffer), typeof(SKBitmap));
                if (converter == null)
                    throw new Exception("Preview stream writer is not registered");

                using (var bitmap = (SKBitmap)converter.Convert(this.previewImage))
                {
                    using (var skStream = new SKManagedWStream(destination))
                    {
                        bitmap.Encode(skStream, SKEncodedImageFormat.Jpeg, 90);
                    }
                }
            }
        }

        public IImageBuffer RenderThumbnail(IImageBuffer image)
        {
            IImageBuffer resizedImage = null;

            if (image.Width == 0 || image.Height == 0)
                return I.CreateRgbF32(0, 0);

            Int2 size;
            IntRect cropRect;
            base.CalculateNewSize(new Int2(image.Width, image.Height), this.DesiredSize, out size, out cropRect);
            if (image.Width <= size.X && image.Height <= size.Y)
                return image;

            resizedImage = image.ToF32().Resize(size, cropRect, ResizeFilterType.Quadratic);
            var f32image = resizedImage as I<float>;
            if (f32image != null)
            {
                return f32image.ToU8();
            }

            return resizedImage;
        }
    }

    [Module(ModuleType="Xamla.Image.ViewImage")]
    public class ViewImage
        : SingleInstanceMethodModule
    {
        public ViewImage(IGraphRuntime runtime)
            : base(runtime)
        {
            this.PreviewGenerator = new ImageBufferPreviewGenerator(runtime) { IgnoreEmptyOutputs = true };
        }

        [ModuleMethod]
        public void View([InputPin(PropertyMode = PropertyMode.Never)] IImageBuffer image)
        {
            this.PreviewGenerator.SetOutputs(image);
        }
    }
}
