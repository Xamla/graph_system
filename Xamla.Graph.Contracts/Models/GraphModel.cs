using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Models
{
    public class GraphModel
        : NodeModel
    {
        public GraphModel()
            : base("Graph")
        {
        }

        public long ChangeSerialNumber { get; set; }
        public IGraphMetadata Metadata { get; set; }
        public string FilePath { get; set; }
        public Int2 ViewCenter { get; set; }
        public double ZoomLevel { get; set; }
        public bool IsNew { get; set; }
        public bool IsModified { get; set; }
        public List<string> PortModuleIds { get; set; }
        public List<string> AssemblyReferences { get; set; }
        public bool StandardAssemblyReferences { get; set; }
    }

    public class GraphInstanceModel
        : ModuleModel
    {
        public GraphInstanceModel()
            : base("GraphInstance")
        {
        }

        public IGraphMetadata Metadata { get; set; }
        public string Source { get; set; }
        public string Filename { get; set; }
    }
}
