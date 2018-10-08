using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xamla.Types.Records
{
    public class RecordFieldAttribute
        : Attribute
    {
        public RecordFieldAttribute()
        {
            this.MaxLength = -1;
        }

        public string SchemaName { get; set; }
        public bool Nullable { get; set; }
        public int MaxLength { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
    }

    public class NonRecordFieldAttribute
        : Attribute
    {
    }

    public interface IReflectionSchemaBuilder
    {
        IList<Tuple<Type, Schema>> BuildSchemaForType(Type type, string schemaName = null, bool onlyIfMissing = false);
        Schema BuildListSchemaForType(Type type);

        IList<Tuple<Type, Schema>> BuildSchemasForTypesWithRecordAttribute(Assembly assembly, bool onlyMissing = true);
    }
}
