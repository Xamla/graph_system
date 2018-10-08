using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.ScaleRanges")]
    public class ScaleRanges
        : SingleInstanceMethodModule
    {
        public ScaleRanges(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Scale(IImageBuffer image, [InputPin(PropertyMode = PropertyMode.Default)] Range<double>[] inRange, [InputPin(PropertyMode = PropertyMode.Default)] Range<double>[] outRange, [InputPin(PropertyMode = PropertyMode.Default)] double[] gamma = null)
        {
            return image.ToF32().ScaleRanges(inRange, outRange, gamma);
        }
    }
}
