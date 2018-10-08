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
    [Module(ModuleType = "Xamla.Sequence.Operators.Flatten")]   
    public class Flatten
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object>> genericDelegate;

        public Flatten(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(typeof(ISequence<>))), PropertyMode.Never);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<object>)));

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(typeof(ISequence<>))), false, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();
                    if (genericType != null)
                        genericType = genericType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                    {
                        genericDelegate = new GenericDelegate<Func<object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                        outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(genericType)));
                    }
                    else
                    {
                        outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<object>)));
                    }
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
        private ISequence<T> EvaluateInternal<T>(ISequence<ISequence<T>> input)
        {
            // ToDo: Switch between merge or parallel with property --
            // return input.SelectManySequential(x => x.AsObjects());

            return input.Merge();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];

            var result = genericDelegate.Delegate(input);

            return Task.FromResult(new object[] { result });
        }
    }
}
