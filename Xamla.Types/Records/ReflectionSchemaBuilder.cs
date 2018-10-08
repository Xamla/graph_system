using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamla.Utilities;

namespace Xamla.Types.Records
{
    public class ReflectionSchemaBuilder
        : IReflectionSchemaBuilder
    {
        IEditableFactory editableFactory;
        ISchemaProvider schemaProvider;
        ISchemaTypeMap typeMap;

        public ReflectionSchemaBuilder(ISchemaProvider schemaProvider, IEditableFactory editableFactory, ISchemaTypeMap schemaTypeMap)
        {
            this.schemaProvider = schemaProvider;
            this.editableFactory = editableFactory;
            this.typeMap = schemaTypeMap;
        }

        Schema BuildSchemaForEnum(Type type, string schemaName, List<Tuple<Type, Schema>> schemaList)
        {
            var dataType = DataType.Choice;

            var choiceSetAttribute = type.GetTypeInfo().GetCustomAttribute<ChoiceSetAttribute>();
            if (choiceSetAttribute != null && choiceSetAttribute.MultiChoice)
                dataType = DataType.MultiChoice;

            var sb = new SchemaBuilder(schemaName, dataType, ChoiceSet.FromEnum(type).ToEditable(editableFactory));
            var result = sb.ToSchema();

            if (choiceSetAttribute != null)
            {
                result.Id = choiceSetAttribute.SchemaId;
            }

            schemaList.Add(Tuple.Create(type, result));
            return result;
        }

        Schema BuildSchemaForClass(Type type, string schemaName, List<Tuple<Type, Schema>> schemaList, bool onlyMissing)
        {
            var excludedProperties = new List<String>();
            if (typeof(Item).IsAssignableFrom(type))
            {
                excludedProperties.AddRange(typeof(Item).GetTypeInfo().GetProperties().Select(x => x.Name));
            }

            var sb = new SchemaBuilder(schemaName, DataType.Class, null);
            foreach (var p in type.GetTypeInfo().GetProperties().Where(x => x.CanRead && x.CanWrite && !excludedProperties.Contains(x.Name)))
            {
                if (p.GetCustomAttribute<NonRecordFieldAttribute>() != null)
                    continue;

                var fieldAttribute = p.GetCustomAttribute<RecordFieldAttribute>();

                string caption = null;
                string description = null;
                int? maxLength = null;
                bool? nullable = null;

                if (fieldAttribute != null)
                {
                    caption = fieldAttribute.Caption;
                    description = fieldAttribute.Description;
                    if (fieldAttribute.MaxLength > 0)
                        maxLength = fieldAttribute.MaxLength;
                    nullable = fieldAttribute.Nullable;
                }

                if (nullable == null && (Nullable.GetUnderlyingType(p.PropertyType) != null || typeof(INullable).IsAssignableFrom(p.PropertyType)))
                {
                    nullable = true;
                }

                Schema schema;
                Type propertyType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (propertyType == typeof(Choice) || propertyType == typeof(MultiChoice))
                {
                    if (fieldAttribute == null)
                        throw new Exception(string.Format("CacheFieldAttribute missing for property '{0}' of type {1}.", p.Name, propertyType));

                    schema = schemaProvider.GetSchemaByName(fieldAttribute.SchemaName);
                    if (p.PropertyType == typeof(Choice) && schema.DataType != DataType.Choice)
                        throw new Exception(string.Format("Choice field expected but field has data type: '{0}'", schema.DataType));
                    else if (p.PropertyType == typeof(MultiChoice) && schema.DataType != DataType.MultiChoice)
                        throw new Exception(string.Format("MultiChoice field expected but field has data type: '{0}'", schema.DataType));
                    sb.AddField(p.Name, schema, nullable.Value, maxLength, caption, description);
                }
                else if (EditablePrimitive.TryGetPrimitiveSchemaForType(p.PropertyType, out schema))
                {
                    // primitive data type first (covers nullable primitive types)
                    nullable = nullable ?? (p.PropertyType == typeof(string) || p.PropertyType == typeof(byte[]));
                    sb.AddField(p.Name, schema, nullable.Value, maxLength, caption, description);
                }
                else if (typeof(ICursor).IsAssignableFrom(p.PropertyType))
                {
                    // create variable field (if no schema was explicitely specified)
                    if (fieldAttribute != null && fieldAttribute.SchemaName != null)
                    {
                        schema = schemaProvider.GetSchemaByName(fieldAttribute.SchemaName);
                    }
                    else
                    {
                        schema = Schema.BuiltIn[BuiltInSchema.Variable];
                    }

                    sb.AddField(p.Name, schema, nullable ?? true, maxLength, caption, description);
                }
                else if (p.PropertyType == typeof(object))
                {
                    sb.AddField(p.Name, Schema.BuiltIn[BuiltInSchema.Variable], nullable ?? true, maxLength, caption, description);
                }
                else
                {
                    if (propertyType.GetTypeInfo().IsEnum && fieldAttribute != null)
                    {
                        schema = schemaProvider.GetSchemaByName(fieldAttribute.SchemaName);
                    }
                    else if (!typeMap.TryGetSchemaForType(propertyType, out schema))
                    {
                        var t = schemaList.Find(x => x.Item1 == propertyType);
                        if (t != null)
                            schema = t.Item2;
                        else
                            schema = BuildSchemaRecursively(propertyType, null, schemaList, onlyMissing);
                    }

                    if (schema == null)
                        throw new MissingTypeMappingException(string.Format("Unable to generate a schema for property '{0}' (type'{1}') of class '{2}'.", p.Name, p.PropertyType.FullName, type.FullName));

                    // a schema was registerd for the property type
                    if (!nullable.HasValue)
                    {
                        nullable = !p.PropertyType.GetTypeInfo().IsValueType;
                    }

                    sb.AddField(p.Name, schema, nullable.Value, maxLength, caption, description);
                }
            }

            var result = sb.ToSchema();

            var recordAttribute = type.GetTypeInfo().GetCustomAttribute<RecordAttribute>();
            if (recordAttribute != null)
            {
                result.Id = recordAttribute.SchemaId;
            }

            schemaList.Add(Tuple.Create(type, result));
            return result;
        }

        Schema BuildListSchemaInternal(Schema elementSchema)
        {
            var sb = new SchemaBuilder(string.Format("List<{0}>", elementSchema.Name), DataType.List);
            sb.AddField(null, elementSchema);
            return sb.ToSchema();
        }

#if DEBUG
        void CompareSchema(Schema a, Schema b, Stack<string> compareList)
        {
            compareList.Push(a.Name);
            if (a.FieldCount != b.FieldCount || a.DataType != b.DataType || a.Name != b.Name)
            {
                Debug.Fail(
                    string.Format("Schema was modified! Increment schema version of {{ {0} }}.", string.Join(", ", compareList))
                );
            }

            foreach (var x in a.Fields)
            {
                var y = b[x.Index];
                if (x.Name != y.Name || x.Nullable != y.Nullable)
                {
                    Debug.Fail(
                        string.Format("Schema field was modified! Old: '{0}' (nullable: {1}), New: '{2}' (nullable: {3}) ! Increment schema version of {{ {4} }}.", x.Name, x.Nullable, y.Name, y.Nullable, string.Join(", ", compareList))
                    );
                }

                CompareSchema(x.Schema, y.Schema, compareList);
            }
            compareList.Pop();
        }

        void VerifySchema(Schema schema, Type type)
        {
            Debug.WriteLine("Verifying {0}", (object)schema.Name);
            var schemaList = new List<Tuple<Type, Schema>>();
            var rebuild = BuildSchemaRecursively(type, schema.Name, schemaList, false);
            CompareSchema(schema, rebuild, new Stack<string>());
        }
#endif

        Schema BuildSchemaRecursively(Type type, string schemaName, List<Tuple<Type, Schema>> schemaList, bool onlyMissing)
        {
            Type targetType = Nullable.GetUnderlyingType(type) ?? type;

            Schema schema;
            if (onlyMissing && typeMap.TryGetSchemaForType(targetType, out schema))
            {
                if (typeMap.GetSchemaName(targetType) == schema.Name)       // check if version really matches
                {
#if DEBUG
                    VerifySchema(schema, type);
#endif
                    return schema;
                }
            }

            schema = schemaList.Where(x => x.Item1 == targetType).Select(x => x.Item2).FirstOrDefault();
            if (schema != null)
                return schema;

            schemaName = schemaName ?? typeMap.GetSchemaName(type);
            if (onlyMissing && schemaProvider.TryGetSchemaByName(schemaName, out schema))
            {
#if DEBUG
                VerifySchema(schema, type);
#endif
                return schema;
            }

            if (targetType.GetTypeInfo().IsEnum)
            {
                return BuildSchemaForEnum(targetType, schemaName, schemaList);
            }
            else if (targetType.GetTypeInfo().IsClass)
            {
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(targetType))
                {
                    var genericEnumerable = targetType.GetGenericTypeBase(typeof(IEnumerable<>));
                    if (genericEnumerable != null)
                    {
                        var elementType = genericEnumerable.GetGenericArguments()[0];
                        Schema elementSchema;
                        if (!typeMap.TryGetSchemaForType(elementType, out elementSchema))
                        {
                            elementSchema = BuildSchemaRecursively(elementType, null, schemaList, onlyMissing);
                        }

                        var listSchema = BuildListSchemaInternal(elementSchema);
                        schemaList.Add(Tuple.Create(targetType, listSchema));
                        return listSchema;
                    }
                }

                return BuildSchemaForClass(targetType, schemaName, schemaList, onlyMissing);
            }
            else
            {
                return null;
            }
        }

        public IList<Tuple<Type, Schema>> BuildSchemaForType(Type type, string schemaName = null, bool onlyIfMissing = false)
        {
            var list = new List<Tuple<Type, Schema>>();
            BuildSchemaRecursively(type, schemaName, list, onlyIfMissing);
            return list;
        }

        public Schema BuildListSchemaForType(Type type)
        {
            Schema elementSchema;
            if (!typeMap.TryGetSchemaForType(type, out elementSchema))
                throw new Exception(string.Format("Schema for type '{0}' not found", type));

            return BuildListSchemaInternal(elementSchema);
        }

        public IList<Tuple<Type, Schema>> BuildSchemasForTypesWithRecordAttribute(Assembly assembly, bool onlyMissing)
        {
            var list = new List<Tuple<Type, Schema>>();
            foreach (var type in typeMap.FindTypesWithRecordAttribue(assembly))
            {
                var schema = BuildSchemaRecursively(type, null, list, onlyMissing);
                if (type.GetTypeInfo().GetCustomAttribute<RecordAttribute>().MapList)
                {
                    BuildSchemaRecursively(typeof(List<>).MakeGenericType(type), null, list, onlyMissing);
                }
            }
            return list;
        }
    }
}
