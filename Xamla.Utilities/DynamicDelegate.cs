using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class DynamicDelegate
    {
        Func<object[], object> handler;
        Type[] argTypes;

        public static DynamicDelegate FromDelegateType(Func<object[], object> handler, Type delegateType)
        {
            return new DynamicDelegate(handler, TypeHelpers.GetDelegateSignature(delegateType));
        }

        public DynamicDelegate(Func<object[], object> handler, params Type[] argTypes)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (argTypes == null)
                throw new ArgumentNullException("argTypes");

            if (argTypes.Length == 0)
                throw new ArgumentException("At least one argument type that specifies the return type has to be specified.");

            this.handler = handler;
            this.argTypes = argTypes;
            this.Delegate = CompileDelegate();
        }

        public Delegate Delegate
        {
            get;
            private set;
        }

        Delegate CompileDelegate()
        {
            var delegateType = Expression.GetDelegateType(argTypes);

            var parameters = argTypes.Take(argTypes.Length - 1).Select(Expression.Parameter).ToArray();
            var returnType = argTypes[argTypes.Length - 1];

            Expression body =
                Expression.Invoke(
                    Expression.Constant(handler),
                    Expression.NewArrayInit(typeof(object), parameters.Select(x => x.Type.GetTypeInfo().IsValueType ? Expression.Convert(x, typeof(object)) : (Expression)x).ToArray())
                );

            if (returnType != typeof(void))
            {
                // add return value conversion
                body = Expression.Convert(body, returnType);
            }

            var exp = Expression.Lambda(
                delegateType,
                body,
                parameters
            );

            return exp.Compile();
        }
    }

    public class DynamicDelegateAsync
    {
        Func<object[], object[], CancellationToken, Task<object>> handler;
        Type[] argTypes;
        object[] additionalInputs;

        public static DynamicDelegateAsync FromDelegateType(Func<object[], object[], CancellationToken, Task<object>> handler, Type delegateType)
        {
            return new DynamicDelegateAsync(handler, TypeHelpers.GetDelegateSignature(delegateType));
        }

        public DynamicDelegateAsync(Func<object[], object[], CancellationToken, Task<object>> handler, object[] additionalInputs, params Type[] argTypes)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (argTypes == null)
                throw new ArgumentNullException("argTypes");

            if (argTypes.Length == 0)
                throw new ArgumentException("At least one argument type that specifies the return type has to be specified.");

            if (!typeof(Task).GetTypeInfo().IsAssignableFrom(argTypes[argTypes.Length - 1]))
                throw new ArgumentException("The last element in argTypes must be a type derived from System.Threading.Tasks.Task.", "argTypes");

            this.handler = handler;
            this.argTypes = argTypes;
            this.additionalInputs = additionalInputs;
            this.Delegate = CompileDelegate();
        }

        public Delegate Delegate
        {
            get;
            private set;
        }

        static Task<T> ConvertResult<T>(Task<object> input)
        {
            var tcs = new TaskCompletionSource<T>();
            input.ContinueWith(
                (t, o) =>
                {
                    var s = (TaskCompletionSource<T>)o;
                    try
                    {
                        switch (t.Status)
                        {
                            case TaskStatus.Canceled:
                                s.TrySetCanceled();
                                break;
                            case TaskStatus.RanToCompletion:
                                s.TrySetResult((T)t.Result);
                                break;
                            case TaskStatus.Faulted:
                                s.TrySetException(t.Exception.InnerExceptions);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        s.TrySetException(e);
                    }
                },
                tcs,
                TaskContinuationOptions.ExecuteSynchronously
            );
            return tcs.Task;
        }

        Delegate CompileDelegate()
        {
            var delegateType = Expression.GetDelegateType(argTypes);

            var parameters = argTypes.Take(argTypes.Length - 1).Select(Expression.Parameter).ToArray();
            var returnType = argTypes[argTypes.Length - 1];

            Expression cancellationTokenParam = parameters.FirstOrDefault(x => x.Type == typeof(CancellationToken));
            if (cancellationTokenParam == null)
                cancellationTokenParam = Expression.Constant(default(CancellationToken));

            Expression body =
                Expression.Invoke(
                    Expression.Constant(handler),
                    Expression.NewArrayInit(typeof(object), parameters.Select(x => x.Type.GetTypeInfo().IsValueType ? Expression.Convert(x, typeof(object)) : (Expression)x).ToArray()),
                    Expression.Constant(additionalInputs),
                    cancellationTokenParam
                );

            if (returnType != typeof(Task))
            {
                // add return value conversion
                body = Expression.Call(typeof(DynamicDelegateAsync), "ConvertResult", new Type[] { returnType.GetTypeInfo().GetGenericArguments()[0] }, body);
            }

            var exp = Expression.Lambda(
                delegateType,
                body,
                parameters
            );

            return exp.Compile();
        }
    }

    public class GenericDelegate<TDelegate>
    {
        object instance;
        MethodInfo method;

        public GenericDelegate(object instance, MethodInfo method)
        {
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance), "Instance can not be null.");
            this.method = method ?? throw new ArgumentNullException(nameof(method), "Method can not be null.");

            this.Delegate = CompileDelegate();
        }

        TDelegate CompileDelegate()
        {
            try
            {
                var parameterExpressions = new List<ParameterExpression>();
                var arguments = new List<Expression>();

                foreach (var parameter in method.GetParameters())
                {
                    var paramExpression = Expression.Parameter(typeof(object));
                    parameterExpressions.Add(paramExpression);
                    arguments.Add(Expression.Convert(paramExpression, parameter.ParameterType));
                }

                var instanceExpression = Expression.Constant(instance);

                var expression = Expression.Call(instanceExpression, method, arguments);

                var lambda = Expression.Lambda<TDelegate>(expression, parameterExpressions.ToArray());
                return lambda.Compile();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not compile generic Method '{0}'. Message: '{1}'", method.Name, e.Message);
                return default(TDelegate);
            }
        }

        public TDelegate Delegate
        {
            get;
            private set;
        }
    }
}
