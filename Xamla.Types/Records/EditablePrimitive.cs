using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Xamla.Types.Records
{
    public class EditablePrimitive
        : IEditable
    {
        //[System.Runtime.InteropServices.DllImport("msvcr120.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        //static extern int memcmp(byte[] b1, byte[] b2, long count);

        //internal static bool ByteArrayCompare(byte[] b1, byte[] b2)
        //{
        //    if (b1 == null || b2 == null)
        //        return b1 == b2;

        //    return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        //}

        internal static unsafe bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null)
                return a1 == a2;

            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                {
                    if (*((long*)x1) != *((long*)x2))
                        return false;
                }
                if ((l & 4) != 0)
                {
                    if (*((int*)x1) != *((int*)x2))
                     return false;

                    x1 += 4;
                    x2 += 4;
                }
                if ((l & 2) != 0)
                {
                    if (*((short*)x1) != *((short*)x2))
                        return false;

                    x1 += 2;
                    x2 += 2;
                }
                if ((l & 1) != 0)
                {
                    if (*((byte*)x1) != *((byte*)x2))
                        return false;
                }
                return true;
            }
        }

        public class PrimitiveType
        {
            public int FixedSize;
            public BuiltInSchema SchemaId;
            public Type Type;
            public Action<BinaryWriter, object> Write;
            public Action<JsonWriter, object> JsonWrite;

            public object DefaultValue
            {
                get
                {
                    if (this.Type.GetTypeInfo().IsValueType)
                    {
                        if (this.SchemaId == BuiltInSchema.ItemPath)
                            return ItemId.GetRoot();
                        else if (this.SchemaId != BuiltInSchema.Void)
                            return Activator.CreateInstance(this.Type);
                        else
                            return null;
                    }
                    else
                    {
                        if (this.Type == typeof(string))
                            return string.Empty;
                        else if (this.Type == typeof(byte[]))
                            return new byte[0];
                        else
                            throw new NotSupportedException();
                    }
                }
            }
        }

        static PrimitiveType[] supportedTypes;

        static void WriteBinaryBuffer(BinaryWriter writer, byte[] buffer)
        {
            if (buffer == null)
            {
                WriteSize(writer, 1);   // serialize as empty array
            }
            else
            {
                int l = buffer.Length;
                if (l + 1 < 128)
                    WriteSize(writer, l + 1, 1);
                else if (l + 2 < 16384)
                    WriteSize(writer, l + 2, 2);
                else
                    WriteSize(writer, l + 4, 4);

                writer.Write(buffer);
            }
        }

        static EditablePrimitive()
        {
            supportedTypes = new PrimitiveType[13];
            supportedTypes[(int)DataType.Void] = new PrimitiveType { SchemaId = BuiltInSchema.Void, FixedSize = 0, Type = typeof(void), Write = (writer, data) => { }, JsonWrite = (writer, data) => { } };
            supportedTypes[(int)DataType.Boolean] = new PrimitiveType { SchemaId = BuiltInSchema.Boolean, FixedSize = 1, Type = typeof(bool), Write = (writer, data) => writer.Write((bool)data), JsonWrite = (writer, data) => writer.WriteValue((bool)data) };
            supportedTypes[(int)DataType.Int32] = new PrimitiveType { SchemaId = BuiltInSchema.Int32, FixedSize = 4, Type = typeof(int), Write = (writer, data) => writer.Write((int)data), JsonWrite = (writer, data) => writer.WriteValue((int)data) };
            supportedTypes[(int)DataType.Int64] = new PrimitiveType { SchemaId = BuiltInSchema.Int64, FixedSize = 8, Type = typeof(long), Write = (writer, data) => writer.Write((long)data), JsonWrite = (writer, data) => writer.WriteValue((long)data) };
            supportedTypes[(int)DataType.Float64] = new PrimitiveType { SchemaId = BuiltInSchema.Float64, FixedSize = 8, Type = typeof(double), Write = (writer, data) => writer.Write((double)data), JsonWrite = (writer, data) => writer.WriteValue((double)data) };
            supportedTypes[(int)DataType.Decimal] = new PrimitiveType { SchemaId = BuiltInSchema.Decimal, FixedSize = 16, Type = typeof(decimal), Write = (writer, data) => writer.Write((decimal)data), JsonWrite = (writer, data) => writer.WriteValue((decimal)data) };
            supportedTypes[(int)DataType.DateTime] = new PrimitiveType { SchemaId = BuiltInSchema.DateTime, FixedSize = 8, Type = typeof(DateTime), Write = (writer, data) => writer.Write(((DateTime)data).ToBinary()), JsonWrite = (writer, data) => writer.WriteValue((DateTime)data) };
            supportedTypes[(int)DataType.TimeSpan] = new PrimitiveType { SchemaId = BuiltInSchema.TimeSpan, FixedSize = 8, Type = typeof(TimeSpan), Write = (writer, data) => writer.Write(((TimeSpan)data).Ticks), JsonWrite = (writer, data) => writer.WriteValue((TimeSpan)data) };
            supportedTypes[(int)DataType.Guid] = new PrimitiveType { SchemaId = BuiltInSchema.Guid, FixedSize = 16, Type = typeof(Guid), Write = (writer, data) => writer.Write(((Guid)data).ToByteArray()), JsonWrite = (writer, data) => writer.WriteValue((Guid)data) };

            supportedTypes[(int)DataType.String] = new PrimitiveType
            {
                SchemaId = BuiltInSchema.String,
                FixedSize = -1,
                Type = typeof(string),
                Write = (writer, data) =>
                {
                    var s = (string)data;
                    WriteBinaryBuffer(writer, s != null ? UTF8Encoding.UTF8.GetBytes((string)data) : null);
                },
                JsonWrite = (writer, data) => writer.WriteValue((string)data)
            };

            supportedTypes[(int)DataType.Binary] = new PrimitiveType
            {
                SchemaId = BuiltInSchema.Binary,
                FixedSize = -1,
                Type = typeof(byte[]),
                Write = (writer, data) => WriteBinaryBuffer(writer, (byte[])data),
                JsonWrite = (writer, data) => writer.WriteValue((byte[])data)
            };

            supportedTypes[(int)DataType.ItemPath] = new PrimitiveType
            {
                SchemaId = BuiltInSchema.ItemPath,
                FixedSize = -1,
                Type = typeof(ItemId),
                Write = (writer, data) => WriteBinaryBuffer(writer, ((ItemId)data).ToArray()),
                JsonWrite = (writer, data) => writer.WriteValue(((ItemId)data).ToBase64Url())
            };

            supportedTypes[(int)DataType.Choice] = supportedTypes[(int)DataType.Int32];
        }

        public static bool IsSupportedType(DataType type)
        {
            return ((int)type >= 0 && (int)type < supportedTypes.Length);
        }

        public static int FixedSizeOf(DataType type)
        {
            if (!IsSupportedType(type))
                throw new ArgumentException(string.Format("Type '{0}' is not a supported primitive type", type), "type");
            int size = supportedTypes[(int)type].FixedSize;
            if (size >= 0)
                return size;
            return 0;
        }

        public static bool TryGetPrimitiveSchemaForType(Type type, out Schema primitiveSchema)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            int i = Array.FindIndex(supportedTypes, x => x.Type == type);
            if (i < 0)
            {
                primitiveSchema = null;
                return false;
            }
            else
            {
                primitiveSchema = Schema.BuiltIn[supportedTypes[i].SchemaId];
                return true;
            }
        }

        public static bool TryGetTypeForPrimitiveSchema(Schema primitiveSchema, out Type type)
        {
            PrimitiveType typeHelper;
            if (TryGetPrimitiveTypeHelper(primitiveSchema, out typeHelper))
            {
                type = typeHelper.Type;
                return true;
            }

            type = null;
            return false;
        }

        public static Schema FindSchemaFor(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value is IEditable)
            {
                return ((IEditable)value).Schema;
            }
            else
            {
                Schema primitiveSchema;
                if (TryGetPrimitiveSchemaForType(value.GetType(), out primitiveSchema))
                    return primitiveSchema;
            }

            throw new ArgumentException(string.Format("Type '{0}' is not a supported primitive type", value.GetType()), "type");
        }

        public static bool TryGetPrimitiveTypeHelper(Schema primitiveSchema, out PrimitiveType typeHelper)
        {
            typeHelper = null;
            if (primitiveSchema == null)
                return false;

            int i = (int)primitiveSchema.DataType;
            if (i >= 0 && i < supportedTypes.Length)
                typeHelper = supportedTypes[i];

            return typeHelper != null;
        }

        IEditableFactory factory;
        Schema schema;
        object data;
        bool nullable;

        public EditablePrimitive(IEditableFactory factory, Schema schema, bool nullable)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            if (!IsSupportedType(schema.DataType))
            {
                throw new ArgumentException("Invalid schema for value class specified", "schema");
            }

            this.factory = factory;
            this.schema = schema;
            this.nullable = nullable;

            if (!nullable)
            {
                data = this.TypeHelper.DefaultValue;
            }
        }

        public EditablePrimitive(IEditableFactory factory, Schema schema, bool nullable, object value)
            : this(factory, schema, nullable)
        {
            this.Set(value);
        }

        public EditablePrimitive(IEditableFactory factory, object value)
            : this(factory, FindSchemaFor(value), value == null || !value.GetType().GetTypeInfo().IsValueType, value)
        {
        }

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

        public object Get()
        {
            return data;
        }

        public T Get<T>()
        {
            return (T)data;
        }

        PrimitiveType TypeHelper
        {
            get { return supportedTypes[(int)schema.DataType]; }
        }

        public object TryConvertValue(object value)
        {
            if (value is string)
            {
                var s = (string)value;
                if (this.TypeHelper.Type == typeof(Guid))
                {
                    Guid g;
                    if (Guid.TryParse(s, out g))
                        return g;
                }
                else if (this.TypeHelper.Type == typeof(bool))
                {
                    var val = s.ToLower();
                    if (val == "true" || val == "on" || val == "1" || val == "yes")
                        return true;
                    else if (val == "false" || val == "off" || val == "0" || val == "no")
                        return false;
                }
                else if (string.IsNullOrWhiteSpace((string)value))
                {
                    if (nullable)
                        return null;
                    else if (new DataType[] { DataType.Decimal, DataType.Int32, DataType.Int64, DataType.Float64 }.Contains(schema.DataType))
                        value = 0;
                }
                else if (this.TypeHelper.Type == typeof(ItemId))
                {
                    ItemId itemPath;
                    if (ItemIdExtensions.TryParse((string)value, out itemPath))
                        return itemPath;

                    try
                    {
                        return ItemIdExtensions.FromBase64Url(s);
                    }
                    catch (FormatException)
                    {
                    }
                    catch (ItemIdException)
                    {
                    }
                }
                else if (this.TypeHelper.Type == typeof(byte[]))
                {
                    try
                    {
                        return Convert.FromBase64String(s);
                    }
                    catch (FormatException)
                    {
                    }
                }

                if (nullable && s.Equals("null", StringComparison.OrdinalIgnoreCase))
                    return null;
            }

            try
            {
                return Convert.ChangeType(value, TypeHelper.Type);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Field type is not compatible", "value", e);
            }
        }

        public void Set(object value)
        {
            if (this.Frozen)
                throw new FrozenObjectReadonlyException();

            if (value is ICursor)
                value = ((ICursor)value).Get();
            else if (ItemId.Null.Equals(value))
                value = null;

            if (value != null)
            {
                if (!this.TypeHelper.Type.GetTypeInfo().IsAssignableFrom(value.GetType()))
                {
                    value = TryConvertValue(value);
                }
            }
            else if (!nullable)
            {
                throw new ArgumentNullException("Field is not nullable");
            }

            data = value;
        }

        public void Freeze()
        {
            this.Frozen = true;
        }

        public bool Frozen
        {
            get;
            private set;
        }

        public IEditable CloneAsEditable(bool frozen)
        {
            var c = new EditablePrimitive(factory, this.schema, nullable);
            c.Set(data);
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
            get { return (data == null); }
            set
            {
                if (this.Frozen)
                    throw new FrozenObjectReadonlyException();

                if (value)
                {
                    this.Set(null);
                }
                else if (data == null)
                {
                    data = this.TypeHelper.DefaultValue;
                }
            }
        }

        internal static int SizeWidthForSize(int size)
        {
            if (size < 128)
                return 1;
            else if (size < 16384)
                return 2;
            else
                return 4;
        }

        internal static void WriteSize(BinaryWriter w, int value)
        {
            if (value < 128)
                w.Write((byte)(value << 1));        // 7 bits | 0
            else if (value < 16384)
                w.Write((short)(value << 2 | 1));   // 14 bits | 01
            else
                w.Write((int)(value << 2 | 3));     // 30 bits | 11
        }

        internal static void WriteSize(BinaryWriter w, int value, int fieldWidth)
        {
            if (fieldWidth == 1)
                w.Write((byte)(value << 1));        // 7 bits | 0
            else if (fieldWidth == 2)
                w.Write((short)(value << 2 | 1));   // 14 bits | 01
            else
                w.Write((int)(value << 2 | 3));     // 30 bits | 11
        }

        internal static int GetTotalFieldSize(int contentSize)
        {
            if (contentSize + 1 < 128)
                return contentSize + 1;
            else if (contentSize + 2 < 16384)
                return contentSize + 2;
            else
                return contentSize + 4;
        }

        public int SerializedSize
        {
            get
            {
                if (this.TypeHelper.FixedSize >= 0)
                    return this.TypeHelper.FixedSize;

                if (this.IsNull)
                    return 1;

                int length;
                switch (schema.DataType)
                {
                    case DataType.String:
                        length = UTF8Encoding.UTF8.GetByteCount((string)data);
                        break;

                    case DataType.Binary:
                        length = ((byte[])data).Length;
                        break;

                    case DataType.ItemPath:
                        length = ((ItemId)data).ToArray().Length;
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                return GetTotalFieldSize(length);
            }
        }

        public void WriteTo(Stream stream)
        {
            this.WriteTo(new BinaryWriter(stream, Encoding.UTF8, true));
        }

        public void WriteTo(BinaryWriter writer)
        {
            if (this.IsNull)
                this.TypeHelper.Write(writer, TypeHelper.DefaultValue);
            else
                this.TypeHelper.Write(writer, data);
        }

        public dynamic ToDynamic()
        {
            return new DynamicEditable(this);
        }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as EditablePrimitive;
            if (other != null)
                obj = other.data;
            else if (obj is ICursor)
                obj = ((ICursor)obj).Get();

            if (object.Equals(this.data, obj))
                return true;

            if (schema.DataType == DataType.Binary && other != null)
                return ByteArrayCompare(data as byte[], other.data as byte[]);

            return false;
        }

        public override int GetHashCode()
        {
            return (data ?? schema).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0}) {1}", this.Schema, data ?? "<null>");
        }

        int ICursor.Count
        {
            get { return 0; }
        }

        ICursor ICursor.GoTo(int index, bool unwrapVariable)
        {
            throw new InvalidOperationException();
        }

        IEnumerable<ICursor> ICursor.Children
        {
            get { return Enumerable.Empty<ICursor>(); }
        }
    }
}
