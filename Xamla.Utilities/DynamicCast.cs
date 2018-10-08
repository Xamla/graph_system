using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class DynamicCast
    {
        public static dynamic DynamicCastTo(dynamic obj, Type castTo)
        {
            MethodInfo castMethod = null;
            if (castTo.IsArray)
                castMethod = typeof(DynamicCast).GetTypeInfo().GetMethod("CastArrayTo").MakeGenericMethod(castTo.GetElementType());
            else
                castMethod = typeof(DynamicCast).GetTypeInfo().GetMethod("CastTo").MakeGenericMethod(castTo);
             
            return castMethod.Invoke(null, new object[] { obj });
        }

        public static T CastTo<T>(dynamic obj)
        {
            return (T)obj;
        }

        public static T[] CastArrayTo<T>(dynamic obj)
        {
            var tempArray = Array.CreateInstance(typeof(T), obj.Length);

            for (int i = 0; i < obj.Length; ++i)
                tempArray.SetValue((T)obj.GetValue(i), i);

            return tempArray;
        }
    }
}
