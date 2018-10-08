namespace Xamla.Graph
{
    public class ModuleCatalogItem
    {
        public string ModuleType { get; set; }

        public string Namespace
        {
            get
            {
                int lastDot = this.ModuleType.LastIndexOf('.');
                return lastDot <= 0 ? string.Empty : this.ModuleType.Remove(lastDot);
            }
        }

        public string DisplayName { get; set; }

        public ModuleDescription Description { get; set; }

        public string AliasedType { get; set; }

        public override string ToString()
        {
            return this.ModuleType;
        }
    }
}
