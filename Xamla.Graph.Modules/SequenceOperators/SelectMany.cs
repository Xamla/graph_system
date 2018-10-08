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
    [Module(ModuleType = "Xamla.Sequence.Operators.SelectMany")]   
    public class SelectMany
        : SubGraphModule
    {
        public static string SUBGRAPH_SOURCE_PIN_ID = "Source";

        GenericInputPin inputPin;
        GenericInputPin sequentialPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object, object, object>> genericDelegate;
        GenericOutputPin subGraphSource;
        GenericInputPin subGraphResult;
        GenericInputPin maxConcurrencyPin;

        public SelectMany(IGraphRuntime runtime)
            : base(runtime, false, PinDataTypeFactory.FromType(typeof(ISequence<>)), SUBGRAPH_SOURCE_PIN_ID)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.sequentialPin = AddInputPin("Sequential", PinDataTypeFactory.Create<bool>(true), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());
            this.maxConcurrencyPin = AddInputPin("MaxConcurrency", PinDataTypeFactory.Create<int>(8), PropertyMode.Default);

            this.subGraphSource = ((GenericOutputPin)this.subGraphInputModule.Outputs[SUBGRAPH_SOURCE_PIN_ID]);
            this.subGraphResult = this.SubGraph.OutputModule.DefaultInputPin;

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    subGraphSource.ChangeType(PinDataTypeFactory.FromType(genericType));

                    BuildGenericDelegate();
                });
            });

            this.subGraphResult.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    outputPin.ChangeType(pinDataType);

                    BuildGenericDelegate();
                });
            });
        }

        private void BuildGenericDelegate()
        {
            var inputType = subGraphSource.DataType.UnderlyingType;
            var outputType = subGraphResult.DataType.UnderlyingType;

            var outputElementType = outputType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);

            genericDelegate = new GenericDelegate<Func<object, object, object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(inputType, outputElementType));
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
        private ISequence<TOut> EvaluateInternal<T, TOut>(ISequence<T> input, bool sequential, int maxConcurrency, Delegate subGraphDelegate)
        {
            var selector = (Func<T, CancellationToken, Task<ISequence<TOut>>>)subGraphDelegate;
            var sequences = input.SelectAsync(selector);
            return sequential ? sequences.Concat() : sequences.Merge(maxConcurrency);
        }

        public override Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];
            var sequential = inputs[1];
            var maxConcurrency = inputs[2];

            var result = genericDelegate.Delegate(input, sequential, maxConcurrency, subGraphDelegate);

            return Task.FromResult(new object[] { result });
        }
    }
}
