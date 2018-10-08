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
    [Module(ModuleType = "Xamla.Sequence.Operators.Broadcast")]
    public class Broadcast
        : ModuleBase
        , IInterfaceModule
    {
        DynamicOutputPin dynamicOutputPin;
        GenericInputPin inputPin;
        GenericDelegate<Func<object, object[]>> genericDelegate;

        public Broadcast(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.dynamicOutputPin = new DynamicOutputPin(runtime, outputs, "Output", PinDataTypeFactory.FromType(typeof(ISequence<>)));

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                        genericDelegate = new GenericDelegate<Func<object, object[]>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                    else
                        genericDelegate = null;


                    foreach (var pin in dynamicOutputPin.Pins.OfType<IDataTypeChangeable>())
                    {
                        pin.ChangeType(pinDataType);
                    }
                });
            });
        }

        private void DefaultState()
        {
            dynamicOutputPin.AdjustPinCount(2);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.DefaultState();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            if (dynamicOutputPin.Count == 0)
                DefaultState();
        }

        public IInputPin InputPin
        {
            get { return inputPin; }
        }

        [EvaluateInternal]
        private ISequence<T>[] EvaluateInternal<T>(ISequence<T> input)
        {
            var outputSequences = new ISequence<T>[this.Outputs.Count];

            var connectedPins = dynamicOutputPin.ConnectedPins.ToList();
            var sources = input.Broadcast(dynamicOutputPin.Count);
            for (int i = 0; i < connectedPins.Count; ++i)
            {
                var pin = connectedPins[i];
                outputSequences[pin.Index] = sources[i];
            }

            return outputSequences;
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var result = genericDelegate.Delegate(inputs[0]);

            return Task.FromResult<object[]>(result);
        }

        public PinDirection InterfaceDirection
        {
            get { return PinDirection.Input; }
        }

        public IPin AddInterfacePin(string id, bool loading = false)
        {
            return dynamicOutputPin.AddPin(id, loading);
        }

        public bool RemoveInterfacePin(string id)
        {
            return dynamicOutputPin.RemovePin(id);
        }

        public override bool OutputAddable
        {
            get { return true; }
        }

        public bool ReorderPins(IList<string> objectIds)
        {
            return this.outputs.Reorder(objectIds) != null;
        }
    }
}
