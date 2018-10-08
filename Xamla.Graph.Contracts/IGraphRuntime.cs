using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Graph.Models;
using Xamla.Types.Converters;
using Xamla.Types.Records;

namespace Xamla.Graph
{
    public enum RuntimeEvent
    {
        Start,
        Started,
        Stop,
        Stopped
    }

    public enum RuntimeStatus
    {
        Initializing,
        Stopped,
        Starting,
        Running,
        Stopping
    }

    public interface IGraphRuntime
        : IContainer<INode>
    {
        object SyncRoot { get; }
        long ChangeSerialNumber { get; }

        RuntimeStatus Status { get; }
        IObservable<RuntimeEvent> WhenExecutionControlEvent { get; }
        IObservable<Unit> WhenGraphLoadedEvent { get; }
        ISubject<InspectResultEvent> WhenInspectResult { get; }

        Task Completion { get; }
        Task<object>[] OutputInterfaceTasks { get; }
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Asynchronously change the running state of the graph.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>A task that asynchronously signals completion.</returns>
        void Start();
        Task Stop();

        IGraph Root { get; set; }
        IServiceLocator ServiceLocator { get; }
        IModuleFactory ModuleFactory { get; }
        IFileSystem FileSystem { get; }
        ILoggerFactory LoggerFactory { get; }

        //IPythonInterpreter Python { get; }
        IDependencyDownloader DependencyDownloader { get; }
        ITypeConverterMap TypeConverters { get; }
        Dictionary<Type, SerializationFunctions> TypeSerializers { get; }

        /// <summary>
        /// The sink drives the graph by merging the sequences of output modules.
        /// The graph completes when all connected sequences have signalled
        /// their completion.
        /// </summary>
        IInputPin Sink { get; }

        void AddSchemaProvider(ISchemaProvider provider);
        void RemoveSchemaProvider(ISchemaProvider provider);
        ISchemaProvider SchemaProvider { get; }
        int NextSchemaId { get; }

        INode GetNodeByObjectId(string objectId);
        void RegisterNode(INode node);
        void UnregisterNode(INode node);

        ContainerChangeCollection GetChangesAndMarkClean();

        IRuntimeSettingsNode Settings { get; }

        ICSharpCodeManager CodeManager { get; }
        ICompilerDiagnosticsNode CompilerDiagnostics { get; }

        IScriptContext ScriptContext { get; }
    }

    public class ValueStoreChangedEvent
    {
        public IValueStore Sender { get; }
        public IImmutableDictionary<string, object> Before { get; }
        public IImmutableDictionary<string, object> After { get; }

        public ValueStoreChangedEvent(IValueStore sender, IImmutableDictionary<string, object> before, IImmutableDictionary<string, object> after)
        {
            this.Sender = sender;
            this.Before = before;
            this.After = after;
        }
    }

    public interface IValueStore
        : IDictionary<string, object>
    {
        IObservable<ValueStoreChangedEvent> WhenChanged { get; }
        IImmutableDictionary<string, object> Snapshot { get; }

        object GetObject(string key);
        bool GetBoolean(string key);
        byte GetByte(string key);
        int GetInt32(string key);
        long GetInt64(string key);
        float GetFloat32(string key);
        double GetFloat64(string key);
        string GetString(string key);

        object GetObject(string key, object defaultValue);
        bool GetBoolean(string key, bool defaultValue);
        byte GetByte(string key, byte defaultValue);
        int GetInt32(string key, int defaultValue);
        long GetInt64(string key, long defaulValue);
        float GetFloat32(string key, float defaultValue);
        double GetFloat64(string key, double defaultValue);
        string GetString(string key, string defaultValue);

        void SetObject(string key, object value);
        void SetBoolean(string key, bool value);
        void SetByte(string key, byte value);
        void SetInt32(string key, int value);
        void SetInt64(string key, long value);
        void SetFloat32(string key, float value);
        void SetFloat64(string key, double value);
        void SetString(string key, string value);
    }

    public interface IScriptContext
        : IValueStore
    {
    }

    public interface IPersistentValueStore
        : IValueStore
    {
        /// <summary>
        /// Path to file which contains the values
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Flag indicating whether the value store has been modified since the last load operation.
        /// </summary>
        bool Modified { get; }

        void Save();
        void Load();
    }

    public interface ICSharpCodeModule
        : IModule
    {
        string Source { get; }
        bool SourceModified { get; }

        void OnBeforeCompile();
        void OnCompileCompleted(bool succeeded, Type[] generatedTypes);
        void OnAfterCompile();
    }

    public interface ICSharpCodeManager
        : INode
    {
        string OutputDirectory { get; set; }
        bool SourceModified { get; set; }

        void Register(ICSharpCodeModule module);
        void Unregister(ICSharpCodeModule module);

        bool Compile();
    }

    public static class GraphEnvironmentExtensions
    {
        public static INode FindNodeByTrace(this IGraphRuntime runtime, string trace)
        {
            if (runtime.Root == null || !trace.StartsWith(runtime.Root.Id))
                return null;

            if (trace == runtime.Root.Id)
                return runtime.Root;

            return runtime.Root.FindNodeByTrace(trace.Substring(runtime.Root.Id.Length + 1));
        }

        public static IMutableContainer<INode> GetContainer(this IGraphRuntime runtime, string objectId)
        {
            var node = runtime.GetNodeByObjectId(objectId);
            if (node is IMutableContainer<INode> container)
                return container;

            if (node is IGraph graph)
                return graph;

            if (node is ISubGraphModule module)
                return module.SubGraph;

            throw new Exception($"Node container not found: '{objectId}'");
        }
    }
}
