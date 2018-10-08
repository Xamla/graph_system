using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.Catch", Flow = true, FlowMode = FlowMode.WaitAny | FlowMode.EvaluateWithException)]
    public class Catch
        : ModuleBase
    {
        public Catch(IGraphRuntime runtime)
            : base(runtime)
        {
            this.AddOutputPin("OnError", PinDataTypeFactory.CreateFlow(), 1);
            this.AddOutputPin("Exception", PinDataTypeFactory.Create<Exception>());
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            Flow flow = (Flow)inputs[0];
            if (flow.IsFaulted)
            {
                return Task.FromResult(new object[] { null, Flow.Default, flow.Exception });
            }
            else
            {
                return Task.FromResult(new object[] { Flow.Default, null, null });
            }
        }
    }
}
