using System.Threading.Tasks;
using Xamla.Graph.MethodModule;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Yield", Description = "The following Graph connected to the output will be executed on a separated Thread.")]
    public class Yield
        : SingleInstanceMethodModule
    {
        public Yield(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Collapsed)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "Object", Description = "Object")]
        public async Task<object> AsyncOperation(
            [InputPin(Description = "The value will just be pass through to the other Thread.", PropertyMode = PropertyMode.Allow)] object value
        )
        {
            await Task.Yield();
            return value;
        }
    }
}
