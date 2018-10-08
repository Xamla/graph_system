using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamla.Utilities;
using System.IO;

namespace Xamla.Types.Records
{
    public class EditableList
        : IEditableList
    {
        IEditableFactory factory;
        Schema schema;
        Schema itemSchema;
        bool itemNullable;
        List<IEditable> list;
        bool nullable;
        int serializedSize;

        public EditableList(IEditableFactory factory, Schema schema, bool nullable)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (schema == null)
                throw new ArgumentNullException("schema");

            this.factory = factory;
            this.schema = schema;

            if (schema.DataType == DataType.MultiChoice)
            {
                this.itemSchema = Schema.BuiltIn[BuiltInSchema.Int32];
                this.itemNullable = false;
            }
            else
            {
                if (schema.DataType != DataType.List && schema.DataType != DataType.MultiChoice || schema.FieldCount != 1)
                    throw new ArgumentException("Invalid schema specified for item list", "schema");

                this.itemSchema = schema[0].Schema;
                this.itemNullable = schema[0].Nullable;
            }

            this.nullable = nullable;
            if (!this.nullable)
                this.Clear();
        }

        #region IEditableList Members

        public Schema ItemSchema
        {
            get { return itemSchema; }
        }

        public IEditableFactory Factory
        {
            get { return factory; }
        }

        public IEditable Add()
        {
            if (this.Frozen)
                throw new FrozenObjectReadonlyException();

            if (list == null)
                list = new List<IEditable>();

            var item = factory.Create(itemSchema, itemNullable);
            list.Add(item);
            return item;
        }

        public IEditable GetItem(int index)
        {
            return list[index];
        }

        public IEditable InsertAt(int index)
        {
            if (list == null)
                list = new List<IEditable>();

            var item = factory.Create(itemSchema, itemNullable);
            list.Insert(index, item);
            return item;
        }

        public void RemoveAt(int index)
        {
            if (list == null)
                return;

            list.RemoveAt(index);
        }

        public int Count
        {
            get { return (list != null) ? list.Count : -1; }
        }

        public void Clear()
        {
            if (list == null)
                list = new List<IEditable>();

            list.Clear();
        }

        public int Capacity
        {
            get { return list != null ? list.Capacity : -1; }
            set
            {
                if (list == null)
                    list = new List<IEditable>(value);
                else
                    list.Capacity = value;
            }
        }

        #endregion

        #region IEditable Members

        public Schema Schema
        {
            get { return schema; }
        }

        public ISchemaProvider SchemaProvider
        {
            get { return this.Factory.SchemaProvider; }
        }

        public object Get()
        {
            return this;
        }

        public void Set(object value)
        {
            if (this.Frozen)
                throw new FrozenObjectReadonlyException();

            var cursor = value as ICursor;
            if (value == null || (cursor != null && cursor.IsNull))
            {
                if (!nullable)
                    throw new ArgumentNullException("Field is not nullable");

                list = null;
                return;
            }

            this.Clear();
            if (list.Capacity < cursor.Count)
                list.Capacity = cursor.Count;
            foreach (var i in cursor.Children)
            {
                this.Add().Set(i);
            }
        }

        public void Freeze()
        {
            if (this.Frozen)
                return;

            if (list != null)
            {
                foreach (var i in list)
                    i.Freeze();
            }

            this.Frozen = true;

            serializedSize = CalculateSerializedSize();
        }

        public bool Frozen
        {
            get;
            private set;
        }

        public IEditable CloneAsEditable(bool frozen)
        {
            var c = new EditableList(factory, schema, nullable);
            c.Set(this);
            if (frozen)
                c.Freeze();
            return c;
        }

        public ICursor CloneAsCursor()
        {
            return this.CloneAsEditable(true);
        }

        public bool IsNull
        {
            get { return (list == null); }
            set
            {
                if (this.Frozen)
                    throw new FrozenObjectReadonlyException();

                if (value)
                {
                    this.Set(null);
                }
                else
                {
                    if (list == null)
                        Clear();
                }
            }
        }

