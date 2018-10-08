using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.Skip")]   
    public class Skip
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericInputPin countPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public Skip(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.countPin = AddInputPin("Count", PinDataTypeFactory.CreateInt32(), PropertyMode.Default);
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

        public IInputPin CountPin
        {
            get { return countPin; }
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
        private ISequence<T> EvaluateInternal<T>(ISequence<T> input, int count)
        {
            return input.Skip(count);
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
