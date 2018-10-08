using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Models
{
    public abstract class PinModel
        : NodeModel
    {
        public PinModel(string nodeType)
            : base(nodeType)
        {
        }

        public PinDirection Direction { get; set; }
        public int? MaxConnections { get; set; }
        public List<ConnectionModel> Connections { get; set; }
        public PinDataTypeModel PinDataType { get; set; }
        public bool Removable { get; set; }
    }

    public class GenericInputPinModel
        : PinModel
    {
        public GenericInputPinModel()
            : base("InputPin")
        {
        }

        public PropertyMode PropertyConnectionMode { get; set; }
        public bool IsConnectedToPropertyContainer { get; set; }
    }

    public class GenericOutputPinModel
        : PinModel
    {
        public GenericOutputPinModel()
            : base("OutputPin")
        {
        }

        public bool InspectResult { get; set; }
    }

    public class PreviewGeneratorModel
        : NodeModel
    {
        public PreviewGeneratorModel()
            : base("PreviewGenerator")
        {
        }

        public string ModuleObjId { get; set; }
        public bool Active { get; set; }
        public bool HasPreview { get; set; }
        public Int2? Size { get; set; }
    }

    public class ModuleModel
        : NodeModel
    {
        public ModuleModel()
            : this("Module")
        {
        }

        public ModuleModel(string nodeType)
            : base(nodeType)
        {
        }

        public string DisplayName { get; set; }
        public string ModuleType { get; set; }
        public ModuleKind ModuleKind { get; set; }
        public Int2 Position { get; set; }
        public bool ShowPreview { get; set; }
        public bool Active { get; set; }
        public DisplayMode Display { get; set; }
        public bool HasFlowPins { get; set; }
        public FlowMode FlowMode { get; set; }
        public bool InputAddable { get; set; }
        public bool OutputAddable { get; set; }
        public TimeSpan EvaluationTime { get; set; }
    }

    public class SubGraphModuleModel
        : ModuleModel
    {
        public SubGraphModuleModel()
            : base("SubGraphModule")
        {
        }

        public Int2 Size { get; set; }
        public bool ShowSubGraph { get; set; }
    }

    public class ControlModuleModel
        : ModuleModel
    {
        public ControlModuleModel()
            : base("ControlModule")
        {
        }

        public Int2 Size { get; set; }
        public string Control { get; set; }
    }
}
