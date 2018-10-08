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
    [Module(ModuleType = "Xamla.Sequence.Operators.SelectManyFunc")]   
    public class SelectManyFunc
        : ModuleBase
    {
        public static string SUBGRAPH_PARAM = "Source";

        private GenericInputPin inputPin;
        private GenericInputPin funcPin;
        private GenericInputPin sequentialPin;
        private GenericOutputPin outputPin;

        public SelectManyFunc(IGraphRuntime runtime)
            : base(runtime)
        {
            inputPin = AddInputPin("Input", PinDataTypeFactory.Create<ISequence>(), PropertyMode.Never);
            funcPin = AddInputPin("Function", PinDataTypeFactory.Create<Delegate>(), PropertyMode.Allow);
            sequentialPin = AddInputPin("Sequential", PinDataTypeFactory.Create<bool>(true), PropertyMode.Default);
            outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());
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

        private ISequence<object> Evaluate(ISequence input, Delegate func, bool sequential)
        {
            var selector = (Func<object, CancellationToken, Task<object>>)func;
            var sequences = input.AsObjects().SelectAsync(selector).Cast<ISequence>().Select(x => x.AsObjects());
            return sequential ? sequences.Concat() : sequences.Merge();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var input = (ISequence)inputs[0];
            var func = (Delegate)inputs[1];
            var sequential = (bool)inputs[2];

            var result = Evaluate(input, func, sequential);

            return Task.FromResult(new object[] { result });
        }
    }
}
