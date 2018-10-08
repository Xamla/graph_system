using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xamla.Types;
using Xamla.Types.Records;
using Xamla.Utilities;

namespace Xamla.Graph
{
    public static class PinDataTypeFactory
    {
        static JsonSerializer CreateDefaultJsonSerializer()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Converters = {
                    new Newtonsoft.Json.Converters.StringEnumConverter()
                }
            };

            return JsonSerializer.Create(serializerSettings);
        }

        public static JsonSerializer Serializer { get; set; } = CreateDefaultJsonSerializer();

        static readonly Dictionary<Type, string> defaultEditors = new Dictionary<Type, string>
        {
            { typeof(bool), WellKnownEditors.CheckBox },
            { typeof(string), WellKnownEditors.SingleLineText },
            { typeof(DateTimeOffset), WellKnownEditors.DateTimePicker },
            { typeof(DateTime), WellKnownEditors.DateTimePicker },
            { typeof(TimeSpan), WellKnownEditors.TimeSpan },
            { typeof(byte), WellKnownEditors.IntNumber },
            { typeof(sbyte), WellKnownEditors.IntNumber },
            { typeof(short), WellKnownEditors.IntNumber },
            { typeof(ushort), WellKnownEditors.IntNumber },
            { typeof(int), WellKnownEditors.IntNumber },
            { typeof(uint),WellKnownEditors.IntNumber },
            { typeof(long), WellKnownEditors.IntNumber },
            { typeof(ulong), WellKnownEditors.IntNumber },
            { typeof(float), WellKnownEditors.FloatNumber },
            { typeof(double), WellKnownEditors.FloatNumber }
        };

        static readonly Dictionary<Type, SerializationFunctions> customSerializedTypes = new Dictionary<Type, SerializationFunctions>
        {
            { typeof(Int2), new SerializationFunctions { Serialize = x => new JValue(x.ToString()), Deserialize = x => Int2.Parse((string)x) } },
            { typeof(Int3), new SerializationFunctions { Serialize = x => new JValue(x.ToString()), Deserialize = x => Int3.Parse((string)x) } },
            { typeof(IntRect), new SerializationFunctions { Serialize = x => new JValue(x.ToString()), Deserialize = x => IntRect.Parse((string)x) } },
            { typeof(Float2), new SerializationFunctions { Serialize = x => new JValue(x.ToString()), Deserialize = x => Float2.Parse((string)x) } },
            { typeof(Float3), new SerializationFunctions { Serialize = x => new JValue(x.ToString()), Deserialize = x => Float3.Parse((string)x) } },
            { typeof(FloatRect), new SerializationFunctions { Serialize = x => new JValue(x.ToString()), Deserialize = x => FloatRect.Parse((string)x) } }
        };

        public static Dictionary<Type, SerializationFunctions> CustomSerializedTypes => customSerializedTypes;

        public static IPinDataType FromType(Type type)
        {
            return FromType(type, TypeHelpers.GetDefaultValue(type), null, null, null);
        }

        public static IPinDataType FromType(Type type, object defaultValue)
        {
            return FromType(type, defaultValue, null, null, null);
        }

        public static IPinDataType Create<T>()
        {
            return Create<T>(default(T));
        }

        public static IPinDataType Create<T>(T defaultValue)
        {
            return FromType(typeof(T), defaultValue, null, null, null);
        }

        public static IPinDataType Create<T>(T defaultValue, string editor)
        {
            return FromType(typeof(T), defaultValue, editor, null, null);
        }

        public static IPinDataType FromType(Type type, object defaultValue, string editor, object parameters, Func<object, ValidationResult> validator)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type == typeof(Flow))
            {
                return CreateFlow();
            }

            if (editor == null)
            {
                defaultEditors.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out editor);
            }

            if (customSerializedTypes.TryGetValue(type, out SerializationFunctions serializationFunctions))
            {
                return new CustomSerializedObjectPinDataType(type, defaultValue, editor, parameters, validator, serializationFunctions.Serialize, serializationFunctions.Deserialize);
            }
            else if (type.GetTypeInfo().IsEnum)
            {
                return CreateEnum(type, defaultValue, editor);
            }
            else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(A<>))
            {
                var fieldType = type.GetGenericArguments()[0];
                return CreateArray(fieldType, null, defaultValue, editor);
            }
            else
            {
                return new ObjectPinDataType(type, defaultValue, editor, null, PinDataTypeFactory.Serializer);
            }
        }

        public static IPinDataType CreateDynamicEnum(string[] enumValues, string defaultValue)
        {
            return new DynamicEnumPinDataType(enumValues, defaultValue, Serializer);
        }

        public static IPinDataType CreateEnum(Type enumType, object defaultValue, string editor = WellKnownEditors.DropDown)
        {
            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("The 'enumType' argument must be an enumerated type.", "enumType");

            if (defaultValue == null)
            {
                defaultValue = TypeHelpers.GetDefaultValue(enumType);       // will choose the enum value returned first by reflection
            }

            if (defaultValue.GetType() != enumType)
            {
                defaultValue = Enum.ToObject(enumType, defaultValue);
            }

            return new EnumPinDataType(enumType, defaultValue, Serializer);
        }

        public static IPinDataType CreateEnum<TEnum>(TEnum defaultValue = default(TEnum), string editor = WellKnownEditors.DropDown)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            return CreateEnum(typeof(TEnum), defaultValue, editor);
        }

        public static IPinDataType CreateArray(Type fieldType, int[] dimension, object defaultValue, string editor = null)
        {
            return new ArrayPinDataType(fieldType, dimension, defaultValue, Serializer, editor);
        }

        public static IPinDataType CreateArray<T>()
        {
            return CreateArray(typeof(T), null, null);
        }

        public static IPinDataType CreateArray<T>(int[] dimension, A<T> defaultValue)
        {
            return CreateArray(typeof(T), dimension, defaultValue);
        }

        public static IPinDataType CreateArray<T>(int length, A<T> defaultValue)
        {
            return CreateArray(typeof(T), new int[] { length }, defaultValue);
        }

        public static IPinDataType CreateRecord(string schema, ICursor defaultValue, string editor = WellKnownEditors.MultiLineText)
        {
            return new RecordPinDataType(schema, defaultValue, Serializer, editor);
        }

        public static IPinDataType CreateAny() { return Create<object>(); }
        public static IPinDataType CreateUnit() { return Create<System.Reactive.Unit>(); }
        public static IPinDataType CreateBoolean() { return Create<bool>(); }
        public static IPinDataType CreateBoolean(bool defaultValue) { return Create<bool>(defaultValue); }
        public static IPinDataType CreateInt16() { return Create<short>(); }
        public static IPinDataType CreateInt16(short defaultValue) { return Create<short>(defaultValue); }
        public static IPinDataType CreateInt32() { return Create<int>(); }
        public static IPinDataType CreateInt32(int defaultValue) { return Create<int>(defaultValue); }
        public static IPinDataType CreateInt64() { return Create<long>(); }
        public static IPinDataType CreateInt64(long defaultValue) { return Create<long>(defaultValue); }
        public static IPinDataType CreateFloat32() { return Create<float>(); }
        public static IPinDataType CreateFloat32(float defaultValue) { return Create<float>(defaultValue); }
        public static IPinDataType CreateFloat64() { return Create<double>(); }
        public static IPinDataType CreateFloat64(double defaultValue) { return Create<double>(defaultValue); }
        public static IPinDataType CreateString() { return Create<string>(); }
        public static IPinDataType CreateString(string defaultValue) { return Create<string>(defaultValue); }
        public static IPinDataType CreateTimeSpan() { return Create<TimeSpan>(); }
        public static IPinDataType CreateDateTime() { return Create<DateTime>(); }
        public static IPinDataType CreateImageBuffer() { return Create<IImageBuffer>(); }

        public static IPinDataType CreateFlow() { return new FlowPinDataType(); }
    }
}
