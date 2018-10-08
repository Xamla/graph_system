using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.Count")]
    public class Count
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        public Count(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.CreateInt32());

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true);
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

        protected override async Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var input = (ISequence)inputs[0];

            var result = await input.Count().FirstAsync(cancel).ConfigureAwait(false);

            return new object[] { result };
        }
    }
}
