using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Func")]
    public class Func
        : SubGraphModule
    {
        private GenericOutputPin funcPin;

        public Func(IGraphRuntime runtime)
            : base(runtime, true)
        {
            funcPin = AddOutputPin("Function", PinDataTypeFactory.Create<Delegate>());
        }

        public IOutputPin FuncPin
        {
            get { return funcPin; }
        }

        public override Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            return Task.FromResult(new object[] { subGraphDelegate });
        }
    }
}
