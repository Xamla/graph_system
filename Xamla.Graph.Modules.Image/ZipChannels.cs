using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.ZipChannels")]
    public class ZipChannels
        : SingleInstanceMethodModule
    {
        public ZipChannels(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Zip(IImageBuffer[] images, [InputPin(PropertyMode = PropertyMode.Default)]  PixelChannels resultChannels)
        {
            if (images == null)
                throw new ArgumentNullException("images");


            if (images.Length < 1 || images.Length > 4 || images.Length == 2)
                throw new NotSupportedException(string.Format("Number of Channels '{0}' not supported for Image", images.Length));

            var firstImage = images.First();
            foreach (var source in images)
            {
                if (!source.Format.Equals(firstImage.Format))
                    throw new ArgumentException("Images should have all the same Format", "images");
            }

            var channelRanges = new Range<double>[images.Length];
            for (int i = 0; i < images.Count(); ++i)
                channelRanges[i] = Range.Unit;

            var pixelType = (PixelType)((int)ChannelType.F32 | images.Length);
            var format = new PixelFormat(pixelType, resultChannels, typeof(float), channelRanges, ColorSpace.Unknown);

            var destination = new I<float>(format, firstImage.Height, firstImage.Width, images.Count());
            for (int c = 0; c < images.Length; ++c)
                destination.SetChannel(images[c].ToF32(), c);
            return destination;
        }
    }
}
