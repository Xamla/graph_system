using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.ConvertToU8")]
    public class ConvertToU8
        : SingleInstanceMethodModule
    {
        public ConvertToU8(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<byte> ConvertDepth(IImageBuffer image)
        {
            return image.ToU8();
        }
    }

    [Module(ModuleType = "Xamla.Image.ConvertToF32")]
    public class ConvertToF32
        : SingleInstanceMethodModule
    {
        public ConvertToF32(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> ConvertDepth(IImageBuffer image)
        {
            return image.ToF32();
        }
    }
}
