using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types.Converters;

namespace Xamla.Graph.Models
{
    public class TypeConverterModel
    {
        public string SourceType { get; set; }
        public string IntermediateType { get; set; }
        public string DestinationType { get; set; }
        public string Description { get; set; }
    }

    public class ConnectionModel
        : NodeModel
    {
        static string GetShortTypeName(Type type)
        {
            return type != null ? ObjectPinDataType.GetShortTypeName(type) : "any";
        }

        public ConnectionModel(ITypeConverter typeConverter)
            : base("Connection")
        {
            if (typeConverter == null)
            {
                this.TypeConverter = null;
            }
            else
            {
                this.TypeConverter = new TypeConverterModel
                {
                    SourceType = GetShortTypeName(typeConverter.SourceType),
                    IntermediateType = typeConverter.IntermediateType == null ? null : GetShortTypeName(typeConverter.IntermediateType),
                    DestinationType = GetShortTypeName(typeConverter.DestinationType),
                    Description = string.Concat(GetShortTypeName(typeConverter.SourceType), " -> ", GetShortTypeName(typeConverter.DestinationType))
                };
            }
        }

        public TypeConverterModel TypeConverter
        {
            get;
            set;
        }
    }
}
