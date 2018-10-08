using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.All")]
    public class All
        : SubGraphModule
    {
        public static string SUBGRAPH_SOURCE_PIN_ID = "Source";

        GenericInputPin sourceInput;
        GenericOutputPin output;

        GenericDelegate<Func<object, object, object, Task<bool>>> genericDelegate;
        GenericOutputPin subGraphInput;

        public All(IGraphRuntime runtime)
            : base(runtime, false, PinDataTypeFactory.Create<bool>(), SUBGRAPH_SOURCE_PIN_ID)
        {
            this.sourceInput = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.output = AddOutputPin("Output", PinDataTypeFactory.CreateBoolean());

            this.subGraphInput = ((GenericOutputPin)this.subGraphInputModule.Outputs[SUBGRAPH_SOURCE_PIN_ID]);

            this.sourceInput.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    subGraphInput.ChangeType(PinDataTypeFactory.FromType(genericType));

                    BuildGenericDelegate();
                });
            });
        }

        public IInputPin SourceInputPin
        {
            get { return sourceInput; }
        }

        public IOutputPin OutputPin
        {
            get { return output; }
        }

        private void BuildGenericDelegate()
        {
            var inputType = subGraphInput.DataType.UnderlyingType;

            genericDelegate = new GenericDelegate<Func<object, object, object, Task<bool>>>(this, EvaluateInternalAttribute.GetMethod(GetType()));
        }

        [EvaluateInternal]
        private Task<bool> EvaluateInternal<T>(ISequence<T> source, Delegate subGraphDelegate, CancellationToken cancel)
        {
            var predicate = (Func<T, CancellationToken, Task<bool>>)subGraphDelegate;

            return source.SelectAsync(predicate).All(x => x == true).FirstAsync(null, cancel);
        }

        public override async Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            bool result = await genericDelegate.Delegate(inputs[0], subGraphDelegate, cancel).ConfigureAwait(false);

            return new object[] { result };
        }
    }
}
