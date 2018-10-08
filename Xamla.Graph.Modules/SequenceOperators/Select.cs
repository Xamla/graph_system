using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.Select")]   
    public class Select
        : SubGraphModule
    {
        public static string SUBGRAPH_SOURCE_PIN_ID = "Source";

        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object, object>> genericDelegate;
        GenericOutputPin subGraphSource;
        GenericInputPin subGraphResult;

        public Select(IGraphRuntime runtime)
            : base(runtime, false, SUBGRAPH_SOURCE_PIN_ID)
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

                    BuildGenericDelegate();
                });
            });

            this.subGraphResult.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.CreateAny(), false, pinDataType =>
                {
                    outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(pinDataType.UnderlyingType)));

                    BuildGenericDelegate();
                });
            });
        }

        private void BuildGenericDelegate()
        {
            var inputType = subGraphSource.DataType.UnderlyingType;
            var outputType = subGraphResult.DataType.UnderlyingType;

            genericDelegate = new GenericDelegate<Func<object, object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(inputType, outputType));
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
        private ISequence<TOut> EvaluateInternal<TIn, TOut>(ISequence<TIn> input, Delegate subGraphDelegate, CancellationToken cancel)
        {
            var func = (Func<TIn, CancellationToken, Task<TOut>>)subGraphDelegate;

            return input.SelectAsync(func);
        }

        public override Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var result = genericDelegate.Delegate(inputs[0], subGraphDelegate, cancel);

            return Task.FromResult(new object[] { result });
        }
    }
}
