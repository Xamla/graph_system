using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.Resize")]
    public class ResizeImage
        : SingleInstanceMethodModule
    {
        public ResizeImage(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Filter(
            IImageBuffer image, 
            [InputPin(PropertyMode = PropertyMode.Default)] int width, 
            [InputPin(PropertyMode = PropertyMode.Default)] int height, 
            [InputPin(PropertyMode = PropertyMode.Default)] ResizeFilterType resizeFilterType = ResizeFilterType.Lanczos8
        )
        {
            return image.ToF32().Resize(new Int2(width, height), resizeFilterType);
        }
    }
}
