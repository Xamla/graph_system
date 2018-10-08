using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceSources
{
    [Module(ModuleType = "Xamla.Sequence.Sources.Interval")]   
    public class Interval
        : ModuleBase
    {
        private GenericInputPin periodPin;
        private GenericOutputPin outputPin;

        public Interval(IGraphRuntime runtime)
            : base(runtime)
        {
            this.periodPin = AddInputPin("Period", PinDataTypeFactory.CreateTimeSpan(), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<long>>());
        }

        public IInputPin PeriodPin
        {
            get { return periodPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        private ISequence<long> Evaluate(TimeSpan period)
        {
            return Observable.Interval(period).ToSequence();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var period = (TimeSpan)inputs[0];
            var result = Evaluate(period);

            return Task.FromResult(new object[] { result });
        }
    }
}
