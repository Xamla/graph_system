using System;

namespace Xamla.Graph
{
    public interface IGraphInstance
        : IModule
    {
        string Source { get; }
        String ContainerAlias { get; }
    }
}
