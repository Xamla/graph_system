using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xamla.Graph;

[assembly: GraphRuntimeInitializer(typeof(Xamla.Graph.Modules.Initializer))]

namespace Xamla.Graph.Modules
{
    /// <summary>
    /// Initializer for static standard modules
    /// </summary>
    class Initializer
        : IGraphRuntimeInitializer
    {
        public void Initialize(IGraphRuntime runtime)
        {
            RunProcessModule.Init(
                runtime.ServiceLocator.GetService<ILoggerFactory>()
            );
            VariableModules.Init(runtime);
        }
    }
}
