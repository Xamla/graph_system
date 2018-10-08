using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.FlowOperators
{
    [Module(ModuleType = "Xamla.Flow.ForEach", Flow = true)]
    public class ForEach
        : SubGraphModule
    {
        public static string SUBGRAPH_SOURCE_PIN_ID = "Value";

        readonly GenericInputPin inputPin;
        readonly GenericOutputPin subGraphSource;

        public ForEach(IGraphRuntime runtime)
            : base(runtime, false, (IPinDataType)null, SUBGRAPH_SOURCE_PIN_ID)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);

            this.subGraphSource = ((GenericOutputPin)this.subGraphInputModule.Outputs[SUBGRAPH_SOURCE_PIN_ID]);

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    subGraphSource.ChangeType(PinDataTypeFactory.FromType(genericType));
                });
            });
        }

        public IInputPin InputPin =>
            inputPin;

        public override async Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            var sequence = (ISequence)inputs[1];
            using (var iter = sequence.Start(null))
            {
                while (await iter.MoveNext(cancel))
                {
                    var value = iter.Current;
                    var subGraphTask = (Task)subGraphDelegate.DynamicInvoke(Flow.Default, value, cancel);
                    await subGraphTask;
                }
            }

            return new object[] { Flow.Default };
        }
    }
}
