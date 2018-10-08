using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xamla.Types.Records
{
    public class RecordSerializer
        : IRecordSerializer
    {
        ISchemaProvider schemaProvider;
        ISchemaTypeMap schemaTypeMap;
        IEditableFactory editableFactory;

        public RecordSerializer(ISchemaProvider schemaProvider, ISchemaTypeMap schemaTypeMap, IEditableFactory editableFactory)
        {
            this.schemaProvider = schemaProvider;
            this.schemaTypeMap = schemaTypeMap;
            this.editableFactory = editableFactory;
        }

        public ISchemaProvider SchemaProvider 
        {
            get	{ return schemaProvider; }
        }

        public ISchemaTypeMap SchemaTypeMap
        {
            get { return schemaTypeMap; }
        }

        public IEditableFactory EditableFactory
        {
            get { return editableFactory; }
        }

        public void ApplyToEditable(IEditable target, object source)
        {
            if (source == null)
            {
                target.Set(null);
                return;
            }

            if (source is ICursor)
            {
                target.Set(source);
                return;
            }

            var obj = target as IEditableObject;
            if (obj != null)
            {
                if (target.Schema.Id == (int)BuiltInSchema.DateTimeOffset && source is DateTimeOffset)
                {
                    var sourceDate = (DateTimeOffset)source;
                    obj.GetField("Time").Set(sourceDate.DateTime);
                    obj.GetField("Offset").Set(sourceDate.Offset);
                }
                else if (target.Schema.Id == (int)BuiltInSchema.Version && source is Version)
                {
                    var sourceVersion = (Version)source;
                    obj.GetField("Major").Set(sourceVersion.Major);
                    obj.GetField("Minor").Set(sourceVersion.Minor);
                    obj.GetField("Build").Set(sourceVersion.Build);
                    obj.GetField("Revision").Set(sourceVersion.Revision);
                }
                else
                {
                    var sourceType = source.GetType();

                    // read properties
                    var properties = sourceType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var p in properties.Where(x => x.CanRead))
                    {
                        Field f;
                        if (obj.Schema.TryLookup(p.Name, out f))
                        {
                            var field = obj.GetField(f.Index);
                            var value = p.GetGetMethod().Invoke(source, null);

                            try
                            {
                                ApplyToEditable(field, value);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(string.Format("Error setting field '{0}': {1}", p.Name, e.Message), e);
                            }
                        }
                    }

                    // read fields
                    var fields = sourceType.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var p in fields)
                    {
                        Field f;
                        if (obj.Schema.TryLookup(p.Name, out f))
                        {
                            var field = obj.GetField(f.Index);
                            var value = p.GetValue(source);

                            try
                            {
                                ApplyToEditable(field, value);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(string.Format("Error setting field '{0}': {1}", p.Name, e.Message), e);
                            }
                        }
                    }

                    if (obj.Schema.Id == (int)BuiltInSchema.Item
                        && (!sourceType.GetTypeInfo().IsGenericType || sourceType.GetGenericTypeDefinition() != typeof(Item<>))
                        && typeof(Item).GetTypeInfo().IsAssignableFrom(sourceType))
                    {
                        var field = obj.GetField(ItemLayout.Data);
                        ApplyToEditable(field, source);
                    }
                }
                return;
            }

            var list = target as IEditableList;
            if (list != null)
            {
                var sourceEnum = source as System.Collections.IEnumerable;
                if (sourceEnum == null)
                    throw new Exception("Provider type for list values does not implement IEnumerable");

                list.Clear();   // ensure the list in non-null

                foreach (var x in sourceEnum)
                {
                    ApplyToEditable(list.Add(), x);
                }

                return;
            }

            var variable = target as IEditableVariable;
            if (variable != null)
            {
                if (source != null)
                {
                    Schema schema;
                    if (schemaTypeMap.TryGetSchemaForType(source.GetType(), out schema))
                    {
                        variable.DataSchema = schema;
                        if (variable.IsNull)
                        {
                            variable.Set(editableFactory.Create(schema));
                        }
                        ApplyToEditable(variable.Get<IEditable>(), source);
                    }
                    else
                    {
                        if (source.GetType() != typeof(Item))
                            throw new MissingTypeMappingException(source.GetType());
                        else
                            variable.Set(null);
                    }
                }
                else
                {
                    variable.Set(null);
                }

                return;
            }

            if (source is Choice)
            {
                source = ((Choice)source).Value;
            }

            target.Set(source);
        }

        public IEditable ToEditable(object source, bool mayThrow = true, bool frozen = false)
        {
            if (source == null)
            {
                if (!mayThrow)
                    return null;

                throw new ArgumentNullException("value");
            }

            // find object schema and create editable
            Schema schema;
            if (source is Item)
            {
                schema = Schema.BuiltIn[BuiltInSchema.Item];
            }
            else
            {
                var type = source.GetType();
                if (!schemaTypeMap.TryGetSchemaForType(type, out schema))
                {
                    // forward IEnumerable<T> mapping fallback
                    Type foundType = type.GetTypeInfo().GetInterfaces().FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (foundType == null || !schemaTypeMap.TryGetSchemaForType(foundType, out schema))
                    {
                        if (!mayThrow)
                            return null;
                        throw new MissingTypeMappingException(type);
                    }
                }
            }

            var result = editableFactory.Create(schema);

            ApplyToEditable(result, source);

            if (frozen)
                result.Freeze();

            return result;
        }

        bool TryGetValueForProperty(ICursor source, string name, Type propertyType, out object value)
        {
            Field f;
            if (!source.Schema.TryLookup(name, out f))
            {
                value = null;
                return false;
            }

            ICursor propertyCursor = source.GoTo(f.Index, !(propertyType == typeof(IEditableVariable)));
            Type targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            // null value handling
            if (propertyCursor.IsNull)
            {
                value = null;
                if (!propertyType.GetTypeInfo().IsValueType || propertyType.GetTypeInfo().IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (targetType == typeof(Choice))
                    {
                        value = new Choice(propertyCursor.Schema, null, f.Nullable);
                    }

                    return true;
                }
                else if (typeof(INullable).GetTypeInfo().IsAssignableFrom(propertyType))                    // null handling for SqlTypes (e.g. SqlHierarchyId)
                {
                    value = propertyType.GetTypeInfo().GetProperty("Null").GetValue(null);       // get static null value of type
                    return true;
                }
                
                // skipping null value for non nullable property...
                return false;
            }

            // now handle non-null record field values...
            if (propertyCursor.Schema.DataType == DataType.Class
                || propertyCursor.Schema.DataType == DataType.List
                || propertyCursor.Schema.DataType == DataType.MultiChoice)
            {
                if (propertyCursor.Schema.FieldCount > 0 && propertyCursor.Schema[0].Schema.Id == (int)BuiltInSchema.Variable)
                {
                    // special handling for interface type IList<> when deserializing from record type List<Variable>
                    if (targetType.GetTypeInfo().IsInterface && targetType.GetTypeInfo().IsGenericType
                        && (targetType.GetGenericTypeDefinition() == typeof(IList<>) || targetType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        targetType = typeof(List<>).MakeGenericType(targetType.GetTypeInfo().GetGenericArguments());
                    }

                    value = Construct(targetType, propertyCursor);
                    ApplyToObject(value, propertyCursor);
                }
                else
                {
                    value = this.FromCursorInternal(propertyCursor, targetType);
                }
            }
            else
            {
                if (propertyType.GetTypeInfo().IsAssignableFrom(typeof(IEditable)) || propertyType.GetTypeInfo().IsAssignableFrom(typeof(IEditableVariable)))
                {
                    value = editableFactory.Create(propertyCursor);
                }
                else if (propertyType.GetTypeInfo().IsAssignableFrom(typeof(ICursor)))
                {
                    value = propertyCursor;
                }
                else
                {
                    value = propertyCursor.Get();
                }
            }

            if (targetType.GetTypeInfo().IsEnum)
            {
                var underlyingEnumType = Enum.GetUnderlyingType(targetType);
                if (!underlyingEnumType.GetTypeInfo().IsAssignableFrom(value.GetType()))
                {
                    value = Convert.ChangeType(value, underlyingEnumType);
                }

                value = Enum.ToObject(targetType, value);
            }
            else if (targetType == typeof(Choice))
            {
                System.Diagnostics.Debug.Assert(value != null);
                if (!typeof(int).GetTypeInfo().IsAssignableFrom(value.GetType()))
                {
                    value = Convert.ChangeType(value, typeof(int));
                }

                value = new Choice(propertyCursor.Schema, (int?)value, f.Nullable);
            }

            if (!targetType.GetTypeInfo().IsAssignableFrom(value.GetType()))
            {
                value = Convert.ChangeType(value, targetType);
            }

            return true;
        }

        public void ApplyToObject(object target, ICursor source)
        {
            var type = target.GetType();
            var schema = source.Schema;

            if (schema.DataType == DataType.Class)
            {
                var fields = type.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var f in fields)
                {
                    object value;
                    if (TryGetValueForProperty(source, f.Name, f.FieldType, out value))
                    {
                        f.SetValue(target, value);
                    }
                }

                var properties = type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in properties.Where(x => x.CanWrite))
                {
                    object value;
                    if (TryGetValueForProperty(source, p.Name, p.PropertyType, out value))
                    {
                        var setter = p.GetSetMethod();
                        setter.Invoke(target, new [] { value });
                    }
                }
                   
                if (schema.Id == (int)BuiltInSchema.Item
                    && (!type.GetTypeInfo().IsGenericType || type.GetGenericTypeDefinition() != typeof(Item<>))
                    && typeof(Item).GetTypeInfo().IsAssignableFrom(type))
                {
                    ICursor propertyCursor = source.GoTo((int)ItemLayout.Data, true);
                    ApplyToObject(target, propertyCursor);
                }
            }
            else if (schema.DataType == DataType.List || schema.DataType == DataType.MultiChoice)
            {
                if (type.IsArray)
                {
                    int i = 0;
                    var targetArray = (Array)target;
                    foreach (var v in source.Children.Select(FromCursor))
                        targetArray.SetValue(v, i++);
                }
                else
                {
                    Schema itemSchema;
                    if (schema.DataType == DataType.MultiChoice)
                        itemSchema = Schema.BuiltIn[BuiltInSchema.Int32];
                    else
                        itemSchema = schema[0].Schema;

                    if (itemSchema.Id == (int)BuiltInSchema.Variable)
                    {
                        foreach (var itemCursor in source.Children)
                        {
                            object value = this.FromCursor(itemCursor);
                            if (value == null)
                                continue;

                            var addMethod = type.GetTypeInfo().GetMethod("Add", new Type[] { value.GetType() });
                            if (addMethod != null)
                            {
                                addMethod.Invoke(target, new object[] { value });
                            }
                        }
                    }
                    else
                    {
                        Type itemType;
                        if (schemaTypeMap.TryGetTypeForSchema(itemSchema, out itemType))
                        {
                            var addMethod = type.GetTypeInfo().GetMethod("Add", new Type[] { itemType });
                            if (addMethod == null)
                                return;

                            foreach (var itemCursor in source.Children)
                            {
                                object value = this.FromCursor(itemCursor);

                                if (!itemType.GetTypeInfo().IsAssignableFrom(value.GetType()))
                                {
                                    value = Convert.ChangeType(value, itemType);
                                }

                                addMethod.Invoke(target, new object[] { value });
                            }
                        }
                    }
                }
            }
        }

        object Construct(Type type, ICursor cursor)
        {
            if (type.IsArray)
            {
                return Array.CreateInstance(type.GetElementType(), cursor.Count);
            }
            else
            {
                var ctor = type.GetTypeInfo().GetConstructor(Type.EmptyTypes);
                if (ctor != null)
                    return ctor.Invoke(null);

                ctor = type.GetTypeInfo().GetConstructor(new Type[] { typeof(ISchemaCache) });
                if (ctor != null)
                {
                    return ctor.Invoke(new object[] { schemaProvider });
                }

                ctor = type.GetTypeInfo().GetConstructor(new Type[] { typeof(Schema) });
                if (ctor != null)
                {
                    return ctor.Invoke(new object[] { cursor.Schema });
                }

                return Activator.CreateInstance(type);
            }
        }

        public object FromCursor(ICursor cursor)
        {
            return FromCursorInternal(cursor, null);
        }

        public T FromCursor<T>(ICursor cursor)
        {
            return (T)FromCursorInternal(cursor, typeof(T));
        }

        public object FromCursor(Cursor cursor)
        {
            // high-speed handling not yet implemented...
            return FromCursor((ICursor)cursor);
        }

        public T FromCursor<T>(Cursor cursor)
        {
            // high-speed handling not yet implemented...
            return FromCursor<T>((ICursor)cursor);
        }

        object FromCursorInternal(ICursor cursor, Type targetType)
        {
            if (cursor == null || cursor.IsNull)
                return null;

            if (EditablePrimitive.IsSupportedType(cursor.Schema.DataType))
            {
                return cursor.Get();
            }
            else if (cursor.Schema.Id == (int)BuiltInSchema.DateTimeOffset)
            {
                return new DateTimeOffset(cursor.GoTo("Time").Get<DateTime>(), cursor.GoTo("Offset").Get<TimeSpan>());
            }
            else if (cursor.Schema.Id == (int)BuiltInSchema.Version)
            {
                int major = cursor.GoTo("Major").Get<int>();
                int minor = cursor.GoTo("Minor").Get<int>();
                int build = cursor.GoTo("Build").Get<int>();
                int revision = cursor.GoTo("Revision").Get<int>();

                if (revision >= 0)
                    return new Version(major, minor, build, revision);
                
                if (build >= 0)
                    return new Version(major, minor, build);

                return new Version(major, minor);
   
            }
            else if (cursor.Schema.Id == (int)BuiltInSchema.Item)
            {
                return DeserializeItem(cursor);
            }

            Type type;
            if (!schemaTypeMap.TryGetTypeForSchema(cursor.Schema, out type))
            {
                if (cursor.Schema.DataType == DataType.Choice)
                {
                    return new Choice(cursor.Schema, cursor.Get<int?>(), true);
                }
                else
                {
                    if (targetType != null && (targetType.GetTypeInfo().IsAssignableFrom(typeof(IEditable)) || targetType.GetTypeInfo().IsAssignableFrom(typeof(IEditableVariable))))
                    {
                        return editableFactory.Create(cursor);
                    }

                    throw new MissingTypeMappingException(cursor.Schema);
                }
            }
            else
            {
                if (targetType != null && !targetType.GetTypeInfo().IsAssignableFrom(type))
                {
                    if (targetType.GetTypeInfo().IsAssignableFrom(typeof(IEditable)) || targetType.GetTypeInfo().IsAssignableFrom(typeof(IEditableVariable)))
                    {
                        return editableFactory.Create(cursor);
                    }
                    else if (targetType.IsArray && typeof(System.Collections.IEnumerable).GetTypeInfo().IsAssignableFrom(type))
                    {
                        type = targetType;
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("Cannot cast from {0} to {1}.", type.Name, targetType.Name));
                    }
                }
            }

            object obj = Construct(type, cursor);
            ApplyToObject(obj, cursor);
            return obj;
        }

        public bool CanDeserialize(ICursor cursor)
        {
            if (cursor == null || cursor.IsNull)
                return true;

            var schema = cursor.Schema;
            if (schema.Id == (int)BuiltInSchema.Variable)
            {
                if (!schemaProvider.TryGetSchemaById(cursor.GoTo((int)VariableLayout.DataSchemaId).Get<int>(), out schema))
                    return false;
            }
            else if (schema.Id == (int)BuiltInSchema.Item)
            {
                return CanDeserialize(cursor.GoTo((int)ItemLayout.Data, false));
            }

            Type type;
            return schemaTypeMap.TryGetTypeForSchema(schema, out type);
        }

        public Item<T> DeserializeItemT<T>(ICursor cursor)
        {
            return (Item<T>)Deserialize(cursor, typeof(Item<T>));
        }

        public object Deserialize(ICursor cursor, Type objectType)
        {
            if (cursor == null || cursor.IsNull)
                return null;

            if (objectType == typeof(Item))
                return DeserializeItem(cursor);

            var obj = Construct(objectType, cursor);
            ApplyToObject(obj, cursor);
            return obj;
        }

        public object DeserializeItem(ICursor cursor)
        {
            if (cursor == null || cursor.IsNull)
                return null;

            var data = cursor.GoTo((int)ItemLayout.Data, false);
            if (data.IsNull)
            {
                return DeserializeObjectItem(cursor);        // we need the schema information from the variable field
            }

            int dataSchemaId = data.GoTo((int)VariableLayout.DataSchemaId).Get<int>();
            var dataSchema = schemaProvider.GetSchemaById(dataSchemaId);
            var dataType = schemaTypeMap.GetTypeForSchema(dataSchema);
            if (typeof(Item).GetTypeInfo().IsAssignableFrom(dataType))
            {
                return Deserialize(cursor, dataType);
            }
            else
            {
                return Deserialize(cursor, typeof(Item<>).MakeGenericType(dataType));
            }
        }

        public Item<object> DeserializeObjectItem(ICursor cursor)
        {
            if (cursor == null || cursor.IsNull)
                return null;

            Item<object> obj = (Item<object>)Construct(typeof(Item<object>), cursor);
            ApplyToObject(obj, cursor);
            return obj;
        }

        public object FromMemory(ArraySegment<byte> segment, Schema schema)
        {
            return FromCursor(new Cursor(schemaProvider, schema, segment));
        }

        public T FromMemory<T>(ArraySegment<byte> segment)
        {
            Schema schema;
            if (!schemaTypeMap.TryGetSchemaForType(typeof(T), out schema))
            {
                throw new MissingTypeMappingException(typeof(T));
            }

            return (T)FromMemory(segment, schema);
        }

        public bool IsOfType(ICursor cursor, Type type)
        {
            if (cursor.IsNull)
                return false;

            Schema schema = cursor.Schema;
            if (cursor.Schema.Id == (int)BuiltInSchema.Item)
            {
                schema = cursor.GoTo((int)ItemLayout.Data).Schema;
                if (!typeof(Item).GetTypeInfo().IsAssignableFrom(type))
                    return false;

                if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Item<>))
                    type = type.GetTypeInfo().GetGenericArguments().First();
            }

            Type mappedType;
            return schemaTypeMap.TryGetTypeForSchema(schema, out mappedType) && type.GetTypeInfo().IsAssignableFrom(mappedType);
        }

        public T CloneObject<T>(T source)
            where T : class
        {
            return (T)this.FromCursor(this.ToEditable(source));
        }
    }
}
