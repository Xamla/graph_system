using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.If")]
    public class If
        : ModuleBase
        , IInterfaceModule
    {
        readonly GenericInputPin condition;
        readonly DynamicInputPin arguments;

        readonly GenericOutputPin trueOut;
        readonly GenericOutputPin falseOut;

        public If(IGraphRuntime runtime)
            : base(runtime)
        {
            this.flowMode = FlowMode.WaitAny;
            this.AddInputPin("flowIn", PinDataTypeFactory.CreateFlow(), PropertyMode.Never);
            this.condition = this.AddInputPin("condition", PinDataTypeFactory.CreateString("x > 0"), PropertyMode.Default);
            this.arguments = new DynamicInputPin(runtime, base.inputs, "x", PinDataTypeFactory.CreateAny(), OnDynamicInputAdd) { Renameable = true };
            this.AddInterfacePin("x");
            this.trueOut = this.AddOutputPin("true", PinDataTypeFactory.CreateFlow(), 1);
            this.falseOut = this.AddOutputPin("false", PinDataTypeFactory.CreateFlow(), 1);

            this.DynamicDisplayName = new DynamicDisplayNameFunc(FormatDisplayName);
        }

        private string FormatDisplayName()
        {
            if (this.properties.IsConnected(condition))
            {
                string conditionExpression = this.properties.Get<string>("condition");
                return $"If ({conditionExpression})";
            }
            return null;
        }

        private bool OnDynamicInputAdd(string id)
        {
            arguments.Pin(id).WhenNodeEvent.Subscribe(evt => PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.CreateAny(), false));
            return true;
        }

        public override bool InputAddable => true;
        public PinDirection InterfaceDirection => PinDirection.Input;
        public IPin AddInterfacePin(string id = null, bool loading = false) => arguments.AddPin(id, loading, true);
        public bool RemoveInterfacePin(string id) => arguments.RemovePin(id);
        public bool ReorderPins(IList<string> objectIds) => arguments.Reorder(objectIds);

        // AKo: Using workaround here since CSharp script does not directly support dynamic global scopes (e.g. Dictionary or Expando)
        // see https://github.com/dotnet/roslyn/issues/3194
        public class Globals
        {
            public Dictionary<string, object> Inputs;
        }

        protected override async Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            string condition = (string)inputs[1];

            var globals = new Globals { Inputs = new Dictionary<string, object>() };
            var sb = new StringBuilder();
            for (int i = 1; i < this.Inputs.Count; i++)
            {
                var inputPin = this.inputs[i];
                string pinId = inputPin.Id;
                globals.Inputs.Add(this.inputs[i].Id, inputs[i]);
                string fullType = inputPin.DataType.UnderlyingType.FullName;
                sb.AppendFormat("{0} {1} = ({0})Inputs[\"{1}\"];\n", fullType, pinId);
            }

            sb.Append(condition);

            var result = (bool)await CSharpScript.EvaluateAsync(sb.ToString(), ScriptOptions.Default, globals, typeof(Globals), cancel);
            if (result)
                return new object[] { Flow.Default, null };
            else
                return new object[] { null, Flow.Default };
        }
    }
}
