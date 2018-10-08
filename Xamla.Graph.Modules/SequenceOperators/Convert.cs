using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using System.Threading;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    public enum ConvertTypeCode
    {
        Any,
        Boolean,
        Byte,
        Int16,
        Int32,
        Int64,
        Decimal,
        Float32,
        Float64,
        String
    }

    [Module(ModuleType = "Xamla.Sequence.Operators.Convert")]
    public class Convert
        : ModuleBase
    {
        static readonly Dictionary<ConvertTypeCode, TypeCode> typeCodeLookUp = new Dictionary<ConvertTypeCode, TypeCode>
        {
            { ConvertTypeCode.Any, TypeCode.Object },
            { ConvertTypeCode.Boolean, TypeCode.Boolean },
            { ConvertTypeCode.Byte, TypeCode.Byte },
            { ConvertTypeCode.Int16, TypeCode.Int16 },
            { ConvertTypeCode.Int32, TypeCode.Int32 },
            { ConvertTypeCode.Int64, TypeCode.Int64 },
            { ConvertTypeCode.Float32, TypeCode.Single },
            { ConvertTypeCode.Float64, TypeCode.Double },
            { ConvertTypeCode.String, TypeCode.String },
        };

        private static IPinDataType CreateTargetPinDataType(ConvertTypeCode typeCode)
        {
            switch (typeCode)
            {
                case ConvertTypeCode.Any:
                    return PinDataTypeFactory.CreateAny();;
                case ConvertTypeCode.Boolean:
                    return PinDataTypeFactory.CreateBoolean();
                case ConvertTypeCode.Byte:
                    return PinDataTypeFactory.FromType(typeof(byte));
                case ConvertTypeCode.Int16:
                    return PinDataTypeFactory.FromType(typeof(short));
                case ConvertTypeCode.Int32:
                    return PinDataTypeFactory.CreateInt32();
                case ConvertTypeCode.Int64:
                    return PinDataTypeFactory.FromType(typeof(long));
                case ConvertTypeCode.Float32:
                    return PinDataTypeFactory.FromType(typeof(float));
                case ConvertTypeCode.Float64:
                    return PinDataTypeFactory.FromType(typeof(double));
                case ConvertTypeCode.String:
                    return PinDataTypeFactory.CreateString();
            }

            return PinDataTypeFactory.Create<object>();
        }

        GenericInputPin inputPin;
        GenericInputPin targetTypePin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public Convert(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.CreateFloat64(), PropertyMode.Never);
            this.targetTypePin = AddInputPin("TargetType", PinDataTypeFactory.CreateEnum<ConvertTypeCode>(ConvertTypeCode.Float64), PropertyMode.Always);
            this.outputPin = AddOutputPin("Output", CreateTargetPinDataType(ConvertTypeCode.Float64));
            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, CreateTargetPinDataType(ConvertTypeCode.Float64), false, pinDataType =>
                {
                    var genericInputType = pinDataType.UnderlyingType;
                    BuildGenericDelegate(genericInputType, outputPin.DataType.UnderlyingType);
                });
            });


            this.properties[targetTypePin.Id].WhenNodeEvent.OfType<PropertyChangedEvent>().Subscribe(x =>
            {
                var targetTypeCode = (ConvertTypeCode)x.Value.Value;

                IPinDataType targetPinDataType = CreateTargetPinDataType(targetTypeCode);
                var errors = outputPin.ChangeType(targetPinDataType);
                if (errors.Any())
                {
                    throw new AggregateException("One or more connections could not be reestablished after a data type change.", errors);
                }
                BuildGenericDelegate(inputPin.DataType.UnderlyingType, outputPin.DataType.UnderlyingType);
            });
        }

        private void BuildGenericDelegate(Type genericInputType, Type genericOutputType)
        {
            if (genericInputType != null && genericOutputType != null)
                genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericInputType, genericOutputType));
            else
                genericDelegate = null;
        }


        public IInputPin TargetTypePin
        {
            get { return targetTypePin; }
        }

        public IInputPin InputPin
        {
            get { return inputPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        [EvaluateInternal]
        private object EvaluateInternal<T, TResult>(T input, ConvertTypeCode typeCode)
        {
            var conversionType = typeCodeLookUp[typeCode];

            switch (typeCode)
            {
                case ConvertTypeCode.Boolean:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Boolean, CultureInfo.InvariantCulture);
                case ConvertTypeCode.Byte:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Byte, CultureInfo.InvariantCulture);
                case ConvertTypeCode.Int16:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Int16, CultureInfo.InvariantCulture);
                case ConvertTypeCode.Int32:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Int32, CultureInfo.InvariantCulture);
                case ConvertTypeCode.Int64:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Int64, CultureInfo.InvariantCulture);
                case ConvertTypeCode.Decimal:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Decimal, CultureInfo.InvariantCulture);
                case ConvertTypeCode.Float32:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Single, CultureInfo.InvariantCulture);
                case ConvertTypeCode.Float64:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Double, CultureInfo.InvariantCulture);
                case ConvertTypeCode.String:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.String, CultureInfo.InvariantCulture);

                case ConvertTypeCode.Any:
                default:
                    return (TResult)System.Convert.ChangeType(input, TypeCode.Object, CultureInfo.InstalledUICulture);
            }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];
            var targetType = inputs[1];

            var result = genericDelegate.Delegate(input, targetType);

            return Task.FromResult(new object[] { result });
        }
    }
}
