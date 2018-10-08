using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Xml.Linq;

namespace Xamla.Graph
{
    public interface IGraphRuntimeContext
    {
        IGraphRuntime Runtime { get; set; }
    }

    public static class GraphRuntimeContextExtension
    {
        public static IDisposable Establish(this IGraphRuntimeContext context, IGraphRuntime runtime)
        {
            var old = context.Runtime;
            context.Runtime = runtime;
            return Disposable.Create(() =>
            {
                if (object.ReferenceEquals(context.Runtime, runtime))
                    context.Runtime = old;
            });
        }
    }

    public interface IGraphRuntimeFactory
    {
        void AddDefaultModuleAssembly(Assembly assembly);
        void RegisterModuleFactoryInitializer(Action<IModuleFactory> initializer);
        IGraphRuntime Create();
        IGraph ParseXDocument(IGraphRuntime runtime, XDocument document, string filePath);
        IGraphInstance ParseGraphInstance(IGraphRuntime runtime, IContainer parentGraph, string container, string source);
    }
}
