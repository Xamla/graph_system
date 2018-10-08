using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xamla.Utilities;

namespace Xamla.Types.Records
{
    public class EditableObject
        : IEditableObject
    {
        protected IEditableFactory factory;
        protected Schema schema;
        protected IEditable[] fields;
        protected bool nullable;
        int serializedSize;

        public EditableObject(IEditableFactory factory, Schema schema, bool nullable)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (schema == null)
                throw new ArgumentNullException("schema");

            this.factory = factory;
            this.schema = schema;
            this.nullable = nullable;

            if (!this.nullable)
                CreateFields();
        }

        void CreateFields()
        {
            this.fields = this.schema.Fields.Select(x => factory.Create(x.Schema, x.Nullable)).ToArray();
        }

        #region IEditableObject Members

        public IEditable GetField(string name)
        {
            return GetField(schema.Lookup(name).Index);
        }

        public bool TryGetField(string name, out IEditable result)
        {
            Field f;
            result = null;

            if (schema.TryLookup(name, out f))
            {
                result = GetField(f.Index);
                return true;
            }

            return false;
        }

        public virtual IEditable GetField(int index)
        {
            if (this.fields == null)
                CreateFields();
            return fields[index];
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

        public IEditableFactory Factory
        {
            get { return factory; }
        }

        public virtual object Get()
        {
            return this;
        }

        public T Get<T>()
        {
            return (T)(this as IEditableObject);
        }

        public virtual void Set(object value)
        {
            if (this.Frozen)
                throw new FrozenObjectReadonlyException();

            var other = value as ICursor;
            if (value == null || (other != null && other.IsNull))
            {
                if (!nullable)
                    throw new ArgumentNullException("Field is not nullable");

                this.fields = null;
                return;
            }

            if (other == null || other.Schema.DataType != DataType.Class)
                throw new ArgumentException("Passed object is not an compatible.", "value");

            if (other.Schema == this.Schema)
            {
                for (int i = 0; i < this.Schema.FieldCount; ++i)
                {
                    if (schema[i].Schema.Id == (int)BuiltInSchema.Variable)
                    {
                        this.GetField(i).Set(other.GoTo(i, false));
                    }
                    else
                    {
                        this.GetField(i).Set(other.GoTo(i).Get());
                    }
                }
            }
            else
            {
                Field otherField;
                foreach (var f in this.Schema.Fields)
                {
                    if (other.Schema.TryLookup(f.Name, out otherField))
                    {
                        this.GetField(f.Index).Set(other.GoTo(otherField.Index).Get());
                    }
                }
            }
        }

        public void Freeze()
        {
            if (this.Frozen)
                return;

            if (fields != null)
            {
                foreach (var f in fields)
                    f.Freeze();
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
            var c = new EditableObject(factory, schema, nullable);
            c.Set(this);
            if (frozen)
                c.Freeze();
            return c;
        }

        public ICursor CloneAsCursor()
        {
            return CloneAsEditable(true);
        }

        public bool IsNull
        {
            get { return (this.fields == null); }
            set
            {
                if (this.Frozen)
                    throw new FrozenObjectReadonlyException();

                if (value)
                    this.Set(null);
                else if (fields == null)
                    CreateFields();
            }
        }

        internal static int CalculateSerializedSize(Schema schema, int variableSize)
        {
            int fixedSize = schema.FixedSize;
            int variableFieldCount = schema.variableFieldCount;

            if (variableFieldCount == 0)
                return fixedSize;

            // determine size of field offsets and object size 
            int size = fixedSize + variableSize + (1 + variableFieldCount) * 1;
            if (size >= 128)
            {
                size = fixedSize + variableSize + (1 + variableFieldCount) * 2;
                if (size >= 16384)
                {
                    size = fixedSize + variableSize + (1 + variableFieldCount) * 4;
                    if (size >= 1073741824)
                        throw new OverflowException("Record is too big to be serialized");
                }
            }
            return size;
        }

        protected virtual int CalculateSerializedSize()
        {
            int fixedSize = schema.FixedSize;
            int variableSize = 0;

            if (!this.IsNull)
            {
                fixedSize = 0;

                Debug.Assert(schema.FieldCount == fields.Length);
                Debug.Assert((schema.variableFieldCount > 0) == (schema.VariableSizeOffset >= 0));

                for (int i = 0; i < fields.Length; ++i)
                {
                    var f = fields[i];
                    Debug.Assert(f.Schema == schema[i].Schema);
                    Debug.Assert(i == schema[i].Index);

                    if (f.Schema.VariableSize)
                    {
                        if (!f.IsNull)
                        {
                            variableSize += f.SerializedSize;
                        }
                    }
                    else
                    {
                        fixedSize += f.SerializedSize;
                    }
                }

                Debug.Assert(schema.NullBitmapOffset < 0 || schema.NullBitmapOffset == fixedSize);

                if (schema.NullBitmap)
                    fixedSize += (fields.Length + 7) / 8;

                Debug.Assert(schema.VariableSizeOffset < 0 || schema.VariableSizeOffset == fixedSize);
            }

            Debug.Assert(fixedSize == schema.FixedSize);

            return CalculateSerializedSize(schema, variableSize);
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
            this.WriteTo(new BinaryWriter(stream, Encoding.UTF8, true));
        }

        public virtual void WriteTo(BinaryWriter writer)
        {
            if (!this.Frozen)
            {
                this.CloneAsEditable(true).WriteTo(writer);
                return;
            }

            int serializedSize;
            if (this.IsNull)
            {
                serializedSize = this.SerializedSize;
                writer.Write(new byte[schema.FixedSize]);
                if (this.Schema.VariableSize)
                {
                    int sizeWidth = EditablePrimitive.SizeWidthForSize(serializedSize);
                    EditablePrimitive.WriteSize(writer, serializedSize, sizeWidth);
                    writer.Write(new byte[serializedSize - schema.FixedSize - sizeWidth]);      // pad after skip size with zero bytes (object is null)
                }
                return;
            }

            int i = 0, offset = 0;

#if DEBUG
            serializedSize = this.SerializedSize;
            //long start = writer.Seek(0, SeekOrigin.Current);
#endif

            // fixed size part
            for (; i < fields.Length; ++i)
            {
                var f = fields[i];
                if (f.Schema.VariableSize)
                    break;

                offset += f.SerializedSize;
                f.WriteTo(writer);

                //#if DEBUG
                //                Debug.Assert((writer.Seek(0, SeekOrigin.Current) - start) == offset);
                //#endif
            }

            Debug.Assert(schema.variableFieldCount == fields.Length - i);
            Debug.Assert(schema.NullBitmapOffset < 0 || schema.NullBitmapOffset == offset);

            if (schema.NullBitmap)
            {
                byte[] nullBitmap = new byte[(fields.Length + 7) / 8];

                // write null bitmap
                for (int k = 0; k < fields.Length; ++k)
                {
                    var f = fields[k];
                    Debug.Assert(!f.IsNull || schema.fields[k].Nullable);
                    if (!f.IsNull)
                        nullBitmap[k / 8] |= (byte)(1 << (k % 8));
                }

                offset += nullBitmap.Length;
                writer.Write(nullBitmap);
            }

            if (schema.VariableSize)
            {
                Debug.Assert(schema.VariableSizeOffset == offset);

#if !DEBUG
                serializedSize = this.SerializedSize;
#endif

                int sizeWidth = EditablePrimitive.SizeWidthForSize(serializedSize);

                EditablePrimitive.WriteSize(writer, serializedSize, sizeWidth);
                offset += (1 + schema.variableFieldCount) * sizeWidth;	// set offset behind variable size offset slots

                // store variable field offsets
                int[] variableOffsets = new int[schema.variableFieldCount];
                int j = i;
                for (; i < fields.Length; ++i)
                {
                    var f = fields[i];
                    Debug.Assert(f.Schema.VariableSize);
                    Debug.Assert(!f.IsNull || schema.fields[i].Nullable);

                    EditablePrimitive.WriteSize(writer, offset, sizeWidth);
                    variableOffsets[i - j] = offset;
                    if (!f.IsNull)
                    {
                        offset += f.SerializedSize;
                    }
                }

                // write variable field data
                for (i = j; i < fields.Length; ++i)
                {
                    //#if DEBUG
                    //                    Debug.Assert((writer.Seek(0, SeekOrigin.Current) - start) == variableOffsets[i - j]);
                    //#endif
                    var f = fields[i];
                    if (!f.IsNull)
                    {
                        f.WriteTo(writer);
                    }
                }
            }

#if DEBUG
            Debug.Assert(offset == serializedSize);
            //Debug.Assert((writer.Seek(0, SeekOrigin.Current) - start) == serializedSize);
#endif
        }

        public dynamic ToDynamic()
        {
            return new DynamicEditable(this);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return this.IsNull;

            var other = obj as ICursor;
            if (other == null)
                return false;
            else if (other.IsNull)
                return this.IsNull;

            if (this.Schema.FieldCount != other.Schema.FieldCount)
                return false;

            Field otherField;
            foreach (var f in this.Schema.Fields)
            {
                if (other.Schema.TryLookup(f.Name, out otherField))
                {
                    if (!this.GetField(f.Index).Equals(other.GoTo(otherField.Index)))
                        return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (this.IsNull)
                return schema.GetHashCode();
            else
                return HashHelper.GetHashCode(this.Schema.Fields.Select(f => this.GetField(f.Index)));
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        int ICursor.Count
        {
            get { return schema.FieldCount; }
        }

        ICursor ICursor.GoTo(int index, bool unwrapVariable)
        {
            ICursor field = this.GetField(index);
            if (unwrapVariable && field is IEditableVariable)
            {
                var variable = (IEditableVariable)field;
                field = variable.DataCursor;
                if (field.IsNull)
                    field = variable.Get<ICursor>();
            }

            return field;
        }

        IEnumerable<ICursor> ICursor.Children
        {
            get { return this.fields.Cast<ICursor>(); }
        }
    }
}
