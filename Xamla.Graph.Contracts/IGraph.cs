using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamla.Types;
using Xamla.Types.Records;
using Xamla.Utilities;
using Python.Runtime;
namespace Xamla.Graph
{
    public enum AuthorRole
    {
        Owner,
        Contributor,
        Reviewer
    }

    public interface IAuthor
    {
        string UserId { get; set; }
        string DisplayName { get; set; }
        AuthorRole? Role { get; set; }
    }

    public interface IGraphMetadata
    {
        string Title { get; set; }
        string Description { get; set; }
        Uri Container { get; set; }
        ICollection<IAuthor> Authors { get; set; }
        ICollection<string> Tags { get; set; }
        Version Version { get; set; }
        DateTimeOffset? Created { get; set; }
        DateTimeOffset? Modified { get; set; }
        string License { get; set; }
    }

    public interface ICompilerSettings
    {
        IList<string> AssemblyReferences { get; set; }

        /// <summary>
        /// Indicates whether references to mscorlib, System.dll, System.Core.dll, Xamla.Utilities, Xamla.Types and Xamla.Graph.Contracts should be added automatically.
        /// </summary>
        bool StandardAssemblyReferences { get; set; }
        bool Debug { get; set; }
        IList<string> PreprocessorSymbols { get; set; }

        /// <summary>
        /// Indicates that settings were changed and a recompile is necessary. Flag is reset after successful build.
        /// </summary>
        bool SettingsModified { get; set; }
    }

    public interface IGraph
        : IMutableContainer<INode>
    {
        PyScope PythonScope { get; }
        IInputPin Sink { get; }
        IGraphMetadata Metadata { get; set; }
        string FilePath { get; set; }
        ISchemaProvider SchemaProvider { get; }
        IContainerDependencies Dependencies { get; }

        ICompilerSettings CompilerSettings { get; }
        Assembly DynamicCodeAssembly { get; set; }
        IPersistentValueStore ValueStore { get; }

        void AddSchema(Schema schema);
        void RemoveSchema(Schema schema);
        void AddDependency(Uri containerSource, string alias = null, IEnumerable<Uri> importTypesFromFiles = null);
        void RemoveDependency(Uri source);
        void RemoveDependency(string alias);

        IGraphInterfaceModule InputInterface { get; }
        IGraphInterfaceModule OutputInterface { get; }

        IGraphFlowInterfaceModule Start { get; }
        IGraphFlowInterfaceModule End { get; }

        /// <summary>
        /// Try to find a node with the specified ID path.
        /// </summary>
        /// <param name="trace">Dot delimited id path of the node to search.</param>
        /// <returns>The node instance if found, null otherwise.</returns>
        INode FindNodeByTrace(string trace);
        void RemoveById(string nodeId);

        Int2 ViewCenter { get; set; }
        double ZoomLevel { get; set; }

        bool IsNew { get; set; }
        bool IsModified { get; set; }

        void SetFilePath(string path, bool adjustRelativePaths);
        string ResolvePath(string path);
        string MakeRelative(string path);

        XGraphViewModel ToViewModel();
        XDocument ToXDocument(bool includeViewModel = false);
    }
}
