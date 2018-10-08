using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Converters
{
    public interface IDynamicTypeConverter
    {
        Type SourceType { get; }
        Type DestinationType {get;}
        Func<Type, Type, ITypeConverter> ConverterFactory { get; }
    }
}
