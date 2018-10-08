using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph.Models
{
    public enum ContainerChangeType
    {
        Add,
        Remove,
        Modify,
        Rename,      // ID changed
        Reorder
    }

    public class ContainerChangeCollection
    {
        public long ChangeSerialNumber { get; set; }
        public IEnumerable<object> Changes { get; set; }
    }

    public class ContainerChangeModel
    {
        public NodeModel Container { get; set; }
        public NodeModel NewItem { get; set; }
        public NodeModel OldItem { get; set; }
        public string Kind { get; set; }
        public ContainerChangeType Change { get; set; }

        public ContainerChangeModel()
        {
            this.Kind = "ContainerChange";
        }
    }
}
