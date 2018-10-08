using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public enum BuiltInSchema
    {
        // fixed size pimitives
        Void = 0,
        Boolean = 2,
        Int32 = 4,
        Int64 = 6,
        Float64 = 8,
        Decimal = 10,
        DateTime = 12,
        TimeSpan = 14,
        Guid = 16,

        // variable size primitives
        String = 18,
        Binary = 20,
        ItemPath = 22,

        // elementary classes
        DateTimeOffset = 24,
        Money = 26,
        Measurement = 28,
        GeoPosition = 30,
        Variable = 32,
        Version = 34,

        // Choice Description
        ChoiceOption = 36,
        ChoiceSet = 38,

        // item
        ItemFile = 40,
        //ItemBlob = 42,
        //ItemText = 44,
        ItemLink = 46,
        ItemStatistics = 48,
        Item = 50,

        // schema
        Field = 52,
        Schema = 54,

        // query
        SortField = 56,
        Comparison = 58,
        LogicGroup = 60,
        NameValue = 62,
        AggregationTask = 64,
        CacheQuery = 66,
        AggregationResult = 68,
        CacheResponse = 70,
        Pair = 72,
        HistogramBin = 74,
        Histogram = 76,
        Folder = 78,

        Int2 = 80, 
        Int3 = 82,
        IntRect = 84,
        Float2 = 86,
        Float3 = 88, 
        FloatRect = 90,

        // list types
        ListOfVoid = 1,
        ListOfBoolean = 3,
        ListOfInt32 = 5,
        ListOfInt64 = 7,
        ListOfFloat64 = 9,
        ListOfDecimal = 11,
        ListOfDateTime = 13,
        ListOfTimeSpan = 15,
        ListOfGuid = 17,
        ListOfString = 19,
        ListOfBinary = 21,
        ListOfItemPath = 23,
        ListOfDateTimeOffset = 25,
        ListOfMoney = 27,
        ListOfMeasurement = 29,
        ListOfGeoPosition = 31,
        ListOfVariable = 33,
        ListOfVersion = 35,
        ListOfChoiceOption = 37,
        ListOfChoiceSet = 39,
        ListOfItemFile = 41,
        ListOfItemBlob = 43,
        ListOfItemText = 45,
        ListOfItemLink = 47,
        ListOfItemStats = 49,
        ListOfItem = 51,
        ListOfField = 53,
        ListOfSchema = 55,
        ListOfSortField = 57,
        ListOfComparision = 59,
        ListOfLogicGroup = 61,
        ListOfNameValue = 63,
        ListOfAggregationTask = 65,
        ListOfQuery = 67,
        ListOfAggregationResult = 69,
        ListOfCacheResponse = 71,
        ListOfPair = 73,
        ListOfHistogramBin = 75,
        ListOfHistogram = 77,
        ListOfFolder = 79,
        ListOfInt2 = 81, 
        ListOfInt3 = 83,
        ListOfIntRect = 85,
        ListOfFloat2 = 87,
        ListOfFloat3 = 89, 
        ListOfFloatRect = 91
    }

    internal enum VariableLayout
    {
        DataSchemaId = 0,
        Data = 1
    }

    [Flags]
    public enum ItemFlags
    {
        None = 0,

        Original = 0x1,
        CountAsChild = 0x2,
        Propagate = 0x4,
        Hidden = 0x8,
        Cachable = 0x10,

        HasFile = 0x40,
        HasLink = 0x80,
        HasChildren = 0x100,
        IsContainer = 0x200,
        Modified = 0x400,
        AutoStorageSize = 0x800,

        FetchFile = 0x1000,
        FetchLink = 0x2000,
    }

    public enum ItemLayout
    {
        Flags = 0,
        Revision = 1,
        Id = 2,
        OriginalItemId = 3,
        Name = 4,
        Data = 5,
        Files = 6,
        Links = 7,
        Statistics = 8,
        Children = 9
    }

    public class BuiltInSchemas
        : ISchemaProvider
    {
        public const int HIGHEST_ID = 91;
        private static Schema[] builtInSchemas = new Schema[HIGHEST_ID + 1];
        private static Dictionary<string, Schema> builtInSchemasByName = new Dictionary<string, Schema>(HIGHEST_ID + 1);

        public static readonly Schema Void;
        public static readonly Schema Boolean;
        public static readonly Schema Int32;
        public static readonly Schema Int64;
        public static readonly Schema Float64;
        public static readonly Schema Decimal;
        public static readonly Schema DateTime;
        public static readonly Schema TimeSpan;
        public static readonly Schema Guid;

        // variable size primitives
        public static readonly Schema String;
        public static readonly Schema Binary;
        public static readonly Schema ItemPath;

        // elementary classes
        public static readonly Schema DateTimeOffset;
        public static readonly Schema Money;
        public static readonly Schema Measurement;
        public static readonly Schema GeoPosition;
        public static readonly Schema Variable;
        public static readonly Schema Version;
        
        // Choice Description
        public static readonly Schema ChoiceOption;
        public static readonly Schema ChoiceSet;
         
        // item        
        public static readonly Schema ItemFile;
        public static readonly Schema ItemLink;
        public static readonly Schema ItemStatistics;
        public static readonly Schema ItemSchema;           // name 'Item' conflicts with indexer
        
        // schema
        public static readonly Schema Field;
        public static readonly Schema Schema;
        
        // query
        public static readonly Schema SortField;
        public static readonly Schema Comparison;
        public static readonly Schema LogicGroup;
        public static readonly Schema NameValue;
        public static readonly Schema AggregationTask;
        public static readonly Schema CacheQuery;
        public static readonly Schema AggregationResult;
        public static readonly Schema CacheResponse;
        public static readonly Schema Pair;
        public static readonly Schema HistogramBin;
        public static readonly Schema Histogram;
        public static readonly Schema Folder;

        // elementary math types
        public static readonly Schema Int2;
        public static readonly Schema Int3;
        public static readonly Schema IntRect;
        public static readonly Schema Float2;
        public static readonly Schema Float3;
        public static readonly Schema FloatRect;

        // list types
        public static readonly Schema ListOfVoid;
        public static readonly Schema ListOfBoolean;
        public static readonly Schema ListOfInt32;
        public static readonly Schema ListOfInt64;
        public static readonly Schema ListOfFloat64;
        public static readonly Schema ListOfDecimal;
        public static readonly Schema ListOfDateTime;
        public static readonly Schema ListOfTimeSpan;
        public static readonly Schema ListOfGuid;
        public static readonly Schema ListOfString;
        public static readonly Schema ListOfBinary;
        public static readonly Schema ListOfItemPath;
        public static readonly Schema ListOfDateTimeOffset;
        public static readonly Schema ListOfMoney;
        public static readonly Schema ListOfMeasurement;
        public static readonly Schema ListOfGeoPosition;
        public static readonly Schema ListOfVariable;
        public static readonly Schema ListOfVersion;
        public static readonly Schema ListOfChoiceOption;
        public static readonly Schema ListOfChoiceSet;
        public static readonly Schema ListOfItemFile;
        public static readonly Schema ListOfItemBlob;
        public static readonly Schema ListOfItemText;
        public static readonly Schema ListOfItemLink;
        public static readonly Schema ListOfItemStats;
        public static readonly Schema ListOfItem;
        public static readonly Schema ListOfField;
        public static readonly Schema ListOfSchema;
        public static readonly Schema ListOfSortField;
        public static readonly Schema ListOfComparision;
        public static readonly Schema ListOfLogicGroup;
        public static readonly Schema ListOfNameValue;
        public static readonly Schema ListOfAggregationTask;
        public static readonly Schema ListOfQuery;
        public static readonly Schema ListOfAggregationResult;
        public static readonly Schema ListOfCacheResponse;
        public static readonly Schema ListOfPair;
        public static readonly Schema ListOfHistogramBin;
        public static readonly Schema ListOfHistogram;
        public static readonly Schema ListOfFolder;
        public static readonly Schema ListOfInt2;
        public static readonly Schema ListOfInt3;
        public static readonly Schema ListOfIntRect;
        public static readonly Schema ListOfFloat2;
        public static readonly Schema ListOfFloat3;
        public static readonly Schema ListOfFloatRect;

        public Schema this[int schemaId]
        {
            get { return GetSchemaById(schemaId); }
        }

        public Schema this[BuiltInSchema schema]
        {
            get { return GetSchemaById((int)schema); }
        }

        public Schema this[string schemaName]
        {
            get { return GetSchemaByName(schemaName); }
        }

        public Schema GetSchemaById(int schemaId)
        {
            return builtInSchemas[schemaId];
        }

        public Schema GetSchemaByName(string schemaName)
        {
            Schema result;
            if (!TryGetSchemaByName(schemaName, out result))
                throw new SchemaNotFoundException(schemaName);
            return result;
        }

        public bool TryGetSchemaById(int schemaId, out Schema result)
        {
            if (schemaId < 0 || schemaId >= builtInSchemas.Length)
                result = null;
            else
                result = builtInSchemas[schemaId];

            return result != null;
        }

        public bool TryGetSchemaByName(string schemaName, out Schema result)
        {
            return builtInSchemasByName.TryGetValue(schemaName, out result);
        }

        public IEnumerable<Schema> GetAll()
        {
            return builtInSchemas.Where(x => x != null);
        }

        private static Schema CreateListSchema(BuiltInSchema listType, BuiltInSchema itemType, bool itemNullable = false)
        {
            return new Schema
            {
                Id = (int)listType,
                Name = string.Format("List<{0}>", itemType),
                DataType = DataType.List,
                VariableSizeOffset = 0,
                fields = new Field[] { 
                    new Field { Schema = builtInSchemas[(int)itemType], Nullable = itemNullable },
                }
            };
        }

        static BuiltInSchemas()
        {
            builtInSchemas[(int)BuiltInSchema.Void] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Void,
                    Name = "Void",
                    DataType = DataType.Void
                };

            builtInSchemas[(int)BuiltInSchema.Boolean] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Boolean,
                    Name = "Boolean",
                    DataType = DataType.Boolean,
                };

            builtInSchemas[(int)BuiltInSchema.Int32] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Int32,
                    Name = "Int32",
                    DataType = DataType.Int32,
                };

            builtInSchemas[(int)BuiltInSchema.Int64] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Int64,
                    Name = "Int64",
                    DataType = DataType.Int64,
                };

            builtInSchemas[(int)BuiltInSchema.Float64] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Float64,
                    Name = "Float64",
                    DataType = DataType.Float64,
                };

            builtInSchemas[(int)BuiltInSchema.Decimal] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Decimal,
                    Name = "Decimal",
                    DataType = DataType.Decimal,
                };

            builtInSchemas[(int)BuiltInSchema.DateTime] =
                new Schema
                {
                    Id = (int)BuiltInSchema.DateTime,
                    Name = "DateTime",
                    DataType = DataType.DateTime,
                };

            builtInSchemas[(int)BuiltInSchema.TimeSpan] =
                new Schema
                {
                    Id = (int)BuiltInSchema.TimeSpan,
                    Name = "TimeSpan",
                    DataType = DataType.TimeSpan,
                };

            builtInSchemas[(int)BuiltInSchema.Guid] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Guid,
                    Name = "Guid",
                    DataType = DataType.Guid,
                };

            builtInSchemas[(int)BuiltInSchema.String] =
                new Schema
                {
                    Id = (int)BuiltInSchema.String,
                    Name = "String",
                    DataType = DataType.String,
                    VariableSizeOffset = 0
                };

            builtInSchemas[(int)BuiltInSchema.Binary] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Binary,
                    Name = "Binary",
                    DataType = DataType.Binary,
                    VariableSizeOffset = 0
                };

            builtInSchemas[(int)BuiltInSchema.ItemPath] =
                new Schema
                {
                    Id = (int)BuiltInSchema.ItemPath,
                    Name = "ItemPath",
                    DataType = DataType.ItemPath,
                    VariableSizeOffset = 0
                };

            builtInSchemas[(int)BuiltInSchema.DateTimeOffset] =
                new Schema
                {
                    Id = (int)BuiltInSchema.DateTimeOffset,
                    Name = "DateTimeOffset",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Time", Schema = builtInSchemas[(int)BuiltInSchema.DateTime] },
                        new Field { Name = "Offset", Schema = builtInSchemas[(int)BuiltInSchema.TimeSpan] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.Money] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Money,
                    Name = "Money",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Amount", Schema = builtInSchemas[(int)BuiltInSchema.Decimal] },
                        new Field { Name = "Currency", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 64 }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.Measurement] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Measurement,
                    Name = "Measurement",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Value", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Unit", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 64 }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.GeoPosition] =
                new Schema
                {
                    Id = (int)BuiltInSchema.GeoPosition,
                    Name = "GeoPosition",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Longitude", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Latitude", Schema = builtInSchemas[(int)BuiltInSchema.Float64] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.Variable] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Variable,
                    Name = "Variable",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "DataSchemaId", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Data", Schema = builtInSchemas[(int)BuiltInSchema.Binary], Nullable = true }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.Version] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Version,
                    Name = "Version",
                    DataType = DataType.Class,
                    fields = new Field[] {
                        new Field { Name = "Major", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Minor", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Build", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Revision", Schema = builtInSchemas[(int)BuiltInSchema.Int32] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfBoolean] = CreateListSchema(BuiltInSchema.ListOfBoolean, BuiltInSchema.Boolean);
            builtInSchemas[(int)BuiltInSchema.ListOfInt32] = CreateListSchema(BuiltInSchema.ListOfInt32, BuiltInSchema.Int32);
            builtInSchemas[(int)BuiltInSchema.ListOfInt64] = CreateListSchema(BuiltInSchema.ListOfInt64, BuiltInSchema.Int64);
            builtInSchemas[(int)BuiltInSchema.ListOfFloat64] = CreateListSchema(BuiltInSchema.ListOfFloat64, BuiltInSchema.Float64);
            builtInSchemas[(int)BuiltInSchema.ListOfDecimal] = CreateListSchema(BuiltInSchema.ListOfDecimal, BuiltInSchema.Decimal);
            builtInSchemas[(int)BuiltInSchema.ListOfDateTime] = CreateListSchema(BuiltInSchema.ListOfDateTime, BuiltInSchema.DateTime);
            builtInSchemas[(int)BuiltInSchema.ListOfTimeSpan] = CreateListSchema(BuiltInSchema.ListOfTimeSpan, BuiltInSchema.TimeSpan);
            builtInSchemas[(int)BuiltInSchema.ListOfGuid] = CreateListSchema(BuiltInSchema.ListOfGuid, BuiltInSchema.Guid);
            builtInSchemas[(int)BuiltInSchema.ListOfString] = CreateListSchema(BuiltInSchema.ListOfString, BuiltInSchema.String);
            builtInSchemas[(int)BuiltInSchema.ListOfBinary] = CreateListSchema(BuiltInSchema.ListOfBinary, BuiltInSchema.Binary);
            builtInSchemas[(int)BuiltInSchema.ListOfItemPath] = CreateListSchema(BuiltInSchema.ListOfItemPath, BuiltInSchema.ItemPath);
            builtInSchemas[(int)BuiltInSchema.ListOfDateTimeOffset] = CreateListSchema(BuiltInSchema.ListOfDateTimeOffset, BuiltInSchema.DateTimeOffset);
            builtInSchemas[(int)BuiltInSchema.ListOfMoney] = CreateListSchema(BuiltInSchema.ListOfMoney, BuiltInSchema.Money);
            builtInSchemas[(int)BuiltInSchema.ListOfMeasurement] = CreateListSchema(BuiltInSchema.ListOfMeasurement, BuiltInSchema.Measurement);
            builtInSchemas[(int)BuiltInSchema.ListOfGeoPosition] = CreateListSchema(BuiltInSchema.ListOfGeoPosition, BuiltInSchema.GeoPosition);
            builtInSchemas[(int)BuiltInSchema.ListOfVariable] = CreateListSchema(BuiltInSchema.ListOfVariable, BuiltInSchema.Variable);
            builtInSchemas[(int)BuiltInSchema.ListOfVersion] = CreateListSchema(BuiltInSchema.ListOfVersion, BuiltInSchema.Version);

            builtInSchemas[(int)BuiltInSchema.ChoiceOption] =
                new Schema
                {
                    Id = (int)BuiltInSchema.ChoiceOption,
                    Name = "ChoiceOption",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Value", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Order", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Hidden", Schema = builtInSchemas[(int)BuiltInSchema.Boolean] },
                        new Field { Name = "Name", Schema = builtInSchemas[(int)BuiltInSchema.String] },
                        new Field { Name = "Caption", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true },
                        new Field { Name = "Description", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true },
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfChoiceOption] = CreateListSchema(BuiltInSchema.ListOfChoiceOption, BuiltInSchema.ChoiceOption);

            builtInSchemas[(int)BuiltInSchema.ChoiceSet] =
                new Schema
                {
                    Id = (int)BuiltInSchema.ChoiceSet,
                    Name = "ChoiceSet",
                    DataType = DataType.Class,
                    fields = new Field[] {
                        new Field { Name = "Caption", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true },
                        new Field { Name = "Description", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true },
                        new Field { Name = "Defaults", Schema = builtInSchemas[(int)BuiltInSchema.ListOfInt32] },
                        new Field { Name = "Options", Schema = builtInSchemas[(int)BuiltInSchema.ListOfChoiceOption] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfChoiceSet] = CreateListSchema(BuiltInSchema.ListOfChoiceSet, BuiltInSchema.ChoiceSet);

            builtInSchemas[(int)BuiltInSchema.ItemFile] =
                new Schema
                {
                    Id = (int)BuiltInSchema.ItemFile,
                    Name = "ItemFile",
                    DataType = DataType.Class,
                    fields = new Field[] {
                        new Field { Name = "Id", Schema = builtInSchemas[(int)BuiltInSchema.Guid] },
                        new Field { Name = "Size", Schema = builtInSchemas[(int)BuiltInSchema.Int64] },
                        new Field { Name = "Uri", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 300 }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfItemFile] = CreateListSchema(BuiltInSchema.ListOfItemFile, BuiltInSchema.ItemFile);

            builtInSchemas[(int)BuiltInSchema.ItemLink] =
                new Schema
                {
                    Id = (int)BuiltInSchema.ItemLink,
                    Name = "ItemLink",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "LinkType", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Weight", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "FromId", Schema = builtInSchemas[(int)BuiltInSchema.ItemPath], MaxLength = 1024 },
                        new Field { Name = "ToId", Schema = builtInSchemas[(int)BuiltInSchema.ItemPath], MaxLength = 1024 }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfItemLink] = CreateListSchema(BuiltInSchema.ListOfItemLink, BuiltInSchema.ItemLink);

            builtInSchemas[(int)BuiltInSchema.ItemStatistics] =
                new Schema
                {
                    Id = (int)BuiltInSchema.ItemStatistics,
                    Name = "ItemStats",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Children", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Subtree", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "StorageSize", Schema = builtInSchemas[(int)BuiltInSchema.Int64] },
                        new Field { Name = "SubtreeStorageSize", Schema = builtInSchemas[(int)BuiltInSchema.Int64] },
                        new Field { Name = "_", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true }       // ensure variable size
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfItemStats] = CreateListSchema(BuiltInSchema.ListOfItemStats, BuiltInSchema.ItemStatistics);

            builtInSchemas[(int)BuiltInSchema.Item] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Item,
                    Name = "Item",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Flags", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Revision", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Id", Schema = builtInSchemas[(int)BuiltInSchema.ItemPath], MaxLength = 1024, Nullable = true },
                        new Field { Name = "OriginalItemId", Schema = builtInSchemas[(int)BuiltInSchema.ItemPath], MaxLength = 1024, Nullable = true },
                        new Field { Name = "Name", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true, MaxLength = 127 },
                        new Field { Name = "Data", Schema = builtInSchemas[(int)BuiltInSchema.Variable], Nullable = true, MaxLength = 1024 * 1024 },
                        new Field { Name = "Files", Schema = builtInSchemas[(int)BuiltInSchema.ListOfItemFile], Nullable = true },
                        new Field { Name = "Links", Schema = builtInSchemas[(int)BuiltInSchema.ListOfItemLink], Nullable = true },
                        new Field { Name = "Statistics", Schema = builtInSchemas[(int)BuiltInSchema.ItemStatistics], Nullable = true },
                        new Field { Name = "Children", Schema = builtInSchemas[(int)BuiltInSchema.ListOfVariable], Nullable = true }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfItem] = CreateListSchema(BuiltInSchema.ListOfItem, BuiltInSchema.Item, true);

            builtInSchemas[(int)BuiltInSchema.Field] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Field,
                    Name = "Field",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "SchemaId", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Nullable", Schema = builtInSchemas[(int)BuiltInSchema.Boolean] },
                        new Field { Name = "MaxLength", Schema = builtInSchemas[(int)BuiltInSchema.Int32], Nullable = true },
                        new Field { Name = "Name", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true, MaxLength = 127 },
                        new Field { Name = "Caption", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true, MaxLength = 256 },
                        new Field { Name = "Description", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true, MaxLength = 2048 }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfField] = CreateListSchema(BuiltInSchema.ListOfField, BuiltInSchema.Field);

            builtInSchemas[(int)BuiltInSchema.Schema] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Schema,
                    Name = "Schema",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Id", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "DataType", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Name", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 127 },
                        new Field { Name = "Fields", Schema = builtInSchemas[(int)BuiltInSchema.ListOfField], Nullable = true },
                        new Field { Name = "Description", Schema = builtInSchemas[(int)BuiltInSchema.String], Nullable = true, MaxLength = 1024 },
                        new Field { Name = "DeclarationItem", Schema = builtInSchemas[(int)BuiltInSchema.Item], Nullable = true }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfSchema] = CreateListSchema(BuiltInSchema.ListOfSchema, BuiltInSchema.Schema);

            // query
            builtInSchemas[(int)BuiltInSchema.SortField] =
                new Schema
                {
                    Id = (int)BuiltInSchema.SortField,
                    Name = "SortField",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Direction", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Path", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 1024 }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfSortField] = CreateListSchema(BuiltInSchema.ListOfSortField, BuiltInSchema.SortField);

            builtInSchemas[(int)BuiltInSchema.Comparison] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Comparison,
                    Name = "Comparison",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Op", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Left", Schema = builtInSchemas[(int)BuiltInSchema.String] },
                        new Field { Name = "Right", Schema = builtInSchemas[(int)BuiltInSchema.Variable] },
                        new Field { Name = "Accessor", Schema = builtInSchemas[(int)BuiltInSchema.Variable], Nullable = true }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfComparision] = CreateListSchema(BuiltInSchema.ListOfComparision, BuiltInSchema.Comparison);

            builtInSchemas[(int)BuiltInSchema.LogicGroup] =
                new Schema
                {
                    Id = (int)BuiltInSchema.LogicGroup,
                    Name = "LogicGroup",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Op", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Conditions", Schema = builtInSchemas[(int)BuiltInSchema.ListOfVariable] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfLogicGroup] = CreateListSchema(BuiltInSchema.ListOfLogicGroup, BuiltInSchema.LogicGroup);

            builtInSchemas[(int)BuiltInSchema.NameValue] =
                new Schema
                {
                    Id = (int)BuiltInSchema.NameValue,
                    Name = "NameValue",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Name", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 127 },
                        new Field { Name = "Value", Schema = builtInSchemas[(int)BuiltInSchema.Variable] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfNameValue] = CreateListSchema(BuiltInSchema.ListOfNameValue, BuiltInSchema.NameValue);

            builtInSchemas[(int)BuiltInSchema.AggregationTask] =
                new Schema
                {
                    Id = (int)BuiltInSchema.AggregationTask,
                    Name = "AggregationTask",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                            new Field { Name = "Depth", Schema = builtInSchemas[(int)BuiltInSchema.Int32], Nullable = true },
                            new Field { Name = "Name", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 128, Nullable = true },
                            new Field { Name = "Path", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 256 },
                            new Field { Name = "Method", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 128 },
                            new Field { Name = "Arguments", Schema = builtInSchemas[(int)BuiltInSchema.ListOfNameValue] }
                        }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfAggregationTask] = CreateListSchema(BuiltInSchema.ListOfAggregationTask, BuiltInSchema.AggregationTask);

            builtInSchemas[(int)BuiltInSchema.CacheQuery] =
                new Schema
                {
                    Id = (int)BuiltInSchema.CacheQuery,
                    Name = "CacheQuery",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Flags", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Depth", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "ResultSetId", Schema = builtInSchemas[(int)BuiltInSchema.Guid], Nullable = true },
                        new Field { Name = "TotalCount", Schema = builtInSchemas[(int)BuiltInSchema.Int32], Nullable = true },
                        new Field { Name = "ViewOffset", Schema = builtInSchemas[(int)BuiltInSchema.Int32], Nullable = true },
                        new Field { Name = "ViewCount", Schema = builtInSchemas[(int)BuiltInSchema.Int32], Nullable = true },
                        new Field { Name = "Source", Schema = builtInSchemas[(int)BuiltInSchema.ItemPath], MaxLength = 1024 },
                        new Field { Name = "Aggregates", Schema = builtInSchemas[(int)BuiltInSchema.ListOfAggregationTask], Nullable = true, MaxLength =128 },
                        new Field { Name = "Where", Schema = builtInSchemas[(int)BuiltInSchema.Variable], Nullable = true, MaxLength = 64 * 1024  },
                        new Field { Name = "SortBy", Schema = builtInSchemas[(int)BuiltInSchema.ListOfSortField], Nullable = true, MaxLength = 128 }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfQuery] = CreateListSchema(BuiltInSchema.ListOfQuery, BuiltInSchema.CacheQuery);

            builtInSchemas[(int)BuiltInSchema.AggregationResult] =
                new Schema
                {
                    Id = (int)BuiltInSchema.AggregationResult,
                    Name = "AggregationResult",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Name", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 128, Nullable = true },
                        new Field { Name = "Path", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 256 },
                        new Field { Name = "Method", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 128 },
                        new Field { Name = "Value", Schema = builtInSchemas[(int)BuiltInSchema.Variable] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfAggregationResult] = CreateListSchema(BuiltInSchema.ListOfAggregationResult, BuiltInSchema.AggregationResult);

            builtInSchemas[(int)BuiltInSchema.CacheResponse] =
                new Schema
                {
                    Id = (int)BuiltInSchema.CacheResponse,
                    Name = "CacheResponse",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Query", Schema = builtInSchemas[(int)BuiltInSchema.CacheQuery] },
                        new Field { Name = "Item", Schema = builtInSchemas[(int)BuiltInSchema.Item], Nullable = true },
                        new Field { Name = "Children", Schema = builtInSchemas[(int)BuiltInSchema.ListOfItem], Nullable = true },
                        new Field { Name = "Trace", Schema = builtInSchemas[(int)BuiltInSchema.ListOfItem], Nullable = true },
                        new Field { Name = "Aggregates", Schema = builtInSchemas[(int)BuiltInSchema.ListOfAggregationResult], Nullable = true }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfCacheResponse] = CreateListSchema(BuiltInSchema.ListOfCacheResponse, BuiltInSchema.CacheResponse);

            builtInSchemas[(int)BuiltInSchema.Pair] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Pair,
                    Name = "Pair",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Value1", Schema = builtInSchemas[(int)BuiltInSchema.Variable] },
                        new Field { Name = "Value2", Schema = builtInSchemas[(int)BuiltInSchema.Variable] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfPair] = CreateListSchema(BuiltInSchema.ListOfPair, BuiltInSchema.Pair);

            builtInSchemas[(int)BuiltInSchema.HistogramBin] =
                new Schema
                {
                    Id = (int)BuiltInSchema.HistogramBin,
                    Name = "HistogramBin",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "LowerBound", Schema = builtInSchemas[(int)BuiltInSchema.Variable] },
                        new Field { Name = "UpperBound", Schema = builtInSchemas[(int)BuiltInSchema.Variable], Nullable = true },
                        new Field { Name = "Count", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Children", Schema = builtInSchemas[(int)BuiltInSchema.Variable], Nullable = true }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfHistogramBin] = CreateListSchema(BuiltInSchema.ListOfHistogramBin, BuiltInSchema.HistogramBin);

            builtInSchemas[(int)BuiltInSchema.Histogram] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Histogram,
                    Name = "Histogram",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Type", Schema = builtInSchemas[(int)BuiltInSchema.String] },
                        new Field { Name = "Granularity", Schema = builtInSchemas[(int)BuiltInSchema.Variable] },
                        new Field { Name = "Bins", Schema = builtInSchemas[(int)BuiltInSchema.ListOfHistogramBin] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfHistogram] = CreateListSchema(BuiltInSchema.ListOfHistogram, BuiltInSchema.Histogram);

            builtInSchemas[(int)BuiltInSchema.Folder] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Folder,
                    Name = "Folder",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Type", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 1024, Nullable = true },
                        new Field { Name = "Description", Schema = builtInSchemas[(int)BuiltInSchema.String], MaxLength = 32 * 1024, Nullable = true },
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfFolder] = CreateListSchema(BuiltInSchema.ListOfFolder, BuiltInSchema.Folder);

            builtInSchemas[(int)BuiltInSchema.Int2] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Int2,
                    Name = "Int2",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "X", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Y", Schema = builtInSchemas[(int)BuiltInSchema.Int32] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfInt2] = CreateListSchema(BuiltInSchema.ListOfInt2, BuiltInSchema.Int2);

            builtInSchemas[(int)BuiltInSchema.Int3] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Int3,
                    Name = "Int3",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "X", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Y", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Z", Schema = builtInSchemas[(int)BuiltInSchema.Int32] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfInt3] = CreateListSchema(BuiltInSchema.ListOfInt3, BuiltInSchema.Int3);
            
            builtInSchemas[(int)BuiltInSchema.IntRect] =
                new Schema
                {
                    Id = (int)BuiltInSchema.IntRect,
                    Name = "IntRect",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Left", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Top", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Right", Schema = builtInSchemas[(int)BuiltInSchema.Int32] },
                        new Field { Name = "Bottom", Schema = builtInSchemas[(int)BuiltInSchema.Int32] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfIntRect] = CreateListSchema(BuiltInSchema.ListOfIntRect, BuiltInSchema.IntRect);

            builtInSchemas[(int)BuiltInSchema.Float2] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Float2,
                    Name = "Float2",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "X", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Y", Schema = builtInSchemas[(int)BuiltInSchema.Float64] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfFloat2] = CreateListSchema(BuiltInSchema.ListOfFloat2, BuiltInSchema.Float2);

            builtInSchemas[(int)BuiltInSchema.Float3] =
                new Schema
                {
                    Id = (int)BuiltInSchema.Float3,
                    Name = "Float3",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "X", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Y", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Z", Schema = builtInSchemas[(int)BuiltInSchema.Float64] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfFloat3] = CreateListSchema(BuiltInSchema.ListOfFloat3, BuiltInSchema.Float3);

            builtInSchemas[(int)BuiltInSchema.FloatRect] =
                new Schema
                {
                    Id = (int)BuiltInSchema.FloatRect,
                    Name = "FloatRect",
                    DataType = DataType.Class,
                    fields = new Field[] { 
                        new Field { Name = "Left", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Top", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Right", Schema = builtInSchemas[(int)BuiltInSchema.Float64] },
                        new Field { Name = "Bottom", Schema = builtInSchemas[(int)BuiltInSchema.Float64] }
                    }
                };

            builtInSchemas[(int)BuiltInSchema.ListOfFloatRect] = CreateListSchema(BuiltInSchema.ListOfFloatRect, BuiltInSchema.FloatRect);

            foreach (var s in builtInSchemas)
            {
                if (s == null)
                    continue;

                s.isBuiltIn = true;
                s.UpdateLayout();

                builtInSchemasByName.Add(s.Name, s);
            }

            Void = builtInSchemas[(int)BuiltInSchema.Void];
            Boolean = builtInSchemas[(int)BuiltInSchema.Boolean];
            Int32 = builtInSchemas[(int)BuiltInSchema.Int32];
            Int64 = builtInSchemas[(int)BuiltInSchema.Int64];
            Float64 = builtInSchemas[(int)BuiltInSchema.Float64];
            Decimal = builtInSchemas[(int)BuiltInSchema.Decimal];
            DateTime = builtInSchemas[(int)BuiltInSchema.DateTime];
            TimeSpan = builtInSchemas[(int)BuiltInSchema.TimeSpan];
            Guid = builtInSchemas[(int)BuiltInSchema.Guid];
            String = builtInSchemas[(int)BuiltInSchema.String];
            Binary = builtInSchemas[(int)BuiltInSchema.Binary];
            ItemPath = builtInSchemas[(int)BuiltInSchema.ItemPath];
            DateTimeOffset = builtInSchemas[(int)BuiltInSchema.DateTimeOffset];
            Money = builtInSchemas[(int)BuiltInSchema.Money];
            Measurement = builtInSchemas[(int)BuiltInSchema.Measurement];
            GeoPosition = builtInSchemas[(int)BuiltInSchema.GeoPosition];
            Variable = builtInSchemas[(int)BuiltInSchema.Variable];
            Version = builtInSchemas[(int)BuiltInSchema.Version];
            ChoiceOption = builtInSchemas[(int)BuiltInSchema.ChoiceOption];
            ChoiceSet = builtInSchemas[(int)BuiltInSchema.ChoiceSet]; 
            ItemFile = builtInSchemas[(int)BuiltInSchema.ItemFile];
            ItemLink = builtInSchemas[(int)BuiltInSchema.ItemLink];
            ItemStatistics = builtInSchemas[(int)BuiltInSchema.ItemStatistics];
            ItemSchema = builtInSchemas[(int)BuiltInSchema.Item];
            Field = builtInSchemas[(int)BuiltInSchema.Field];
            Schema = builtInSchemas[(int)BuiltInSchema.Schema];
            SortField = builtInSchemas[(int)BuiltInSchema.SortField];
            Comparison = builtInSchemas[(int)BuiltInSchema.Comparison];
            LogicGroup = builtInSchemas[(int)BuiltInSchema.LogicGroup];
            NameValue = builtInSchemas[(int)BuiltInSchema.NameValue];
            AggregationTask = builtInSchemas[(int)BuiltInSchema.AggregationTask];
            CacheQuery = builtInSchemas[(int)BuiltInSchema.CacheQuery];
            AggregationResult = builtInSchemas[(int)BuiltInSchema.AggregationResult];
            CacheResponse = builtInSchemas[(int)BuiltInSchema.CacheResponse];
            Pair = builtInSchemas[(int)BuiltInSchema.Pair];
            HistogramBin = builtInSchemas[(int)BuiltInSchema.HistogramBin];
            Histogram = builtInSchemas[(int)BuiltInSchema.Histogram];
            Folder = builtInSchemas[(int)BuiltInSchema.Folder];
            Int2 = builtInSchemas[(int)BuiltInSchema.Int2];
            Int3 = builtInSchemas[(int)BuiltInSchema.Int3];
            IntRect = builtInSchemas[(int)BuiltInSchema.IntRect];
            Float2 = builtInSchemas[(int)BuiltInSchema.Float2];
            Float3 = builtInSchemas[(int)BuiltInSchema.Float3];
            FloatRect = builtInSchemas[(int)BuiltInSchema.FloatRect];

            ListOfVoid = builtInSchemas[(int)BuiltInSchema.ListOfVoid];
            ListOfBoolean = builtInSchemas[(int)BuiltInSchema.ListOfBoolean];
            ListOfInt32 = builtInSchemas[(int)BuiltInSchema.ListOfInt32];
            ListOfInt64 = builtInSchemas[(int)BuiltInSchema.ListOfInt64];
            ListOfFloat64 = builtInSchemas[(int)BuiltInSchema.ListOfFloat64];
            ListOfDecimal = builtInSchemas[(int)BuiltInSchema.ListOfDecimal];
            ListOfDateTime = builtInSchemas[(int)BuiltInSchema.ListOfDateTime];
            ListOfTimeSpan = builtInSchemas[(int)BuiltInSchema.ListOfTimeSpan];
            ListOfGuid = builtInSchemas[(int)BuiltInSchema.ListOfGuid];
            ListOfString = builtInSchemas[(int)BuiltInSchema.ListOfString];
            ListOfBinary = builtInSchemas[(int)BuiltInSchema.ListOfBinary];
            ListOfItemPath = builtInSchemas[(int)BuiltInSchema.ListOfItemPath];
            ListOfDateTimeOffset = builtInSchemas[(int)BuiltInSchema.ListOfDateTimeOffset];
            ListOfMoney = builtInSchemas[(int)BuiltInSchema.ListOfMoney];
            ListOfMeasurement = builtInSchemas[(int)BuiltInSchema.ListOfMeasurement];
            ListOfGeoPosition = builtInSchemas[(int)BuiltInSchema.ListOfGeoPosition];
            ListOfVariable = builtInSchemas[(int)BuiltInSchema.ListOfVariable];
            ListOfVersion = builtInSchemas[(int)BuiltInSchema.ListOfVersion];
            ListOfChoiceOption = builtInSchemas[(int)BuiltInSchema.ListOfChoiceOption];
            ListOfChoiceSet = builtInSchemas[(int)BuiltInSchema.ListOfChoiceSet];
            ListOfItemFile = builtInSchemas[(int)BuiltInSchema.ListOfItemFile];
            ListOfItemBlob = builtInSchemas[(int)BuiltInSchema.ListOfItemBlob];
            ListOfItemText = builtInSchemas[(int)BuiltInSchema.ListOfItemText];
            ListOfItemLink = builtInSchemas[(int)BuiltInSchema.ListOfItemLink];
            ListOfItemStats = builtInSchemas[(int)BuiltInSchema.ListOfItemStats];
            ListOfItem = builtInSchemas[(int)BuiltInSchema.ListOfItem];
            ListOfField = builtInSchemas[(int)BuiltInSchema.ListOfField];
            ListOfSchema = builtInSchemas[(int)BuiltInSchema.ListOfSchema];
            ListOfSortField = builtInSchemas[(int)BuiltInSchema.ListOfSortField];
            ListOfComparision = builtInSchemas[(int)BuiltInSchema.ListOfComparision];
            ListOfLogicGroup = builtInSchemas[(int)BuiltInSchema.ListOfLogicGroup];
            ListOfNameValue = builtInSchemas[(int)BuiltInSchema.ListOfNameValue];
            ListOfAggregationTask = builtInSchemas[(int)BuiltInSchema.ListOfAggregationTask];
            ListOfQuery = builtInSchemas[(int)BuiltInSchema.ListOfQuery];
            ListOfAggregationResult = builtInSchemas[(int)BuiltInSchema.ListOfAggregationResult];
            ListOfCacheResponse = builtInSchemas[(int)BuiltInSchema.ListOfCacheResponse];
            ListOfPair = builtInSchemas[(int)BuiltInSchema.ListOfPair];
            ListOfHistogramBin = builtInSchemas[(int)BuiltInSchema.ListOfHistogramBin];
            ListOfHistogram = builtInSchemas[(int)BuiltInSchema.ListOfHistogram];
            ListOfFolder = builtInSchemas[(int)BuiltInSchema.ListOfFolder];
            ListOfInt2 = builtInSchemas[(int)BuiltInSchema.ListOfInt2];
            ListOfInt3 = builtInSchemas[(int)BuiltInSchema.ListOfInt3];
            ListOfIntRect = builtInSchemas[(int)BuiltInSchema.ListOfIntRect];
            ListOfFloat2 = builtInSchemas[(int)BuiltInSchema.ListOfFloat2];
            ListOfFloat3 = builtInSchemas[(int)BuiltInSchema.ListOfFloat3];
            ListOfFloatRect = builtInSchemas[(int)BuiltInSchema.ListOfFloatRect];
        }
    }
}
