using System;
using System.Collections.Generic;

namespace Xamla.Types.Converters
{
    public interface ITypeConverterMap
        : IEnumerable<ITypeConverter>
    {
        void AddConverter(ITypeConverter converter);
        void RemoveConverter(ITypeConverter converter);

        void AddDynamicConverter(IDynamicTypeConverter dynamicConverter);
        void RemoveDynamicConverter(Type sourceType, Type destinationType);

        ITypeConverter GetConverter(Type sourceType, Type destinationType);
        ITypeConverter TryGetConverter(Type sourceType, Type destinationType, bool findIntermediateConverter = true, int depth = 1);

        int Count { get; }
    }

    public static class TypeConverterMapExtensions
    {
        public static void AddConverters(this ITypeConverterMap typeConverterMap, IEnumerable<ITypeConverter> converters)
        {
            foreach (var c in converters)
                typeConverterMap.AddConverter(c);
        }

        public static ITypeConverter GetConverter<TSource, TDestination>(this ITypeConverterMap typeConverterMap)
        {
            return typeConverterMap.GetConverter(typeof(TSource), typeof(TDestination));
        }

        public static ITypeConverter TryGetConverter<TSource, TDestination>(this ITypeConverterMap typeConverterMap)
        {
            return typeConverterMap.TryGetConverter(typeof(TSource), typeof(TDestination));
        }
    }
}
