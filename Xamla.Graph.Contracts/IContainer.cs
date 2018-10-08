using System;
using System.Collections.Generic;
using Xamla.Graph.Models;

namespace Xamla.Graph
{
    public enum ContainerChangeAction
    {
        Add,
        Remove
    }

    public class ContainerChangeEvent
        : NodeChangedEvent
    {
        public ContainerChangeEvent(INode source, ContainerChangeAction change, INode node)
            : base(source)
        {
            this.Change = change;
            this.Node = node;
        }

        public ContainerChangeAction Change { get; private set; }
        public INode Node { get; private set; }
    }

     public interface IContainer
         : INode
     {
         bool HasDirtyChildren { get; set; }
         int Count { get; }
         IEnumerable<INode> Nodes { get; }
         INode GetNode(string id);
         void CopyChangesTo(ICollection<object> result);
     }

    public interface IContainer<out T>
        : IContainer
        , IEnumerable<T>
        where T : INode
    {
        T this[string id] { get; }
    }

    public interface IMutableContainer<T>
        : IContainer<T>
        where T : INode
    {
        void Add(T item);
        bool Remove(T item);
        void InsertAt(int index, T item);
    }

    public static class MutableContainerExtensions
    {
        public static bool RemoveById<T>(this IMutableContainer<T> container, string id)
            where T : INode
        {
            var item = container[id];
            if (item == null)
                return false;
            return container.Remove(item);
        }
    }
}
