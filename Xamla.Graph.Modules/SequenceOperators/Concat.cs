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
    [Module(ModuleType = "Xamla.Sequence.Operators.Concat")]
    public class Concat
        : ModuleBase
        , IInterfaceModule
    {
        DynamicInputPin dynamicInputPin;
        GenericOutputPin output;

        GenericDelegate<Func<object, object>> genericDelegate;

        public Concat(IGraphRuntime runtime)
            : base(runtime)
        {
            this.dynamicInputPin = new DynamicInputPin(runtime, inputs, "Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), OnDynamicInputAdd);
            this.output = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<object>)));
        }

        private bool OnDynamicInputAdd(string id)
        {
            return dynamicInputPin.Pin(id).WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                        genericDelegate = new GenericDelegate<Func<object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                    else
                        genericDelegate = null;

                    var pinsToChange = dynamicInputPin.Pins.Where(x => x.Id != id && x.DataType.UnderlyingType != pinDataType.UnderlyingType).OfType<IDataTypeChangeable>();
                    foreach (var pin in pinsToChange)
                    {
                        pin.SetType(pinDataType);
                    }

                    if (!pinsToChange.Any())
                    {
                        if (genericType != null)
                            output.ChangeType(pinDataType);
                        else
                            output.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<object>)));
                    }
                });
            }) != null;
        }

        private void DefaultState()
        {
            dynamicInputPin.AdjustPinCount(2);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.DefaultState();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            if (dynamicInputPin.Count == 0)
                DefaultState();
        }

        public IOutputPin OutputPin
        {
            get { return output; }
        }

        public PinDirection InterfaceDirection
        {
            get { return PinDirection.Input; }
        }

        public IPin AddInterfacePin(string id, bool loading = false)
        {
            return dynamicInputPin.AddPin(id, loading);
        }

        public bool RemoveInterfacePin(string id)
        {
            return dynamicInputPin.RemovePin(id);
        }

        public override bool InputAddable
        {
            get { return true; }
        }

        public bool ReorderPins(IList<string> objectIds)
        {
            return this.inputs.Reorder(objectIds) != null;
        }

        [EvaluateInternal]
        private ISequence<T> EvaluateInternal<T>(ISequence[] input)
        {
            return input.Cast<ISequence<T>>().ToSequence().Concat();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var result = genericDelegate.Delegate(inputs.Cast<ISequence>().ToArray());

            return Task.FromResult(new object[] { result });
        }
    }
}
