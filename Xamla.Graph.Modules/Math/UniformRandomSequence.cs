using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Sequence.Sources.UniformRandomSequence")]
    class UniformRandomSequence
        : ModuleBase
    {
        GenericInputPin seedPin;
        GenericOutputPin outputPin;

        public UniformRandomSequence(IGraphRuntime runtime)
            : base(runtime)
        {
            this.seedPin = AddInputPin("Seed", PinDataTypeFactory.Create<int?>(null, WellKnownEditors.IntNumber), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<double>>());
        }

        public IInputPin SeedPin
        {
            get { return seedPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        private ISequence<double> Evaluate(int? seed)
        {
            var rng = seed.HasValue ? new Random(seed.Value) : ThreadSafeRandom.Generator;
            return rng.DoubleSequence().ToSequence();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            int? seed = (int?)inputs[0];

            var result = Evaluate(seed);

            return Task.FromResult(new object[] { result });
        }
    }
}
