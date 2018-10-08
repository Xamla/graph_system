using System;
using System.Reflection;

public static class InfoExtensions
{
    public static TAttr GetCustomAttributeOrDefault<TAttr>(this MemberInfo memberInfo)
        where TAttr : Attribute
    {
        if (memberInfo == null)
            throw new ArgumentNullException(nameof(memberInfo));

        if (memberInfo.IsDefined(typeof(TAttr)))
        {
            return memberInfo.GetCustomAttribute<TAttr>();
        }
        else
        {
            return null;
        }
    }

    public static TAttr GetCustomAttributeOrDefault<TAttr>(this ParameterInfo parameterInfo)
        where TAttr : Attribute
    {
        if (parameterInfo == null)
            throw new ArgumentNullException(nameof(parameterInfo));

        if (parameterInfo.IsDefined(typeof(TAttr)))
        {
            return (TAttr)parameterInfo.GetCustomAttribute<TAttr>();
        }
        else
        {
            return null;
        }
    }

    public static bool IsParams(this ParameterInfo parameterInfo)
    {
        return parameterInfo.IsDefined(typeof(ParamArrayAttribute));
    }
}
