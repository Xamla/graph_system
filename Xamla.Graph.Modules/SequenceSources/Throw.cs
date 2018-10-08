using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceSources
{
    [Module(ModuleType = "Xamla.Sequence.Sources.Throw")]
    public class Throw
        : ModuleBase
    {
        private GenericInputPin messagePin;
        private GenericOutputPin outputPin;

        public Throw(IGraphRuntime runtime)
            : base(runtime)
        {
            this.messagePin = AddInputPin("Message", PinDataTypeFactory.Create<string>(string.Empty, WellKnownEditors.MultiLineText), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            var message = (string)inputs[0];

            var result = Evaluate(message);

            return Task.FromResult(new object[] { result });
        }

        ISequence<object> Evaluate(string message)
        {
            return Sequence.Throw<object>(new Exception(message));
        }
    }
}
