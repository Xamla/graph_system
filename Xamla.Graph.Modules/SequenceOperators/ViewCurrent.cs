using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.ViewCurrent")]
    public class ViewCurrent
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericInputPin data;

        public ViewCurrent (IGraphRuntime runtime)
            : base(runtime, ModuleKind.ViewModule)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<string>), string.Empty, WellKnownEditors.Hidden, null, null), PropertyMode.Never);
            this.data = AddInputPin("data", PinDataTypeFactory.CreateString(), PropertyMode.Always);
            EnableVirtualOutputPin();
        }

        protected async override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var values = (ISequence<string>)inputs[0];

            using ( var text = values.Start(cancel))
            {
                while (await text.MoveNext(cancel))
                {
                    var current = text.Current;
                    Show(current);
                }
            }
            return new Object[0];
        }

        void Show(string input)
        {
            this.Properties.SetValue("data", input);
        }
    }
}
