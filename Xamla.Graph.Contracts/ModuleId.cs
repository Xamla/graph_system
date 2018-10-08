using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph
{
    public class ModuleId
        : IComparable<ModuleId>
    {
        public ModuleId(string moduleId)
        {
            var tokens = moduleId.Split(',');

            if (tokens.Count() == 1 && string.IsNullOrEmpty(tokens[0]) || tokens.Count() > 2)
                throw new FormatException(string.Format("Parameter moduleId '{0}' has an invalid format. Expected: '{{Id}}, Version={{version}}'.", moduleId));

            this.Id = tokens[0];

            if (tokens.Count() == 2)
            {
                var versionTokens = tokens[1].Split('=');
                if (versionTokens.Count() != 2 
                    || string.IsNullOrEmpty(versionTokens[0])
                    || string.IsNullOrEmpty(versionTokens[1])
                    || !versionTokens[0].Trim().Equals("Version", StringComparison.OrdinalIgnoreCase)
                    )
                    throw new FormatException(string.Format("Parameter moduleId'{0}' has an invalid Version format. Expected: '{{Id}}, Version={{version}}'.", moduleId));

                this.Version = new Version(versionTokens[1].Trim());
            }
        }

        public ModuleId(string id, string version)
            : this(id, new Version(version))
        {
        }

        public ModuleId(string id, Version version)
        {
            this.Id = id;
            this.Version = version;
        }

        public String Id
        {
            get;
            private set;
        }

        public string Namespace
        {
            get 
            {
                int lastDot = this.Id.LastIndexOf('.');
                return lastDot < 0 ? this.Id : this.Id.Remove(lastDot);
            }
        }

        public string ModuleName
        {
            get
            {
                int lastDot = this.Id.LastIndexOf('.');
                return lastDot < 0 ? this.Id : this.Id.Substring(lastDot + 1);
            }
        }

        public Version Version
        {
            get;
            private set;
        }

        public static ModuleId Parse(string moduleId)
        {
            return new ModuleId(moduleId);
        }

        public override string ToString()
        {
            if (this.Version == null)
                return this.Id;

            return this.Id + ", Version=" + this.Version.ToString();
        }

        public int CompareTo(ModuleId other)
        {
            if (this.Version == null && other.Version == null)
                return 0;
            else if (this.Version == null)
                return 1;
            else if (other.Version == null)
                return -1;

            if (this.Version < other.Version)
                return 1;
            else if (this.Version > other.Version)
                return -1;
            else
                return 0;
        }
    }
}
