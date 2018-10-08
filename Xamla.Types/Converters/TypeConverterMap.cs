using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Types.Converters
{
    public class TypeConverterMap
        : ITypeConverterMap
    {
        readonly Dictionary<Type, List<Type>> converterList = new Dictionary<Type, List<Type>>();
        readonly Dictionary<Tuple<Type, Type>, ITypeConverter> converters = new Dictionary<Tuple<Type, Type>, ITypeConverter>();
        readonly Dictionary<Tuple<Type, Type>, Func<Type, Type, ITypeConverter>> converterFactories = new Dictionary<Tuple<Type, Type>, Func<Type, Type, ITypeConverter>>();

        public void AddConverter(ITypeConverter converter)
        {
            converters.Add(Tuple.Create(converter.SourceType, converter.DestinationType), converter);
            if (converterList.TryGetValue(converter.SourceType, out List<Type> intermediateConverterList))
            {
                intermediateConverterList.Add(converter.DestinationType);
            }
            else
            {
                converterList[converter.SourceType] = new List<Type> { converter.DestinationType };
            }
        }

        public void RemoveConverter(ITypeConverter converter)
        {
            converters.Remove(Tuple.Create(converter.SourceType, converter.DestinationType));
        }

        public void AddDynamicConverter(IDynamicTypeConverter dynamicConverter)
        {
            var typeTuple = Tuple.Create(dynamicConverter.SourceType, dynamicConverter.DestinationType);
            this.converterFactories.Add(typeTuple, dynamicConverter.ConverterFactory);
        }

        public void RemoveDynamicConverter(Type sourceType, Type destinationType)
        {
            this.converterFactories.Remove(Tuple.Create(sourceType, destinationType));
        }

        public ITypeConverter GetConverter(Type sourceType, Type destinationType)
        {
            var converter = this.TryGetConverter(sourceType, destinationType);

            if (converter == null)
                throw new Exception(string.Format("Converter from '{0}' to '{1}' is missing.", sourceType.FullName, destinationType.FullName));

            return converter;
        }

        private static Func<object, object> CreateArrayConverter(Type destinationElementType, ITypeConverter converter)
        {
            return x =>
            {
                if (x == null)
                    return null;

                var input = (Array)x;
                var output = Array.CreateInstance(destinationElementType, input.Length);
                for (int i = 0; i < input.Length; ++i)
                    output.SetValue(converter.Convert(input.GetValue(i)), i);

                return output;
            };
        }

        private ITypeConverter AssignmentConverter(Type sourceType, Type destinationType)
        {
            return new TypeConverter(sourceType, destinationType, source => source);
        }

        private ITypeConverter ToStringConverter(Type sourceType, Type destinationType)
        {
            return new TypeConverter(sourceType, destinationType, source => source?.ToString() ?? "null");
        }

        public ITypeConverter TryGetConverter(Type sourceType, Type destinationType, bool findIntermediateConverter = true, int depth = 1)
        {
            // first check if a custom converter is registered
            // then try other conversions
            if (converters.TryGetValue(Tuple.Create(sourceType, destinationType), out ITypeConverter converter))
            {
                return converter;
            }
            else if (sourceType == destinationType || destinationType.IsAssignableFrom(sourceType))
            {
                return AssignmentConverter(sourceType, destinationType);
            }
            else if (typeof(IConvertible).IsAssignableFrom(sourceType) && typeof(IConvertible).IsAssignableFrom(destinationType))
            {
                return new TypeConverter(sourceType, destinationType, x => Convert.ChangeType(x, destinationType, CultureInfo.InvariantCulture));
            }

            if (sourceType == typeof(object))
            {
                return new TypeConverter(sourceType, destinationType, value =>
                {
                    var valueType = value.GetType();

                    var innerConverter = TryGetConverter(valueType, destinationType);
                    if (innerConverter == null)
                        throw new Exception(string.Format("Converter from type {0} to type {1} not found.", valueType, destinationType));

                    return innerConverter.Convert(value);
                });
            }
            else if (sourceType.IsArray && destinationType.IsArray)
            {
                var sourceElementType = sourceType.GetElementType();
                var destinationElementType = destinationType.GetElementType();

                if (sourceElementType != typeof(object) && destinationElementType != typeof(object))
                {
                    converter = TryGetConverter(sourceElementType, destinationElementType, findIntermediateConverter, depth);
                    if (converter != null)
                        return new TypeConverter(sourceType, destinationType, CreateArrayConverter(destinationElementType, converter));
                }
                else
                {
                    return new TypeConverter(sourceElementType, destinationElementType, source =>
                    {
                        var sourceArray = (Array)source;
                        var destinationArray = Array.CreateInstance(destinationElementType, sourceArray.Length);
                        sourceArray.CopyTo(destinationArray, 0);
                        return destinationArray;
                    });
                }
            }
            else if (sourceType.IsArray && (destinationType.HasGenericTypeBase(typeof(ISequence<>)) || destinationType == typeof(ISequence)))
            {
                Type destinationGenericType = destinationType;
                if (destinationType.GetTypeInfo().IsGenericType)
                    destinationGenericType = destinationType.GetGenericTypeDefinition();

                Func<Type, Type, ITypeConverter> converterFactory;
                if (this.converterFactories.TryGetValue(Tuple.Create(typeof(Array), destinationGenericType), out converterFactory))
                {
                    return converterFactory.Invoke(sourceType, destinationType);
                }
            }
            else if (sourceType.HasGenericTypeBase(typeof(ISequence<>)) && destinationType.IsArray)
            {
                Func<Type, Type, ITypeConverter> converterFactory;
                if (this.converterFactories.TryGetValue(Tuple.Create(typeof(ISequence<>), typeof(Array)), out converterFactory))
                {
                    return converterFactory.Invoke(sourceType, destinationType);
                }
            }
            else if (sourceType.GetTypeInfo().IsGenericType || destinationType.GetTypeInfo().IsGenericType)
            {
                Type sourceGenericType = sourceType;
                if (sourceType.GetTypeInfo().IsGenericType)
                    sourceGenericType = sourceType.GetGenericTypeDefinition();

                Type destinationGenericType = destinationType;
                if (destinationType.GetTypeInfo().IsGenericType)
                    destinationGenericType = destinationType.GetGenericTypeDefinition();

                var typeTuple = Tuple.Create(sourceGenericType, destinationGenericType);
                Func<Type, Type, ITypeConverter> converterFactory;
                if (this.converterFactories.TryGetValue(typeTuple, out converterFactory))
                {
                    return converterFactory.Invoke(sourceType, destinationType);
                }

                // check if * -> destination type converter exists
                if (this.converterFactories.TryGetValue(Tuple.Create<Type, Type>(null, destinationGenericType ?? destinationType), out converterFactory))
                {
                    converter = converterFactory.Invoke(sourceType, destinationType);
                    if (converter != null)
                        return converter;
                }
            }

            if (sourceType == typeof(object[]))
            {
                return new TypeConverter(sourceType, destinationType, value =>
                {
                    var sourceArray = (Array)value;

                    if (sourceArray.Length == 0)
                    {
                        if (destinationType.IsArray || destinationType.HasGenericTypeBase(typeof(IEnumerable<>)))
                        {
                            var underlyingDestinationType = destinationType.GetElementType() ?? destinationType.GetGenericArguments().First();
                            return Array.CreateInstance(underlyingDestinationType, 0);
                        }
                        else if (!destinationType.GetTypeInfo().IsInterface)
                        {
                            return Activator.CreateInstance(destinationType);
                        }
                        else
                        {
                            throw new Exception("Cannot estimate destinationType due to empty array.");
                        }
                    }

                    var valueType = sourceArray.GetValue(0).GetType();

                    var innerConverter = TryGetConverter(valueType.MakeArrayType(), destinationType);
                    if (innerConverter == null)
                        throw new Exception(string.Format("Converter from type {0} to type {1} not found.", valueType.MakeArrayType(), destinationType));

                    return innerConverter.Convert(DynamicCast.DynamicCastTo(value, innerConverter.SourceType));
                });
            }

            if (destinationType != typeof(string) && findIntermediateConverter)
            {
                if (converterList.TryGetValue(sourceType, out List<Type> list))
                {
                    var intermediateConverter2 = list.Select(x => TryGetConverter(x, destinationType, false, depth)).FirstOrDefault();
                    if (intermediateConverter2 != null)
                    {
                        var intermediateConverter1 = this.TryGetConverter(sourceType, intermediateConverter2.SourceType, false, depth);
                        return new TypeConverter(sourceType, intermediateConverter1.DestinationType, destinationType, x => intermediateConverter2.Convert(intermediateConverter1.Convert(x)));
                    }
                }
            }

            // check base class converter
            if (depth > 0)
            {
                // filter object out, because it will match for every type..

                var sourceBaseTypes = new[] { sourceType, sourceType.GetTypeInfo().BaseType }
                    .Concat(sourceType.GetInterfaces())
                    .Where(x => x != null && x != typeof(object))
                    .ToList();

                var destinationBaseTypes = new[] { destinationType.GetTypeInfo().BaseType }
                    .Concat(destinationType.GetInterfaces())
                    .Where(x => x != null && x != typeof(object))
                    .ToList();

                destinationBaseTypes.Sort(new TypeSorter());

                // check if a simple conversion exists
                foreach (var sourceBaseType in sourceBaseTypes)
                {
                    converter = TryGetConverter(sourceBaseType, destinationType, findIntermediateConverter, depth - 1);
                    if (converter != null)
                        return converter;
                }

                foreach (var sourceBaseType in sourceBaseTypes)
                {
                    foreach (var destinationBaseType in destinationBaseTypes)
                    {
                        var intermediateConverter2 = TryGetConverter(destinationBaseType, destinationType, false, depth - 1);
                        if (intermediateConverter2 != null)
                        {
                            var intermediateConverter1 = TryGetConverter(sourceBaseType, destinationBaseType, false, depth - 1);
                            if (intermediateConverter1 != null)
                                return new TypeConverter(sourceType, destinationBaseType, destinationType,
                                    x => intermediateConverter2.Convert(intermediateConverter1.Convert(x)));
                        }
                    }
                }
            }

            if (destinationType == typeof(string) && depth > 0)
            {
                return ToStringConverter(sourceType, destinationType);
            }

            return null;
        }

        class TypeSorter
            : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                if (x.GetTypeInfo().IsGenericType && !y.GetTypeInfo().IsGenericType)
                    return -1;
                else if (!x.GetTypeInfo().IsGenericType && y.GetTypeInfo().IsGenericType)
                    return 1;

                return 0;
            }
        }

        public int Count
        {
            get { return converters.Count; }
        }

        public IEnumerator<ITypeConverter> GetEnumerator()
        {
            return converters.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
