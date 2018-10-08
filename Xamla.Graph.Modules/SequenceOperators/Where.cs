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
    [Module(ModuleType = "Xamla.Sequence.Operators.Where")]   
    public class Where
        : SubGraphModule
    {
        public static string SUBGRAPH_SOURCE_PIN_ID = "Source";

        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;
        GenericOutputPin subGraphSource;
        GenericInputPin subGraphResult;

        public Where(IGraphRuntime runtime)
            : base(runtime, false, PinDataTypeFactory.Create<bool>(), SUBGRAPH_SOURCE_PIN_ID)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());

            this.subGraphSource = ((GenericOutputPin)this.subGraphInputModule.Outputs[SUBGRAPH_SOURCE_PIN_ID]);
            this.subGraphResult = this.SubGraph.OutputModule.DefaultInputPin;

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    subGraphSource.ChangeType(PinDataTypeFactory.FromType(genericType));

                    var outputType = PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(genericType));
                    if (outputPin.DataType.UnderlyingType != outputType.UnderlyingType)
                        outputPin.ChangeType(outputType);

                    genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                });
            });
        }

        public IInputPin InputPin
        {
            get { return inputPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        [EvaluateInternal]
        private ISequence<T> EvaluateInternal<T>(ISequence<T> input, Delegate subGraphDelegate)
        {
            var predicate = (Func<T, CancellationToken, Task<bool>>)subGraphDelegate;
            return input.WhereAsync(predicate);
        }

        public override Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];

            var result = genericDelegate.Delegate(inputs[0], subGraphDelegate);

            return Task.FromResult(new object[] { result });
        }
    }
}
