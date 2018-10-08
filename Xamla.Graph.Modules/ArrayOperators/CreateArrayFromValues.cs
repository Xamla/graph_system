using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.ArrayOperators
{
    [Module(ModuleType = "Xamla.Utilities.CreateArrayFromValues")]
    public class CreateArrayFromValues
        : StaticMethodModule
    {
        public CreateArrayFromValues(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        public static object[] ModuleMain(
            [InputPin(PropertyMode = PropertyMode.Allow)] params object[] values)
        {
            return values;
        }
    }


    [Module(ModuleType = "Xamla.Utilities.CreateArray")]
    public class CreateArray
        : ModuleBase
        , IInterfaceModule
    {
        DynamicInputPin dynamicInputPin;
        GenericOutputPin output;

        GenericDelegate<Func<object, object>> genericDelegate;
        Type outputType;

        public CreateArray(IGraphRuntime runtime)
            : base(runtime)
        {
            this.dynamicInputPin = new DynamicInputPin(runtime, inputs, "Input", PinDataTypeFactory.CreateAny(), OnDynamicInputAdd);
            this.output = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(Array)));
        }

        private bool OnDynamicInputAdd(string id)
        {
            return dynamicInputPin.Pin(id).WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.CreateAny(), false, pinDataType =>
                {
                    var dataType = pinDataType.UnderlyingType;
                    this.outputType = dataType;
                    var count = this.dynamicInputPin.Count;
                    var outputType = Array.CreateInstance(dataType, count).GetType();
                    this.output.ChangeType(PinDataTypeFactory.FromType(outputType));

                    if (dataType != null)
                        genericDelegate = new GenericDelegate<Func<object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(dataType));
                    else
                        genericDelegate = null;
                });
            }) != null;
        }

        public static object[] ModuleMain(
            [InputPin(PropertyMode = PropertyMode.Allow)] params object[] values)
        {
            return values;
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
        private TResult[] EvaluateInternal<TResult>(object[] input)
        {
            return input.Select(x => (TResult)System.Convert.ChangeType(x, outputType, CultureInfo.InvariantCulture)).ToArray();
        }


        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var result = (Array)genericDelegate.Delegate(inputs);
            var type = result.GetType();
            return Task.FromResult( new object[] { result });
        }
    }
}
