using System;
using System.Linq;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Sequence.SequenceToArray", Description = "Evaluates a sequence and returns an array with all elements.")]
    public class SequenceToArray
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object>> genericDelegate;

        public SequenceToArray(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Collapsed)
        {
            this.inputPin = AddInputPin("Sequence", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Allow);
            this.outputPin = AddOutputPin("Array", PinDataTypeFactory.FromType(typeof(object[])));

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var method = EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(pinDataType.UnderlyingType);

                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? pinDataType.UnderlyingType;
                    if (genericType != null)
                        genericDelegate = new GenericDelegate<Func<object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                    else
                        genericDelegate = null;

                    outputPin.ChangeType(PinDataTypeFactory.FromType(genericType.MakeArrayType()));
                });
            });
        }

        public IInputPin StartPin
        {
            get { return inputPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        [EvaluateInternal]
        private T[] EvaluateInternal<T>(ISequence<T> seq)
        {
            return seq.ToList().ToArray();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];

            var result = genericDelegate.Delegate(input);

            return Task.FromResult(new object[] { result });
        }
    }
}
