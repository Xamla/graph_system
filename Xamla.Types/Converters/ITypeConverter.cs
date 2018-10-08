using System;

namespace Xamla.Types.Converters
{
    public interface ITypeConverter
    {
        Type SourceType { get; }
        Type IntermediateType { get; }
        Type DestinationType { get; }
        object Convert(object value);
    }
}
