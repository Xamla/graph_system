using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.Throw", Flow = true)]
    public class Throw
        : ModuleBase
    {
        public Throw(IGraphRuntime runtime)
            : base(runtime)
        {
            this.AddInputPin("Message", PinDataTypeFactory.CreateString("An unspecified error occured."), PropertyMode.Default);
        }

        protected override async Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var message = (string)inputs[1];
            throw new Exception(message);
        }
    }
}
