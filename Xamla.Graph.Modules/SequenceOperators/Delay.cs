using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.TimeOperatorsOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.Delay")]
    public class Delay
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericInputPin delayPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public Delay(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.delayPin = AddInputPin("Delay", PinDataTypeFactory.CreateTimeSpan(), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<>)));

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                        genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                    else
                        genericDelegate = null;

                    outputPin.ChangeType(pinDataType);
                });
            });
        }

        public IInputPin DelayPin
        {
            get { return delayPin; }
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
        private ISequence<T> EvaluateInternal<T>(ISequence<T> input, TimeSpan delay)
        {
            return input.SelectAsync(async (x, cancel) =>
            {
                await Task.Delay(delay, cancel).ConfigureAwait(false);
                return x;
            });
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];
            var delay = inputs[1];

            var result = genericDelegate.Delegate(input, delay);

            return Task.FromResult(new object[] { result });
        }
    }
}
