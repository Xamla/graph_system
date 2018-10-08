using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.Models;

namespace Xamla.Graph
{
    public enum InspectionKind
    {
        Inspection,
        Output
    }

    public class InspectResultEvent
    {
        public object Result { get; set; }
        public IPin Pin { get; set; }
        public InspectionKind InspectionKind { get; set; }
        public int? EvaluationId { get; set; }
        public int? SequenceIteratorId { get; set; }
        public int SequenceResultId { get; set; }
    }
}
