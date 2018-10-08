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
    [Module(ModuleType = "Xamla.Sequence.Operators.SequenceEqual")]
    public class SequenceEqual
        : ModuleBase
    {
        readonly GenericInputPin firstInputPin;
        readonly GenericInputPin secondInputPin;
        readonly GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object, Task<bool>>> genericDelegate;

        public SequenceEqual(IGraphRuntime runtime)
            : base(runtime)
        {
            this.firstInputPin = AddInputPin("First", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.secondInputPin = AddInputPin("Second", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.outputPin = AddOutputPin("Result", PinDataTypeFactory.CreateBoolean());

            this.firstInputPin.WhenNodeEvent.Subscribe(HandleInputPinEvent);
            this.secondInputPin.WhenNodeEvent.Subscribe(HandleInputPinEvent);
        }

        private bool inHandleInputPinEvent;

        private void HandleInputPinEvent(NodeEvent evt)
        {
            if (inHandleInputPinEvent)
                return;
            try
            {
                inHandleInputPinEvent = true;

                var pin = evt.Source as GenericInputPin;

                var pinDataTypeA = PinDataTypeFactory.FromType(typeof(ISequence<>));
                var pinDataTypeB = PinDataTypeFactory.FromType(typeof(ISequence<>));

                // determine common type
                IPinDataType pinDataType;
                if (firstInputPin.Connections.Any() && secondInputPin.Connections.Any())
                {
                    // check if both data types are compatible
                    var firstType = firstInputPin.Connections[0].DataType;
                    var secondType = secondInputPin.Connections[0].DataType;

                    if (firstType.IsAssignableFrom(secondType))
                        pinDataType = PinDataTypeFactory.FromType(firstType);
                    else if (secondType.IsAssignableFrom(firstType))
                        pinDataType = PinDataTypeFactory.FromType(secondType);
                    else
                        throw new Exception("Connected sequence pin data types are not compatible.");
                }
                else
                {
                    if (firstInputPin.Connections.Any())
                        pinDataType = PinDataTypeFactory.FromType(firstInputPin.Connections[0].DataType);
                    else if (secondInputPin.Connections.Any())
                        pinDataType = PinDataTypeFactory.FromType(secondInputPin.Connections[0].DataType);
                    else
                        pinDataType = PinDataTypeFactory.FromType(typeof(ISequence<>));
                }

                var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                if (genericType != null)
                {
                    genericDelegate = new GenericDelegate<Func<object, object, object, Task<bool>>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                }
                else
                {
                    genericDelegate = null;
                }

                pin.SetType(pinDataType);

                if (pin == firstInputPin)
                    secondInputPin.ChangeType(pinDataType);
                else
                    firstInputPin.ChangeType(pinDataType);
            }
            finally
            {
                inHandleInputPinEvent = false;
            }
        }

        [EvaluateInternal]
        private Task<bool> EvaluateInternal<T>(ISequence<T> first, ISequence<T> second, CancellationToken cancel)
        {
            return first.SequenceEqual(second).FirstAsync(null, cancel);
        }

        protected override async Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            bool result = await genericDelegate.Delegate(inputs[0], inputs[1], cancel);

            return new object[] { result };
        }
    }
}
