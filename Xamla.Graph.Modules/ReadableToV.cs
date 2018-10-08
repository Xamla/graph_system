using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Graph.Modules.SequenceOperators;
using Xamla.Types;
using Xamla.Utilities;
using Xamla.Utilities.Csv.Import;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.IO.ReadableToV")]
    public class ReadableToV
        : ModuleBase
    {
        static IPinDataType CreateTargetPinDataType(ConvertTypeCode typeCode)
        {
            switch (typeCode)
            {
                case ConvertTypeCode.Any:
                    return PinDataTypeFactory.Create<V<object>>();
                case ConvertTypeCode.Boolean:
                    return PinDataTypeFactory.Create<V<bool>>();
                case ConvertTypeCode.Byte:
                    return PinDataTypeFactory.Create<V<byte>>();
                case ConvertTypeCode.Int16:
                    return PinDataTypeFactory.Create<V<short>>();
                case ConvertTypeCode.Int32:
                    return PinDataTypeFactory.Create<V<int>>();
                case ConvertTypeCode.Int64:
                    return PinDataTypeFactory.Create<V<long>>();
                case ConvertTypeCode.Float32:
                    return PinDataTypeFactory.Create<V<float>>();
                case ConvertTypeCode.Float64:
                    return PinDataTypeFactory.Create<V<double>>();
            }

            return PinDataTypeFactory.Create<object>();
        }

        GenericOutputPin outputPin;
        GenericInputPin data;
        GenericInputPin targetTypePin;
        GenericInputPin delimiter;
        public ReadableToV(IGraphRuntime runtime)
            : base(runtime)
        {
            this.targetTypePin = AddInputPin("TargetType", PinDataTypeFactory.CreateEnum<ConvertTypeCode>(ConvertTypeCode.Float64), PropertyMode.Always);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(V<>)));
            this.outputPin.ChangeType(CreateTargetPinDataType(ConvertTypeCode.Float64));
            this.data = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(IReadable)), PropertyMode.Allow);

            this.delimiter = AddInputPin("Delimiter", PinDataTypeFactory.FromType(typeof(char), ','), PropertyMode.Default);

            this.properties[targetTypePin.Id].WhenNodeEvent.OfType<PropertyChangedEvent>().Subscribe(x =>
            {
                var targetTypeCode = (ConvertTypeCode)x.Value.Value;

                IPinDataType targetPinDataType = CreateTargetPinDataType(targetTypeCode);
                var errors = outputPin.ChangeType(targetPinDataType);
                if (errors.Any())
                {
                    throw new AggregateException("One or more connections could not be reestablished after a data type change.", errors);
                }
            });

            var del = (char)this.Properties.GetValue("Delimiter");
        }
        private static V<double> Transform(IReadable readable, CsvReaderSettings csvReaderSettings)
        {
            using (var sr = new StreamReader(readable.Open()))
            {
                var csvReader = new CsvReader(csvReaderSettings);
                var splitted = csvReader.Read(sr).ToArray();
                var rows = splitted[0].Length;

                var v = V<double>.Generate<double>((row) =>
                {
                    double value;
                    if (row < rows && double.TryParse(splitted[0][row], NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        return value;
                    return 0;
                }, rows);
                return v;
            }
        }

        public static readonly Dictionary<ConvertTypeCode, TypeCode> typeCodeLookUp = new Dictionary<ConvertTypeCode, TypeCode>
        {
            { ConvertTypeCode.Any, TypeCode.Object },
            { ConvertTypeCode.Boolean, TypeCode.Boolean },
            { ConvertTypeCode.Byte, TypeCode.Byte },
            { ConvertTypeCode.Int16, TypeCode.Int16 },
            { ConvertTypeCode.Int32, TypeCode.Int32 },
            { ConvertTypeCode.Int64, TypeCode.Int64 },
            { ConvertTypeCode.Float32, TypeCode.Single },
            { ConvertTypeCode.Float64, TypeCode.Double },
        };

        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {

            var input = (IReadable)inputs[1];
            var targetType = inputs[0];
            TypeCode conversionType = typeCodeLookUp[(ConvertTypeCode)targetType];
            var csvSettings = new CsvReaderSettings
            {
                Delimiters = new char[] { (char)this.Properties.GetValue("Delimiter") },
                SkipEmptyLines = false,
                SkipFirstLine = false,
                MaxTokenLength = 1024 * 1024,
                StartOfComment = ""
            };
            var v = Transform(input, csvSettings);

            switch ((ConvertTypeCode)targetType)
            {
                case ConvertTypeCode.Any:
                    return Task.FromResult(new object[] { v.Convert(typeof(object)) });
                case ConvertTypeCode.Boolean:
                    return Task.FromResult(new object[] { v.Convert(typeof(bool)) });
                case ConvertTypeCode.Byte:
                    return Task.FromResult(new object[] { v.Convert(typeof(byte)) });
                case ConvertTypeCode.Int16:
                    return Task.FromResult(new object[] { v.Convert(typeof(Int16)) });
                case ConvertTypeCode.Int32:
                    return Task.FromResult(new object[] { v.Convert(typeof(Int32)) });
                case ConvertTypeCode.Int64:
                    return Task.FromResult(new object[] { v.Convert(typeof(decimal)) });
                case ConvertTypeCode.Float32:
                    return Task.FromResult(new object[] { v.Convert(typeof(Single)) });
                case ConvertTypeCode.Float64:
                    return Task.FromResult(new object[] { v.Convert(typeof(double)) });
            }

            return Task.FromResult(new object[] { v.Convert(typeof(object)) });
        }
    }
}
