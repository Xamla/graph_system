using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using System.Reactive.Linq;
using System.Threading;

namespace Xamla.Graph.Modules.ArrayOperators
{
    [Module(ModuleType = "Xamla.Utilities.GetValuesFromArray")]
    public class GetValuesFromArray
        : ModuleBase
    {
        public const string COUNT_PIN_NAME = "count";
        DynamicOutputPin dynamicOutputPin;

        public GetValuesFromArray(IGraphRuntime runtime)
            : base(runtime)
        {
            this.AddInputPin(COUNT_PIN_NAME, PinDataTypeFactory.FromType(typeof(int)), PropertyMode.Always);
            this.AddInputPin("array", PinDataTypeFactory.FromType(typeof(Array)), PropertyMode.Never);

            this.AddOutputPin("OutputRemainings", PinDataTypeFactory.FromType(typeof(Array)));
            this.dynamicOutputPin = new DynamicOutputPin(runtime, outputs, "Output", PinDataTypeFactory.Create<object>());

            this.properties[COUNT_PIN_NAME]
                .WhenNodeEvent
                .OfType<PropertyChangedEvent>()
                .Subscribe(x => UpdateOutputs((int)x.Value.Value));
        }

        private void UpdateOutputs(int count)
        {
            // increase count by one because output pin for remaining items are not in count included
            var dif = count + 1 - this.outputs.Count;

            if (dif > 0)
            {
                for (int i = 0; i < dif; i++)
                {
                    this.dynamicOutputPin.AddPin();
                }
            }
            else if (dif < 0)
            {
                for (int i = 0; i < -dif; i++)
                {
                    this.dynamicOutputPin.RemoveLastPin();
                }
            }
        }

        private object[] ModuleMain(Array array, int count)
        {
            var newArray = new object[count + 1];

            if (newArray.Length > 0)
                Array.Copy(array, 0, newArray, 1, count);

            var remainings = new object[array.Length - count];

            if (remainings.Length > 0)
                Array.Copy(array, count, remainings, 0, remainings.Length);

            newArray[0] = array; // remainings
            return newArray;
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var count = (int)inputs[0];
            var array = (Array)inputs[1];

            return Task.FromResult(ModuleMain(array, count));
        }
    }
}
