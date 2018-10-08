using System.Collections.Generic;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.SplitChannels")]
    public class SplitChannels
        : SingleInstanceMethodModule
    {
        public SplitChannels(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public IEnumerable<I<float>> Split(IImageBuffer image)
        {
            var img = image.ToF32();
            for (int c = 0; c < img.Channels; ++c)
                yield return img.GetChannel(c);
        }
    }
}
