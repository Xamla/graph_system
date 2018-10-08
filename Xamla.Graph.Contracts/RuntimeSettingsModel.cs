using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.Models;

namespace Xamla.Graph
{
    public class RuntimeSettingsModel
        : NodeModel
    {
        public RuntimeSettingsModel()
            : base("RuntimeSettings")
        {
        }

        public bool PreviewGenerationEnabled { get; set; }
        public bool MeasureEvaluationTime { get; set; }
    }
}
