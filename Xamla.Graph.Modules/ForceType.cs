using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Converter.ForceType", Description = "Examples: System.Object, System.Boolean, System.Double, System.Collections.Generic.IEnumerable`1[System.String]")]
    class ForceType
        : ModuleBase
    {
        GenericInputPin input;
        GenericOutputPin output;
        GenericInputPin type;

        public ForceType(IGraphRuntime runtime)
            : base(runtime)
        {
            this.input = AddInputPin("Input", PinDataTypeFactory.CreateAny(), PropertyMode.Never);
            this.output = AddOutputPin("Output", PinDataTypeFactory.CreateAny());
            this.type = AddInputPin("Type", PinDataTypeFactory.CreateString("System.Object"), PropertyMode.Always);
            this.properties[this.type.Id].WhenNodeEvent.OfType<PropertyChangedEvent>().Subscribe(x =>
            {

                var genericInputType = Type.GetType((string)x.Value.Value, false, true) ?? typeof(object);
                var dataType = PinDataTypeFactory.FromType(genericInputType);
                var errors = input.ChangeType(dataType).Concat(output.ChangeType(dataType));
                if (errors.Any())
                {
                    throw new AggregateException("One or more connections could not be reestablished after a data type change.", errors);
                }
            });
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            return Task.FromResult(new object[] { inputs[0] });
        }
    }
}
