using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Graph.Modules;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.SelectFunc")]   
    public class SelectFunc
        : ModuleBase
    {
        public static string SUBGRAPH_PARAM = "Source";

        private GenericInputPin inputPin;
        private GenericInputPin funcPin;
        private GenericOutputPin outputPin;

        public SelectFunc(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.Create<ISequence>(), PropertyMode.Never);
            this.funcPin = AddInputPin("Function", PinDataTypeFactory.Create<Delegate>(), PropertyMode.Allow);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());
        }

        public IInputPin InputPin
        {
            get { return inputPin; }
        }

        public IInputPin FuncPin
        {
            get { return funcPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        private ISequence<object> Evaluate(ISequence input, Delegate func)
        {
            var selector = (Func<object, CancellationToken, Task<object>>)func;
            return input.AsObjects().SelectAsync(selector);
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var input = (ISequence)inputs[0];
            var func = (Delegate)inputs[1];

            var result = Evaluate(input, func);

            return Task.FromResult(new object[] { result });
        }
    }
}
