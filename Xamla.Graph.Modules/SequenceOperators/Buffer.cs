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
    [Module(ModuleType = "Xamla.Sequence.Operators.Buffer")]
    public class Buffer
        : ModuleBase
    {
        GenericInputPin input;
        GenericInputPin count;
        GenericOutputPin output;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public Buffer(IGraphRuntime runtime)
            : base(runtime)
        {
            this.input = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.count = AddInputPin("Count", PinDataTypeFactory.Create<int?>(null, WellKnownEditors.IntNumber), PropertyMode.Default);
            this.output = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(typeof(IList<>))));

            this.input.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                    {
                        genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                        output.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(typeof(IList<>).MakeGenericType(genericType))));
                    }
                    else
                    {
                        genericDelegate = null;
                        output.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(typeof(IList<>))));
                    }
                });
            });
        }

        public IInputPin CountPin
        {
            get { return count; }
        }

        public IInputPin InputPin
        {
            get { return input; }
        }

        public IOutputPin OutputPin
        {
            get { return output; }
        }

        [EvaluateInternal]
        private object EvaluateInternal<T>(ISequence<T> input, int? count)
        {
            return count.HasValue ? input.Buffer(count.Value) : input.Buffer();
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var result = genericDelegate.Delegate(inputs[0], inputs[1]);

            return Task.FromResult(new object[] { result });
        }
    }
}
