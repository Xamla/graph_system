using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.Fork")]
    public class Fork
        : ModuleBase
        , IInterfaceModule
    {
        readonly DynamicOutputPin flowOutputs;

        public Fork(IGraphRuntime runtime)
            : base(runtime)
        {
            this.flowMode = FlowMode.WaitAny;
            this.AddInputPin("flowIn", PinDataTypeFactory.CreateFlow(), PropertyMode.Never);
            this.flowOutputs = new DynamicOutputPin(runtime, outputs, "Flow", PinDataTypeFactory.CreateFlow());
        }

        public override bool OutputAddable => true;
        public PinDirection InterfaceDirection => PinDirection.Output;
        public IPin AddInterfacePin(string id = null, bool loading = false) => flowOutputs.AddPin(loading ? id : null, loading);
        public bool RemoveInterfacePin(string id) => flowOutputs.RemovePin(id);
        public bool ReorderPins(IList<string> objectIds) => flowOutputs.Reorder(objectIds);

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var result = new object[this.flowOutputs.Count];
            Array.Fill(result, Flow.Default);
            return Task.FromResult(result);
        }
    }
}
