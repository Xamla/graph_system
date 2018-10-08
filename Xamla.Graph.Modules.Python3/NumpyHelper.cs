using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xamla.Types;

namespace Xamla.Graph.Modules.Python3
{
    public class NumpyException
        : ApplicationException
    {
        public NumpyException()
            : base("NumpyException")
        {
        }

        public NumpyException(string message)
            : base(message)
        {
        }

        public NumpyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class NumpyHelper
    {
        public class NumpyArrayInterface
        {
            public NumpyArrayInterface(PyObject o)
            {
                if (o.GetPythonType().Handle != NumpyArrayType)
                {
                    throw new Exception("object is not a numpy array");
                }

                var meta = o.GetAttr("__array_interface__");
                IsCStyleContiguous = meta["strides"] == null;
                Address = new System.IntPtr((long)meta["data"][0].As<long>());

                var typestr = meta["typestr"].As<string>();
                var dtype = typestr.Substring(1);
                switch (dtype)
                {
                    case "b1":
                        DataType = typeof(bool);
                        break;
                    case "f4":
                        DataType = typeof(float);
                        break;
                    case "f8":
                        DataType = typeof(double);
                        break;
                    case "i2":
                        DataType = typeof(short);
                        break;
                    case "i4":
                        DataType = typeof(int);
                        break;
                    case "i8":
                        DataType = typeof(long);
                        break;
                    case "u1":
                        DataType = typeof(byte);
                        break;
                    case "u2":
                        DataType = typeof(ushort);
                        break;
                    case "u4":
                        DataType = typeof(uint);
                        break;
                    case "u8":
                        DataType = typeof(ulong);
                        break;
                    default:
                        throw new NumpyException($"type '{dtype}' not supported");
                }
                Shape = o.GetAttr("shape").As<long[]>();
                NBytes = o.GetAttr("nbytes").As<int>();
            }

            public readonly IntPtr Address;

            public readonly Type DataType;

            public readonly long[] Shape;

            public readonly int NBytes;

            public readonly bool IsCStyleContiguous;
        }

        /// <summary>
        /// numpy Module
        /// </summary>
        internal static PyObject np;

        internal static Dictionary<Type, PyObject> np_dtypes = new Dictionary<Type, PyObject>();

        internal static Dictionary<string, Type> np_dtypesToTypes = new Dictionary<string, Type>();

        internal static PyObject deepcopy;

        internal static IntPtr NumpyArrayType;

        public static bool TryInitialize()
        {
            using (Py.GIL())
            {
                try
                {
                    np = Py.Import("numpy");
                    NumpyArrayType = np.GetAttr("ndarray").Handle;
                    np_dtypes.Clear();
                    np_dtypes.Add(typeof(bool), np.GetAttr("bool_"));
                    np_dtypes.Add(typeof(byte), np.GetAttr("uint8"));
                    np_dtypes.Add(typeof(short), np.GetAttr("int16"));
                    np_dtypes.Add(typeof(int), np.GetAttr("int32"));
                    np_dtypes.Add(typeof(long), np.GetAttr("int64"));
                    np_dtypes.Add(typeof(ushort), np.GetAttr("uint16"));
                    np_dtypes.Add(typeof(uint), np.GetAttr("uint32"));
                    np_dtypes.Add(typeof(ulong), np.GetAttr("uint64"));
                    np_dtypes.Add(typeof(float), np.GetAttr("float"));
                    np_dtypes.Add(typeof(double), np.GetAttr("float64"));
                    var copy = Py.Import("copy");
                    deepcopy = copy.GetAttr("deepcopy");
                }
                catch (PythonException)
                {
                    return false;
                }

                np_dtypesToTypes.Clear();
                np_dtypesToTypes.Add("bool_", typeof(bool));
                np_dtypesToTypes.Add("int16", typeof(short));
                np_dtypesToTypes.Add("int32", typeof(int));
                np_dtypesToTypes.Add("int64", typeof(long));
                np_dtypesToTypes.Add("uint16", typeof(ushort));
                np_dtypesToTypes.Add("uint32", typeof(uint));
                np_dtypesToTypes.Add("uint64", typeof(ulong));
                np_dtypesToTypes.Add("float", typeof(float));
                np_dtypesToTypes.Add("float64", typeof(double));
            }

            return true;
        }

        public static PyObject GetNumpyDataType(Type type)
        {
            PyObject dtype;
            np_dtypes.TryGetValue(type, out dtype);
            if (dtype == null)
            {
                throw new NumpyException($"type '{type}' not supported.");
            }
            return dtype;
        }

        public static PyObject CreateNdArray(A content)
        {
            // Create an python tuple with the dimensions of the input array
            PyObject[] lengths = new PyObject[content.Rank];
            for (int i = 0; i < content.Rank; i++)
                lengths[i] = new PyInt(content.Dimension[i]);
            PyTuple shape = new PyTuple(lengths);

            // Create an empty numpy array in correct shape and datatype
            var dtype = GetNumpyDataType(content.ElementType);
            var arr = np.InvokeMethod("empty", shape, dtype);

            var meta = arr.GetAttr("__array_interface__");
            var address = new IntPtr((long)meta["data"][0].As<long>());

            if (content is A<byte> byteArray)
                Marshal.Copy(byteArray.Buffer, 0, address, content.Count);
            else if (content is A<short> shortArray)
                Marshal.Copy(shortArray.Buffer, 0, address, content.Count);
            else if (content is A<int> intArray)
                Marshal.Copy(intArray.Buffer, 0, address, content.Count);
            else if (content is A<long> longArray)
                Marshal.Copy(longArray.Buffer, 0, address, content.Count);
            else if (content is A<double> doubleArray)
                Marshal.Copy(doubleArray.Buffer, 0, address, content.Count);
            else if (content is A<double> floatArray)
                Marshal.Copy(floatArray.Buffer, 0, address, content.Count);
            else
            {
                int nbytes = content.SizeInBytes;
                byte[] data = new byte[nbytes];
                Buffer.BlockCopy(content.Buffer, 0, data, 0, nbytes);
                Marshal.Copy(data, 0, address, nbytes);
            }

            return arr;
        }

        public static PyObject CreateNdArray(Array content)
        {
            // BlockCopy possibly multidimensional array of arbitrary type to onedimensional byte array
            Type elementType = content.GetType().GetElementType();
            int nbytes = content.Length * Marshal.SizeOf(elementType);
            byte[] data = new byte[nbytes];
            Buffer.BlockCopy(content, 0, data, 0, nbytes);

            // Create an python tuple with the dimensions of the input array
            PyObject[] lengths = new PyObject[content.Rank];
            for (int i = 0; i < content.Rank; i++)
                lengths[i] = new PyInt(content.GetLength(i));
            PyTuple shape = new PyTuple(lengths);

            // Create an empty numpy array in correct shape and datatype
            var dtype = GetNumpyDataType(elementType);
            var arr = np.InvokeMethod("empty", shape, dtype);

            var meta = arr.GetAttr("__array_interface__");
            var address = new System.IntPtr((long)meta["data"][0].As<long>());

            // Copy the data to that array
            Marshal.Copy(data, 0, address, nbytes);
            return arr;
        }

        public static bool IsNumpyPrimitive(string type)
        {
            if (np_dtypesToTypes.ContainsKey(type))
                return true;
            else
                return false;
        }
        public static object ToPrimitiveType(string typePy, PyObject obj)
        {
            Type type = np_dtypesToTypes.GetValueOrDefault(typePy);
            if (type == typeof(bool))
                return obj.As<bool>();
            else if (type == typeof(short))
                return obj.As<short>();
            else if (type == typeof(int))
                return obj.As<int>();
            else if (type == typeof(long))
                return obj.As<long>();
            else if (type == typeof(ushort))
                return obj.As<ushort>();
            else if (type == typeof(uint))
                return obj.As<uint>();
            else if (type == typeof(ulong))
                return obj.As<ulong>();
            else if (type == typeof(float))
                return obj.As<float>();
            else if (type == typeof(double))
                return obj.As<double>();
            else
                throw new NumpyException($"type '{typePy}' not supported");
        }

        public static A ToA(PyObject array)
        {
            var info = new NumpyArrayInterface(array);

            PyObject arr = array;
            // If the array is not contiguous in memory, copy it first.
            // This overwrites the array (but obviously the contents stay the same).
            if (!info.IsCStyleContiguous)
            {
                arr = deepcopy.Invoke(array);
            }

            var result = A.Create(info.DataType, info.Shape.Select(x => (int)x).ToArray());
            int nElements = info.NBytes / Marshal.SizeOf(info.DataType);

            if (result.Count != nElements)
                throw new NumpyException("Element count mismatch");

            if (result is A<byte> byteArray)
                Marshal.Copy(info.Address, byteArray.Buffer, 0, result.Count);
            else if (result is A<short> shortArray)
                Marshal.Copy(info.Address, shortArray.Buffer, 0, result.Count);
            else if (result is A<int> intArray)
                Marshal.Copy(info.Address, intArray.Buffer, 0, result.Count);
            else if (result is A<long> longArray)
                Marshal.Copy(info.Address, longArray.Buffer, 0, result.Count);
            else if (result is A<double> doubleArray)
                Marshal.Copy(info.Address, doubleArray.Buffer, 0, result.Count);
            else if (result is A<double> floatArray)
                Marshal.Copy(info.Address, floatArray.Buffer, 0, result.Count);
            else
            {
                byte[] data = new byte[info.NBytes];
                Marshal.Copy(info.Address, data, 0, info.NBytes);
                Buffer.BlockCopy(data, 0, result.Buffer, 0, info.NBytes);
            }

            return result;
        }

        public static Array ToArray(PyObject array)
        {
            var info = new NumpyArrayInterface(array);
            // If the array is not contiguous in memory, copy it first.
            // This overwrites the array (but obviously the contents stay the same).
            PyObject arr = array;
            if (!info.IsCStyleContiguous)
            {
                arr = deepcopy.Invoke(array);
            }

            byte[] data = new byte[info.NBytes];
            Marshal.Copy(info.Address, data, 0, info.NBytes);
            if (info.DataType == typeof(byte) && info.Shape.Length == 1)
            {
                return data;
            }
            var result = System.Array.CreateInstance(info.DataType, info.Shape);
            System.Buffer.BlockCopy(data, 0, result, 0, info.NBytes);
            return result;
        }
    }
}
