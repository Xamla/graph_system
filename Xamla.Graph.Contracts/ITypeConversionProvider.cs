using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xamla.Types.Converters;

namespace Xamla.Graph
{
    public interface ITypeConversionProvider
    {
        IEnumerable<ITypeConverter> GetConverters();
        IEnumerable<IDynamicTypeConverter> GetDynamicConverters();

        // Tuple: <serializer, deserializer>
        Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> GetSerializers();
    }
}
