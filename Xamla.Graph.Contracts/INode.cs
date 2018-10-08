using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.Models;

namespace Xamla.Graph
{
    public abstract class NodeEvent
    {
        public INode Source { get; private set; }
        public bool Bubble { get; set; }

        protected NodeEvent(INode source, bool bubble)
        {
            this.Source = source;
            this.Bubble = bubble;
        }
    }

    public class NodeChangedEvent
        : NodeEvent
    {
        public NodeChangedEvent(INode source)
            : base(source, true)
        {
        }
    }

    public class BeforePropertyChangeEvent
        : NodeEvent
    {
        bool cancel;
        public BeforePropertyChangeEvent(INode source, PropertyValue value)
            : base(source, false)
        {
            this.Value = value;
            this.cancel = false;
        }

        /// <summary>
        /// Once the cancel flag has been to set to true, it cannot be set to false anymore
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set
            {
                if (!value)
                    return;

                cancel = true;
            }
        }

        public PropertyValue Value { get; private set; }
    }

    public class PropertyChangedEvent
        : NodeEvent
    {
        public PropertyChangedEvent(INode source, PropertyValue value)
            : base(source, false)
        {
            this.Value = value;
        }

        public PropertyValue Value
        {
            get;
            private set;
        }
    }

    public interface INode
    {
        string Id { get; set; }
        string Trace { get; }
        IEnumerable<INode> Ancestors();

        /// <summary>
        /// The object ID is generated when the node is instantiated and stays constant for the lifetime
        /// of the node object. It allows to uniquely identify a node instance. It simplifies reflecting
        /// changes in a graph editor independent of the node ID which may be edited by the user.
        /// </summary>
        string ObjectId { get; }

        IContainer<INode> Container { get; set; }
        IGraphRuntime Runtime { get; }
        IGraph Graph { get; }

        bool IsRegistered { get; set; }
        bool IsDirty { get; set; }
        void MarkClean();

        void VisitSubtree(Func<INode, bool> visitor);

        void PropagateEvent(NodeEvent evt);
        IObservable<NodeEvent> WhenNodeEvent { get; }

        NodeModel ToModel(int? depthLimit);
    }
}
