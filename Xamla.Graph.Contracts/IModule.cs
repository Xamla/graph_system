using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph
{
    public enum DisplayMode
    {
        Collapsed,
        Expanded
    }

    public interface IPreviewGenerator
        : INode
    {
        bool Active { get; set; }
        bool HasPreview { get; }
        void SetOutputs(params object[] outputs);
        void WritePreviewTo(Stream destination);
        Int2? DesiredSize { get; set; }
    }

    public interface IDynamicDisplayName
    {
        string Format(IModule module);
    }

    public enum ModuleKind
    {
        Module,
        Control,
        ScriptModule,
        GraphInstance,
        SubGraphModule,
        InputModule,
        OutputModule,
        StartModule,
        EndModule,
        ViewModule,
        Comment,
        Iframe,
        Port,
        PortReference,
        Other,
        ModuleNotFound
    }

    [Flags]
    public enum FlowMode
    {
        /// <summary>
        /// The module has no flow pins.
        /// </summary>
        NoFlow = 0,

        /// <summary>
        /// The module should be evaluated when any flow pin got a value.
        /// </summary>
        WaitAny = 1,

        /// <summary>
        /// The module should be evaluated when all flow pins got a value.
        /// </summary>
        WaitAll = 2,

        /// <summary>
        /// The module waits for all flow pins but fails early if an exception is received on one pin.
        /// </summary>
        WaitAllOrFirstException = WaitAll | 4,

        /// <summary>
        /// Flag indicating that the module show be evaluated even if the flow propagates an exception.
        /// This flag allows modules to how to continue in an exception case, e.g. by passing on the
        /// faulted flow or handling the excetpion and continuing with normal flow.
        /// </summary>
        EvaluateWithException = 8
    };

    public interface IModule
        : IContainer
    {
        IContainer<IInputPin> Inputs { get; }
        IContainer<IOutputPin> Outputs { get; }
        IOutputPin VirtualOutputPin { get; }
        IPropertyContainer Properties { get; }
        IPreviewGenerator PreviewGenerator { get; }
        string ModuleType { get; }
        ModuleKind ModuleKind { get; }
        Int2 Position { get; set; }
        bool ShowPreview { get; set; }
        FlowMode FlowMode { get; }
        bool Active { get; }
        string DisplayName { get; }
        DisplayMode Display { get; set; }
        Task<object[]> Evaluate(object[] inputs, CancellationToken cancel);
        ModuleDescription Description { get; }
        TimeSpan EvaluationTime { get; set; }
    }

    public interface IControlModule
    {
        IOutputPin OutputPin { get; }
        string Caption { get; set; }
        Int2 Size { get; set; }
    }

    public interface IReorderableInterfaceModule
    {
        bool ReorderPins(IList<string> objectIds);
    }

    public interface IInterfaceModule
        : IModule
        , IReorderableInterfaceModule
    {
        PinDirection InterfaceDirection { get; }

        IPin AddInterfacePin(string id = null, bool loading = false);
        bool RemoveInterfacePin(string id);
    }

    public interface IGraphInterfaceModule
        : IInterfaceModule
    {
        IPin GetExternalPin(string id);
        IPin GetInternalPin(string id);

        IEnumerable<IPin> ExternalPins { get; }
    }

    public interface IGraphFlowInterfaceModule
        : IModule
    {
        IPin ExternalPin { get; }
    }

    public interface ISubGraph
        : IMutableContainer<INode>
    {
        IInputPin Sink { get; }
        IInterfaceModule InputInterface { get; }
        IInterfaceModule OutputInterface { get; }
        IEnumerable<PinConnection> AdditionalConnections { get; }
    }

    public interface ISubGraphModule
        : IModule
    {
        ISubGraph SubGraph { get; }
        bool ShowSubGraph { get; set; }
        Int2 Size { get; set; }
    }

    public static class ModuleExtensions
    {
        public static IInputPin GetInputPin(this IModule module, string id)
        {
            return module.Inputs.FirstOrDefault(x => x.Id == id);
        }

        public static IOutputPin GetOutputPin(this IModule module, string id)
        {
            return module.Outputs.FirstOrDefault(x => x.Id == id);
        }
    }
}
