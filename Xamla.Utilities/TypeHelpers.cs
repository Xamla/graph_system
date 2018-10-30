using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xamla.Utilities
{
    public static class TypeHelpers
    {
        public static Type GetGenericTypeBase(this Type type, Type genericTypeDefinition)
        {
            while (type != typeof(object) && type != null)
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return type;
                }
                if (genericTypeDefinition.GetTypeInfo().IsInterface)
                {
                    foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
                    {
                        var foundType = interfaceType.GetGenericTypeBase(genericTypeDefinition);
                        if (foundType != null)
                            return foundType;
                    }
                }
                type = type.GetTypeInfo().BaseType;
            }
            return null;
        }

        public static bool HasGenericTypeBase(this Type type, Type genericType)
        {
            return GetGenericTypeBase(type, genericType) != null;
        }

        public static Type CreateTupleType(Type[] types)
        {            switch (types.Length)
            {
                case 1:
                    return typeof(Tuple<>).MakeGenericType(types);
                case 2:
                    return typeof(Tuple<,>).MakeGenericType(types);
                case 3:
                    return typeof(Tuple<,,>).MakeGenericType(types);
                case 4:
                    return typeof(Tuple<,,,>).MakeGenericType(types);
                case 5:
                    return typeof(Tuple<,,,,>).MakeGenericType(types);
                case 6:
                    return typeof(Tuple<,,,,,>).MakeGenericType(types);
                case 7:
                    return typeof(Tuple<,,,,,,>).MakeGenericType(types);
            }

            throw new NotSupportedException(string.Format("Tuple type with {0} elements is not supported.", types.Length));
        }

        static readonly Type[] tupleGenericBaseTypes = new[] { typeof(Tuple<>), typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>), typeof(Tuple<,,,,>), typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>) };

        public static bool IsTuple(this Type type)
        {
            return type != null && type.GetTypeInfo().IsGenericType && Array.IndexOf(tupleGenericBaseTypes, type.GetGenericTypeDefinition()) >= 0;
        }

        public static Type[] GetTupleTypes(Type tupleType)
        {
            if (tupleType == null)
                throw new ArgumentNullException(nameof(tupleType));

            if (!IsTuple(tupleType))
                throw new ArgumentException("Type is no tuple type.", "tupleType");

            return tupleType.GetTypeInfo().GetGenericArguments();
        }

        public static object[] GetTupleValues(object tuple)
        {
            if (tuple == null)
                throw new ArgumentNullException(nameof(tuple));

            if (!(tuple is ITuple t))
                throw new ArgumentException("GetTupleValue can only be used on Tuple type", nameof(tuple));

            var values = new object[t.Length];
            for (int i = 0; i < values.Length; i += 1)
            {
                values[i] = t[i];
            }

            return values;
        }

        public static object GetDefaultValue(this Type t)
        {
            return t.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(t) == null ? Activator.CreateInstance(t) : null;
        }

        public static Type[] GetDelegateSignature(Type delegateType)
        {
            if (delegateType == null)
                throw new ArgumentNullException("delegateType");

            if (!typeof(Delegate).GetTypeInfo().IsAssignableFrom(delegateType))
                throw new ArgumentException("Specified type is not a delegate type.", "delegateType");

            var method = delegateType.GetTypeInfo().GetMethod("Invoke");
            return method.GetParameters()
                .Select(x => x.ParameterType)
                .Concat(new[] { method.ReturnType })
                .ToArray();
        }

        public static bool AreGenericParametersFullySpecified(this Type t)
        {
            var genericTypes = t.GetTypeInfo().GetGenericArguments();
            if (genericTypes == null || genericTypes.Count() == 0)
                return true;

            foreach (var genericType in genericTypes)
            {
                if (genericType.GetTypeInfo().IsGenericType)
                    return AreGenericParametersFullySpecified(genericType);

                if (genericType.GetTypeInfo().IsGenericParameter && t.GetTypeInfo().IsGenericTypeDefinition)
                    return false;
            }

            return true;
        }

        public static string ToCSharpName(this Type type, bool full)
        {
            StringBuilder sb = new StringBuilder();

            if (type.IsArray)
            {
                sb.Append(ToCSharpName(type.GetElementType(), full));
                sb.Append('[');
                int rank = type.GetArrayRank();
                for (int i = 1; i < rank; i++)
                    sb.Append(',');
                sb.Append(']');
                return sb.ToString();
            }

            // Generic nested types return the complete list of type arguments,
            // including type arguments for the declaring class. This requires
            // some special handling
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type[] args = type.GetGenericArguments();
                int nt = args.Length - 1;
                Type pt = type;
                // Loop throguh the declaring class chain, consuming type arguments for every
                // generic class in the chain
                while (pt != null)
                {
                    if (sb.Length > 0)
                        sb.Insert(0, '.');
                    int i = pt.Name.IndexOf('`');
                    if (i != -1)
                    {
                        int na = nt - int.Parse(pt.Name.Substring(i + 1));
                        sb.Insert(0, '>');
                        for (; nt > na; nt--)
                        {
                            sb.Insert(0, ToCSharpName(args[nt], full));
                            if (nt - 1 != na)
                                sb.Insert(0, ',');
                        }
                        sb.Insert(0, '<');
                        sb.Insert(0, pt.Name.Substring(0, i));
                    }
                    else
                        sb.Insert(0, pt.Name);
                    pt = pt.DeclaringType;
                }
                if (full && type.Namespace.Length > 0)
                    sb.Insert(0, type.Namespace + ".");
                return sb.ToString();
            }

            if (type.DeclaringType != null)
            {
                sb.Append(ToCSharpName(type.DeclaringType, full)).Append('.');
                sb.Append(type.Name);
            }
            else
            {
                if (full && type.Namespace.Length > 0)
                    sb.Append(type.Namespace).Append('.');
                sb.Append(type.Name);
            }
            return sb.ToString();
        }
    }
}
