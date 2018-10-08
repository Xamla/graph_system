using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceSources
{
    [Module(ModuleType = "Xamla.Sequence.Sources.Return")]
    public class Return
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object>> genericDelegate;

        public Return(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Value", PinDataTypeFactory.CreateAny(), PropertyMode.Allow);
            this.outputPin = AddOutputPin("Sequence", PinDataTypeFactory.Create<ISequence<object>>());

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.CreateAny(), false, pinDataType =>
                {
                    var method = EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(pinDataType.UnderlyingType);
                    genericDelegate = new GenericDelegate<Func<object, object>>(this, method);

                    outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(pinDataType.UnderlyingType)));
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
        private ISequence<T> EvaluateInternal<T>(T value)
        {
            return Sequence.Return(value);
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
