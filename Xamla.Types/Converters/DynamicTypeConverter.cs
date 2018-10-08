using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Converters
{
    public class DynamicTypeConverter
        : IDynamicTypeConverter
    {
        public Type SourceType { get; private set; }
        public Type DestinationType { get; private set; }
        public Func<Type, Type, ITypeConverter> ConverterFactory { get; private set; }

        public DynamicTypeConverter(Type sourceType, Type destinationType, Func<Type, Type, ITypeConverter> converterFactory)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
            this.ConverterFactory = converterFactory;
        }
    }
}
