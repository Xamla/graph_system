using System;

namespace Xamla.Graph
{
    public interface IGraphRuntimeInitializer
    {
        void Initialize(IGraphRuntime runtime);
    }
}
