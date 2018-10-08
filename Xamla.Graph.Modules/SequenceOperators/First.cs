using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.First")]
    public class First
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, Task<object>>> genericDelegate;

        public First(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<object>());

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var method = EvaluateInternalAttribute.GetMethod(GetType());
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                    {
                        genericDelegate = new GenericDelegate<Func<object, object, Task<object>>>(this, method.MakeGenericMethod(genericType));
                    }
                    else
                    {
                        genericDelegate = null;
                        genericType = typeof(object);
                    }

                    outputPin.ChangeType(PinDataTypeFactory.FromType(genericType));
                });
            });
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
        private Task<object> EvaluateInternal<T>(ISequence<T> input, CancellationToken cancel)
        {
            return input.FirstAsync(null, cancel).ResultAsObject();
        }

        protected override async Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];

            var result = await genericDelegate.Delegate(input, cancel).ConfigureAwait(false);

            return new object[] { result };
        }
    }

    [Module(ModuleType = "Xamla.Sequence.Operators.FirstOrDefault")]
    public class FirstOrDefault
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, Task<object>>> genericDelegate;

        public FirstOrDefault(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<object>());

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                    {
                        genericDelegate = new GenericDelegate<Func<object, object, Task<object>>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                    }
                    else
                    {
                        genericDelegate = null;
                        genericType = typeof(object);
                    }

                    outputPin.ChangeType(PinDataTypeFactory.FromType(genericType));
                });
            });
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
        private Task<object> EvaluateInternal<T>(ISequence<T> input, CancellationToken cancel)
        {
            return input.AsObjects().FirstOrDefaultAsync().ResultAsObject();
        }

        protected override async Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];

            var result = await genericDelegate.Delegate(input, cancel).ConfigureAwait(false);

            return new object[] { result };
        }
    }
}
