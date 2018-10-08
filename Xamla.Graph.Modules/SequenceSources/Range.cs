using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceSources
{
    [Module(ModuleType = "Xamla.Sequence.Sources.Range")]
    public class Range
        : ModuleBase
    {
        private readonly GenericInputPin startPin;
        private readonly GenericInputPin countPin;
        private readonly GenericOutputPin outputPin;

        public Range(IGraphRuntime runtime)
            : base(runtime)
        {
            this.startPin = AddInputPin("Start", PinDataTypeFactory.CreateInt32(0), PropertyMode.Default);
            this.countPin = AddInputPin("Count", PinDataTypeFactory.CreateInt32(10), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<int>>());
        }

        public IInputPin StartPin => startPin;
        public IInputPin CountPin => countPin;
        public IOutputPin OutputPin => outputPin;

        private ISequence<int> Evaluate(int start, int count) =>
            Sequence.Range(start, count);

        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            var start = (int)inputs[0];
            var count = (int)inputs[1];

            var result = Evaluate(start, count);

            return Task.FromResult(new object[] { result });
        }
    }
}
