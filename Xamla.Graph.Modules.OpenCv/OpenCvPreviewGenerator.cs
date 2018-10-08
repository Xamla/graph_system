using OpenCvSharp;
using System;
using System.IO;
using System.Linq;
using Xamla.Types;

namespace Xamla.Graph.Modules.OpenCv
{
    public class OpenCvPreviewGenerator
        : PreviewGeneratorBase
    {
        object gate = new object();
        Mat previewImage;

        public OpenCvPreviewGenerator(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        internal bool IgnoreEmptyOutputs { get; set; }

        protected override object PreviewImage
        {
            get { return previewImage; }
        }

        protected override void SetOutputsInternal(params object[] outputs)
        {
            Mat supportedOutput = null;

            try
            {
                supportedOutput = Enumerable.Concat(
                        outputs.OfType<Mat>(),
                        outputs.OfType<OutputArray>().Select(x => x.GetMat())
                    )
                    .FirstOrDefault();
            }
            catch (InvalidOperationException)
            {
                //ignore
            }

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
                    this.previewImage = this.RenderThumbnailInternal(supportedOutput);
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

                using (var ms = this.previewImage.ToMemoryStream(".jpg"))
                {
                    ms.WriteTo(destination);
                }
            }
        }

        private Mat RenderThumbnailInternal(Mat image)
        {
            Mat croppedMat = null, resizedMat = null;

            try
            {
                var inputSize = new Int2(image.Width, image.Height);
                if (base.CalculateNewSize(inputSize, this.DesiredSize, out Int2 size, out IntRect cropArea))
                {
                    //croppedMat = image.SubMat(new Rect(cropArea.Left, cropArea.Top, cropArea.Width, cropArea.Height));
                }

                resizedMat = new Mat(size.Y, size.X, image.Type());
                if (inputSize.X > 0 && inputSize.Y > 0 && size.X > 0 && size.Y > 0)
                {
                    Cv2.Resize((croppedMat ?? image), resizedMat, new Size(size.X, size.Y));
                }
            }
            catch
            {
                // ignore error
            }
            finally
            {
                if (croppedMat != null)
                    croppedMat.Dispose();
            }

            return resizedMat;
        }
    }
}
