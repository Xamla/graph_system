using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceSources
{
    [Module(ModuleType = "Xamla.Sequence.Sources.Never")]   
    public class Never
        : ModuleBase
    {
        private GenericOutputPin outputPin;

        public Never(IGraphRuntime runtime)
            : base(runtime)
        {
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        private ISequence<object> Evaluate()
        {
            return Sequence.Never<object>();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var result = Evaluate();

            return Task.FromResult(new object[] { result });
        }
    }
}
