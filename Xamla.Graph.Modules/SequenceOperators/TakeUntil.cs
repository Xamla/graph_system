using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.TakeUntil")]
    public class TakeUntil
        : ModuleBase
    {
        GenericInputPin sourcePin;
        GenericInputPin otherPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public TakeUntil(IGraphRuntime runtime)
            : base(runtime)
        {
            this.sourcePin = AddInputPin("Source", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.otherPin = AddInputPin("Other", PinDataTypeFactory.FromType(typeof(ISequence)), PropertyMode.Never);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<>)));

            this.sourcePin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                        genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                    else
                        genericDelegate = null;

                    outputPin.ChangeType(pinDataType);
                });
            });
        }

        public IInputPin SourceInputPin
        {
            get { return sourcePin; }
        }

        public IInputPin OtherInputPin
        {
            get { return otherPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        [EvaluateInternal]
        private ISequence<T> EvaluateInternal<T>(ISequence<T> source, ISequence other)
        {
            return source.TakeUntil(other);
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var source = inputs[0];
            var other = inputs[1];

            var result = genericDelegate.Delegate(source, other);

            return Task.FromResult(new object[] { result });
        }
    }
}
