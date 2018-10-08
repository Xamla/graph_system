using System;

namespace Xamla.Types.Converters
{
    public class TypeConverter
        : ITypeConverter
    {
        readonly Func<object, object> converter;

        public TypeConverter(Type sourceType, Type destinationType, Func<object, object> converter)
            : this(sourceType, null, destinationType, converter)
        {
        }

        public TypeConverter(Type sourceType, Type intermediateType, Type destinationType, Func<object, object> converter)
        {
            this.SourceType = sourceType;
            this.IntermediateType = intermediateType;
            this.DestinationType = destinationType;
            this.converter = converter;
        }

        public Type SourceType { get; }
        public Type IntermediateType { get; }
        public Type DestinationType { get; }

        public object Convert(object value) =>
            converter(value);

        public static ITypeConverter Create<TSource, TDestination>(Func<TSource, TDestination> converter) =>
            new TypeConverter(typeof(TSource), typeof(TDestination), x => converter((TSource)x));
    }
}
