using System;
using System.Linq;
using System.Collections.Generic;

namespace Xamla.Utilities.Collections
{
    public class MruCache<TKey, TValue>
    {
        struct Element
        {
            public int Index;
            public TValue Value;
        }

        struct Node
        {
            public int Prev;
            public int Next;
            public TKey Key;

            public override string ToString()
            {
                return string.Format("Prev: {0}; Next: {1}; Key: {2}", Prev, Next, Key);
            }
        }

        Dictionary<TKey, Element> map;
        Node[] nodes;

        int head;
        int tail;
        int free;

        public MruCache(int capacity)
            : this(capacity, EqualityComparer<TKey>.Default)
        {
        }

        public MruCache(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.map = new Dictionary<TKey, Element>();
            nodes = new Node[capacity];
            InitializeNodes();
        }

        void InitializeNodes()
        {
            for (int i = 0; i < nodes.Length - 1; ++i)
            {
                nodes[i].Next = i + 1;
                nodes[i + 1].Prev = i;
            }

            nodes[0].Prev = -1;
            nodes[nodes.Length - 1].Next = -1;

            head = -1;
            tail = -1;
            free = 0;
        }

        public void Clear()
        {
            lock (map)
            {
                map.Clear();
                InitializeNodes();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (map)
            {
                Element v;
                if (!map.TryGetValue(key, out v))
                {
                    value = default(TValue);
                    return false;
                }

                value = v.Value;
                MoveFront(v.Index);
                return true;
            }
        }

        public void RemoveOldest(int count)
        {
            while (count > 0 && tail >= 0)
            {
                FreeNode(tail);
                count -= 1;
            }
        }

        public void RemoveYoungest(int count)
        {
            while (count > 0 && head >= 0)
            {
                FreeNode(head);
                count -= 1;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (map)
                {
                    var v = map[key];
                    MoveFront(v.Index);
                    return v.Value;
                }
            }
            set
            {
                lock (map)
                {
                    Element v;
                    if (map.TryGetValue(key, out v))
                    {
                        v.Value = value;
                        map[key] = v;
                        MoveFront(v.Index);
                    }
                    else
                    {
                        Add(key, value);
                    }
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (map)
            {
                if (map.ContainsKey(key))
                    throw new ArgumentException("An element with the same key already exists.");

                if (free < 0)
                    RemoveOldest(1);

                int index = free;
                free = nodes[index].Next;

                nodes[index].Key = key;
                nodes[index].Prev = -1;
                nodes[index].Next = head;

                if (head >= 0)
                    nodes[head].Prev = index;
                else
                    tail = index;
                head = index;

                map[key] = new Element { Index = index, Value = value };
            }
        }

        public bool Remove(TKey key)
        {
            lock (map)
            {
                Element v;
                if (!map.TryGetValue(key, out v))
                    return false;

                FreeNode(v.Index);
                return true;
            }
        }

        public int Count
        {
            get { return map.Count; }
        }

        public int Capacity
        {
            get { return nodes.Length; }
        }

        void MoveFront(int index)
        {
            if (index == head)
                return;

            Unlink(index);
            nodes[index].Prev = -1;
            nodes[index].Next = head;
            nodes[head].Prev = index;
            head = index;
        }

        void Unlink(int index)
        {
            var next = nodes[index].Next;
            var prev = nodes[index].Prev;

            if (next >= 0)
                nodes[next].Prev = prev;
            else if (tail == index)
                tail = prev;

            if (prev >= 0)
                nodes[prev].Next = next;
            else if (head == index)
                head = next;
        }

        void FreeNode(int index)
        {
            Unlink(index);

            map.Remove(nodes[index].Key);
            nodes[index].Key = default(TKey);
            nodes[index].Prev = -1;
            nodes[index].Next = -1;

            nodes[index].Next = free;
            free = index;
        }

        public List<TKey> YoungestKeys(int count)
        {
            var l = new List<TKey>(count);
            int i = head;
            while (i >= 0 && count > 0)
            {
                l.Add(nodes[i].Key);
                i = nodes[i].Next;
                count -= 1;
            }
            return l;
        }

        public List<TKey> OldestKeys(int count)
        {
            var l = new List<TKey>(count);
            int i = tail;
            while (i >= 0 && count > 0)
            {
                l.Add(nodes[i].Key);
                i = nodes[i].Prev;
                count -= 1;
            }
            return l;
        }

        public IEnumerable<TKey> Keys
        {
            get { return map.Keys; }
        }

        public IEnumerable<TValue> Values
        {
            get { return map.Values.Select(x => x.Value); }
        }
    }
}
