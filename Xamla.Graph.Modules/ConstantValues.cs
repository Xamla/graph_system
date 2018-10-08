using System.Linq;
using System.Globalization;
using Xamla.Types.Sequence;
using System.Threading.Tasks;
using System.Threading;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Sequence.Sources.ConstantValues")]
    public class ConstantValues
        : ModuleBase
    {
        GenericInputPin listPin;
        GenericOutputPin valuesPin;

        public ConstantValues(IGraphRuntime runtime)
            : base(runtime)
        {
            listPin = AddInputPin("List", PinDataTypeFactory.Create<string>(string.Empty, WellKnownEditors.MultiLineText), PropertyMode.Default);
            valuesPin = AddOutputPin("Values", PinDataTypeFactory.Create<ISequence<double>>());
        }

        public IInputPin ListPin
        {
            get { return listPin; }
        }

        public string List
        {
            get { return this.properties.Get<string>(listPin.Id); }
            set { this.Properties.Set(listPin.Id, value); }
        }

        public IOutputPin ValuesPin
        {
            get { return valuesPin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var values = (string)inputs[0];
            return Task.FromResult(new object[] { values.Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToSequence() });
        }
    }
}
