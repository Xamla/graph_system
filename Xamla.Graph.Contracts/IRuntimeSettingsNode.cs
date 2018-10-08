using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph
{
    public interface IRuntimeSettingsNode
    {
        bool MeasureEvaluationTime { get; set; }
        bool PreviewGenerationEnabled { get; set; }
    }
}
