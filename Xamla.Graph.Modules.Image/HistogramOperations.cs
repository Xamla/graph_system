using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.HistogramEqualization")]
    public class HistogramEqualization
        : SingleInstanceMethodModule
    {
        public HistogramEqualization(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Equalize(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] int bins
        )
        {
            return image.ToF32().HistogramEqualization(bins);
        }
    }

    [Module(ModuleType = "Xamla.Image.SingleChannelHistogrammEqualization")]
    public class SingleChannelHistogrammEqualization
        : SingleInstanceMethodModule
    {
        public SingleChannelHistogrammEqualization(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Equalize(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] int channel,
            [InputPin(PropertyMode = PropertyMode.Default)] int bins
        )
        {
            return image.ToF32().EqualizeSingleChannel(channel, bins);
        }
    }

    [Module(ModuleType = "Xamla.Image.AdaptiveHistogramEqualization")]
    public class AdaptiveHistogramEqualization
       : SingleInstanceMethodModule
    {
        public AdaptiveHistogramEqualization(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> EqualizeAdaptive(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] int bins,
            [InputPin(PropertyMode = PropertyMode.Default)] int numTilesY,
            [InputPin(PropertyMode = PropertyMode.Default)] int numTilesX,
            [InputPin(PropertyMode = PropertyMode.Default)] double normalizedClipLimit
        )
        {
            return image.ToF32().AdaptiveHistogramEqualization(bins, numTilesY, numTilesX, normalizedClipLimit);
        }
    }

    [Module(ModuleType = "Xamla.Image.HistogramGenerator")]
    public class HistogramGenerator
        : SingleInstanceMethodModule
    {
        public HistogramGenerator(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public int[][] CreateHistogramm(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] int bins
        )
        {
            return image.ToF32().CreateHistogram(bins);
        }
    }

    [Module(ModuleType = "Xamla.Image.ChannelHistogramGenerator")]
    public class ChannelHistogramGenerator
        : SingleInstanceMethodModule
    {
        public ChannelHistogramGenerator(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public int[] CreateChannelHistogram(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] int bins,
            [InputPin(PropertyMode = PropertyMode.Default)] int channel
        )
        {
            return image.ToF32().CreateChannelHistogram(bins, channel);
        }
    }

    [Module(ModuleType = "Xamla.Image.ChannelHistogrammOfRectGenerator")]
    public class ChannelHistogrammOfRectGenerator
        : SingleInstanceMethodModule
    {
        public ChannelHistogrammOfRectGenerator(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public int[] CreateChannelHistogramOfRect(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] int startRow,
            [InputPin(PropertyMode = PropertyMode.Default)] int startCol,
            [InputPin(PropertyMode = PropertyMode.Default)] int height,
            [InputPin(PropertyMode = PropertyMode.Default)] int width,
            [InputPin(PropertyMode = PropertyMode.Default)] int bins,
            [InputPin(PropertyMode = PropertyMode.Default)] int channel
        )
        {
            return image.ToF32().CreateChannelHistogramOfRect(startRow, startCol, height, width, bins, channel);
        }
    }
}
