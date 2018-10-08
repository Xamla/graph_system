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
    [Module(ModuleType = "Xamla.Sequence.Sources.GaussianRandomSequence")]
    class GaussianRandomSequence
        : ModuleBase
    {
        GenericInputPin seedPin;
        GenericInputPin muPin;
        GenericInputPin sigmaPin;
        GenericOutputPin outputPin;

        public GaussianRandomSequence(IGraphRuntime runtime)
            : base(runtime)
        {
            this.seedPin = AddInputPin("Seed", PinDataTypeFactory.Create<int?>(null, WellKnownEditors.IntNumber), PropertyMode.Default);
            this.muPin = AddInputPin("µ", PinDataTypeFactory.Create<double>(0, WellKnownEditors.FloatNumber), PropertyMode.Default);
            this.sigmaPin = AddInputPin("σ", PinDataTypeFactory.Create<double>(1, WellKnownEditors.FloatNumber), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<double>>());
        }

        public IInputPin SeedPin
        {
            get { return seedPin; }
        }

        public IInputPin MuPin
        {
            get { return muPin; }
        }

        public IInputPin SigmaPin
        {
            get { return sigmaPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        private ISequence<double> Evaluate(int? seed, double mu, double sigma)
        {
            var rng = seed.HasValue ? new Random(seed.Value) : ThreadSafeRandom.Generator;
            return rng.Normal(mu, sigma).ToSequence();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            int? seed = (int?)inputs[0];
            double mu = (double)inputs[1];
            double sigma = (double)inputs[2];

            var result = Evaluate(seed, mu, sigma);

            return Task.FromResult(new object[] { result });
        }
    }
}
