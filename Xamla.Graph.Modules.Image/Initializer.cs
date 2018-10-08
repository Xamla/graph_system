using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Xamla.Graph;
using Xamla.Types.Converters;

[assembly: GraphRuntimeInitializer(typeof(Xamla.Graph.Modules.Image.Initializer))]

namespace Xamla.Graph.Modules.Image
{
    class Initializer
        : IGraphRuntimeInitializer
    {
        public void Initialize(IGraphRuntime runtime)
        {
            var converter = new ImageBufferConverter();

            foreach (var convert in converter.GetConverters())
                runtime.TypeConverters.AddConverter(convert);

            //foreach (var c in converter.GetDynamicConverters())
            //    runtime.TypeConverters.AddDynamicConverter(c);
        }
    }
}
