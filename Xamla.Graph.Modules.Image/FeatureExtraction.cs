using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.DifferenceOfGaussianFilter")]
    public class DifferenceOfGaussianFilter
        : SingleInstanceMethodModule
    {
        public DifferenceOfGaussianFilter(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Filter(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] double weight
        )
        {
            return image.ToF32().FastDoGFilter(weight);
        }
    }

    [Module(ModuleType = "Xamla.Image.DivisionOfGaussianFilter")]
    public class DivisionOfGaussianFilter
        : SingleInstanceMethodModule
    {
        public DivisionOfGaussianFilter(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Filter(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] int matrixSize,
            [InputPin(PropertyMode = PropertyMode.Default)] double weight,
            [InputPin(PropertyMode = PropertyMode.Default)] double scaling
        )
        {
            return image .ToF32().DivisionOfGaussianFilter(matrixSize, weight, scaling);
        }
    }

    [Module(ModuleType = "Xamla.Image.FastDivisionFilter")]
    public class FastDivisionFilter
        : SingleInstanceMethodModule
    {
        public FastDivisionFilter(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Filter(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] double weight,
            [InputPin(PropertyMode = PropertyMode.Default)] double scaling
        )
        {
            return image.ToF32().FastDivisionFilter(weight, scaling);
        }
    }

    [Module(ModuleType = "Xamla.Image.CenteringAroundMean")]
    public class CenteringAroundMean
        : SingleInstanceMethodModule
    {
        public CenteringAroundMean(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public I<float> Filter(IImageBuffer image)
        {
            return image.ToF32().CenteringAroundMean();
        }
    }
}
