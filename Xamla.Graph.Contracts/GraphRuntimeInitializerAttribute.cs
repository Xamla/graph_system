using System;

namespace Xamla.Graph
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class GraphRuntimeInitializerAttribute
        : Attribute
    {
        public GraphRuntimeInitializerAttribute(Type initializationClassType)
        {
            this.InitiaizationClassType = initializationClassType;
        }

        public Type InitiaizationClassType { get; }
    }
}
