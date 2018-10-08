using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph
{
    public class PinDescription
    {
        public string Id { get; set; }
        public string HelpText { get; set; }

        public PinDescription Clone()
        {
            return (PinDescription)this.MemberwiseClone();
        }
    }

    public class ModuleDescription
    {
        public string ModuleType { get; set; }
        public string DisplayName { get; set; }
        public string HelpText { get; set; }
        public string ReferenceUrl { get; set; }
        public string IconPath { get; set; }
        public List<PinDescription> Inputs { get; set; }
        public List<PinDescription> Outputs { get; set; }

        public ModuleDescription Clone()
        {
            var c = (ModuleDescription)this.MemberwiseClone();
            if (this.Inputs != null)
                c.Inputs = this.Inputs.Select(x => x.Clone()).ToList();
            if (this.Outputs != null)
                c.Outputs = this.Outputs.Select(x => x.Clone()).ToList();
            return c;
        }
    }

    public class NamespaceDescription
    {
        public string Namespace { get; set; }
        public string IconPath { get; set; }
        public string HelpText { get; set; }
        public string ReferenceUrl { get; set; }

        public NamespaceDescription Clone()
        {
            return (NamespaceDescription)this.MemberwiseClone();
        }
    }
}
