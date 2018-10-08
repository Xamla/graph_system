using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Xamla.Utilities.Collections
{
    /// <summary>
    /// Basic bidirectional iterator abstraction
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public abstract class Iterator<T> : IComparable
    {
        public static Iterator<T> operator ++(Iterator<T> iter)
        {
            return iter + 1;
        }

        public static Iterator<T> operator --(Iterator<T> iter)
        {
            return iter - 1;
        }

        public static Iterator<T> operator +(Iterator<T> iter, int distance)
        {
            var clone = iter.Clone();
            clone.Move(distance);
            return clone;
        }

        public static Iterator<T> operator -(Iterator<T> iter, int distance)
        {
            var clone = iter.Clone();
            clone.Move(-distance);
            return clone;
        }

        public static int operator -(Iterator<T> left, Iterator<T> right)
        {
            return left.Distance(right);
        }

        public static bool operator ==(Iterator<T> left, Iterator<T> right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(Iterator<T> left, Iterator<T> right)
        {
            return !object.Equals(left, right);
        }

        public override bool Equals(object other)
        {
            return base.Equals(other);    // implemented to avoid warning
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();  // implemented to avoid warning
        }

        public int CompareTo(object other)
        {
            Iterator<T> iter = other as Iterator<T>;
            if (iter == null || !object.ReferenceEquals(iter.Collection, this.Collection))
                throw new ArgumentException("Cannot compare iterators of different origin.");

            return Distance(iter);
        }

        public void Next()
        {
            Move(1);
        }

        public void Prev()
        {
            Move(-1);
        }

        public abstract T Current { get; set; }
        public abstract object Collection { get; }
        public abstract Iterator<T> Clone();

        public abstract void Move(int count);
        public abstract int Distance(Iterator<T> iter);
    }

    public class InvalidPositionException : InvalidOperationException
    {
        public InvalidPositionException()
            : this("Iterator positioned on End or invalid element.")
        {
        }

        public InvalidPositionException(string message)
            : base(message)
        {
        }
    }

    public class IteratorPinnedException : InvalidOperationException
    {
        public IteratorPinnedException()
            : base("Cannot move pinned iterator. Use Clone() to create a movable copy.")
        {
        }
    }

    public class IteratorEnumerator<T> : IEnumerator<T>
    {
        protected Iterator<T> current;
        protected Iterator<T> begin, end;
        protected bool fresh;

        public IteratorEnumerator(Iterator<T> begin, Iterator<T> end)
        {
            this.begin = begin;
            this.end = end;
            this.fresh = true;
        }

        #region IEnumerator<T> Members

        public bool MoveNext()
        {
            if (fresh)
            {
                current = begin.Clone();
                fresh = false;
            }
            else if (current != end)
                current.Next();

            return (current != end);
        }

        public void Reset()
        {
            fresh = true;
            current = null;
        }

        public T Current
        {
            get
            {
                if (fresh || current == end)
                    throw new InvalidPositionException("The enumerator is positioned before the first element of the collection or after the last element.");
                return current.Current;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        #endregion
    }

    public class IteratorRange<T>
        : IEnumerable<T>
    {
        protected Iterator<T> begin, end;

        public IteratorRange(Iterator<T> begin, Iterator<T> end)
        {
            this.begin = begin;
            this.end = end;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new IteratorEnumerator<T>(begin, end);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    
    public static class IteratorRange
    {
        public static IteratorRange<T> Create<T>(Iterator<T> begin, Iterator<T> end)
        {
            return new IteratorRange<T>(begin, end);
        }
    }

    /// <summary>
    /// Base interface for all containers
    /// </summary>
    public interface IContainer<T> : ICollection<T>
    {
        Iterator<T> Begin { get; }
        Iterator<T> End { get; }

        bool IsEmpty { get; }

        Iterator<T> Find(T value);

        Iterator<T> Erase(Iterator<T> where);
        Iterator<T> Erase(Iterator<T> first, Iterator<T> last);
    }
    /// <summary>
    /// Interface for containers providing key to value mapping (HashTable)
    /// </summary>
    public interface IMap<TKey, TValue> : IContainer<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        Iterator<KeyValuePair<TKey, TValue>> FindKey(TKey key);

        void Insert(Iterator<KeyValuePair<TKey, TValue>> sourceBegin, Iterator<KeyValuePair<TKey, TValue>> sourceEnd);
    }

    /// <summary>
    /// Interface for sorted mapping containers (SkipList, Tree)
    /// </summary>
    public interface ISortedMap<TKey, TValue> : IMap<TKey, TValue>
    {
        IComparer<TKey> KeyComparer { get; }

        Iterator<KeyValuePair<TKey, TValue>> LowerBound(TKey key);
        Iterator<KeyValuePair<TKey, TValue>> UpperBound(TKey key);
    }

    /// <summary>
    /// Tree is a red-black balanced search tree (BST) implementation.
    /// Complexity of find, insert and erase operations is near O(lg n).
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class Tree<TKey, TValue> : ISortedMap<TKey, TValue>
        //, ISerializable
    {
        #region Node Implementation

        private class Node
        {
            private TKey key;
            private TValue value;
            private bool red;
            internal Node left, right;
            internal Node parent;       // public to simplify fixup & clone (passing by ref)

            public Node(TKey key, TValue value, Node parent)
            {
                this.key = key;
                this.value = value;
                this.parent = parent;
                this.red = true;
            }

            public TKey Key
            {
                get { return key; }
            }

            public TValue Value
            {
                get { return value; }
                set { this.value = value; }
            }

            public KeyValuePair<TKey, TValue> Item
            {
                get { return new KeyValuePair<TKey, TValue>(key, value); }
            }

            public bool Red
            {
                get { return red; }
                set { red = value; }
            }

            public static void SwapItems(Node a, Node b)
            {
                TKey k = a.key;
                a.key = b.key;
                b.key = k;

                TValue v = a.value;
                a.value = b.value;
                b.value = v;
            }
        }

        #endregion

        #region NodeIterator Implementation

        private class NodeIterator : Iterator<KeyValuePair<TKey, TValue>>
        {
            private Tree<TKey, TValue> tree;
            private Node current;

            public NodeIterator(Tree<TKey, TValue> tree, Node node)
            {
                this.tree = tree;
                this.current = node;
            }

            public override KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (current == null)
                        throw new InvalidPositionException();
                    return current.Item;
                }
                set
                {
                    if (current == null)
                        throw new InvalidPositionException();
                    if (tree.keyComparer.Compare(value.Key, current.Key) != 0)
                        throw new ArgumentException("Key values must not be changed.");
                    current.Value = value.Value;
                }
            }

            public override object Collection
            {
                get { return tree; }
            }

            public override void Move(int count)
            {
                Node newPos = current;

                if (count > 0)
                {
                    while (count-- > 0)
                    {
                        if (newPos == null)
                            throw new InvalidOperationException("Tried to moved beyond end element.");

                        newPos = Tree<TKey, TValue>.Next(newPos);
                    }
                }
                else
                {
                    while (count++ < 0)
                    {
                        if (newPos == null)
                            newPos = tree.right;
                        else
                            newPos = Tree<TKey, TValue>.Previous(newPos);

                        if (newPos == null)
                            throw new InvalidOperationException("Tried to move before first element.");
                    }
                }

                current = newPos;
            }

            public override int Distance(Iterator<KeyValuePair<TKey, TValue>> iter)
            {
                NodeIterator nodeIter = iter as NodeIterator;
                if (nodeIter == null || !object.ReferenceEquals(nodeIter.Collection, this.Collection))
                    throw new ArgumentException("Cannot determine distance of iterators belonging to different collections.");

                var end = tree.end;

                int keyDiff;
                if (this == end || iter == end)
                    keyDiff = (this == end && this != iter) ? 1 : 0;
                else
                    keyDiff = tree.keyComparer.Compare(current.Key, nodeIter.current.Key);

                if (keyDiff <= 0)
                {
                    int diff = 0;
                    var rwd = this.Clone();
                    for (; rwd != iter && rwd != end; rwd.Next())
                        --diff;

                    if (rwd == iter)
                        return diff;
                }

                if (keyDiff >= 0)
                {
                    int diff = 0;
                    var fwd = iter.Clone();
                    for (; fwd != this && fwd != end; fwd.Next())
                        ++diff;

                    if (fwd == this)
                        return diff;
                }

                throw new Exception("Inconsistent state. Concurrency?");
            }

            public override bool Equals(object other)
            {
                NodeIterator iter = other as NodeIterator;
                if (iter == null)
                    return false;
                return object.ReferenceEquals(current, iter.current);
            }

            public override int GetHashCode()
            {
                if (current == null)
                    return tree.GetHashCode();
                return current.GetHashCode();
            }

            public override Iterator<KeyValuePair<TKey, TValue>> Clone()
            {
                return new NodeIterator(tree, current);
            }

            internal Node Node
            {
                get { return current; }
            }
        }

        private class PinnedNodeIterator : NodeIterator
        {
            public PinnedNodeIterator(Tree<TKey, TValue> tree, Node node)
                : base(tree, node)
            {
            }

            public override void Move(int count)
            {
                throw new IteratorPinnedException();
            }
        }
        #endregion

        private Node head;
        private Node left, right;
        private int count;
        private readonly IComparer<TKey> keyComparer;
        private readonly bool allowDuplicateKeys;
        private readonly Iterator<KeyValuePair<TKey, TValue>> end;

        public Tree()
            : this(false)
        {
        }

        public Tree(bool allowDuplicateKeys)
            : this(allowDuplicateKeys, Comparer<TKey>.Default)
        {
        }

        public Tree(bool allowDuplicateKeys, IComparer<TKey> comparer)
        {
            this.allowDuplicateKeys = allowDuplicateKeys;
            this.keyComparer = comparer;
            this.end = new PinnedNodeIterator(this, null);
        }

        //protected Tree(SerializationInfo info, StreamingContext context)
        //{
        //    if (info == null)
        //        throw new ArgumentNullException("info");

        //    this.allowDuplicateKeys = info.GetBoolean("Duplicates");
        //    this.keyComparer = (IComparer<TKey>)info.GetValue("Comparer", typeof(IComparer<TKey>));

        //    this.end = new PinnedNodeIterator(this, null);

        //    try // Currently there is no way to know whether an element ("Pairs") in a SerializationInfo is present
        //    {
        //        var pairs = (KeyValuePair<TKey, TValue>[])info.GetValue("Pairs", typeof(KeyValuePair<TKey, TValue>[]));
        //        Array.ForEach(pairs, Add);
        //    }
        //    catch
        //    {
        //        // ignore
        //    }
        //}

        // IContainer<KeyValuePair<TKey, TValue> > Members
        public Iterator<KeyValuePair<TKey, TValue>> Begin
        {
            get
            {
                if (this.count == 0)
                    return this.end;
                return new NodeIterator(this, this.left);
            }
        }

        public Iterator<KeyValuePair<TKey, TValue>> End
        {
            get { return this.end; }
        }

        public bool IsEmpty
        {
            get { return this.count == 0; }
        }

        public Iterator<KeyValuePair<TKey, TValue>> Find(KeyValuePair<TKey, TValue> item)
        {
            var lower = LowerBound(item.Key);
            var upper = UpperBound(item.Key);

            for (var i = lower; i != upper; i.Next())
            {
                if (object.Equals(item.Value, i.Current.Value))
                    return i;
            }

            return this.end;
        }

        public Iterator<KeyValuePair<TKey, TValue>> Erase(Iterator<KeyValuePair<TKey, TValue>> where)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(this, where.Collection), "Iterator does not belong to this tree.");
            var successor = where + 1;
            RemoveNode(((NodeIterator)where).Node);
            return successor;
        }

        public Iterator<KeyValuePair<TKey, TValue>> Erase(Iterator<KeyValuePair<TKey, TValue>> first, Iterator<KeyValuePair<TKey, TValue>> last)
        {
            if (first == this.Begin && last == this.end)
            {
                Clear();
                return last.Clone();
            }

            var current = first;
            while (current != last)
                current = Erase(current);
            return current;
        }

        // IMap<TKey, TValue> Members
        public Iterator<KeyValuePair<TKey, TValue>> FindKey(TKey key)
        {
            return new NodeIterator(this, FindInternal(this.head, key));
        }

        public void Insert(Iterator<KeyValuePair<TKey, TValue>> sourceBegin, Iterator<KeyValuePair<TKey, TValue>> sourceEnd)
        {
            for (var i = sourceBegin.Clone(); i != sourceEnd; i.Next())
                Add(i.Current);
        }

        // ISortedMap Members
        public IComparer<TKey> KeyComparer
        {
            get { return this.keyComparer; }
        }

        public Iterator<KeyValuePair<TKey, TValue>> LowerBound(TKey key)
        {
            Node found = null;

            Node node = this.head;
            while (node != null)
            {
                if (this.keyComparer.Compare(node.Key, key) < 0)
                    node = node.right;
                else
                {
                    found = node;
                    node = node.left;
                }
            }
            
            return new NodeIterator(this, found);
        }

        public Iterator<KeyValuePair<TKey, TValue>> UpperBound(TKey key)
        {
            Node found = null;

            var node = this.head;
            while (node != null)
            {
                if (this.keyComparer.Compare(node.Key, key) > 0)
                {
                    found = node;
                    node = node.left;
                }
                else
                    node = node.right;
            }

            return new NodeIterator(this, found);
        }

        #region IDictionary<TKey, TValue> Members

        public void Add(TKey key, TValue value)
        {
            Insert(ref this.head, null, key, value, false);
            this.head.Red = false;
            ++this.count;
        }

        public bool ContainsKey(TKey key)
        {
            return FindInternal(this.head, key) != null;
        }

        public bool Remove(TKey key)
        {
            var node = FindInternal(this.head, key);
            if (node == null)
                return false;

            RemoveNode(node);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var node = FindInternal(this.head, key);
            if (node == null)
            {
                value = default(TValue);
                return false;
            }

            value = node.Value;
            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                var node = FindInternal(this.head, key);
                if (node == null)
                    throw new KeyNotFoundException();
                return node.Value;
            }
            set
            {
                var node = FindInternal(this.head, key);
                if (node == null)
                    Add(key, value);
                else
                    node.Value = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                int i = 0;
                var keys = new TKey[this.count];
                foreach (var x in this)
                    keys[i++] = x.Key;
                return keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                int i = 0;
                var values = new TValue[this.count];
                foreach (var x in this)
                    values[i++] = x.Value;
                return values;
            }
        }

        #endregion

        #region ICollection<<KeyValuePair<TKey, TValue> > Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.count = 0;
            this.head = null;
            this.left = null;
            this.right = null;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var lower = LowerBound(item.Key);
            var upper = UpperBound(item.Key);

            for (var i = lower.Clone(); i != upper; i.Next())
            {
                if (object.Equals(item.Value, i.Current.Value))
                    return true;
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            foreach (var x in this)
                array[index++] = x;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var lower = LowerBound(item.Key);
            var upper = UpperBound(item.Key);

            var i = lower.Clone();
            bool removed = false;
            while (i != upper)
            {
                if (object.Equals(item.Value, i.Current.Value))
                {
                    i = Erase(i);
                    removed = true;
                }
                else
                    i.Next();
            }
            return removed;
        }

        public int Count
        {
            get { return this.count; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey, TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new IteratorEnumerator<KeyValuePair<TKey, TValue>>(this.Begin, this.end);
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new IteratorEnumerator<KeyValuePair<TKey, TValue>>(this.Begin, this.end);
        }

        #endregion


        public Tree<TKey, TValue> Clone()
        {
            var clone = new Tree<TKey, TValue>(this.allowDuplicateKeys, this.keyComparer);
            clone.count = this.count;
            CloneRecursive(this.head, null, ref clone.head);
            clone.left = Tree<TKey, TValue>.LeftMost(clone.head);
            clone.right = Tree<TKey, TValue>.RightMost(clone.head);

            return clone;
        }

        //#region ISerializable Members

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    if (info == null)
        //        throw new ArgumentNullException("info");

        //    info.AddValue("Duplicates", this.allowDuplicateKeys);
        //    info.AddValue("Comparer", this.keyComparer, typeof(IComparer<TKey>));
        //    if (this.count != 0)
        //    {
        //        var pairs = new KeyValuePair<TKey, TValue>[this.count];
        //        CopyTo(pairs, 0);
        //        info.AddValue("Pairs", pairs);
        //    }
        //}

        //#endregion

        private void CloneRecursive(Node node, Node parent, ref Node link)
        {
            if (node == null)
                return;

            link = new Node(node.Key, node.Value, parent);
            link.Red = node.Red;

            CloneRecursive(node.left, link, ref link.left);
            CloneRecursive(node.right, link, ref link.right);
        }

        /*
         A right rotation: node.Left takes old position of node.
         Makes the old root the right subtree of the new root.

              5                 2
           2     7     ->    1     5  
          1 3   6 8              3   7
                                    6 8
        */
        private Node RotateRight(Node node)
        {
            var tmp = node.left;

            tmp.parent = node.parent;
            node.parent = tmp;

            node.left = tmp.right;
            if (node.left != null)
                node.left.parent = node;
            tmp.right = node;

            // correct parent
            if (tmp.parent == null)
                this.head = tmp;
            else if (tmp.parent.right == node)
                tmp.parent.right = tmp;
            else
                tmp.parent.left = tmp;

            return tmp;
        }

        /*
         A left rotation: node.Right takes old position of node.
         Makes the old root the left subtree of the new root.
		 
              5                   7
           2     7     ->      5     8
          1 3   6 8          2   6
                            1 3      
        */
        private Node RotateLeft(Node node)
        {
            var tmp = node.right;

            tmp.parent = node.parent;
            node.parent = tmp;

            node.right = tmp.left;
            if (node.right != null)
                node.right.parent = node;
            tmp.left = node;

            // correct parent
            if (tmp.parent == null)
                this.head = tmp;
            else if (tmp.parent.right == node)
                tmp.parent.right = tmp;
            else
                tmp.parent.left = tmp;

            return tmp;
        }

        private bool IsRed(Node node)
        {
            return (node != null && node.Red);
        }

        private Node FindInternal(Node node, TKey key)
        {
            while (node != null)
            {
                int delta = this.keyComparer.Compare(key, node.Key);
                if (delta == 0)
                    return node;
                node = (delta < 0) ? node.left : node.right;
            }
            return null;
        }

        /// <summary>
        /// Return leftmost node in subtree.
        /// </summary>
        /// <param name="node">Node where to start search</param>
        /// <returns>Found node</returns>
        private static Node LeftMost(Node node)
        {
            if (node == null)
                return null;
            while (node.left != null)
                node = node.left;
            return node;
        }

        /// <summary>
        /// Return rightmost node in subtree.
        /// </summary>
        /// <param name="node">Node where to start search</param>
        /// <returns>Found node</returns>
        private static Node RightMost(Node node)
        {
            if (node == null)
                return null;
            while (node.right != null)
                node = node.right;
            return node;
        }

        private static Node Previous(Node node) // the next smaller
        {
            if (node.left != null)
                return RightMost(node.left);

            var parent = node.parent;
            while (parent != null && parent.left == node)
            {
                node = parent;
                parent = parent.parent;
            }
            return parent;
        }

        private static Node Next(Node node)
        {
            if (node.right != null)
                return LeftMost(node.right);

            var parent = node.parent;
            while (parent != null && parent.right == node)
            {
                node = parent;
                parent = parent.parent;
            }
            return parent;
        }

        private void RemoveNode(Node node)
        {
            if (node == null)
                return;
            if (node == this.head)
                UnlinkNode(ref this.head);
            else if (node == node.parent.right)
                UnlinkNode(ref node.parent.right);
            else
                UnlinkNode(ref node.parent.left);
        }

        private void UnlinkNode(ref Node node)
        {
            bool red = node.Red;
            Node erased = node;

            Node patchNode = null;
            if (node.right == null)
                patchNode = node.left;
            else if (node.left == null)
                patchNode = node.right;
            else
                patchNode = node;

            Node patchParent = null, fixNode = null;
            if (patchNode == null)
            {
                patchParent = node.parent;
                node = null;
            }
            else if (patchNode != node)
            {
                patchNode.parent = node.parent;
                node = patchNode;
                patchParent = patchNode.parent;
            }
            else
            {
                // two subtrees
                patchNode = RightMost(node.left);
                if (patchNode.parent.right == patchNode)
                    patchNode.parent.right = patchNode.left;
                else
                    patchNode.parent.left = patchNode.left;

                red = patchNode.Red;
                if (patchNode.left != null)
                    patchNode.left.parent = patchNode.parent;

                patchParent = patchNode.parent;
                fixNode = patchNode.left;

                Node.SwapItems(node, patchNode);

                // ensure that this.Left and/or this.Right are corrected after unlink
                erased = patchNode;
            }

            if (!red && patchParent != null)
            {
                // erased node was black link - rebalance the tree
                while (!IsRed(fixNode) && fixNode != this.head)
                {
                    if (patchParent.left != null || patchParent.right != null)
                    {
                        if (patchParent.left == fixNode)
                        {
                            // fixup right subtree
                            var n = patchParent.right;

                            if (IsRed(n))
                            {
                                n.Red = false;
                                patchParent.Red = true;
                                RotateLeft(patchParent);
                                n = patchParent.right;
                            }

                            if (n != null)
                            {
                                if (!IsRed(n.left) && !IsRed(n.right))
                                    n.Red = true;
                                else
                                {
                                    if (!IsRed(n.right))
                                    {
                                        n.Red = true;
                                        n.left.Red = false;
                                        RotateRight(n);
                                        n = patchParent.right;
                                    }

                                    n.Red = patchParent.Red;
                                    patchParent.Red = false;
                                    n.right.Red = false;
                                    RotateLeft(patchParent);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // fixup leftsubtree
                            var n = patchParent.left;

                            if (IsRed(n))
                            {
                                n.Red = false;
                                patchParent.Red = true;
                                RotateRight(patchParent);
                                n = patchParent.left;
                            }

                            if (n != null)
                            {
                                if (!IsRed(n.left) && !IsRed(n.right))
                                    n.Red = true;
                                else
                                {
                                    if (!IsRed(n.left))
                                    {
                                        n.Red = true;
                                        n.right.Red = false;
                                        RotateLeft(n);
                                        n = patchParent.left;
                                    }

                                    n.Red = patchParent.Red;
                                    patchParent.Red = false;
                                    n.left.Red = false;
                                    RotateRight(patchParent);
                                    break;
                                }
                            }
                        }
                    }
                    fixNode = patchParent;
                    patchParent = patchParent.parent;
                }

                if (fixNode != null)
                    fixNode.Red = false;
            }

            if (object.ReferenceEquals(erased, this.right))
                this.right = Tree<TKey, TValue>.RightMost(this.head);
            if (object.ReferenceEquals(erased, this.left))
                this.left = Tree<TKey, TValue>.LeftMost(this.head);

            --this.count;
        }

        private void Insert(ref Node node, Node parent, TKey key, TValue value, bool rightMove)
        {
            if (node == null)
            {
                node = new Node(key, value, parent);
                if (object.ReferenceEquals(parent, this.right) && (parent == null || rightMove))
                    this.right = node;
                if (object.ReferenceEquals(parent, this.left) && (parent == null || !rightMove))
                    this.left = node;
                return;
            }

            if (IsRed(node.left) && IsRed(node.right))
            {
                node.Red = true;
                node.left.Red = false;
                node.right.Red = false;
            }

            int delta = this.keyComparer.Compare(key, node.Key);
            if (!this.allowDuplicateKeys && delta == 0)
                throw new ArgumentException("An element with the same key already exists in the tree.");

            if (delta < 0)
            {
                Insert(ref node.left, node, key, value, false);
                if (IsRed(node) && IsRed(node.left) && rightMove)
                    node = RotateRight(node);
                if (IsRed(node.left) && IsRed(node.left.left))
                {
                    node = RotateRight(node);
                    node.Red = false;
                    node.right.Red = true;
                }
            }
            else
            {
                Insert(ref node.right, node, key, value, true);
                if (IsRed(node) && IsRed(node.right) && !rightMove)
                    node = RotateLeft(node);
                if (IsRed(node.right) && IsRed(node.right.right))
                {
                    node = RotateLeft(node);
                    node.Red = false;
                    node.left.Red = true;
                }
            }
        }
    }
}
