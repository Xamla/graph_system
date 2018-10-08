using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamla.Utilities;

namespace Xamla.Types.Records
{
    public class EditableVariable
        : IEditableVariable
    {
        IEditableFactory factory;
        IEditable data;
        Schema dataSchema;
        Cursor dataCursor;
        bool nullable;
        int serializedSize;

        public EditableVariable(IEditableFactory factory, bool nullable)
            : this(factory, nullable, default(Cursor))
        {
        }

        internal EditableVariable(IEditableFactory factory, bool nullable, Cursor dataCursor)
        {
            this.nullable = nullable;
            this.factory = factory;
            this.dataSchema = dataCursor.Schema ?? BuiltInSchemas.Boolean;
            this.dataCursor = dataCursor;
        }

        #region IEditable Members

        public Schema Schema
        {
            get { return BuiltInSchemas.Variable; }
        }

        public ISchemaProvider SchemaProvider
        {
            get { return this.Factory.SchemaProvider; }
        }

        public IEditableFactory Factory
        {
            get { return factory; }
        }

        public object Get()
        {
            if (data == null)
            {
                if (!dataCursor.IsNull)
                {
                    // caller expects to get an IEditable, 'convert' cursor to editable
                    data = factory.Create(dataCursor);
                    dataCursor = default(Cursor);
                }
                else
                {
                    // special auto-create logic of variable fields (even if variable was null)
                    data = factory.Create(dataSchema, nullable);
                }

                if (this.Frozen)
                    data.Freeze();
            }

            return data;
        }

        public T Get<T>()
        {
            return (T)Get();
        }

        public void Set(object value)
        {
            if (this.Frozen)
                throw new FrozenObjectReadonlyException();

            ICursor source = null;
            if (value != null)
            {
                source = value as ICursor;
                if (source == null)
                {
                    source = new EditablePrimitive(factory, value);
                }
                else
                {
                    var sourceVariable = source as IEditableVariable;
                    if (sourceVariable != null)
                    {
                        if (!sourceVariable.DataCursor.IsNull)
                        {
                            this.DataCursor = sourceVariable.DataCursor;
                            return;
                        }

                        if (!sourceVariable.IsNull)
                            source = source.Get() as ICursor;
                        else
                            source = null;
                    }
                    else if (source.Schema.Id == (int)BuiltInSchema.Variable)
                    {
                        if (source.IsNull)
                        {
                            this.Set(null);
                        }
                        else
                        {
                            var schema = this.SchemaProvider.GetSchemaById(source.GoTo((int)VariableLayout.DataSchemaId).Get<int>());
                            this.DataCursor = new Cursor(this.SchemaProvider, schema, new ArraySegment<byte>(source.GoTo((int)VariableLayout.Data, false).Get<byte[]>()));
                        }
                        return;
                    }
                }
            }

            if (source == null)
            {
                if (!nullable)
                {
                    throw new InvalidOperationException("Tried to set null on a variable field that is not nullable.");
                }

                data = null;
                dataCursor = default(Cursor);
                return;
            }
            
            if (data == null || dataSchema != source.Schema)
            {
                data = factory.Create(source.Schema, nullable);
                dataSchema = source.Schema;
            }

            dataCursor = default(Cursor);
            data.Set(source);
        }

        public void Freeze()
        {
            if (this.Frozen)
                return;

            if (data != null)
            {
                data.Freeze();
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
            EditableVariable c;
            if (!dataCursor.IsNull)
            {
                c = new EditableVariable(factory, nullable, dataCursor);
            }
            else
            {
                c = new EditableVariable(factory, nullable);
                c.Set(this);
            }

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
            get
            {
                return dataCursor.Schema == null && data == null;
            }
            set
            {
                if (this.Frozen)
                    throw new FrozenObjectReadonlyException();

                if (value)
                {
                    this.Set(null);
                }
                else if (this.IsNull)
                {
                    this.Set(factory.Create(dataSchema, false));
                }
            }
        }

        private int DataSize
        {
            get
            {
                System.Diagnostics.Debug.Assert(this.Frozen);
   
                if (!dataCursor.IsNull)
                    return dataCursor.GetFieldSegment().Count;
                else if (data != null)
                    return data.SerializedSize;
                else
                    return 0;
            }
        }

        protected virtual int CalculateSerializedSize()
        {
            int variableSize = this.IsNull ? 0 : EditablePrimitive.GetTotalFieldSize(this.DataSize);
            return EditableObject.CalculateSerializedSize(this.Schema, variableSize);
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

        public void WriteTo(BinaryWriter writer)
        {
            if (!this.Frozen)
            {
                this.CloneAsEditable(true).WriteTo(writer);
                return;
            }

            if (this.IsNull)
            {
                writer.Write(new byte[this.Schema.FixedSize]);
                int sizeWidth = EditablePrimitive.SizeWidthForSize(this.SerializedSize);
                EditablePrimitive.WriteSize(writer, this.SerializedSize, sizeWidth);
                writer.Write(new byte[this.SerializedSize - this.Schema.FixedSize - sizeWidth]);      // fill variable offset slot with zero bytes
            }
            else
            {
                writer.Write((int)dataSchema.Id);       // schema field

                bool dataIsNull = dataCursor.IsNull && data == null;
                byte nullBitmap = dataIsNull ? (byte)1 : (byte)3;
                writer.Write(nullBitmap);

                int sizeWidth = EditablePrimitive.SizeWidthForSize(serializedSize);
                EditablePrimitive.WriteSize(writer, serializedSize, sizeWidth);         // write total serialized size

                int offset = 5 + 2 * sizeWidth;                                         // set offset behind variable size offset slots
                EditablePrimitive.WriteSize(writer, offset, sizeWidth);                 // variable field offset slot

                // write size of data
                EditablePrimitive.WriteSize(writer, EditablePrimitive.GetTotalFieldSize(this.DataSize));

                if (!dataCursor.IsNull)
                {
                    var segment = dataCursor.GetFieldSegment();
                    writer.Write(segment.Array, segment.Offset, segment.Count);
                }
                else if (data != null)
                {
                    data.WriteTo(writer);
                }
            }
        }

        public dynamic ToDynamic()
        {
            return new DynamicEditable(this);
        }

        #endregion

        #region IEditableVariable Members

        public Schema DataSchema
        {
            get { return dataSchema; }
            set
            {
                if (value == dataSchema)
                    return;

                if (value == null)
                    throw new ArgumentNullException("value");

                dataSchema = value;
                if ((data != null && data.Schema != dataSchema) || (!dataCursor.IsNull && dataCursor.Schema != dataSchema))
                {
                    data = factory.Create(dataSchema, nullable);
                    dataCursor = default(Cursor);
                }
            }
        }

        public Cursor DataCursor
        {
            set
            {
                if (value.IsNull)
                    this.Set(null);

                data = null;
                dataCursor = value;
                dataSchema = dataCursor.Schema;
            }
            get
            {
                return dataCursor;
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as IEditableVariable;
            if (other != null)
                return object.Equals(this.Get(), other.Get());

            return object.Equals(this.Get(), obj);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Schema, this.IsNull ? this.DataSchema : this.Get());
        }

        public override string ToString()
        {
            if (this.IsNull)
                return "null";
            if (!dataCursor.IsNull)
                return "{Variable}";

            return this.Get().ToString();
        }

        int ICursor.Count
        {
            get { return 2; }
        }

        ICursor ICursor.GoTo(int index, bool unwrapVariable)
        {
            if (index == 0)
                return new EditablePrimitive(factory, this.DataSchema.Id);
            else if (index == 1)
            {
                if (data != null)
                    return data;
                else if (!dataCursor.IsNull || nullable)
                    return dataCursor;
                else
                {
                    data = factory.Create(dataSchema, nullable);
                    return data;
                }
            }

            throw new IndexOutOfRangeException();
        }

        IEnumerable<ICursor> ICursor.Children
        {
            get 
            {
                yield return new EditablePrimitive(factory, this.dataSchema.Id);
                yield return this.data; 
            }
        }
    }
}
