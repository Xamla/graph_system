using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamla.Types;
using Xamla.Utilities;
using Xamla.Robotics.Types;

namespace Xamla.Graph.Modules.Python3
{
    public class PyArgument
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public class PySignature
    {
        public List<PyArgument> Arguments { get; set; } = new List<PyArgument>();
        public Type ReturnType { get; set; }


        public bool HasArgument(string name) =>
            this.Arguments.Any(x => x.Name == name);

        static Type ToClrType(PyObject pyType)
        {
            var typeName = pyType.GetAttr("__name__").ToString();
            switch (typeName)
            {
                case "NoneType":
                    return null;
                case "str":
                    return typeof(string);
                case "int":
                    return typeof(int);
                case "float":
                    return typeof(double);
                case "bool":
                    return typeof(bool);
                case "ndarray":
                    return typeof(A);
                case "bool_":
                    return typeof(bool);
                case "int16":
                    return typeof(short);
                case "int32":
                    return typeof(int);
                case "int64":
                    return typeof(long);
                case "uint16":
                    return typeof(ushort);
                case "uint32":
                    return typeof(uint);
                case "uint64":
                    return typeof(ulong);
                case "float64":
                    return typeof(double);
                case "Tuple":
                    {
                        PyObject tupleArgs;
                        if (pyType.HasAttr("__tuple_params__"))
                            tupleArgs = pyType.GetAttr("__tuple_params__");
                        else
                            tupleArgs = pyType.GetAttr("__args__");
                        Type[] tupleTypes = tupleArgs
                            .OfType<PyObject>()
                            .Select(ToClrType)
                            .ToArray();
                        return TypeHelpers.CreateTupleType(tupleTypes);
                    }
                case "List":
                    {
                        Type elementType = pyType.GetAttr("__args__")
                            .OfType<PyObject>()
                            .Select(ToClrType)
                            .Single();
                        return typeof(IList<>).MakeGenericType(elementType);
                    }
                case "Dict":
                    {
                        var args = pyType.GetAttr("__args__")
                            .OfType<PyObject>()
                            .Select(ToClrType)
                            .ToArray();
                        return typeof(IDictionary<,>).MakeGenericType(args);
                    }
                case "Iterable":
                    {
                        Type elementType = pyType.GetAttr("__args__")
                            .OfType<PyObject>()
                            .Select(ToClrType)
                            .Single();
                        return typeof(IEnumerable<>).MakeGenericType(elementType);
                    }
                case "JointSet":
                    return typeof(JointSet);
                case "JointValues":
                    return typeof(JointValues);
                case "Pose":
                    return typeof(Pose);
                case "JointTrajectoryPoint":
                    return typeof(JointTrajectoryPoint);
                case "JointTrajectory":
                    return typeof(JointTrajectory);
                case "JointStates":
                    return typeof(JointStates);
                case "JointPath":
                    return typeof(JointPath);
                case "CartesianPath":
                    return typeof(CartesianPath);
                case "PlanParameters":
                    return typeof(PlanParameters);
                case "CollisionPrimitive":
                    return typeof(CollisionPrimitive);
                case "CollisionObject":
                    return typeof(CollisionObject);
                default:
                    throw new Exception($"Encountered unsupported type annotation '{pyType.ToString()}'.");
            }
        }

        public static PySignature GetSignature(PyScope scope, string functionName)
        {
            var s = new PySignature();

            PyObject fn = scope.Get(functionName);

            var inspect = Py.Import("inspect");
            var signature = inspect.InvokeMethod("signature", fn);
            var parameters = signature.GetAttr("parameters");

            var keys = parameters.InvokeMethod("keys");
            var argumentNames = new List<string>();
            foreach (var key in keys.OfType<PyObject>())
            {
                var p = parameters[key];
                var name = p.GetAttr("name").As<string>();
                argumentNames.Add(name);
            }

            PyDict annotations = new PyDict(fn.GetAttr("__annotations__"));
            foreach (var argumentName in argumentNames)
            {
                Type type;
                if (annotations.HasKey(argumentName))
                {
                    var pyType = annotations.GetItem(argumentName);
                    type = ToClrType(pyType);
                }
                else
                {
                    type = typeof(object);
                }
                s.Arguments.Add(new PyArgument { Name = argumentName, Type = type });
            }

            Type returnType;
            if (annotations.HasKey("return"))
            {
                var returnPyType = annotations.GetItem("return");
                returnType = ToClrType(returnPyType);
            }
            else
            {
                returnType = typeof(object);
            }

            s.ReturnType = returnType;

            return s;
        }
    }
}
