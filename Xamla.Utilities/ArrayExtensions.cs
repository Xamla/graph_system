using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public static class ArrayExtensions
    {
        internal delegate TOutput Converter<in TInput, out TOutput>(TInput input);

        internal static TOutput[] ConvertAll<TInput, TOutput>(TInput[] input, Converter<TInput, TOutput> converter)
        {
            var y = new TOutput[input.Length];
            for (int i = 0; i < y.Length; ++i)
                y[i] = converter(input[i]);
            return y;
        }

        public static Func<Array, Array> CreatePrimitiveArrayConverter(Type sourceElementType, Type destinationElementType)
        {
            var sourceArrayType = sourceElementType.MakeArrayType();
            var destinationArrayType = destinationElementType.MakeArrayType();

            var convertMethod = typeof(Convert).GetTypeInfo().GetMethod("To" + destinationElementType.Name, new[] { sourceElementType });
            if (convertMethod == null)
                throw new Exception("Conversion not supported.");

            var delegateType = typeof(Converter<,>).MakeGenericType(sourceElementType, destinationElementType);
            var convertDelegate = convertMethod.CreateDelegate(delegateType);
            var inputParam = Expression.Parameter(typeof(Array));
            var exp = Expression.Lambda<Func<Array, Array>>(
                Expression.Call(typeof(ArrayExtensions), "ConvertAll", new Type[] { sourceElementType, destinationElementType }, Expression.Convert(inputParam, sourceArrayType), Expression.Convert(Expression.Constant(convertDelegate), delegateType)),
                inputParam
            );

            return exp.Compile();
        }

        static Dictionary<Tuple<Type, Type>, Func<Array, Array>> primitiveArrayConverters = new Dictionary<Tuple<Type, Type>, Func<Array, Array>>();

        public static Func<Array, Array> GetPrimitiveConverter(Type sourceElementType, Type destinationElementType)
        {
            lock (primitiveArrayConverters)
            {
                Func<Array, Array> converter;
                var key = Tuple.Create(sourceElementType, destinationElementType);
                if (!primitiveArrayConverters.TryGetValue(key, out converter))
                {
                    converter = CreatePrimitiveArrayConverter(sourceElementType, destinationElementType);
                    primitiveArrayConverters.Add(key, converter);
                }

                return converter;
            }

        }

        public static T[] CloneT<T>(this T[] array) =>
            (T[])array.Clone();

        public static void Clear<T>(this T[] array) =>
            Array.Clear(array, 0, array.Length);

        public static void Clear<T>(this T[] array, int startIndex, int count) =>
            Array.Clear(array, startIndex, count);

        public static void Fill<T>(this T[] array, T value) =>
            Array.Fill(array, value);

        public static void Fill<T>(this T[] array, T value, int startIndex, int count) =>
            Array.Fill(array, value, startIndex, count);
    }
}
