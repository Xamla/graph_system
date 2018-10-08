using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Xamla.Utilities;

namespace Xamla.Types.Records
{
    public sealed class NameValue
    {
        public NameValue()
        {
        }

        public NameValue(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }
        public object Value { get; set; }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Name, this.Value);
        }

        public override bool Equals(object obj)
        {
            var other = obj as NameValue;
            return other != null && object.Equals(this.Name, other.Name) && object.Equals(this.Value, other.Value);
        }
    }

    public class SchemaTypeMap
        : ISchemaTypeMap
    {
        class RangeMapping
        {
            public int MinVersion;
            public int MaxVersion;
            public Type Type;
        }

        ISchemaProvider schemaProvider;
        Dictionary<string, List<RangeMapping>> schemaBaseNameToRanges;
        Dictionary<Schema, Type> typeBySchema;
        Dictionary<Type, Schema> schemaByType;
        
        public SchemaTypeMap(ISchemaProvider schemaProvider)
        {
            this.schemaProvider = schemaProvider;
            this.typeBySchema = new Dictionary<Schema, Type>();
            this.schemaByType = new Dictionary<Type, Schema>();
            this.schemaBaseNameToRanges = new Dictionary<string, List<RangeMapping>>();

            // elementary list types
            this.MapBuiltInList<bool>(BuiltInSchema.ListOfBoolean);
            this.MapBuiltInList<int>(BuiltInSchema.ListOfInt32);
            this.MapBuiltInList<long>(BuiltInSchema.ListOfInt64);
            this.MapBuiltInList<decimal>(BuiltInSchema.ListOfDecimal);
            this.MapBuiltInList<Guid>(BuiltInSchema.ListOfGuid);
            this.MapBuiltInList<DateTime>(BuiltInSchema.ListOfDateTime);
            this.MapBuiltInList<ItemId>(BuiltInSchema.ListOfItemPath);
            this.MapBuiltInList<string>(BuiltInSchema.ListOfString);
            this.MapBuiltInList<byte[]>(BuiltInSchema.ListOfBinary);

            // DateTimeOffset, Measurement, Money, GeoPosition
            this.MapBidirectional(BuiltInSchema.DateTimeOffset, typeof(DateTimeOffset));
            this.MapBuiltInList<DateTimeOffset>(BuiltInSchema.ListOfDateTimeOffset);
            this.MapBidirectional(BuiltInSchema.Measurement, typeof(Measurement));
            this.MapBuiltInList<Measurement>(BuiltInSchema.ListOfMeasurement);
            this.MapBidirectional(BuiltInSchema.Money, typeof(Money));
            this.MapBuiltInList<Money>(BuiltInSchema.ListOfMoney);
            this.MapBidirectional(BuiltInSchema.GeoPosition, typeof(GeoPosition));
            this.MapBuiltInList<GeoPosition>(BuiltInSchema.ListOfGeoPosition);
            this.MapBidirectional(BuiltInSchema.NameValue, typeof(NameValue));
            this.MapBuiltInList<NameValue>(BuiltInSchema.ListOfNameValue);
            this.MapBidirectional(BuiltInSchema.ChoiceOption, typeof(ChoiceOption));
            this.MapBuiltInList<ChoiceOption>(BuiltInSchema.ListOfChoiceOption);
            this.MapBidirectional(BuiltInSchema.ChoiceSet, typeof(ChoiceSet));
            this.MapBuiltInList<ChoiceSet>(BuiltInSchema.ListOfChoiceSet);
            this.MapBidirectional(BuiltInSchema.Version, typeof(Version));
            this.MapBuiltInList<Version>(BuiltInSchema.ListOfVersion);
            this.MapBidirectional(BuiltInSchema.Int2, typeof(Int2));
            this.MapBuiltInList<Int2>(BuiltInSchema.ListOfInt2);
            this.MapBidirectional(BuiltInSchema.Int3, typeof(Int3));
            this.MapBuiltInList<Int3>(BuiltInSchema.ListOfInt3);
            this.MapBidirectional(BuiltInSchema.IntRect, typeof(IntRect));
            this.MapBuiltInList<IntRect>(BuiltInSchema.ListOfIntRect);
            this.MapBidirectional(BuiltInSchema.Float2, typeof(Float2));
            this.MapBuiltInList<Float2>(BuiltInSchema.ListOfFloat2);
            this.MapBidirectional(BuiltInSchema.Float3, typeof(Float3));
            this.MapBuiltInList<Float3>(BuiltInSchema.ListOfFloat3);
            this.MapBidirectional(BuiltInSchema.FloatRect, typeof(FloatRect));
            this.MapBuiltInList<FloatRect>(BuiltInSchema.ListOfFloatRect);

            // item sub-structures...
            this.MapBidirectional(BuiltInSchema.ItemFile, typeof(ItemFile));
            this.MapBidirectional(BuiltInSchema.ItemLink, typeof(ItemLink));
            this.MapBidirectional(BuiltInSchema.ItemStatistics, typeof(ItemStatistics));

            this.MapBuiltInList<ItemFile>(BuiltInSchema.ListOfItemFile);
            this.MapBuiltInList<ItemLink>(BuiltInSchema.ListOfItemLink);
            this.MapBuiltInList<ItemStatistics>(BuiltInSchema.ListOfItemStats);

            // non-specific Item<object> mapping
            // (please use IItemFactory.CreateItem<T>() or ICacheSerializer.DeserializeItem<T>() to work with statically typed data fields of Item<T> objects)
            this.MapBidirectional(BuiltInSchema.Item, typeof(Item<object>));
            this.MapBuiltInList<Item<object>>(BuiltInSchema.ListOfItem);

            this.MapBidirectional(BuiltInSchema.Folder, typeof(Folder));
            this.MapBuiltInList<Folder>(BuiltInSchema.ListOfFolder);

            this.MapBuiltInList<object>(BuiltInSchema.ListOfVariable);
        }

        void MapBuiltInList<T>(BuiltInSchema schema)
        {
            this.MapListOf<T>(Schema.BuiltIn[schema]);
        }

        public ISchemaProvider SchemaProvider
        {
            get { return schemaProvider; }
        }

        public void MapSchema(Schema schema, Type type)
        {
            lock (this)
            {
                typeBySchema.Add(schema, type);
            }
        }

        public void MapType(Type type, Schema schema)
        {
            lock (this)
            {
                schemaByType.Add(type, schema);
            }
        }

        public void UnmapSchema(Schema schema)
        {
            lock (this)
            {
                typeBySchema.Remove(schema);
            }
        }

        public void UnmapType(Type type)
        {
            lock (this)
            {
                schemaByType.Remove(type);
            }
        }

        string GetSchemaBaseName(string name, out int version)
        {
            version = -1;

            int dot = name.LastIndexOf('.');
            if (dot >= 0)
            {
                string baseName = name.Substring(0, dot);
                if (int.TryParse(name.Substring(dot + 1), out version))
                {
                    return baseName;
                }
            }

            return name;
        }

        Type FindRangeMappedTypeForSchema(Schema schema)
        {
            int version;
            string baseName = GetSchemaBaseName(schema.Name, out version);
            List<RangeMapping> rangeMappingList;
            if (schemaBaseNameToRanges.TryGetValue(baseName, out rangeMappingList))
            {
                var mapping = rangeMappingList.FirstOrDefault(x => x.MinVersion <= version && x.MaxVersion >= version);
                if (mapping != null)
                {
                    return mapping.Type;
                }
            }

            return null;
        }

        bool IsPrimitiveDataType(DataType type)
        {
            return (int)type < (int)DataType.Choice;
        }

        public bool TryGetTypeForSchema(Schema schema, out Type type)
        {
            if (schema.DataType == DataType.MultiChoice)
            {
                type = typeof(MultiChoice);
                return true;
            }

            if (IsPrimitiveDataType(schema.DataType))
            {
                return EditablePrimitive.TryGetTypeForPrimitiveSchema(schema, out type);
            }

            lock (this)
            {
                if (typeBySchema.TryGetValue(schema, out type))
                    return true;

                // fallback to range mappings
                if (schema.DataType == DataType.List)
                {
                    var elementType = FindRangeMappedTypeForSchema(schema.Fields.First().Schema);
                    if (elementType != null)
                    {
                        type = typeof(List<>).MakeGenericType(elementType);
                        return true;
                    }
                }
                else
                {
                    type = FindRangeMappedTypeForSchema(schema);
                    if (type != null)
                        return true;
                }
            }

            return false;
        }

        public bool TryGetSchemaForType(Type type, out Schema schema)
        {
            lock (this)
            {
                if (schemaByType.TryGetValue(type, out schema))
                    return true;
            }

            return EditablePrimitive.TryGetPrimitiveSchemaForType(type, out schema);
        }

        public void MapListOf<T>(Schema listSchema)
        {
            if (listSchema.DataType != DataType.List)
                throw new ArgumentException("List schema expected");

            this.MapBidirectional(listSchema, typeof(List<T>));
            this.MapType(typeof(IList<T>), listSchema);
            this.MapType(typeof(IEnumerable<T>), listSchema);
        }

        public void UnmapListOf<T>(Schema listSchema)
        {
            if (listSchema.DataType != DataType.List)
                throw new ArgumentException("List schema expected");

            this.UnmapSchema(listSchema);
            this.UnmapType(typeof(T));
            this.UnmapType(typeof(IList<T>));
            this.UnmapType(typeof(IEnumerable<T>));
        }

        public string GetSchemaBaseName(Type type)
        {
            var recordAttribute = type.GetTypeInfo().GetCustomAttribute<RecordAttribute>();
            if (recordAttribute != null && recordAttribute.SchemaBaseName != null)
            {
                return recordAttribute.SchemaBaseName;
            }
            return type.FullName;
        }

        public string GetSchemaName(Type type, int? version = null)
        {
            var recordAttribute = type.GetTypeInfo().GetCustomAttribute<RecordAttribute>();
            string baseName = null;

            if (recordAttribute != null)
            {
                if (!version.HasValue)
                {
                    version = recordAttribute.Version;
                }
                baseName = recordAttribute.SchemaBaseName;
            }

            if (string.IsNullOrEmpty(baseName))
            {
                // list schema name
                var enumerableGeneric = type.GetGenericTypeBase(typeof(IEnumerable<>));
                if (enumerableGeneric != null)
                {
                    var elementType = enumerableGeneric.GetGenericArguments()[0];
                    return string.Format("List<{0}>", GetSchemaName(elementType, version));
                }
            }

            return string.Format("{0}.{1}", baseName ?? type.FullName, version ?? 1);
        }

        public IEnumerable<Type> FindTypesWithRecordAttribue(Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.GetTypeInfo().IsDefined(typeof(RecordAttribute)));
        }

        public void AutoMapType(Type type)
        {
            lock (this)
            {
                var recordAttribute = type.GetTypeInfo().GetCustomAttribute<RecordAttribute>();

                // primary mapping
                var schema = schemaProvider.GetSchemaByName(GetSchemaName(type));
                if (recordAttribute.MapBidirectional)
                {
                    this.MapBidirectional(schema, type);
                }
                else
                {
                    this.MapSchema(schema, type);
                }

                // list mappings
                if (recordAttribute.MapList)
                {
                    string listName = string.Format("List<{0}>", schema.Name);
                    Schema listSchema;
                    if (schemaProvider.TryGetSchemaByName(listName, out listSchema))
                    {
                        if (recordAttribute.MapBidirectional)
                        {
                            this.MapBidirectional(listSchema, typeof(List<>).MakeGenericType(type));
                            this.MapType(typeof(IList<>).MakeGenericType(type), listSchema);
                            this.MapType(typeof(IEnumerable<>).MakeGenericType(type), listSchema);
                        }
                        else
                        {
                            this.MapSchema(listSchema, typeof(List<>).MakeGenericType(type));
                        }
                    }
                }

                // range mapping
                if (recordAttribute.MinVersion.HasValue)
                {
                    var baseName = this.GetSchemaBaseName(type);

                    List<RangeMapping> rangeMappingList;
                    if (!schemaBaseNameToRanges.TryGetValue(baseName, out rangeMappingList))
                    {
                        rangeMappingList = new List<RangeMapping>();
                        schemaBaseNameToRanges.Add(baseName, rangeMappingList);
                    }

                    rangeMappingList.Add(new RangeMapping { MinVersion = recordAttribute.MinVersion.Value, MaxVersion = recordAttribute.Version, Type = type });
                }
            }
        }

        public void AutoMapTypesWithRecordAttribute(Assembly assembly)
        {
            lock (this)
            {
                foreach (var type in FindTypesWithRecordAttribue(assembly))
                {
                    Schema existingMapping;
                    if (this.TryGetSchemaForType(type, out existingMapping))       // skip auto-mapping if type was mapped before
                        continue;

                    AutoMapType(type);
                }
            }
        }
    }
}
