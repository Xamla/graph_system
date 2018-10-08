using System;

namespace Xamla.Graph.MethodModule
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ModuleMethodAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public Type PreviewGeneratorType { get; set; }
        public Type DynamicDisplayNameType { get; set; }
    }

    public abstract class PinAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type PinType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InstancePinAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InputPinAttribute : PinAttribute
    {
        public PropertyMode PropertyMode { get; set; } = PropertyMode.Default;
        public string Editor { get; set; }
        public bool ResolvePath { get; set; }
        public string DefaultValue { get; set; }
        public string JsonDefaultValue { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OutputPinAttribute : PinAttribute
    {
        public int? MaxConnections { get; set; }
    }
}
