using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceSources
{
    [Module(ModuleType = "Xamla.Sequence.Sources.Empty")]
    public class Empty
        : ModuleBase
    {
        private GenericOutputPin outputPin;

        public Empty(IGraphRuntime runtime)
            : base(runtime)
        {
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var result = Evaluate();

            return Task.FromResult(new object[] { result });
        }

        public ISequence<object> Evaluate()
        {
            return Sequence.Empty<object>();
        }
    }
}
