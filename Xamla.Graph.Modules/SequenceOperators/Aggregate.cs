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
    [Module(ModuleType = "Xamla.Sequence.Operators.Aggregate")]   
    public class Aggregate
        : SubGraphModule
    {
        public static string SUBGRAPH_SOURCE_PIN_ID = "Source";
        public static string SUBGRAPH_ACCUMULATE_PIN_ID = "Accumulate";

        GenericInputPin sourceInput;
        GenericInputPin seedInput;
        GenericOutputPin output;

        GenericDelegate<Func<object, object, object, object, object>> genericDelegate;
        GenericOutputPin subGraphInput;
        GenericOutputPin subGraphAccumulate;
        GenericInputPin subGraphOutput;

        public Aggregate(IGraphRuntime runtime)
            : base(runtime, false, SUBGRAPH_SOURCE_PIN_ID, SUBGRAPH_ACCUMULATE_PIN_ID)
        {
            this.sourceInput = AddInputPin("Source", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.seedInput = AddInputPin("Seed", PinDataTypeFactory.CreateAny(), PropertyMode.Allow);
            this.output = AddOutputPin("Output", PinDataTypeFactory.CreateAny());

            this.subGraphInput = ((GenericOutputPin)this.subGraphInputModule.Outputs[SUBGRAPH_SOURCE_PIN_ID]);
            this.subGraphAccumulate = (GenericOutputPin)this.subGraphInputModule.Outputs[SUBGRAPH_ACCUMULATE_PIN_ID];
            this.subGraphOutput = subGraph.OutputModule.DefaultInputPin;

            this.sourceInput.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    subGraphInput.ChangeType(PinDataTypeFactory.FromType(genericType));

                    BuildGenericDelegate();
                });
            });

            this.seedInput.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.CreateAny(), false, pinDataType =>
                {
                    subGraphAccumulate.ChangeType(pinDataType);
                    output.ChangeType(pinDataType);
                    subGraphOutput.ChangeType(pinDataType);

                    BuildGenericDelegate();
                });
            });
        }

        private void BuildGenericDelegate()
        {
            var inputType = subGraphInput.DataType.UnderlyingType;
            var accumulateType = seedInput.DataType.UnderlyingType;

            var method = EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(inputType, accumulateType);
            genericDelegate = new GenericDelegate<Func<object, object, object, object, object>>(this, method);
        }

        public IInputPin SourceInputPin
        {
            get { return sourceInput; }
        }
        public IInputPin SeedInputPin
        {
            get { return seedInput; }
        }

        public IOutputPin OutputPin
        {
            get { return output; }
        }

        [EvaluateInternal]
        private object EvaluateInternal<T, TAccumulate>(ISequence<T> source, TAccumulate seed, Delegate subGraphDelegate, CancellationToken cancel)
        {
            var selector = (Func<T, TAccumulate, CancellationToken, Task<TAccumulate>>)subGraphDelegate;

            return source.Aggregate<T, TAccumulate>(seed, (a, o) => selector(o, a, cancel).Result).LastAsync(cancel).Result;
        }

        public override Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var result = genericDelegate.Delegate(inputs[0], inputs[1], subGraphDelegate, cancel);

            return Task.FromResult(new object[] { result });
        }
    }
}
