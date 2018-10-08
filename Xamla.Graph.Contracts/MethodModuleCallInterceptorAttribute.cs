using System;

namespace Xamla.Graph
{
    public interface IMethodModuleCallInterceptor
    {
        void BeforeCall(object instance, object[] args);
        void AfterCall(object result, Exception error);
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class MethodModuleCallInterceptorAttribute
        : Attribute
    {
        public MethodModuleCallInterceptorAttribute(Type interceptorClassType)
        {
            this.InterceptorClassType = interceptorClassType;
        }

        public Type InterceptorClassType { get; }
    }
}
