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
    [Module(ModuleType = "Xamla.Sequence.Operators.Cast")]
    public class Cast
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
                    return PinDataTypeFactory.Create<ISequence<object>>();
                case ConvertTypeCode.Boolean:
                    return PinDataTypeFactory.Create<ISequence<bool>>();
                case ConvertTypeCode.Byte:
                    return PinDataTypeFactory.Create<ISequence<byte>>();
                case ConvertTypeCode.Int16:
                    return PinDataTypeFactory.Create<ISequence<short>>();
                case ConvertTypeCode.Int32:
                    return PinDataTypeFactory.Create<ISequence<int>>();
                case ConvertTypeCode.Int64:
                    return PinDataTypeFactory.Create<ISequence<long>>();
                case ConvertTypeCode.Float32:
                    return PinDataTypeFactory.Create<ISequence<float>>();
                case ConvertTypeCode.Float64:
                    return PinDataTypeFactory.Create<ISequence<double>>();
                case ConvertTypeCode.String:
                    return PinDataTypeFactory.Create<ISequence<string>>();
            }

            return PinDataTypeFactory.Create<object>();
        }

        GenericInputPin inputPin;
        GenericInputPin targetTypePin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public Cast(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.targetTypePin = AddInputPin("TargetType", PinDataTypeFactory.CreateEnum<ConvertTypeCode>(ConvertTypeCode.Float64), PropertyMode.Always);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<>)));
            this.outputPin.ChangeType(CreateTargetPinDataType(ConvertTypeCode.Float64));
            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericInputType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();
                    var genericOutputType = outputPin.DataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();
                    BuildGenericDelegate(genericInputType, genericOutputType);
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

                var genericInputType = inputPin.DataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();
                var genericOutputType = outputPin.DataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();
                BuildGenericDelegate(genericInputType, genericOutputType);
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
        private ISequence<TResult> EvaluateInternal<T, TResult>(ISequence<T> input, ConvertTypeCode typeCode)
        {
            var conversionType = typeCodeLookUp[typeCode];

            switch (typeCode)
            {
                case ConvertTypeCode.Boolean:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Boolean, CultureInfo.InvariantCulture));
                case ConvertTypeCode.Byte:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Byte, CultureInfo.InvariantCulture));
                case ConvertTypeCode.Int16:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Int16, CultureInfo.InvariantCulture));
                case ConvertTypeCode.Int32:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Int32, CultureInfo.InvariantCulture));
                case ConvertTypeCode.Int64:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Int64, CultureInfo.InvariantCulture));
                case ConvertTypeCode.Decimal:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Decimal, CultureInfo.InvariantCulture));
                case ConvertTypeCode.Float32:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Single, CultureInfo.InvariantCulture));
                case ConvertTypeCode.Float64:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.Double, CultureInfo.InvariantCulture));
                case ConvertTypeCode.String:
                    return input.Select(x => (TResult)System.Convert.ChangeType(x, TypeCode.String, CultureInfo.InvariantCulture));

                case ConvertTypeCode.Any:
                default:
                    return input.Cast<TResult>();
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
