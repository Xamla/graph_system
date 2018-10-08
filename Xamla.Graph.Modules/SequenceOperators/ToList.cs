using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.ToList")]
    public class ToList
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, Task<object>>> genericDelegate;

        public ToList(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(List<>)));

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                    {
                        genericDelegate = new GenericDelegate<Func<object, object, Task<object>>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                        outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(List<>).MakeGenericType(genericType)));
                    }
                    else
                    {
                        genericDelegate = null;
                        outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(List<>)));
                    }
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
            return input.ToListAsync(null, cancel).ResultAsObject();
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
