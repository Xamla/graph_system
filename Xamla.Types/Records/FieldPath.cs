using System;
using System.Collections.Generic;
using System.Text;

namespace Xamla.Types.Records
{
    public class FieldPath
    {
        public enum FieldOp
        {
            Name,
            Index
        }

        public struct Element
        {
            public FieldOp Op;
            public string Name;
            public int Index;
        }

        public List<Element> path = new List<Element>();

        public static FieldPath Parse(string text)
        {
            var path = new FieldPath();
            int start = 0;
            int pos = 0;
            bool number = false;
            string name;
            foreach (char c in text)
            {
                if (number)
                {
                    if (c == ']')
                    {
                        path.AppendIndex(int.Parse(text.Substring(start, pos - start)));
                        number = false;
                        start = pos + 1;
                    }
                }
                else if (c == '.' || c == '[')
                {
                    name = text.Substring(start, pos - start).Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        path.AppendField(name);
                    }
                    if (c == '[')
                    {
                        number = true;
                    }
                    start = pos + 1;
                }
                ++pos;
            }

            if (number)
                throw new FormatException("expected ']'");

            name = text.Substring(start, pos - start).Trim();
            if (!string.IsNullOrEmpty(name))
            {
                path.AppendField(name);
            }

            return path;
        }

        public static FieldPath Create(params string[] parts)
        {
            var r = new FieldPath();
            foreach (var p in parts)
            {
                r.AppendField(p);
            }
            return r;
        }

        public FieldPath()
        {
        }

        public FieldPath AppendField(string memberName)
        {
            path.Add(new Element { Op = FieldOp.Name, Name = memberName });
            return this;
        }

        public FieldPath AppendIndex(int index)
        {
            path.Add(new Element { Op = FieldOp.Index, Index = index });
            return this;
        }

        public FieldPath Clone()
        {
            return new FieldPath() { path = new List<Element>(this.path) };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var x in path)
            {
                if (x.Op == FieldOp.Name)
                {
                    if (sb.Length > 0)
                        sb.Append('.');
                    sb.Append(x.Name);
                }
                else if (x.Op == FieldOp.Index)
                {
                    sb.AppendFormat("[{0}]", x.Index);
                }
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as FieldPath;
            return other != null
                && other.ToString() == this.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
