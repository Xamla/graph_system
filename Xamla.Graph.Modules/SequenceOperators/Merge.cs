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
    [Module(ModuleType = "Xamla.Sequence.Operators.Merge")]
    public class Merge
        : ModuleBase
        , IInterfaceModule
    {
        DynamicInputPin dynamicInputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object>> genericDelegate;

        public Merge(IGraphRuntime runtime)
            : base(runtime)
        {
            this.dynamicInputPin = new DynamicInputPin(runtime, inputs, "Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), OnDynamicInputAdd);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<>)));
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
                        pin.ChangeType(pinDataType);

                    if (!pinsToChange.Any())
                        outputPin.ChangeType(pinDataType);
                });
            }) != null;
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        [EvaluateInternal]
        private ISequence<T> EvaluateInternal<T>(ISequence[] inputs)
        {
            return inputs.Cast<ISequence<T>>().MergeN();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var result = genericDelegate.Delegate(inputs.Cast<ISequence>().ToArray());

            return Task.FromResult(new object[] { result });
        }

        public override bool InputAddable
        {
            get { return true; }
        }

        public PinDirection InterfaceDirection
        {
            get { return PinDirection.Input; }
        }

        public IPin AddInterfacePin(string id = null, bool loading = false)
        {
            return dynamicInputPin.AddPin(id, loading);
        }

        public bool RemoveInterfacePin(string id)
        {
            return dynamicInputPin.RemovePin(id);
        }

        public bool ReorderPins(IList<string> objectIds)
        {
            return this.inputs.Reorder(objectIds) != null;
        }
    }
}
