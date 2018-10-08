using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.FlowScope", Flow = true)]
    public class FlowScope
        : SubGraphModule
    {
        public FlowScope(IGraphRuntime runtime)
            : base(runtime, false, (IPinDataType)null)
        {
        }

        public override async Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            var subGraphTask = (Task)subGraphDelegate.DynamicInvoke(Flow.Default, cancel);
            await subGraphTask;
            return new object[] { Flow.Default };
        }
    }
}
