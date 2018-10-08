using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph.Models
{
    public class CSharpCodeManagerModel
        : NodeModel
    {
        public CSharpCodeManagerModel()
            : base("CSharpCodeManager")
        {
        }

        public string OutputDirectory { get; set; }
        public bool SourceModified { get; set; }
    }
}
