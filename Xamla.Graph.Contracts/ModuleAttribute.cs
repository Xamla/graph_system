using System;

namespace Xamla.Graph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute
        : Attribute
    {
        public string Description { get; set; }
        public string ModuleType { get; set; }
        public string ReferenceUrl { get; set; }
        public string IconPath { get; set; }
        public bool Flow { get; set; }
        public FlowMode FlowMode { get; set; } = FlowMode.WaitAny;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ModuleTypeAliasAttribute
        : Attribute
    {
        public ModuleTypeAliasAttribute(string aliasType)
        {
            this.AliasType = aliasType;
        }

        public string AliasType { get; set; }
        public bool IncludeInCatalog { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class StaticModuleAttribute
        : Attribute
    {
        public string ModuleType { get; set; }
        public string DisplayName { get; set; }
        public Type PreviewGenerator { get; set; }
        public Type DynamicDisplayName { get; set; }
        public string IconPath { get; set; }
        public bool Flow { get; set; }
        public FlowMode FlowMode { get; set; } = FlowMode.WaitAny;
    }
}
