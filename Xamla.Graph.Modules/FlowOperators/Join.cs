using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.Join")]
    public class Join
        : ModuleBase
        , IInterfaceModule
    {
        readonly DynamicInputPin flowInputs;
        readonly GenericOutputPin flowOut;

        public Join(IGraphRuntime runtime)
            : base(runtime)
        {
            this.flowMode = FlowMode.WaitAllOrFirstException;

            this.flowInputs = new DynamicInputPin(runtime, inputs, "Flow", PinDataTypeFactory.CreateFlow())
            {
                MaxConnections = null
            };
            this.flowOut = this.AddOutputPin("flowOut", PinDataTypeFactory.CreateFlow(), 1);
        }

        public override bool InputAddable => true;
        public PinDirection InterfaceDirection => PinDirection.Input;
        public IPin AddInterfacePin(string id = null, bool loading = false) => flowInputs.AddPin(loading ? id : null, loading);
        public bool RemoveInterfacePin(string id) => flowInputs.RemovePin(id);
        public bool ReorderPins(IList<string> objectIds) => flowInputs.Reorder(objectIds);

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel) =>
            Task.FromResult(new object[] { Flow.Default });
    }
}
