using System;
using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.Addition")]
    public class Addition
        : SingleInstanceMethodModule
    {
        public Addition(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, null, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public IImageBuffer Calculate(IImageBuffer image1, IImageBuffer image2)
        {
            return I.Add(image1.ToF32(), image2.ToF32());
        }
    }

    [Module(ModuleType = "Xamla.Image.Difference")]
    public class Difference
        : SingleInstanceMethodModule
    {
        public Difference(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, null, new ImageBufferPreviewGenerator(runtime)) 
        {
        }

        [ModuleMethod]
        public IImageBuffer Calculate(IImageBuffer image1, IImageBuffer image2)
        {
            return I.Difference(image1.ToF32(), image2.ToF32());
        }
    }

    [Module(ModuleType = "Xamla.Image.Multiply")]
    public class Multiply
        : SingleInstanceMethodModule
    {
        public Multiply(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, null, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public IImageBuffer Calculate(IImageBuffer image1, IImageBuffer image2)
        {
            return I.Multiply(image1.ToF32(), image2.ToF32());
        }
    }

    [Module(ModuleType = "Xamla.Image.Division")]
    public class Division
        : SingleInstanceMethodModule
    {
        public Division(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, null, new ImageBufferPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public IImageBuffer Calculate(IImageBuffer image1, IImageBuffer image2)
        {
            return I.Divide(image1.ToF32(), image2.ToF32());
        }
    }
}
