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
    [Module(ModuleType = "Xamla.Image.Quantization")]
    public class Quantization
        : SingleInstanceMethodModule
    {
        public Quantization(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Process(IImageBuffer image, [InputPin(PropertyMode = PropertyMode.Default)] double[] stepSize)
        {
            return image.ToF32().Quantization(stepSize);
        }
    }
}
