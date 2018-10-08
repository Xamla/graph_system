using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public struct Cursor
        : IEnumerable<Cursor>
        , ICursor
        , IEditableConvertible
    {
        byte[] buffer;
        int bufferEnd;

        ISchemaProvider schemaProvider;
        Schema schema;
        int offset;

        public Cursor(ISchemaProvider schemaProvider, Schema schema)
            : this(schemaProvider, schema, null, 0, 0)
        {
        }

        public Cursor(ISchemaProvider schemaProvider, Schema schema, ArraySegment<byte> segment)
            : this(schemaProvider, schema, segment.Array, segment.Offset, segment.Count)
        {
        }

        public Cursor(ISchemaProvider schemaProvider, Schema schema, byte[] buffer)
            : this(schemaProvider, schema, buffer, 0, buffer.Length)
        {
        }

        public Cursor(ISchemaProvider schemaProvider, Schema schema, byte[] buffer, int offset, int count)
        {
            this.schemaProvider = schemaProvider;
            this.schema = schema;
            this.buffer = buffer;
            this.offset = offset;
            this.bufferEnd = offset + count;
        }

        public Cursor Isolate()
        {
            var segment = this.GetFieldSegment();
            var extract = new byte[segment.Count];
            Buffer.BlockCopy(segment.Array, segment.Offset, extract, 0, segment.Count);
            return new Cursor(this.schemaProvider, this.Schema, new ArraySegment<byte>(extract));
        }

        public void WriteTo(Stream stream)
        {
            stream.Write(buffer, offset, bufferEnd - offset);
        }

        public Task WriteToAsync(Stream stream)
        {
            return stream.WriteAsync(buffer, offset, bufferEnd - offset);
        }

        #region ICursor implementation

        public Schema Schema
        {
            get { return schema; }
        }

        public ISchemaProvider SchemaProvider
        {
            get { return schemaProvider; }
        }

        public bool IsNull
        {
            get { return (buffer == null); }
        }

        public int Count
        {
            get
            {
                switch (schema.DataType)
                {
                    case DataType.List:
                    case DataType.MultiChoice:
                        int dummy;
                        return ReadCountOfList(out dummy);
                    case DataType.Class:
                        return schema.FieldCount;
                    default:
                        return 0;
                }
            }
        }

        public object Get()
        {
            if (buffer == null)
                return null;

            var dataType = this.schema.DataType;

            switch (dataType)
            {
                case DataType.Boolean:
                    return BitConverter.ToBoolean(buffer, offset);
                case DataType.Int32:
                case DataType.Choice:
                    return BitConverter.ToInt32(buffer, offset);
                case DataType.Int64:
                    return BitConverter.ToInt64(buffer, offset);
                case DataType.Float64:
                    return BitConverter.ToDouble(buffer, offset);
                case DataType.Decimal:
                    return new Decimal(new int[] {
                        BitConverter.ToInt32(buffer, offset),
                        BitConverter.ToInt32(buffer, offset + 4),
                        BitConverter.ToInt32(buffer, offset + 8),
                        BitConverter.ToInt32(buffer, offset + 12) 
                    });
                case DataType.DateTime:
                    return DateTime.FromBinary(BitConverter.ToInt64(buffer, offset));
                case DataType.TimeSpan:
                    return new TimeSpan(BitConverter.ToInt64(buffer, offset));
                case DataType.Guid:
                    {
                        var b = new byte[16];
                        Array.Copy(buffer, offset, b, 0, 16);
                        return new Guid(b);
                    }
                case DataType.String:
                case DataType.Binary:
                case DataType.ItemPath:
                    {
                        int sizeWidth;
                        int l = DecodeLength(offset, out sizeWidth);
                        if (dataType == DataType.String)
                            return Encoding.UTF8.GetString(buffer, offset + sizeWidth, l - sizeWidth);
                        else if (dataType == DataType.ItemPath)
                            return ItemIdExtensions.Read(buffer, offset + sizeWidth, l - sizeWidth);
                        else
                        {
                            var b = new byte[l - sizeWidth];
                            Buffer.BlockCopy(buffer, offset + sizeWidth, b, 0, b.Length);
                            return b;
                        }
                    }
                case DataType.Class:
                    switch (schema.Id)
                    {
                        case (int)BuiltInSchema.Variable:
                            return AccessVariable();
                        case (int)BuiltInSchema.Money:
                            return new Money(this.GoTo(1/*"Currency"*/).Get<string>(), this.GoTo(0/*"Amount"*/).Get<decimal>());
                        case (int)BuiltInSchema.GeoPosition:
                            return new GeoPosition(this.GoTo(0/*"Longitude"*/).Get<double>(), this.GoTo(1/*"Latitude"*/).Get<double>());
                        case (int)BuiltInSchema.Measurement:
                            return new Measurement(this.GoTo(1/*Unit*/).Get<string>(), this.GoTo(0/*Value*/).Get<double>());
                        case (int)BuiltInSchema.ListOfDateTimeOffset:
                            return new DateTimeOffset(this.GoTo(0/*Time*/).Get<DateTime>(), this.GoTo(1/*Offset*/).Get<TimeSpan>());
                    }
                    break;
            }

            return this;
        }

        ICursor ICursor.GoTo(int index, bool unwrapVariable)
        {
            return this.GoTo(index, unwrapVariable);
        }

        IEnumerable<ICursor> ICursor.Children
        {
            get { return this.Cast<ICursor>(); }
        }

        ICursor ICursor.CloneAsCursor()
        {
            return this;        // public interface of Cursor is immutable
        }

        #endregion

        int ReadCountOfList(out int offsetBehindCount)
        {
            int i = this.offset;
            byte x = buffer[i];
            if ((x & 1) == 0)
            {
                offsetBehindCount = i + 2;
                return buffer[i + 1] >> 1;
            }
            else if ((x & 3) == 1)
            {
                offsetBehindCount = i + 4;
                return BitConverter.ToUInt16(buffer, i + 2) >> 2;
            }
            else
            {
                offsetBehindCount = i + 8;
                return BitConverter.ToInt32(buffer, i + 4) >> 2;
            }
        }

        int DecodeLength(int offset, out int bytesRead)
        {
            int length = buffer[offset];
            if ((length & 1) == 0)
            {
                bytesRead = 1;
                return length >> 1;
            }
            else if ((length & 3) == 1)
            {
                bytesRead = 2;
                return (length | (buffer[offset + 1] << 8)) >> 2;
            }
            else
            {
                bytesRead = 4;
                return BitConverter.ToInt32(buffer, offset) >> 2;
            }
        }

        public ArraySegment<byte> GetFieldSegment()
        {
            if (buffer == null)
                return new ArraySegment<byte>();

            var dataType = this.schema.DataType;
            switch (dataType)
            {
                case DataType.Void:
                    return new ArraySegment<byte>(buffer, offset, 0);

                case DataType.Boolean:
                    return new ArraySegment<byte>(buffer, offset, 1);

                case DataType.Int32:
                case DataType.Choice:
                    return new ArraySegment<byte>(buffer, offset, 4);

                case DataType.Int64:
                case DataType.Float64:
                case DataType.DateTime:
                case DataType.TimeSpan:
                    return new ArraySegment<byte>(buffer, offset, 8);

                case DataType.Decimal:
                case DataType.Guid:
                    return new ArraySegment<byte>(buffer, offset, 16);

                case DataType.String:
                case DataType.Binary:
                case DataType.ItemPath:
                    {
                        int sizeWidth;
                        int l = DecodeLength(offset, out sizeWidth);
                        return new ArraySegment<byte>(buffer, offset, l);
                    }

                case DataType.Class:
                case DataType.List:
                case DataType.MultiChoice:
                    {
                        int sizeWidth;
                        int l = this.schema.VariableSize ? DecodeLength(this.offset + this.schema.VariableSizeOffset, out sizeWidth) : schema.FixedSize;
                        return new ArraySegment<byte>(buffer, offset, l);
                    }

                default:
                    throw new Exception("Unsupported data type");
            }
        }

        public Cursor AccessVariable()
        {
            if (this.IsNull)
                return new Cursor();

            int dataSchemaId = (int)AccessField(this.schema[(int)VariableLayout.DataSchemaId], false).Get();
            var current = AccessField(this.schema[(int)VariableLayout.Data], false);

            byte x = current.buffer[current.offset];
            if ((x & 1) == 0)
                ++current.offset;
            else if ((x & 3) == 1)
                current.offset += 2;
            else
                current.offset += 4;

            current.schema = schemaProvider.GetSchemaById(dataSchemaId);
            return current;
        }

        public Cursor AccessField(Field f, bool unwrapVariable = true)
        {
            var c = this;

            if (this.schema.NullBitmap && (buffer[offset + schema.NullBitmapOffset + f.Index / 8] & (1 << (f.Index % 8))) == 0)
            {
                c.schema = f.Schema;
                c.buffer = null;
                return c;
            }

            if (f.Offset >= 0)
            {
                // fixed size field
                c.schema = f.Schema;
                c.offset += f.Offset;
            }
            else
            {
                if (unwrapVariable && c.schema.Id == (int)BuiltInSchema.Variable)
                {
                    if (f.Index == (int)VariableLayout.Data)
                        return c.AccessVariable();
                }

                // variable size field
                int sizeWidth;
                int fieldOffset = offset + schema.VariableSizeOffset;
                DecodeLength(fieldOffset, out sizeWidth);
                fieldOffset = DecodeLength(fieldOffset + sizeWidth * -f.Offset, out sizeWidth);

                c.schema = f.Schema;
                c.offset += fieldOffset;

                // special handlin for variable field
                if (unwrapVariable && c.schema.Id == (int)BuiltInSchema.Variable)
                {
                    return c.AccessVariable();
                }
            }

            if (c.offset > bufferEnd)
                throw new IndexOutOfRangeException("Cursor ran outside specified block");

            return c;
        }

        public Cursor NavigateTo(FieldPath path)
        {
            var c = this;
            foreach (var x in path.path)
            {
                if (x.Op == FieldPath.FieldOp.Name)
                    c = c.GoTo(x.Name);
                else if (x.Op == FieldPath.FieldOp.Index)
                    c = c.GoTo(x.Index);
            }
            return c;
        }

        public bool TryNavigateTo(FieldPath path, out Cursor result)
        {
            result = this;
            foreach (var x in path.path)
            {
                if (result.IsNull)
                    return false;

                if (x.Op == FieldPath.FieldOp.Name)
                {
                    Field f;
                    if (!result.Schema.TryLookup(x.Name, out f))
                        return false;
                    result = result.GoTo(f.Index, true);
                }
                else if (x.Op == FieldPath.FieldOp.Index)
                {
                    if (x.Index < 0 || x.Index >= this.Count)
                        return false;
                    result = result.GoTo(x.Index);
                }
            }

            return true;
        }

        public Cursor GoTo(string name)
        {
            return AccessField(schema.Lookup(name), true);
        }

        Cursor Skip(int count)
        {
            var c = this;
            if (!schema.VariableSize)
            {
                c.offset += count * schema.FixedSize;
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    int dummy;
                    c.offset += DecodeLength(c.offset + c.schema.VariableSizeOffset, out dummy);
                }
            }
            return c;
        }

        public int SerializedSize
        {
            get { return Skip(1).offset - offset; }
        }

        public Cursor GoTo(int index, bool unwrapVariable = true)
        {
            if (buffer == null)
                throw new NullReferenceException();

            if (schema.DataType == DataType.Class)
            {
                return AccessField(schema[index], unwrapVariable);
            }
            else if (schema.DataType == DataType.List || schema.DataType == DataType.MultiChoice)
            {
                var c = this;
                int nullBitmapOffset;
                int count = ReadCountOfList(out nullBitmapOffset);
                if (index >= count || index < 0)
                    throw new IndexOutOfRangeException();

                Schema itemSchema;
                bool itemNullable;
                if (schema.DataType == DataType.MultiChoice)
                {
                    itemSchema = Schema.BuiltIn[BuiltInSchema.Int32];
                    itemNullable = false;
                }
                else
                {
                    itemSchema = this.schema[0].Schema;
                    itemNullable = this.schema[0].Nullable;
                }

                c.offset = nullBitmapOffset;
                if (schema.DataType != DataType.MultiChoice && c.schema[0].Nullable)
                    c.offset += (count + 7) / 8;	// positioned behind null bitmap

                c.schema = itemSchema;
                c = c.Skip(index);

                if (itemNullable && (buffer[nullBitmapOffset + index / 8] & (1 << (index % 8))) == 0)
                {
                    c.buffer = null;
                    return c;
                }

                // special handling for variable field lists
                if (unwrapVariable && c.schema.Id == (int)BuiltInSchema.Variable)
                {
                    return c.AccessVariable();
                }

                return c;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        class CursorEnumerator
            : IEnumerator<Cursor>
        {
            Cursor parent;
            Cursor current;
            int index;
            Field itemField;
            bool unwrapVariable;

            public CursorEnumerator(Cursor cursor, bool unwrapVariable)
            {
                parent = cursor;
                this.unwrapVariable = unwrapVariable;

                if (cursor.Schema.DataType == DataType.MultiChoice)
                    this.unwrapVariable = false;
            }

            #region IEnumerator<Cursor> Members

            public Cursor Current
            {
                get
                {
                    if (unwrapVariable && itemField.Schema.Id == (int)BuiltInSchema.Variable)
                        return current.AccessVariable();

                    return current;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose() { }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (parent.IsNull || index >= parent.Count)
                    return false;

                if (index == 0)
                {
                    if (parent.schema.DataType != DataType.MultiChoice)
                    {
                        itemField = parent.schema[0];
                    }

                    current = parent.GoTo(0, false);

                    index = 1;
                    return true;
                }
                else
                {
                    if (parent.schema.DataType == DataType.List || parent.schema.DataType == DataType.MultiChoice)
                    {
                        int nullBitmapOffset;
                        int count = parent.ReadCountOfList(out nullBitmapOffset);

                        current.buffer = parent.buffer;
                        current = current.Skip(1);
                        if (itemField != null && itemField.Nullable && (parent.buffer[nullBitmapOffset + index / 8] & (1 << (index % 8))) == 0)
                        {
                            current.buffer = null;
                        }

                        ++index;
                        return true;
                    }
                    else if (parent.schema.DataType == DataType.Class)
                    {
                        itemField = parent.schema[index];
                        current = parent.AccessField(itemField, false);      //     unwrapping is currently done in Current property
                        ++index;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                index = 0;
            }

            #endregion
        }

        public IEnumerator<Cursor> GetEnumerator(bool unwrapVariable)
        {
            return new CursorEnumerator(this, unwrapVariable);
        }

        public IEnumerator<Cursor> GetEnumerator()
        {
            return new CursorEnumerator(this, true);
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        bool ComparePrimitive(object other)
        {
            if (this.IsNull)
                return other == null;

            switch (this.schema.DataType)
            {
                case DataType.Boolean:
                case DataType.Int32:
                case DataType.Int64:
                case DataType.Float64:
                case DataType.Decimal:
                case DataType.DateTime:
                case DataType.TimeSpan:
                case DataType.Guid:
                case DataType.String:
                case DataType.ItemPath:
                case DataType.Choice:
                    return object.Equals(this.Get(), other);

                case DataType.Binary:
                    return EditablePrimitive.ByteArrayCompare(this.Get<byte[]>(), other as byte[]);
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return this.IsNull;

            if (base.Equals(obj))
                return true;

            ICursor cursor = obj as ICursor;
            if (cursor == null)
                return ComparePrimitive(obj);

            if (this.IsNull != cursor.IsNull)
                return false;

            if (this.IsNull)
                return true;

            switch (this.schema.DataType)
            {
                case DataType.Void:
                    return cursor.Schema.DataType == DataType.Void;

                case DataType.MultiChoice:
                case DataType.List:
                    return this.Cast<ICursor>().SequenceEqual(cursor.Children);

                case DataType.Class:
                    {
                        if (this.Schema == cursor.Schema)
                            return this.Cast<ICursor>().SequenceEqual(cursor.Children);
                        else
                        {
                            foreach (var f in this.Schema.Fields)
                            {
                                ICursor otherField;
                                if (!cursor.TryGoTo(f.Name, out otherField))
                                    return false;
                                if (!this.GoTo(f).Equals(otherField))
                                    return false;
                            }
                        }
                    }
                    break;

                default:
                    return ComparePrimitive(cursor.Get());
            }

            return true;
        }

        public IEditable ToEditable(IEditableFactory factory)
        {
            return factory.Create(this);
        }

        public dynamic ToDynamic()
        {
            return new DynamicCursor(this);
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class CursorHelpers
    {
        private static int ReadSizeWithCopy(Stream stream, Stream destination = null)
        {
            // default: byte with one type bit
            var numTypeBits = 1;

            // buffer for size
            var sizeBytes = new byte[4];

            // read single byte
            sizeBytes[0] = ReadByteWithCopy(stream, destination);

            if ((sizeBytes[0] & 1) == 1)     // case: byte
            {
                sizeBytes[1] = ReadByteWithCopy(stream, destination);

                if ((sizeBytes[0] & 3) == 3) // case: int
                {
                    sizeBytes[2] = ReadByteWithCopy(stream, destination);
                    sizeBytes[3] = ReadByteWithCopy(stream, destination);
                }

                numTypeBits = 2;
            }

            var size = BitConverter.ToInt32(sizeBytes, 0);

            // drop smallest bits for type encoding
            size >>= numTypeBits;

            return size;
        }

        public static byte[] ReadBlock(Stream stream)
        {
            var destination = new MemoryStream();

            var size = ReadSizeWithCopy(stream, destination);

            for (int i = 0; i < size; i++)
			{
                ReadByteWithCopy(stream, destination);
			}

            return destination.ToArray();
        }

        private static byte ReadByteWithCopy(Stream source, Stream destination = null)
        {
            int value = source.ReadByte();
            if (value < 0)
                throw new EndOfStreamException();

            var byteValue = (byte)value;

            if (destination != null)
                destination.WriteByte(byteValue);

            return byteValue;
        }
    }
}
