using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph.Models
{
    public class StatusNodeModel
        : NodeModel
    {
        public StatusNodeModel()
            : base("Status")
        {
        }

        public RuntimeStatus RuntimeStatus { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public TimeSpan RunningDuration { get; set; }
        public ExceptionModel[] Exceptions { get; set; }

        // provide basic process information that can be used even when the graph is running as debug node

        public long PrivateMemorySize { get; set; }
        public long VirtualMemorySize { get; set; }
        public int AppDomainAssemblyCount { get; set; }
        public int HandleCount { get; set; }
    }
}
