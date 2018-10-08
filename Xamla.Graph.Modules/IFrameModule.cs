using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    public class IFrameModuleBase
        : ModuleBase
    {
        public IFrameModuleBase(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Iframe, DisplayMode.Expanded)
        {
            this.AddInputPin("Caption", PinDataTypeFactory.Create<string>(), PropertyMode.Always);
            this.AddInputPin("SourceUri", PinDataTypeFactory.Create<string>(), PropertyMode.Always);
            this.AddInputPin("Size", PinDataTypeFactory.Create<Int2>(new Int2(320, 240)), PropertyMode.Always);
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            return null;
        }
    }

    [Module(ModuleType = "Xamla.Graph.Iframe")]
    public class IframeModule
        : IFrameModuleBase
    {
        public IframeModule(IGraphRuntime runtime)
            : base(runtime)
        {
        }
    }
}
