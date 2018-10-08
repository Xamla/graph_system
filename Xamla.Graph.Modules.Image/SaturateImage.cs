using System.Collections.Generic;
using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.AutoSaturate")]
    public class AutoSaturateImage
        : SingleInstanceMethodModule
    {
        public AutoSaturateImage(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Saturate(IImageBuffer image, [InputPin(PropertyMode = PropertyMode.Default)] double saturationFactor = 0.01, [InputPin(PropertyMode = PropertyMode.Default)] int bins = 500)
        {
            return image.ToF32().AutoSaturate(saturationFactor, bins);
        }
    }
}
