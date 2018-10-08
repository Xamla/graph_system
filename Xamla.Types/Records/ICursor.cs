using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Xamla.Types.Records
{
    public interface ICursor
    {
        Schema Schema { get; }
        ISchemaProvider SchemaProvider { get; }
        bool IsNull { get; }
        int Count { get; }
        IEnumerable<ICursor> Children { get; }
        object Get();
        ICursor GoTo(int index, bool unwrapVariable = true);
        ICursor CloneAsCursor();
        void WriteTo(Stream writer);
        dynamic ToDynamic();
    }

    public static class CursorExtensions
    {
        public static object Get(this ICursor source, FieldPath path)
        {
            return source.NavigateTo(path).Get();
        }

        public static T Get<T>(this ICursor cursor)
        {
            object value = cursor.Get();
            if (value == null)
                return default(T);
            return (T)value;
        }

        public static T Get<T>(this ICursor cursor, T defaultValue)
        {
            if (cursor.IsNull)
                return defaultValue;
            return (T)cursor.Get();
        }

        public static ICursor GoTo(this ICursor source, string name)
        {
            return source.GoTo(source.Schema.Lookup(name).Index, true);
        }

        public static ICursor GoTo(this ICursor source, Field field, bool unwrapVariable = true)
        {
            return source.GoTo(field.Index, unwrapVariable);
        }

        public static ICursor NavigateTo(this ICursor source, FieldPath path)
        {
            foreach (var x in path.path)
            {
                if (x.Op == FieldPath.FieldOp.Name)
                    source = source.GoTo(x.Name);
                else if (x.Op == FieldPath.FieldOp.Index)
                    source = source.GoTo(x.Index, true);
            }
            return source;
        }

        public static ICursor NavigateTo(this ICursor source, string path)
        {
            return source.NavigateTo(FieldPath.Parse(path));
        }

        public static bool TryGoTo(this ICursor source, string name, out ICursor cursor)
        {
            Field f;
            if (source.IsNull || !source.Schema.TryLookup(name, out f))
            {
                cursor = null;
                return false;
            }

            cursor = source.GoTo(f.Index, true);
            return true;
        }

        public static bool TryNavigateTo(this ICursor source, FieldPath path, out ICursor cursor)
        {
            cursor = null;
            foreach (var x in path.path)
            {
                if (x.Op == FieldPath.FieldOp.Name)
                {
                    if (!source.TryGoTo(x.Name, out source))
                        return false;
                }
                else if (x.Op == FieldPath.FieldOp.Index)
                {
                    if (source.IsNull || source.Count <= x.Index)
                        return false;
                    source = source.GoTo(x.Index, true);
                }
            }
            cursor = source;
            return true;
        }

        public static void WriteTo(this ICursor source, JsonWriter writer, bool stripNullValues = true)
        {
            if (source == null || source.IsNull)
                writer.WriteNull();
            else
            {
                switch (source.Schema.DataType)
                {
                    case DataType.Class:
                        if (source.Schema.Id == (int)BuiltInSchema.Variable)
                        {
                            var content = source.GoTo((int)VariableLayout.Data, true);
                            if (!content.IsNull
                                && content.Schema.DataType != DataType.String
                                && content.Schema.DataType != DataType.Boolean
                                && (content.Schema.DataType == DataType.List
                                    || content.Schema.DataType == DataType.MultiChoice
                                    || EditablePrimitive.IsSupportedType(content.Schema.DataType)
                                    )
                                )
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("@schema");
                                writer.WriteValue(source.Schema.Name);
                                writer.WritePropertyName("DataSchema");
                                writer.WriteValue(content.Schema.Name);
                                writer.WritePropertyName("Data");
                                content.WriteTo(writer, stripNullValues);
                                writer.WriteEndObject();
                            }
                            else
                            {
                                content.WriteTo(writer, stripNullValues);
                            }
                        }
                        else
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("@schema");
                            writer.WriteValue(source.Schema.Name);
                            foreach (var f in source.Schema.Fields)
                            {
                                var field = source.GoTo(f, false);
                                if (!stripNullValues || !field.IsNull)
                                {
                                    writer.WritePropertyName(f.Name);
                                    field.WriteTo(writer, stripNullValues);
                                }
                            }
                            writer.WriteEndObject();
                        }
                        break;
                    case DataType.MultiChoice:
                    case DataType.List:
                        {
                            writer.WriteStartArray();
                            for (int i = 0; i < source.Count; ++i)
                            {
                                var field = source.GoTo(i, false);
                                if (!stripNullValues || !field.IsNull)
                                {
                                    field.WriteTo(writer, stripNullValues);
                                }
                            }
                            writer.WriteEndArray();
                        }
                        break;
                    default:
                        {
                            EditablePrimitive.PrimitiveType typeHelper;
                            if (EditablePrimitive.TryGetPrimitiveTypeHelper(source.Schema, out typeHelper))
                                typeHelper.JsonWrite(writer, source.Get());
                            else
                                throw new Exception(string.Format("Unable to convert data type '{0}' to JSON string.", source.Schema.DataType));
                        }
                        break;
                }
            }
        }

        public static string ToJson(this ICursor source, bool stripNullValues = false)
        {
            var sw = new StringWriter();
            using (var jw = new JsonTextWriter(sw))
            {
                source.WriteTo(jw, stripNullValues);
            }
            return sw.ToString();
        }

        public static MemoryStream ToMemoryStream(this ICursor source)
        {
            var ms = new MemoryStream();
            source.WriteTo(ms);
            ms.Position = 0;
            return ms;
        }

        public static byte[] ToByteArray(this ICursor source)
        {
            if (source is Cursor)
            {
                var cursor = (Cursor)source;
                return cursor.Isolate().GetFieldSegment().Array;
            }
            else 
            {
                IEditable editable = source as IEditable;
                if (editable != null)
                {
                    return editable.ToByteArray(false);
                }
                else
                {
                    return source.ToMemoryStream().ToArray();
                }
            }
        }
    }
}
