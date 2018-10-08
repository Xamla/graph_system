using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types.Converters;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Converter")]
    public class ConverterModule
        : ModuleBase
    {
        GenericInputPin inputPin;
        GenericInputPin converterPin;
        GenericOutputPin outputPin;

        ITypeConverter typeConverter;
        Dictionary<string, ITypeConverter> converterByName = new Dictionary<string, ITypeConverter>();

        static string GetShortTypeName(Type type)
        {
            return type != null ? ObjectPinDataType.GetShortTypeName(type) : "any";
        }

        public ConverterModule(IGraphRuntime runtime)
            : base(runtime)
        {
            converterByName.Add("None", null);

            foreach (var c in runtime.TypeConverters)
            {
                string sourceTypeName = GetShortTypeName(c.SourceType);
                string destinationTypeName = GetShortTypeName(c.DestinationType);

                var converterName = string.Concat(sourceTypeName, " -> ", destinationTypeName);
                if (!converterByName.ContainsKey(converterName))
                    converterByName.Add(converterName, c);
            }

            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(object)), PropertyMode.Never);
            this.converterPin = AddInputPin("Converter", PinDataTypeFactory.CreateDynamicEnum(converterByName.Keys.ToArray(), "None"), PropertyMode.Always);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(object)));

            this.properties[converterPin.Id].WhenNodeEvent.OfType<PropertyChangedEvent>().Subscribe(x =>
            {
                var converterName = (string)x.Value.Value;
                converterByName.TryGetValue(converterName, out typeConverter);

                IPinDataType sourceType, destinationType;
                if (typeConverter == null)
                {
                    sourceType = PinDataTypeFactory.CreateAny();
                    destinationType = PinDataTypeFactory.CreateAny();
                }
                else
                {
                    sourceType = typeConverter.SourceType != null ? PinDataTypeFactory.FromType(typeConverter.SourceType) : PinDataTypeFactory.CreateAny();
                    destinationType = typeConverter.DestinationType != null ? PinDataTypeFactory.FromType(typeConverter.DestinationType) : PinDataTypeFactory.CreateAny();
                }

                var errors = inputPin.ChangeType(sourceType).Concat(outputPin.ChangeType(destinationType));
                if (errors.Any())
                {
                    throw new AggregateException("One or more connections could not be reestablished after a data type change.", errors);
                }
            });
        }

        protected override async Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            var input = inputs[0];
            var converter = inputs[1];

            if (typeConverter == null)
                return new[] { input };

            return new[] { typeConverter.Convert(input) };
        }
    }
}