        int CalculateSerializedSize()
        {
            if (this.IsNull)
                return 1;

            int size = this.Sum(x => x.SerializedSize);
            if (itemNullable)
                size += (list.Count + 7) / 8;   // space for null bitmap

            // determine byte-count of size and count field
            if (size + 2 * 1 < 128)
                size += 2;
            else if (size + 2 * 2 < 16384)
                size += 4;
            else
                size += 8;

            return size;
        }

        public int SerializedSize
        {
            get
            {
                if (!this.Frozen)
                    throw new OperationRequiresFrozenObjectException();

                return serializedSize;
            }
        }

        public void WriteTo(Stream stream)
        {
            this.WriteTo(new System.IO.BinaryWriter(stream, Encoding.UTF8, true));
        }

        public void WriteTo(BinaryWriter writer)
        {
            if (!this.Frozen)
            {
                this.CloneAsEditable(true).WriteTo(writer);
                return;
            }

            if (this.IsNull)
                EditablePrimitive.WriteSize(writer, 1);
            else
            {
                int serializedSize = this.SerializedSize;
                int sizeWidth = EditablePrimitive.SizeWidthForSize(serializedSize);
                EditablePrimitive.WriteSize(writer, serializedSize, sizeWidth);
                EditablePrimitive.WriteSize(writer, this.Count, sizeWidth);

                if (itemNullable)
                {
                    var nullBitmap = new byte[(list.Count + 7) / 8];
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (!list[i].IsNull)
                            nullBitmap[(i / 8)] |= (byte)(1 << (i % 8));
                    }
                    writer.Write(nullBitmap);
                }

                foreach (var i in this)
                {
                    i.WriteTo(writer);
                }
            }
        }

        public void WriteTo(JsonWriter writer, bool stripNullValues)
        {
            if (this.IsNull)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();
                foreach (var item in this)
                {
                    if (!stripNullValues || !item.IsNull)
                    {
                        item.WriteTo(writer, stripNullValues);
                    }
                }
                writer.WriteEndArray();
            }
        }

        public dynamic ToDynamic()
        {
            return new DynamicEditable(this);
        }

        #endregion

        #region IEnumerable<IEditable> Members

        public IEnumerator<IEditable> GetEnumerator()
        {
            return (list != null) ? list.GetEnumerator() : Enumerable.Empty<IEditable>().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.IsNull)
            {
                sb.Append("<null>");
            }
            else
            {
                foreach (var i in this)
                {
                    if (sb.Length > 1000)
                    {
                        sb.Append(", ...");
                        break;
                    }
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(i.ToString());
                }
            }

            return string.Format("({0}) {1}", this.Schema.ToString(), sb.ToString());
        }

        public override bool Equals(object obj)
        {
            var other = obj as ICursor;
            if (other == null && (obj != null || this.IsNull))
                return false;

            if (this.IsNull != other.IsNull)
                return false;

            if (!this.IsNull)
            {
                if (this.Count != other.Count)
                    return false;

                if (!((ICursor)this).Children.SequenceEqual(other.Children.Select(x => (x.Schema != null && x.Schema.Id == (int)BuiltInSchema.Variable) ? x.Get<ICursor>() : x)))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = schema.GetHashCode();
            if (list != null)
            {
                hash ^= HashHelper.GetHashCode(list.Take(8));
                hash ^= list.Count + 1;
            }
            return hash;
        }

        ICursor ICursor.GoTo(int index, bool unwrapVariable)
        {
            ICursor item = GetItem(index);
            if (unwrapVariable && item is IEditableVariable)
            {
                item = item.Get<ICursor>();
            }

            return item;
        }

        IEnumerable<ICursor> ICursor.Children
        {
            get
            {
                if (itemSchema.Id == (int)BuiltInSchema.Variable)
                    return this.Select(x => x.Get<ICursor>());
                else
                    return this.Cast<ICursor>();
            }
        }
    }
}
