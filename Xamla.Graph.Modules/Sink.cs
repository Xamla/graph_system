using Xamla.Graph.MethodModule;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Graph.Sink", Description = "This module simply pulls the connected sources.")]
    public class Sink
        : SingleInstanceMethodModule
    {
        public Sink(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Collapsed)
        {
        }

        [ModuleMethod]
        public void BlackHole(
            [InputPin(Description = "Connect any source to generate the result.", PropertyMode = PropertyMode.Never)] object any
            )
        {
        }
    }
}
