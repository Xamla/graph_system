using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceSources
{
    [Module(ModuleType = "Xamla.Sequence.Sources.Timer")]
    public class Timer
        : ModuleBase
    {
        private GenericInputPin dueTimePin;
        private GenericOutputPin outputPin;

        public Timer(IGraphRuntime runtime)
            : base(runtime)
        {
            this.dueTimePin = AddInputPin("DueTime", PinDataTypeFactory.Create<DateTimeOffset>(), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<long>>());
        }

        public IInputPin DueTimePin
        {
            get { return dueTimePin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var dueTime = (DateTimeOffset)inputs[0];

            ISequence<long> result = Observable.Timer(dueTime).ToSequence();

            return Task.FromResult(new object[] { result });
        }
    }
}
