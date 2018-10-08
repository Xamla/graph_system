using System;
using System.Collections.Generic;

namespace Xamla.Types.Records
{
    public class MissingTypeMappingException
        : Exception
    {
        public MissingTypeMappingException(Type type)
            : base(string.Format("Missing type mapping for type: '{0}'", type.FullName))
        {
        }

        public MissingTypeMappingException(Schema schema)
            : base(string.Format("Deserialization failed: No type mapping is registered for schema '{0}'.", schema.Name))
        {
        }

        public MissingTypeMappingException(string message)
            : base(message)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RecordAttribute
        : Attribute
    {
        public int Version { get; set; }
        public int? MinVersion { get; set; }
        public string SchemaBaseName { get; set; }
        public bool MapBidirectional { get; set; }
        public bool MapList { get; set; }
        public int SchemaId { get; set; }

        public RecordAttribute()
        {
            this.Version = 1;
            this.SchemaId = -1;
            this.MinVersion = int.MinValue;
            this.MapBidirectional = true;
            this.MapList = true;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ChoiceOptionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class ChoiceSetAttribute
        : RecordAttribute
    {
        public bool MultiChoice { get; set; }
    }

    public interface ISchemaTypeMap
    {
        ISchemaProvider SchemaProvider { get; }

        /// <summary>
        /// Maps a schame to a type. The mapped schema will be returned
        /// by calls to TryGetTypeForSchema() if the specified schema is supplied.
        /// </summary>
        /// <param name="schema">Schema to be mapped.</param>
        /// <param name="type">Target type.</param>
        void MapSchema(Schema schema, Type type);

        /// <summary>
        /// Maps a type to a schema. The mapped schema is returned
        /// by calls to TryGetSchemaForType() if the specified type is supplied.
        /// </summary>
        /// <param name="type">Type to be mapped.</param>
        /// <param name="schema">Target schema.</param>
        void MapType(Type type, Schema schema);

        void UnmapSchema(Schema schema);
        void UnmapType(Type type);

        bool TryGetTypeForSchema(Schema schema, out Type type);
        bool TryGetSchemaForType(Type type, out Schema schema);

        void MapListOf<T>(Schema listSchema);
        void UnmapListOf<T>(Schema listSchema);

        string GetSchemaBaseName(Type type);
        string GetSchemaName(Type type, int? version = null);

        IEnumerable<Type> FindTypesWithRecordAttribue(System.Reflection.Assembly assembly);
        void AutoMapType(Type type);
        void AutoMapTypesWithRecordAttribute(System.Reflection.Assembly assembly);
    }

    public static class SchemaTypeMapExtensions
    {
        public static void MapBidirectional(this ISchemaTypeMap typeMap, Schema schema, Type type)
        {
            typeMap.MapSchema(schema, type);
            typeMap.MapType(type, schema);
        }

        public static void MapBidirectional(this ISchemaTypeMap typeMap, BuiltInSchema builtInSchema, Type type)
        {
            typeMap.MapBidirectional(Schema.BuiltIn[builtInSchema], type);
        }

        public static void MapBidirectional(this ISchemaTypeMap typeMap, string schemaName, Type type)
        {
            typeMap.MapBidirectional(typeMap.SchemaProvider.GetSchemaByName(schemaName), type);
        }

        public static Schema GetSchemaForType(this ISchemaTypeMap typeMap, Type type)
        {
            Schema schema;
            if (!typeMap.TryGetSchemaForType(type, out schema))
                throw new MissingTypeMappingException(schema);
            return schema;
        }

        public static Type GetTypeForSchema(this ISchemaTypeMap typeMap, Schema schema)
        {
            Type type;
            if (!typeMap.TryGetTypeForSchema(schema, out type))
                throw new MissingTypeMappingException(schema);
            return type;
        }
    }
}
