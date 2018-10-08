using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using Xamla.Graph;
using Xamla.Types.Converters;

[assembly: GraphRuntimeInitializer(typeof(Xamla.Graph.Modules.OpenCv.Initializer))]
//[assembly: MethodModuleCallInterceptor(typeof(Xamla.Graph.Modules.OpenCv.OpenCvMethodCallInterceptor))]

namespace Xamla.Graph.Modules.OpenCv
{
    // AKo: Call interceptor experiment, did not work since OpenCv throws exception despite the fact that a error-redirect handler is registered.
    // see https://github.com/Xamla/todo/issues/188

    /*public class OpenCvMethodCallInterceptor
        : IMethodModuleCallInterceptor
    {
        public void AfterCall(object result, Exception error) =>
            OpenCvExceptionContext.Reset();

        public void BeforeCall(object instance, object[] args) =>
            OpenCvExceptionContext.ThrowOnError();
    }

    static class OpenCvExceptionContext
    {
        [ThreadStatic]
        static Exception caught;

        static readonly CvErrorCallback ErrorHandlerThrowException =
            delegate (ErrorCode status, string funcName, string errMsg, string fileName, int line, IntPtr userdata)
            {

                Console.WriteLine($"Error handler called {status}, {funcName}, {errMsg}...");
                caught = new OpenCVException(status, funcName, errMsg, fileName, line);
                return 0;
            };

        public static void Reset()
        {
            caught = null;
        }

        public static void ThrowOnError()
        {
            if (caught != null)
                throw caught;
        }

        static OpenCvExceptionContext()
        {
            Console.WriteLine("Setting Error handler...");
            IntPtr zero = IntPtr.Zero;
            IntPtr current = OpenCvSharp.NativeMethods.redirectError(ErrorHandlerThrowException, zero, ref zero);
        }
    }*/

    class Initializer
        : IGraphRuntimeInitializer
    {
        public void Initialize(IGraphRuntime runtime)
        {
            //OpenCvExceptionContext.Reset();

            runtime.ModuleFactory.RegisterAllModules(Assembly.GetExecutingAssembly());

            var converter = new OpenCvConverter();

            foreach (var convert in converter.GetConverters())
                runtime.TypeConverters.AddConverter(convert);

            foreach (var c in converter.GetDynamicConverters())
                runtime.TypeConverters.AddDynamicConverter(c);

            foreach (var serializer in converter.GetSerializers())
                runtime.TypeSerializers.Add(serializer.Key, new SerializationFunctions { Serialize = serializer.Value.Item1, Deserialize = serializer.Value.Item2 });
        }
    }
}
