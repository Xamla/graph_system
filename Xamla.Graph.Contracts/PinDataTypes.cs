using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xamla.Types;
using Xamla.Types.Records;
using Xamla.Types.Sequence;

namespace Xamla.Graph
{
    public sealed class Flow
    {
        public static readonly Flow Default = new Flow();

        public Flow()
        {
        }

        public Flow(Exception exception)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; }

        public bool IsFaulted => this.Exception != null;
    }

    public enum PinTypeCode
    {
        Object,
        Record,
        Enum,
        DynamicEnum,
        Array,
        Flow
    };

    public static class WellKnownEditors
    {
        // public const string ColorPicker = "ColorPicker";
        public const string IntNumber = "IntNumber";
        public const string FloatNumber = "FloatNumber";
        public const string Slider = "Slider";
        public const string DateTimePicker = "DateTimePicker";
        public const string TimeSpan = "TimeSpan";
        public const string SingleLineText = "SingleLineText";
        public const string MultiLineText = "MultiLineText";
        public const string CSharp = "CSharp";
        public const string Python = "Python";
        public const string PortReference = "PortReference";
        public const string DropDown = "DropDown";
        public const string CheckBox = "CheckBox";
        public const string OptionList = "OptionList";
        public const string FileSelector = "FileSelector";
        public const string DirectorySelector = "DirectorySelector";
        public const string Hidden = "Hidden";
        public const string ExceptionViewer = "ExceptionViewer";
    }

    public class RecordTypeParameters
    {
        public string Schema;
    }

    public class ArrayTypeParameters
    {
        public int[] Dimension;
        public Type FieldType;
    }

    public class ObjectPinDataType
        : IPinDataType
    {
        struct NumericTypeInfo
        {
            public bool Float;
            public int Size;
            public bool Signed;
        }

        static readonly Dictionary<Type, string> typeAliases = new Dictionary<Type, string>
        {
            { typeof(object), "object" },
            { typeof(char), "char" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "int8" },
            { typeof(short), "int16" },
            { typeof(ushort), "uint16" },
            { typeof(int), "int32" },
            { typeof(uint), "uint32" },
            { typeof(long), "int64" },
            { typeof(ulong), "uint64" },
            { typeof(float), "float32" },
            { typeof(double), "float64" },
            { typeof(string), "string" },
            { typeof(ISequence), "seq" },
            { typeof(ISequence<>), "seq"}
        };

        static readonly Dictionary<Type, NumericTypeInfo> numericTypes = new Dictionary<Type, NumericTypeInfo>
        {
            { typeof(sbyte), new NumericTypeInfo { Size = sizeof(sbyte), Signed = true }},
            { typeof(short), new NumericTypeInfo { Size = sizeof(short), Signed = true }},
            { typeof(int), new NumericTypeInfo { Size = sizeof(int), Signed = true}},
            { typeof(long), new NumericTypeInfo { Size = sizeof(long), Signed = true}},
            { typeof(decimal), new NumericTypeInfo { Size = sizeof(decimal), Signed = true}},
            { typeof(float), new NumericTypeInfo { Size = sizeof(float), Signed = true, Float = true }},
            { typeof(double), new NumericTypeInfo { Size = sizeof(double), Signed = true, Float = true }},
            { typeof(byte), new NumericTypeInfo { Size = sizeof(byte) }},
            { typeof(ushort), new NumericTypeInfo { Size = sizeof(ushort) }},
            { typeof(uint), new NumericTypeInfo { Size = sizeof(uint) }},
            { typeof(ulong), new NumericTypeInfo { Size = sizeof(ulong) }},
        };

        public static string GetShortTypeName(Type type)
        {
            if (type.IsArray)
            {
                var elementName = GetShortTypeName(type.GetElementType());
                int rank = type.GetArrayRank();
                return string.Concat(elementName, "[", new String(',', rank - 1), "]");
            }

            if (!type.GetTypeInfo().IsGenericType)
            {
                string name;
                if (typeAliases.TryGetValue(type, out name))
                    return name;
                return type.Name;
            }
            else
            {
                var definition = type.GetGenericTypeDefinition();
                var args = type.GetTypeInfo().GetGenericArguments();

                if (definition == typeof(Nullable<>))
                {
                    return GetShortTypeName(args[0]) + "?";
                }
                else
                {
                    string baseName;
                    if (!typeAliases.TryGetValue(definition, out baseName))
                        baseName = type.Name.Substring(0, type.Name.IndexOf('`'));

                    return string.Concat(baseName, "<", string.Join(", ", args.Select(GetShortTypeName)), ">");
                }
            }
        }

        public static bool IsNumericType(Type type)
        {
            return numericTypes.ContainsKey(type);
        }

        public static bool IsIntegerType(Type type)
        {
            NumericTypeInfo info;
            return numericTypes.TryGetValue(type, out info) && !info.Float;
        }

        public static bool IsImplicitConvertible(Type source, Type destination)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (destination == null)
                throw new ArgumentNullException("destination");

            if (destination.Equals(source) || destination.GetTypeInfo().IsAssignableFrom(source))
                return true;

            NumericTypeInfo numericSource, numericDestination;
            if (numericTypes.TryGetValue(source, out numericSource) && numericTypes.TryGetValue(destination, out numericDestination))
            {
                if (numericSource.Size < numericDestination.Size && numericSource.Float == numericDestination.Float)
                    return true;

                if (numericSource.Float == false && numericDestination.Float == true)
                    return true;
            }

            return false;
        }

        Func<object, ValidationResult> validator;
        protected JsonSerializer serializer;

        public ObjectPinDataType(Type underlyingType, object defaultValue, string editor, object parameters, JsonSerializer serializer, Func<object, ValidationResult> validator = null)
            : this(PinTypeCode.Object, underlyingType, defaultValue, editor, parameters, serializer, validator)
        {
        }

        protected ObjectPinDataType(PinTypeCode typeCode, Type underlyingType, object defaultValue, string editor, object parameters, JsonSerializer serializer, Func<object, ValidationResult> validator)
        {
            this.TypeCode = typeCode;
            this.UnderlyingType = underlyingType;
            this.DefaultValue = defaultValue;
            this.Editor = editor;
            this.Parameters = parameters;
            this.serializer = serializer;
            this.validator = validator;
        }

        public string ShortUnderlyingTypeName
        {
            get { return GetShortTypeName(this.UnderlyingType); }
        }

        public PinTypeCode TypeCode
        {
            get;
            private set;
        }

        public string Editor
        {
            get;
            private set;
        }

        public object Parameters
        {
            get;
            private set;
        }

        public Type UnderlyingType
        {
            get;
            private set;
        }

        public virtual bool IsAssignableFrom(IPinDataType dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return IsImplicitConvertible(dataType.UnderlyingType, this.UnderlyingType);
        }

        public virtual bool TryConvert(object source, out object destination)
        {
            if (source == null || this.UnderlyingType.GetTypeInfo().IsAssignableFrom(source.GetType()))
            {
                destination = source;
                return true;
            }

            if (IsImplicitConvertible(source.GetType(), this.UnderlyingType))
            {
                try
                {
                    destination = Convert.ChangeType(source, this.UnderlyingType);
                    return true;
                }
                catch
                {
                }
            }

            destination = null;
            return false;
        }

        public virtual ValidationResult Validate(object value)
        {
            if (validator != null)
                return validator(value);

            if (value == null)
            {
                if (Nullable.GetUnderlyingType(this.UnderlyingType) != null)
                    return ValidationResult.Valid;
                if (this.UnderlyingType.GetTypeInfo().IsValueType)
                    return new ValidationResult("Value cannot be null.");
            }
            else if (!this.UnderlyingType.GetTypeInfo().IsAssignableFrom(value.GetType()))
            {
                return new ValidationResult("Value is of wrong type.");
            }

            return ValidationResult.Valid;
        }

        public object DefaultValue
        {
            get;
            private set;
        }

        public Models.PinDataTypeModel ToModel()
        {
            return new Models.PinDataTypeModel
            {
                TypeCode = this.TypeCode,
                UnderlyingType = this.UnderlyingType,
                TypeName = GetShortTypeName(this.UnderlyingType),
                Parameters = Parameters,
                Editor = this.Editor,
                DefaultValue = Serialize(this.DefaultValue),
                Nullable = !this.UnderlyingType.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(this.UnderlyingType) != null
            };
        }

        public virtual JToken Serialize(object value)
        {
            if (value == null)
                return null;

            return JToken.FromObject(value, serializer);
        }

        public virtual object Deserialize(JToken token)
        {
            return token.ToObject(this.UnderlyingType, serializer);
        }
    }

    public class CustomSerializedObjectPinDataType
        : ObjectPinDataType
    {
        Func<object, JToken> serialize;
        Func<JToken, object> deserialize;

        public CustomSerializedObjectPinDataType(
            Type underlyingType,
            object defaultValue,
            string editor,
            object parameters,
            Func<object, ValidationResult> validator,
            Func<object, JToken> serialize,
            Func<JToken, object> deserialize
        )
            : base(underlyingType, defaultValue, editor, parameters, null, validator)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }
        public override JToken Serialize(object value)
        {
            return serialize(value);
        }

        public override object Deserialize(JToken token)
        {
            return deserialize(token);
        }
    }

    public class EnumPinDataType
        : ObjectPinDataType
    {
        public EnumPinDataType(Type enumType, object defaultValue, JsonSerializer serializer, string editor = WellKnownEditors.DropDown)
            : base(PinTypeCode.Enum, enumType, defaultValue, editor, Enum.GetNames(enumType), serializer, null)
        {
        }

        public override bool IsAssignableFrom(IPinDataType dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.TypeCode == PinTypeCode.Enum || IsIntegerType(dataType.UnderlyingType) || dataType.UnderlyingType == typeof(string);
        }

        public override bool TryConvert(object source, out object destination)
        {
            if (source != null)
            {
                try
                {
                    if (source is string valueName)
                    {
                        destination = Enum.Parse(this.UnderlyingType, valueName);
                    }
                    else
                    {
                        var sourceType = source.GetType();
                        if (!sourceType.GetTypeInfo().IsEnum)
                            source = Convert.ChangeType(source, System.TypeCode.Int32, CultureInfo.InvariantCulture);

                        destination = Enum.ToObject(this.UnderlyingType, source);
                    }

                    return true;
                }
                catch
                {
                    // ignore error
                }
            }

            destination = Enum.ToObject(this.UnderlyingType, 0);
            return false;
        }

        public override ValidationResult Validate(object value)
        {
            if (!Enum.IsDefined(this.UnderlyingType, value))
                return new ValidationResult(string.Format("'{0}' is not a valid input for enum type '{1}'", value, this.UnderlyingType));

            return ValidationResult.Valid;
        }

        public override JToken Serialize(object value)
        {
            return new JValue(value.ToString());
        }

        public override object Deserialize(JToken token)
        {
            if (token == null)
                return this.DefaultValue;

            return Enum.Parse(this.UnderlyingType, (string)token);
        }
    }

    public class DynamicEnumPinDataType
        : ObjectPinDataType
    {
        string[] enumValues;

        public DynamicEnumPinDataType(string[] enumValues, string defaultValue, JsonSerializer serializer, string editor = WellKnownEditors.DropDown)
            : base(PinTypeCode.DynamicEnum, typeof(string), defaultValue, editor, enumValues, serializer, null)
        {
            this.enumValues = enumValues;
        }

        public override bool TryConvert(object source, out object destination)
        {
            try
            {
                var name = (string)Convert.ChangeType(source, System.TypeCode.String, CultureInfo.InvariantCulture);
                if (enumValues.Contains(name))
                {
                    destination = name;
                    return true;
                }
            }
            catch
            {
                // ignore error
            }

            destination = null;
            return false;
        }

        public override bool IsAssignableFrom(IPinDataType dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.TypeCode == PinTypeCode.DynamicEnum || dataType.UnderlyingType == typeof(string);
        }

        public override ValidationResult Validate(object value)
        {
            var name = value as string;
            if (name == null || !enumValues.Contains(name))
                return new ValidationResult(string.Format("'{0}' is not a valid input for dynamic enum type", value));

            return ValidationResult.Valid;
        }

        public override object Deserialize(JToken value)
        {
            if (value == null)
                return this.DefaultValue;

            string name = (string)value;
            var result = Validate(name);
            if (!result.IsValid)
                throw new Exception(result.Message);

            return name;
        }
    }

    public class ArrayPinDataType
        : ObjectPinDataType
    {
        static Type CreateArrayType(Type fieldType)
        {
            return typeof(A<>).MakeGenericType(fieldType);
        }

        ArrayTypeParameters parameters;

        public ArrayPinDataType(Type fieldType, int[] dimension, object defaultValue, JsonSerializer serializer, string editor)
            : base(PinTypeCode.Array, CreateArrayType(fieldType), defaultValue, editor, new ArrayTypeParameters { Dimension = dimension, FieldType = fieldType }, serializer, null)
        {
            this.parameters = (ArrayTypeParameters)this.Parameters;
        }

        public override bool IsAssignableFrom(IPinDataType dataType)
        {
            var parameters = (ArrayTypeParameters)this.Parameters;

            if (dataType == null)
                throw new ArgumentNullException("dataType");

            if (dataType.TypeCode == PinTypeCode.Array)
            {
                var otherParameters = dataType.Parameters as ArrayTypeParameters;
                if (otherParameters != null && otherParameters.FieldType == parameters.FieldType)
                    return true;
            }

            return false;
        }

        public override bool TryConvert(object source, out object destination)
        {
            throw new NotImplementedException();
        }

        public override ValidationResult Validate(object value)
        {
            if (value == null)
                return ValidationResult.Valid;

            var arrayType = CreateArrayType(parameters.FieldType);
            if (value.GetType() != arrayType)
                return new ValidationResult(string.Format("Value is not an array of type {0}.", arrayType.Name));

            if (parameters.Dimension != null)
            {
                var fields = arrayType.GetTypeInfo().GetFields().Where(x => x.Name == "Dimension");
                foreach (FieldInfo field in fields)
                {
                    var valx = parameters.Dimension;
                    var valy = field.GetValue(value) as Array;

                    if (valx.Length != valy.Length)
                        return new ValidationResult(string.Format("Array of rank {0} expected.", valx.Length));

                    for (int i = 0; i < valx.Length; i++)
                        if (!object.Equals(valx.GetValue(i), valy.GetValue(i)))
                            return new ValidationResult("Array has wrong size.");
                }
            }

            return ValidationResult.Valid;
        }

        public class ArrayModel<T>
        {
            public int[] Dimension;
            public T[] Buffer;
        }

        public override object Deserialize(JToken value)
        {
            if (value == null || value.Type == JTokenType.Null)
                return DefaultValue;

            var arrayType = typeof(A<>).MakeGenericType(parameters.FieldType);
            if (value.Type == JTokenType.Array)
            {
                // single dimensional array
                var buffer = (Array)value.ToObject(parameters.FieldType.MakeArrayType(), serializer);
                return Activator.CreateInstance(arrayType, buffer, buffer.Length);
            }
            else if (value.Type == JTokenType.String)
            {
                var v = (string)value;
                var splitted = v.Split(',');
                if (splitted.Count() != this.parameters.Dimension.Aggregate((x, y) => x * y))
                    throw new Exception("Wrong number of elements.");

                var fieldArrayType = parameters.FieldType.MakeArrayType();
                var buffer = (Array)Activator.CreateInstance(fieldArrayType, splitted.Count());

                for(int i = 0; i < splitted.Count(); ++i)
                    buffer.SetValue(Convert.ChangeType(splitted[i], parameters.FieldType), i);

                return Activator.CreateInstance(arrayType, buffer, parameters.Dimension);
            }
            else
            {
                // multi-dimensional array
                var model = value.ToObject(typeof(ArrayModel<>).MakeGenericType(parameters.FieldType), serializer);

                var dimensionField = model.GetType().GetTypeInfo().GetField("Dimension");
                var bufferField = model.GetType().GetTypeInfo().GetField("Buffer");

                var dimension = dimensionField.GetValue(model);
                var buffer = bufferField.GetValue(model);

                return Activator.CreateInstance(arrayType, buffer, dimension);
            }
        }
    }

    public class RecordPinDataType
        : ObjectPinDataType
    {
        RecordTypeParameters parameters;

        public RecordPinDataType(string schema, ICursor defaultValue, JsonSerializer serializer, string editor = WellKnownEditors.MultiLineText)
            : base(PinTypeCode.Record, typeof(ICursor), defaultValue, editor, new RecordTypeParameters { Schema = schema }, serializer, null)
        {
            this.parameters = (RecordTypeParameters)this.Parameters;
        }

        public override bool IsAssignableFrom(IPinDataType dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            if (dataType.TypeCode == PinTypeCode.Record)
            {
                var otherParameters = dataType.Parameters as RecordTypeParameters;
                if (otherParameters != null && otherParameters.Schema == parameters.Schema)
                    return true;
            }

            return false;
        }

        public override bool TryConvert(object source, out object destination)
        {
            throw new NotImplementedException();
        }

        public override ValidationResult Validate(object value)
        {
            throw new NotImplementedException();
        }

        public override JToken Serialize(object value)
        {
            var cursor = (ICursor)value;
            using (var writer = new JTokenWriter())
            {
                cursor.WriteTo(writer);
                return writer.Token;
            }
        }
    }

    public class FlowPinDataType
        : ObjectPinDataType
    {
        public FlowPinDataType()
            : base(PinTypeCode.Flow, typeof(Flow), null, null, null, null, null)
        {
        }

        public override bool IsAssignableFrom(IPinDataType dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");
            return dataType.TypeCode == PinTypeCode.Flow;
        }

        public override JToken Serialize(object value)
        {
            return new JObject();
        }

        public override object Deserialize(JToken token)
        {
            return this.DefaultValue;
        }
    }
}
