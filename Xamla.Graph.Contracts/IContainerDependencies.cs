using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types.Records;

namespace Xamla.Graph
{
    public class ImportTypes
    {
        public Uri Source { get; set; }
        public IEnumerable<Schema> Schemas { get; set; }
    }

    public interface IContainerDependency
    {
        string Alias { get; set; }
        string ContainerName { get; set; }
        string Version { get; set; }
        /// <summary>
        /// Remote source of the container dependency
        /// </summary>
        Uri ContainerRemoteSource { get; set; }
        /// <summary>
        /// Path to the dependencies downloaded to the local containers collection
        /// </summary>
        Uri ContainerLocalSource { get; set; }
        List<ImportTypes> ImportTypes { get; set; }
    }
    
    public interface IContainerDependencies
    {
        /// <summary>
        /// Return the local container source for an alias
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>local container source</returns>
        IContainerDependency Find(string alias);
        IContainerDependency Find(Uri source);
        IEnumerable<IContainerDependency> Dependencies { get; }
        ISchemaRegistry SchemaRegistry { get; }
    }
}
