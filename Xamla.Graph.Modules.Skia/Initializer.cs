using Xamla.Graph;

[assembly: GraphRuntimeInitializer(typeof(Xamla.Graph.Modules.Skia.Initializer))]

namespace Xamla.Graph.Modules.Skia
{
    public class Initializer
        : IGraphRuntimeInitializer
    {
        public void Initialize(IGraphRuntime runtime)
        {
            var converter = new SkiaConverter();

            foreach (var convert in converter.GetConverters())
                runtime.TypeConverters.AddConverter(convert);

            foreach (var c in converter.GetDynamicConverters())
                runtime.TypeConverters.AddDynamicConverter(c);

            foreach (var serializer in converter.GetSerializers())
                runtime.TypeSerializers.Add(serializer.Key, new SerializationFunctions { Serialize = serializer.Value.Item1, Deserialize = serializer.Value.Item2 });
        }
    }
}
