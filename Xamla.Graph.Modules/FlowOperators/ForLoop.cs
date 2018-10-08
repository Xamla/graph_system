using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.ForLoop", Flow = true, FlowMode = FlowMode.WaitAny)]
    public class ForLoop
        : SubGraphModule
    {
        public static string SUBGRAPH_INDEX_PIN_ID = "i";

        public ForLoop(IGraphRuntime runtime)
            : base(runtime, false, (IPinDataType)null)
        {
            subGraph.InputModule.AddModulePin(SUBGRAPH_INDEX_PIN_ID, false, PinDataTypeFactory.CreateInt32());
            subGraph.OutputModule.AddModulePin("Break", PinDataTypeFactory.CreateFlow(), PinFlags.None, null);

            this.AddInputPin("startValue", PinDataTypeFactory.CreateInt32(0), PropertyMode.Default);    // Initial value for counting
            this.AddInputPin("increment", PinDataTypeFactory.CreateInt32(1), PropertyMode.Default);     // Inrement of the counter variable after each evaluation of the loop body.
            this.AddInputPin("endValue", PinDataTypeFactory.CreateInt32(100), PropertyMode.Default);    // Exit loop when the counter variable becomes greater or equal to this value.

            this.DynamicDisplayName = new DynamicDisplayNameFunc(FormatDisplayName);
        }

        private string FormatDisplayName()
        {
            string GetPropertyValue(string name)
            {
                var p = this.Properties.GetProperty(name);
                return p.Connect ? FormattableString.Invariant($"{p.Value}") : "?";
            }

            return $"ForLoop ({GetPropertyValue("startValue")}, {GetPropertyValue("endValue")}, {GetPropertyValue("increment")})";
        }

        public override async Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            var body = (Func<Flow, int, CancellationToken, Task<Tuple<Flow, Flow>>>)subGraphDelegate;

            int startValue = (int)inputs[1];
            int increment = (int)inputs[2];
            int endValue = (int)inputs[3];

            for (int i = startValue; i < endValue; i += increment)
            {
                var loopResult = await body(Flow.Default, i, cancel);
                if (loopResult.Item2 != null)
                    break;
            }

            return new object[] { Flow.Default };
        }
    }
}
