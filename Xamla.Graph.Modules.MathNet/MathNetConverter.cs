using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xamla.Types;
using Xamla.Types.Converters;

namespace Xamla.Graph.Modules.MathNet
{
    public class MathNetConverter
        : ITypeConversionProvider
    {
        public MathNetConverter()
        {
        }

        /* helper methods for conversion
         * note that MathNet implements for each number type single, double, complex all types on its own.
         * since we only have floats and no complex numbers at the point, we only use the double versions
         */

        public Vector<double> VToVector(V<double> vec)
        {
            /* MathNet has dense and sparse Vectors. Until we have our own datatype for sparse vectors, we
             * use their dense vectors.
             */
            return Vector<double>.Build.DenseOfArray(vec.UnderlyingArray.Buffer);
        }

        public V<double> VectorToV(Vector<double> vec)
        {
            return new V<double>(vec.ToArray());
        }

        public Matrix<double> MToMatrix(M<double> mat)
        {
            /* As in the vector case, we use dense matrices
             */
            return Matrix<double>.Build.DenseOfColumnMajor(mat.Rows, mat.Columns, mat.UnderlyingArray.Buffer);
        }

        public M<double> MatrixToM(Matrix<double> mat)
        {
            return new M<double>(
                new A<double>(mat.ToColumnMajorArray(), new int[2] { mat.ColumnCount, mat.RowCount }));
        }

        public Matrix<double> VectorToMatrix(Vector<double> vec)
        {
            /* Many operations on vectors also work column-wise on vectors,
             * like solving systems of linear equations, coerce zero. To
             * allow for maximum interoperability, we implement this converter */

            return Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[] { vec });
        }


        //Implementing the ITypeConversionProvider interface
        public IEnumerable<ITypeConverter> GetConverters()
        {
            return new[]
            {
                TypeConverter.Create<V<double>, Vector<double>>(this.VToVector),
                TypeConverter.Create<Vector<double>, V<double>>(this.VectorToV),
                TypeConverter.Create<M<double>, Matrix<double>>(this.MToMatrix),
                TypeConverter.Create<Matrix<double>, M<double>>(this.MatrixToM),
                TypeConverter.Create<Vector<double>, Matrix<double>>(this.VectorToMatrix)
            };
        }

        public IEnumerable<IDynamicTypeConverter> GetDynamicConverters()
        {
            return null;
        }

        public Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> GetSerializers()
        {
            return customSerializedTypes;
        }

        static readonly Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> customSerializedTypes = new Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> { };
    }
}
