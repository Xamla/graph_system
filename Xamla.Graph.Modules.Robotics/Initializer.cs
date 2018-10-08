using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rosvita.RestApi;
using Rosvita.RosMonitor;
using System.Reflection;
using Xamla.Graph;
using Xamla.MessageRouter.Client;

[assembly: GraphRuntimeInitializer(typeof(Xamla.Graph.Modules.Robotics.Initializer))]

namespace Xamla.Graph.Modules.Robotics
{
    class Initializer
        : IGraphRuntimeInitializer
    {
        public void Initialize(IGraphRuntime runtime)
        {
            runtime.ModuleFactory.RegisterAllModules(Assembly.GetExecutingAssembly());

            StaticModules.Init(
                runtime.ServiceLocator.GetService<ILoggerFactory>(),
                runtime.ServiceLocator.GetService<IManagedConnection>(),
                runtime.ServiceLocator.GetService<RpcAdapter>(),
                runtime.ServiceLocator.GetService<IWorldViewClient>(),
                runtime.ServiceLocator.GetService<IRosClientLibrary>()
            );

            var converter = new RoboticsMotionConverter();

            foreach (var convert in converter.GetConverters())
                runtime.TypeConverters.AddConverter(convert);

            //foreach (var c in converter.GetDynamicConverters())
            //    runtime.TypeConverters.AddDynamicConverter(c);

            foreach (var serializer in converter.GetSerializers())
                runtime.TypeSerializers.Add(serializer.Key, new SerializationFunctions { Serialize = serializer.Value.Item1, Deserialize = serializer.Value.Item2 });
        }
    }
}
