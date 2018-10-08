using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public interface IEditableFactory
    {
        ISchemaProvider SchemaProvider { get; }

        IEditable Create(Schema schema, bool nullable = false);
        IEditable Create(BuiltInSchema schemaId, bool nullable = false);
        IEditable Create(ICursor cursor);
        IEditable Create(Cursor cursor);

        IEditable CreateFromMemory(ArraySegment<byte> segment, Schema schema);
        IEditable CreateFromJson(JsonReader reader, Schema schema);

        IEditableObject CreateItem(bool nullable = false);
        IEditableObject CreateItem(Schema dataSchema, bool nullable = false);
        //IEditableObject CreateItemFromJson(JsonReader reader, Schema dataSchema);

        IEditable CreatePrimitive(object value);
        IEditableVariable CreateVariable(IEditable content = null, bool nullable = true);
    }

    public static class EditableFactoryExtensions
    {
        public static IEditable CreateFromMemory(this IEditableFactory factory, MemoryStream mem, Schema schema)
        {
            ArraySegment<byte> buffer;
            if (!mem.TryGetBuffer(out buffer))
                throw new Exception("Buffer of MemoryStream object is not accessible");
            return factory.CreateFromMemory(buffer, schema);
        }

        public static IEditable CreateFromMemory(this IEditableFactory factory, byte[] mem, Schema schema)
        {
            return factory.CreateFromMemory(new ArraySegment<byte>(mem), schema);
        }

        public static IEditable CreateFromJson(this IEditableFactory factory, string json, Schema schema)
        {
            return factory.CreateFromJson(new JsonTextReader(new StringReader(json)), schema);
        }

        public static IEditable CreateFromJson(this IEditableFactory factory, string json, BuiltInSchema schema)
        {
            return factory.CreateFromJson(json, Schema.BuiltIn[schema]);
        }

        public static IEditable CreateFromJson(this IEditableFactory factory, JsonReader reader, BuiltInSchema schema)
        {
            return factory.CreateFromJson(reader, Schema.BuiltIn[schema]);
        }

        public static IEditableObject CreateItem(this IEditableFactory factory, BuiltInSchema dataSchema, bool nullable = false)
        {
            return factory.CreateItem(Schema.BuiltIn[dataSchema], nullable);
        }

        public static IEditableObject CreateItem(this IEditableFactory factory, string dataSchemaName, bool nullable = false)
        {
            return factory.CreateItem(factory.SchemaProvider.GetSchemaByName(dataSchemaName), nullable);
        }

        public static IEditableObject CreateItemFromJson(this IEditableFactory factory, JsonReader reader)
        {
            return (IEditableObject)factory.CreateFromJson(reader, BuiltInSchema.Item);
        }

        public static IEditableObject CreateItemFromJson(this IEditableFactory factory, string json)
        {
            return (IEditableObject)factory.CreateFromJson(json, BuiltInSchema.Item);
        }
    }
}
