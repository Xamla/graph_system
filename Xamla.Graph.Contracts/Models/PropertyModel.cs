using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph.Models
{
    public class PropertyModel
        : NodeModel
    {
        public PropertyModel()
            : base("Property")
        {
        }

        public PinDataTypeModel PinDataType { get; set; }
        public JToken Value { get; set; }
        public bool Connect { get; set; }
    }

    public class PinDataTypeModel
    {
        public PinTypeCode TypeCode { get; set; }
        public Type UnderlyingType { get; set; }
        public string TypeName { get; set; }
        public object Parameters { get; set; }
        public string Editor { get; set; }
        public JToken DefaultValue { get; set; }
        public bool Nullable { get; set; }
    }
}
