using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.Crop")]
    public class CropImage
        : SingleInstanceMethodModule
    {
        public CropImage(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Crop(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] IntRect area
        )
        {
            return image.ToF32().Crop(area);
        }
    }
}
