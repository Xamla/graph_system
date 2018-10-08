using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamla.Types;
using Xamla.Utilities;
using Xamla.Robotics.Types;

namespace Xamla.Graph.Modules.Python3
{
    public static class PyConvert
    {
        public static PyObject ToPyObject(object obj)
        {
            if (obj == null)
            {
                return obj.ToPython();
            }
            var type = obj.GetType();
            if (type.IsPrimitive)
                return obj.ToPython();
            else if (obj is string)
                return obj.ToPython();
            else if (obj is ITuple t)
            {
                var items = TypeHelpers.GetTupleValues(obj).Select(ToPyObject).ToArray();
                return new PyTuple(items);
            }
            else if (obj is IDictionary d)
            {
                // create dictionary
                var dict = new PyDict();
                foreach (var key in d.Keys)
                {
                    var value = d[key];
                    var pyKey = ToPyObject(key);
                    dict[pyKey] = ToPyObject(value);
                }
                return dict;
            }
            else if (obj is A a)
                return NumpyHelper.CreateNdArray(a);
            else if (obj is IList l)
            {
                // create list
                var list = new PyList();
                foreach (var x in l)
                {
                    list.Append(ToPyObject(x));
                }
                return list;
            }
            else if (obj is JointSet s)
                return s.ToPython();
            else if (obj is JointValues v)
                return v.ToPython();
            else if (obj is Pose p)
                return p.ToPython();
            else if (obj is TimeSpan timeSpan)
                return timeSpan.ToPython();
            else if (obj is JointTrajectoryPoint jPoint)
                return jPoint.ToPython();
            else if (obj is JointTrajectory jTrajectory)
                return jTrajectory.ToPython();
            else if (obj is JointStates states)
                return states.ToPython();
            else if (obj is JointPath jPath)
                return jPath.ToPython();
            else if (obj is CartesianPath cPath)
                return cPath.ToPython();
            else if (obj is PlanParameters parameters)
                return parameters.ToPython();
            else if (obj is CollisionPrimitive primitive)
                return primitive.ToPython();
            else if (obj is CollisionObject collisionObject)
                return collisionObject.ToPython();

            throw new Exception("Object conversion not supported");
        }

        public static object ToClrObject(PyObject obj, Type expectedType)
        {
            if (obj == null)
                return null;

            var pyType = obj.GetPythonType();
            var typeName = pyType.GetAttr("__name__").ToString();
            if (typeName == "NoneType")
                return null;
            else if (typeName == "str")
                return obj.As<string>();
            else if (typeName == "int")
                return obj.As<int>();
            else if (typeName == "float")
                return obj.As<double>();
            else if (typeName == "bool")
                return obj.As<bool>();
            else if (NumpyHelper.IsNumpyPrimitive(typeName))
                return NumpyHelper.ToPrimitiveType(typeName, obj);
            else if (typeName == "ndarray")
                return NumpyHelper.ToA(obj);
            else if (typeName == "tuple")
            {
                int length = obj.Length();
                if (length == 0)
                    return null;

                var pyItems = new List<PyObject>(length);
                for (int i = 0; i < length; i += 1)
                {
                    PyObject item = obj[i];
                    pyItems.Add(item);
                }

                object[] clrItems;
                object tuple;
                if (typeof(ITuple).IsAssignableFrom(expectedType))
                {
                    var types = TypeHelpers.GetTupleTypes(expectedType);
                    clrItems = pyItems.Select((x, i) => ToClrObject(x, types[i])).ToArray();
                    tuple = Activator.CreateInstance(expectedType, clrItems);
                }
                else if (expectedType == typeof(object))
                {
                    var types = new Type[length];
                    Array.Fill(types, typeof(object));
                    var tupleType = TypeHelpers.CreateTupleType(types);
                    clrItems = pyItems.Select(x => ToClrObject(x, typeof(object))).ToArray();
                    tuple = Activator.CreateInstance(tupleType, clrItems);
                }
                else
                {
                    throw new Exception($"Target type {expectedType.Name} cannot be assigned from python tuple.");
                }

                return tuple;
            }
            else if (typeName == "list" || typeName == "deque")
            {
                Type elementType;
                Type listType = TypeHelpers.GetGenericTypeBase(expectedType, typeof(IList<>));
                IList list;
                if (listType != null)
                {
                    elementType = listType.GetGenericArguments()[0];
                    listType = typeof(List<>).MakeGenericType(elementType);
                    list = (IList)Activator.CreateInstance(listType);
                }
                else if (expectedType == typeof(object))
                {
                    elementType = typeof(object);
                    list = new List<object>();
                }
                else
                {
                    throw new Exception($"Target type {expectedType.Name} cannot be assigned from python list.");
                }

                int length = obj.Length();
                for (int i = 0; i < length; i += 1)
                {
                    PyObject item = obj[i];
                    list.Add(ToClrObject(item, elementType));
                }

                return list;
            }
            else if (typeName == "dict")
            {
                Type keyType;
                Type valueType;

                Type dictionaryType = TypeHelpers.GetGenericTypeBase(expectedType, typeof(IDictionary<,>));
                IDictionary dictionary;
                if (dictionaryType != null)
                {
                    var genericArgs = dictionaryType.GetGenericArguments();
                    keyType = genericArgs[0];
                    valueType = genericArgs[1];
                    dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                    dictionary = (IDictionary)Activator.CreateInstance(dictionaryType);
                }
                else if (expectedType == typeof(object))
                {
                    keyType = typeof(string);
                    valueType = typeof(object);
                    dictionary = new Dictionary<string, object>();
                }
                else
                {
                    throw new Exception($"Target type {expectedType.Name} cannot be assigned from python dict.");
                }

                var dict = new PyDict(obj);
                foreach (PyObject key in dict.Keys())
                {
                    PyObject value = dict[key];
                    dictionary.Add(ToClrObject(key, keyType), ToClrObject(value, valueType));
                }

                return dictionary;
            }
            else if (typeName == "JointSet")
                return RoboticsTypesExtensions.FromPyJointSet(obj);
            else if (typeName == "JointValues")
                return RoboticsTypesExtensions.FromPyJointValues(obj);
            else if (typeName == "Pose")
                return RoboticsTypesExtensions.FromPyPose(obj);
            else if (typeName == "timedelta")
                return RoboticsTypesExtensions.FromPyTimeDelta(obj);
            else if (typeName == "JointTrajectoryPoint")
                return RoboticsTypesExtensions.FromPyJoinTrajectoryPoint(obj);
            else if (typeName == "JointTrajectory")
                return RoboticsTypesExtensions.FromPyJointTrajectory(obj);
            else if (typeName == "JointStates")
                return RoboticsTypesExtensions.FromPyJointStates(obj);
            else if (typeName == "JointPath")
                return RoboticsTypesExtensions.FromPyJointPath(obj);
            else if (typeName == "CartesianPath")
                return RoboticsTypesExtensions.FromPyCartesianPath(obj);
            else if (typeName == "PlanParameters")
                return RoboticsTypesExtensions.FromPyPlanParameters(obj);
            else if (typeName == "CollisionPrimitive")
                return RoboticsTypesExtensions.FromPyCollisionPrimitive(obj);
            else if (typeName == "CollisionObject")
                return RoboticsTypesExtensions.FromPyCollisionObject(obj);
            else
                throw new Exception($"Encountered unsupported type annotation '{pyType.ToString()}'.");

        }
    }
}

