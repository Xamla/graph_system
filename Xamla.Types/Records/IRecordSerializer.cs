using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamla.Types.Records
{
    public interface IRecordSerializer
    {
        ISchemaProvider SchemaProvider { get; }
        ISchemaTypeMap SchemaTypeMap { get; }
        IEditableFactory EditableFactory { get; }

        IEditable ToEditable(object source, bool mayThrow = true, bool frozen = false);

        object FromCursor(ICursor cursor);
        T FromCursor<T>(ICursor cursor);

        object FromCursor(Cursor cursor);
        T FromCursor<T>(Cursor cursor);

        object FromMemory(ArraySegment<byte> segment, Schema schema);
        T FromMemory<T>(ArraySegment<byte> segment);

        bool CanDeserialize(ICursor cursor);
        Item<T> DeserializeItemT<T>(ICursor cursor);
        object Deserialize(ICursor cursor, Type objectType);
        Item<object> DeserializeObjectItem(ICursor cursor);

        void ApplyToEditable(IEditable target, object source);
        void ApplyToObject(object target, ICursor source);

        bool IsOfType(ICursor cursor, Type type);
        T CloneObject<T>(T source) where T : class;
    }

    public static class RecordSerializerExtensions
    {
        public static object FromMemory(this IRecordSerializer serializer, byte[] buffer, Schema schema)
        {
            return serializer.FromMemory(new ArraySegment<byte>(buffer), schema);
        }

        public static T FromMemory<T>(this IRecordSerializer serializer, byte[] buffer)
        {
            return serializer.FromMemory<T>(new ArraySegment<byte>(buffer));
        }

        public static bool IsOfType<T>(this IRecordSerializer serializer, ICursor cursor)
        {
            return serializer.IsOfType(cursor, typeof(T));
        }
    }
}
