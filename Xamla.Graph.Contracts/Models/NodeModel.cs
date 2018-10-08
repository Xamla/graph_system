using System.Collections.Generic;

namespace Xamla.Graph.Models
{
    public class NodeModel
    {
        public NodeModel(string nodeType)
        {
            this.NodeType = nodeType;
        }

        public string Id { get; set; }
        public string Trace { get; set; }
        public string ObjectId { get; set; }
        public string NodeType { get; set; }
        public IList<NodeModel> Children { get; set; }
    }
}
