using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.Repeat")]
    public class Repeat
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericInputPin countPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public Repeat(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.countPin = AddInputPin("Count", PinDataTypeFactory.Create<int?>(null, WellKnownEditors.IntNumber), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<object>)));

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                    {
                        genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                        outputPin.ChangeType(pinDataType);
                    }
                    else
                    {
                        genericDelegate = null;
                        outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<object>)));
                    }

                });
            });
        }

        public IInputPin CountPin
        {
            get { return countPin; }
        }

        public int CountProperty
        {
            get { return this.properties.Get<int>(countPin.Id); }
            set { Properties.Set(countPin.Id, value); }
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
        private ISequence<T> EvaluateInternal<T>(ISequence<T> input, int? count)
        {
            return count.HasValue ? input.Repeat(count.Value) : input.Repeat();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];
            var count = inputs[1];

            var result = genericDelegate.Delegate(input, count);

            return Task.FromResult(new object[] { result });
        }
    }
}
