using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xamla.Graph;
using Xamla.Graph.MethodModule;

[assembly: GraphRuntimeInitializer(typeof(Xamla.Graph.Modules.MathNet.Initializer))]

namespace Xamla.Graph.Modules.MathNet
{
    class Initializer
        : IGraphRuntimeInitializer
    {
        public void Initialize(IGraphRuntime runtime)
        {
            runtime.ModuleFactory.RegisterAllModules(Assembly.GetExecutingAssembly());

            var converter = new MathNetConverter();

            foreach (var convert in converter.GetConverters())
                runtime.TypeConverters.AddConverter(convert);

            // We don't have dynamic converters and serializers until now
            //foreach (var c in converter.GetDynamicConverters())
            //    runtime.TypeConverters.AddDynamicConverter(c);

            //foreach (var serializer in converter.GetSerializers())
            //    runtime.TypeSerializers.Add(serializer.Key, new SerializationFunctions { Serialize = serializer.Value.Item1, Deserialize = serializer.Value.Item2 });
        }
    }

    public class MathNetWrapper
    {
        /// <summary>
        /// Adds matrix2 to matrix 1;
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Add"/>
        /// <param name="matrix1">The first matrix</param>
        /// <param name="matrix2">The matrix to add to the first matrix</param>
        /// <returns>The result of the addition</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Add")]
        public static Matrix<double> Add(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix2
            )
        {
            return matrix1.Add(matrix2);
        }

        /// <summary>
        /// Concatenates matrix1 and matrix2.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Append"/>
        /// <param name="left">The left matrix</param>
        /// <param name="right">The matrix that gets to the right side</param>
        /// <returns>The combined matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Append")]
        public static Matrix<double> Append(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> left,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> right
            )
        {
            return left.Append(right);
        }

        /// <summary>
        /// Retrieves the requested element without range checking.
        /// </summary>
        /// <param name="matrix">The matrix to select the element from.</param>
        /// <param name="row">The row of the element.</param>
        /// <param name="column">The column of the element.</param>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#At"/>
        /// <returns>The requested element.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.At")]
        public static double At(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int row,
            [InputPin(PropertyMode = PropertyMode.Allow)] int column
            )
        {
            return matrix.At(row, column);
        }

        /// <summary>
        /// For a symmetric, positive definite matrix $A$ the Cholesky factorization is a lower triangular
        /// matrix $L$ such that $A = LL^*$. If the input matrix is not positive definite and symmetric, this module fails.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/Cholesky%601.htm"/>
        /// <param name="mat">A positive definite and symmetric matrix</param>
        /// <returns>Returns the lower triangular matrix $L$ as well as the factorization object for further use.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Cholesky")]
        public static Tuple<Matrix<double>, Cholesky<double>> Cholesky(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat
            )
        {
            Cholesky<double> cholesky = mat.Cholesky();
            return new Tuple<Matrix<double>, Cholesky<double>>(cholesky.Factor, cholesky);
        }


        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Clear"/>
        /// <param name="matrix">The matrix to be zeroed.</param>
        /// <returns>Returns the matrix with all values zero.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Clear")]
        public static Matrix<double> Clear(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            matrix.Clear();
            return matrix;
        }

        /// <summary>
        /// Sets all values of a column to zero.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ClearColumn"/>
        /// <param name="matrix">The target matrix.</param>
        /// <param name="columnIndex">The column which is to be set zero.</param>
        /// <returns>The matrix with the chosen column set to zero.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ClearColumn")]
        public static Matrix<double> ClearColumn(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int columnIndex
            )
        {
            matrix.ClearColumn(columnIndex);
            return matrix;
        }

        /// <summary>
        /// Sets all the values of all the chosen columns to zero.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ClearColumns"/>
        /// <param name="matrix">The target matrix.</param>
        /// <param name="columnIndices">The columns which are to be set zero.</param>
        /// <returns>The matrix with the chosen columns set to zero.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ClearColumns")]
        public static Matrix<double> ClearColumn(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Int32[] columnIndices
            )
        {
            matrix.ClearColumns(columnIndices);
            return matrix;
        }

        /// <summary>
        /// Sets all values of a row to zero.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ClearRow"/>
        /// <param name="matrix">The target matrix.</param>
        /// <param name="rowIndex">The row which is to be set zero.</param>
        /// <returns>The matrix with the chosen row set to zero.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ClearRow")]
        public static Matrix<double> ClearRow(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int rowIndex
            )
        {
            matrix.ClearRow(rowIndex);
            return matrix;
        }

        /// <summary>
        /// Sets all the values of all the chosen rows to zero.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ClearRows"/>
        /// <param name="matrix">The target matrix.</param>
        /// <param name="rowIndices">The rows which are to be set zero.</param>
        /// <returns>The matrix with the chosen rows set to zero.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ClearRows")]
        public static Matrix<double> ClearRow(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Int32[] rowIndices
            )
        {
            matrix.ClearRows(rowIndices);
            return matrix;
        }

        /// <summary>
        /// Sets all values of a submatrix to zero.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm"/>
        /// <param name="matrix"></param>
        /// <param name="rowIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="columnIndex"></param>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ClearSubMatrix")]
        public static Matrix<double> ClearSubMatrix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Default)] int rowIndex,
            [InputPin(PropertyMode = PropertyMode.Default)] int rowCount,
            [InputPin(PropertyMode = PropertyMode.Default)] int columnIndex,
            [InputPin(PropertyMode = PropertyMode.Default)] int columnCount
            )
        {
            matrix.ClearSubMatrix(rowIndex, rowCount, columnIndex, columnCount);
            return matrix;
        }

        /// <summary>
        /// Set all values whose absolute value is smaller than the threshold to zero
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="threshold"></param>
        /// <param name="zeroPredicate"></param>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#CoerceZero"/>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.CoerceZero")]
        public static Matrix<double> CoerceZero(
           [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat,
           [InputPin(PropertyMode = PropertyMode.Default)] double threshold,
           [InputPin(PropertyMode = PropertyMode.Allow)] Func<double, bool> zeroPredicate
           )
        {
            if (zeroPredicate == null)
            {
                mat.CoerceZero(threshold);
            }
            else
            {
                mat.CoerceZero(zeroPredicate);
            }
            return mat;
        }



        /// <summary>
        /// Returns the requested column elements in a vector. If no values for rowIndex or length
        /// are supplied (i.e. blank field or disconnected), maximal values will be assumed (i.e. all remaining
        /// rows or from the beginning)
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Column"/>
        /// <param name="matrix">The matrix to select the values from.</param>
        /// <param name="index">The column to copy elements from.</param>
        /// <param name="rowIndex">The row to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A vector containing the requested elements.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Column")]
        public static Vector<double> Column(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int index,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? rowIndex = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            return matrix.Column(index, rowIndex ?? 0, length ?? (matrix.RowCount - (rowIndex ?? 0)));
        }

        /// <summary>
        /// The number of columns of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ColumnCount"/>
        /// <param name="matrix"></param>
        /// <returns>The number of columns of this matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ColumnCount")]
        public static int ColumnCount(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ColumnCount;
        }

        /// <summary>
        /// Calculates the absolute value sum of each column vector.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ColumnAbsoluteSums"/>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The absolute value sums of the column vectors.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ColumnAbsoluteSums")]
        public static Vector<double> ColumnAbsoluteSums(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ColumnAbsoluteSums();
        }

        /// <summary>
        /// Calculates the value sum of each column vector.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ColumnSums"/>
        /// <param name="matrix"></param>
        /// <returns>The sum of each column vector as a vector.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ColumnSums")]
        public static Vector<double> ColumnSums(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ColumnSums();
        }

        /// <summary>
        /// Using a singular value decomposition, calculates the condition number of the matrix,
        /// i.e. the value of the asymptotic worst-case relative change in output for a relative change in input.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ConditionNumber"/>
        /// <param name="matrix"></param>
        /// <returns>The condition number of the matrix</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ConditionNumber")]
        public static double ConditionNumber(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ConditionNumber();
        }

        /// <summary>
        /// Calculates the $p$-norm $\|\cdot\|_p$ for earch column vector, for $1 \leq p &lt; \infty$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ColumnNorms"/>
        /// <param name="matrix">The matrix for which you want to calculate the $p$-norms</param>
        /// <param name="p">$p$</param>
        /// <returns>A vector containing the $p$-norm of each columns.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ColumnNorms")]
        public static Vector<double> ColumnNorms(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double p
            )
        {
            return matrix.ColumnNorms(p);
        }

        /// <summary>
        /// Calculate the determinant of a square matrix $M$
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Determinant"/>
        /// <param name="mat">The matrix $M$ you want to calculate the determinate of.</param>
        /// <returns>The determinant of $M$.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Determinant")]
        public static double Determinant(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat
            )
        {
            return mat.Determinant();
        }

        /// <summary>
        /// Returns the elements of the diagonal in a vector. For non-square matrices
        /// $(a_{ij})$ all entries with $i = j$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Diagonal"/>
        /// <param name="matrix"></param>
        /// <returns>Returns the elements of the diagonal in a vector.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Diagonal")]
        public static Vector<double> Diagonal(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Diagonal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Divide"/>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Divide")]
        public static Matrix<double> Divide(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double scalar
            )
        {
            return matrix.Divide(scalar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#DivideByThis"/>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.DivideByThis")]
        public static Matrix<double> DivideByThis(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double scalar
            )
        {
            return matrix.DivideByThis(scalar);
        }

        /// <summary>
        /// Diagonally stacks his matrix on top of the given matrix. The new matrix is a M-by-N matrix,
        /// where M = this.Rows + lower.Rows and N = this.Columns + lower.Columns. The values of off the
        /// off diagonal matrices/blocks are set to zero.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#DiagonalStack"/>
        /// <param name="mat1"></param>
        /// <param name="mat2"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.DiagonalStack")]
        public static Matrix<double> DiagonalStack(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat2
            )
        {
            return mat1.DiagonalStack(mat2);
        }

        /// <summary>
        /// Enumerate all values of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Enumerate"/>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Enumerate")]
        public static IEnumerable<double> Enumerate(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Enumerate();
        }

        /// <summary>
        /// Enumerates the columns of the matrix. If length and index are null (i.e. disconnected or blank) maximal
        /// choices will be made (i.e. leave disconnected for all columns).
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#EnumerateColumns"/>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.EnumerateColumns")]
        public static IEnumerable<Vector<double>> EnumerateColumns(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? index = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            int effectiveIndex = index ?? 0;
            int effectiveLength = length ?? (matrix.ColumnCount - effectiveIndex);
            return matrix.EnumerateColumns(effectiveIndex, effectiveLength);
        }

        /// <summary>
        /// Enumerates the columns of the matrix, with indexes. If length and index are null (i.e. disconnected or blank) maximal
        /// choices will be made (i.e. leave disconnected for all columns).
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#EnumerateColumnsIndexed"/>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.EnumerateColumnsIndexed")]
        public static IEnumerable<Tuple<int, Vector<double>>> EnumerateColumnsIndexed(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? index = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            int effectiveIndex = index ?? 0;
            int effectiveLength = length ?? (matrix.ColumnCount - effectiveIndex);
            return matrix.EnumerateColumnsIndexed(effectiveIndex, effectiveLength);
        }

        /// <summary>
        /// Enumerate all values of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#EnumerateIndexed"/>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.EnumerateIndexed")]
        public static IEnumerable<Tuple<int, int, double>> EnumerateIndexed(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.EnumerateIndexed();
        }

        /// <summary>
        /// Enumerates the rows of the matrix. If length and index are null (i.e. disconnected or blank) maximal
        /// choices will be made (i.e. leave disconnected for all rows).
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#EnumerateRows"/>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.EnumerateRows")]
        public static IEnumerable<Vector<double>> EnumerateRows(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? index = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            int effectiveIndex = index ?? 0;
            int effectiveLength = length ?? (matrix.RowCount - effectiveIndex);
            return matrix.EnumerateRows(effectiveIndex, effectiveLength);
        }

        /// <summary>
        /// Enumerates the rows of the matrix, with indexes. If length and index are null (i.e. disconnected or blank) maximal
        /// choices will be made (i.e. leave disconnected for all rows).
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#EnumerateRowsIndexed"/>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.EnumerateRowsIndexed")]
        public static IEnumerable<Tuple<int, Vector<double>>> EnumerateRowsIndexed(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? index = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            int effectiveIndex = index ?? 0;
            int effectiveLength = length ?? (matrix.RowCount - effectiveIndex);
            return matrix.EnumerateRowsIndexed(effectiveIndex, effectiveLength);
        }


        /// <summary>
        /// Check whether two matrices coincide or not.
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Equals"/>
        /// <returns>True if the matrices are the same, false if not.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Equals")]
        public static bool Equals(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix2
            )
        {
            return matrix1.Equals(matrix2);
        }

        /// <summary>
        /// If the matrix $A$ is symmetric, then $ A=VDV^t$ for a diagonal matrix $D$ and orthogonal matrix $V$,
        /// such that the diagonal of $D$ corresponds with the necessarily real valued eigenvalues of the
        /// matrix. If the matrix $A$ is not symmetric and has imaginary eigenvalues, there is a skew symmetric diagonal block matrix
        /// with $2\times2$ blocks $(b_ij)$ on the diagonal, with $b_ii$ being the real part of the eigenvalues and the skew symmetric
        /// part corresponding to the imaginary eigenvalues.
        ///
        /// Returns the matrix $D$, a vector containing the (possibly complex) eigenvalues, a matrix containing the eigenvectors, as
        /// well as a factorization object for further use, which implements the ISolver Interface if $A$ is symmetric.
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/Evd%601.htm"/>
        /// </summary>
        /// <param name="mat">A matrix for which you wish to compute the eigenvectors</param>
        /// <returns>Returns the matrix $D$, a vector containing the (possibly complex) eigenvalues, a matrix containing the eigenvectors, as
        /// well as a factorization object for further use, which implements the ISolver Interface if $A$ is symmetric.
        /// </returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Evd")]
        public static Tuple<Matrix<double>, Vector<System.Numerics.Complex>, Matrix<double>, Evd<double>> Evd(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat
            )
        {
            Evd<double> evd = mat.Evd();
            return new Tuple<Matrix<double>, Vector<System.Numerics.Complex>, Matrix<double>, Evd<double>>(
                evd.D, evd.EigenValues, evd.EigenVectors, evd);
        }

        /// <summary>
        /// Returns for a matrix $A = (a_{ij})$ the Frobenius norm, i.e.
        /// $\|A\|_{\text{Frobenius}} = \sqrt{\sum a_{ij}^2}$
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#FrobeniusNorm"/>
        /// <param name="matrix"></param>
        /// <returns>The Frobenius norm.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.FrobeniusNorm")]
        public static double FrobeniusNorm(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.FrobeniusNorm();
        }


        /// <summary>
        /// A hashcode for this instance.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#GetHashCode"/>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.GetHashCode")]
        public static int GetHashCode(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#GetType"/>  
        /// <param name="matrix"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.GetType")]
        public static Type Type(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.GetType();
        }


        /// <summary>
        /// Calculates the infinity norm $\|\cdot\|_\infty$ of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#InfinityNorm"/>  
        /// <param name="matrix"></param>
        /// <returns>The infinity norm.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.InfinityNorm")]
        public static double InfinityNorm(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.InfinityNorm();
        }

        /// <summary>
        /// Inserts a column at the given index.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#InsertColumn"/>  
        /// <param name="matrix"></param>
        /// <param name="columnIndex">Where to insert the column</param>
        /// <param name="column">The column to insert</param>
        /// <returns>A new matrix with the inserted column</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.InsertColumn")]
        public static Matrix<double> InsertColumn(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int columnIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] Vector<double> column
            )
        {
            return matrix.InsertColumn(columnIndex, column);
        }

        /// <summary>
        /// Inserts a row at the given index.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#InsertRow"/>  
        /// <param name="matrix"></param>
        /// <param name="rowIndex">Where to insert the row</param>
        /// <param name="row">The row to insert</param>
        /// <returns>A new matrix with the inserted row</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.InsertRow")]
        public static Matrix<double> InsertRow(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int rowIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] Vector<double> row
            )
        {
            return matrix.InsertRow(rowIndex, row);
        }

        /// <summary>
        /// If the matrix is invertible, returns the inverse of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Inverse"/>  
        /// <param name="matrix"></param>
        /// <returns>The inverse of the matrix</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Inverse")]
        public static Matrix<double> Inverse(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Inverse();
        }

        /// <summary>
        /// Evaluates whether the matrix is symmetric.
        /// </summary>
        /// <param name="matrix"></param>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#IsSymmetric"/>
        /// <returns>True if the matrix is symmetric, false otherwise.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.IsSymmetric")]
        public static bool IsSymmetric(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.IsSymmetric();
        }

        /// <summary>
        /// Returns an orthonormal basis of the kernel of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Kernel"/>
        /// <param name="matrix"></param>
        /// <returns>An orthonormal basis of the kernel</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Kernel")]
        public static Vector<double>[] Kernel(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Kernel();
        }

        /// <summary>
        /// Returns the Kronecker product of the first matrix with the second matrix, i.e. $A \otimes B$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#KroneckerProduct"/>
        /// <param name="matrix1">The first matrix</param>
        /// <param name="matrix2">The second matrix</param>
        /// <returns>The Kronecker product of the two matrices</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.KroneckerProduct")]
        public static Matrix<double> KroneckerProduct(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix2
            )
        {
            return matrix1.KroneckerProduct(matrix2);
        }


        /// <summary>
        /// Calculates the $L^1$ norm  $\|\cdot\|_1$ of the matrix, i.e. the maximum absolute column sum of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#KroneckerProduct"/>
        /// <param name="matrix"></param>
        /// <returns>The $L^1$-norm of the Matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.L1Norm")]
        public static double L1Norm(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.L1Norm();
        }

        /// <summary>
        /// Calculates the $L^2$ norm  $\|\cdot\|_2$ of the matrix, i.e. the largest singular value of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#KroneckerProduct"/>
        /// <param name="matrix"></param>
        /// <returns>The $L^2$-norm of the Matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.L2Norm")]
        public static double L2Norm(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.L2Norm();
        }

        /// <summary>
        /// Returns the left multiplication $vA$ for a (row) vector $v$ and matrix $A$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#LeftMultiply"/>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns>$vA$</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.LeftMultiply")]
        public static Vector<double> LeftMultiply(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Vector<double> vector
            )
        {
            return matrix.LeftMultiply(vector);
        }

        /// <summary>
        /// Returns the lower triangle of the matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>The lower triangle of the matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.LowerTriangle")]
        public static Matrix<double> LowerTriangle(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.LowerTriangle();
        }


        /// <summary>
        /// For a matrix $A$ the LU factorization is a pair of a lower triangular matrix $L$ and upper triangular matrix $U$
        /// so that $A = LU$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/LU%601.htm"/>
        /// <param name="mat">The matrix you want to factorize</param>
        /// <returns>Returns the matrices $L$, $U$ as well as the factorization object for further use.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.LU")]
        public static Tuple<Matrix<double>, Matrix<double>, LU<double>> LU(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat
            )
        {
            LU<double> lu = mat.LU();
            return new Tuple<Matrix<double>, Matrix<double>, LU<double>>(lu.L, lu.U, lu);
        }


        /// <summary>
        /// Applies function f to each element in the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#MapInplace"/>
        /// <param name="matrix"></param>
        /// <param name="f"></param>
        /// <returns>The matrix with f applied.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.MapInplace")]
        public static Matrix<double> MapInplace(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Func<double, double> f
            )
        {
            matrix.MapInplace(f);
            return matrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Modulus"/>
        /// <param name="matrix"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Modulus")]
        public static Matrix<double> Modulus(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double divisor
            )
        {
            return matrix.Modulus(divisor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ModulusByThis"/>
        /// <param name="matrix"></param>
        /// <param name="dividend"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ModulusByThis")]
        public static Matrix<double> ModulusByThis(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double dividend
            )
        {
            return matrix.ModulusByThis(dividend);
        }

        /// <summary>
        /// Multiplies matrices $A, B$ with each other.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Multiply"/>
        /// <param name="leftMatrix">$A$</param>
        /// <param name="rightMatrix">$B$</param>
        /// <returns>$AB$</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Multiply")]
        public static Matrix<double> Multiply(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> leftMatrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> rightMatrix
            )
        {
            return leftMatrix.Multiply(rightMatrix);
        }

        /// <summary>
        /// Negates each element of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Negate"/>
        /// <param name="matrix"></param>
        /// <returns>A matrix containing the negated values.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Negate")]
        public static Matrix<double> Negate(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Negate();
        }

        /// <summary>
        /// Normalizes all column vectors w.r.t. the $p$-norm.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#NormalizeColumns"/>
        /// <param name="matrix"></param>
        /// <param name="p"></param>
        /// <returns>A matrix containing the normalized column vectors.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.NormalizeColumns")]
        public static Matrix<double> NormalizeColumns(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double p
            )
        {
            return matrix.NormalizeColumns(p);
        }

        /// <summary>
        /// Normalizes all row vectors w.r.t. the $p$-norm.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#NormalizeRows"/>
        /// <param name="matrix"></param>
        /// <param name="p"></param>
        /// <returns>A matrix containing the normalized row vectors.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.NormalizeRows")]
        public static Matrix<double> NormalizeRows(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double p
            )
        {
            return matrix.NormalizeRows(p);
        }

        /// <summary>
        /// Calculates the nullity, i.e. dimension of the kernel of the matrix, using an SVD,
        /// so it is actually the effective numerical nullity.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Nullity"/>
        /// <param name="matrix"></param>
        /// <returns>The effictive numerical nullity of the matrix, obtained via SVD.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Nullity")]
        public static int Nullity(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Nullity();
        }

        /// <summary>
        /// Permute the columns of a matrix according to a permutation, given by an array indices, describing the permutation
        /// as follows:
        /// indices[i] represents that integer i is permuted to location indices[i].
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PermuteColumns"/>
        /// <param name="matrix"></param>
        /// <param name="indices"></param>
        /// <returns>A matrix with the columns permuted according to the permutation.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PermuteColumns")]
        public static Matrix<double> PermuteColumns(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Int32[] indices
            )
        {
            matrix.PermuteColumns(new Permutation(indices));
            return matrix;
        }

        /// <summary>
        /// Permute the rows of a matrix according to a permutation, given by an array indices, describing the permutation
        /// as follows:
        /// indices[i] represents that integer i is permuted to location indices[i].
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PermuteRows"/>
        /// <param name="matrix"></param>
        /// <param name="indices"></param>
        /// <returns>A matrix with the rows permuted according to the permutation.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PermuteRows")]
        public static Matrix<double> PermuteRows(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Int32[] indices
            )
        {
            matrix.PermuteRows(new Permutation(indices));
            return matrix;
        }

        /// <summary>
        /// Pointwise divide the dividend by the divisor.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PointwiseDivide"/>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns>The result of the pointwise divsion.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PointwiseDivide")]
        public static Matrix<double> PointwiseDivide(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> dividend,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> divisor
            )
        {
            return dividend.PointwiseDivide(divisor);
        }

        /// <summary>
        /// Apply $\exp$ to each entry of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PointwiseExp"/>
        /// <param name="matrix"></param>
        /// <returns>The matrix with $\exp$ applied to each of its entries.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PointwiseExp")]
        public static Matrix<double> PointwiseExp(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.PointwiseExp();
        }

        /// <summary>
        /// Apply $\log$ to each entry of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PointwiseLog"/>
        /// <param name="matrix"></param>
        /// <returns>The matrix with $\log$ applied to each of its entries.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PointwiseLog")]
        public static Matrix<double> PointwiseLog(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.PointwiseLog();
        }

        /// <summary>
        /// Pointwise modulus, result having the sign of the divisor.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PointwiseModulus"/>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns>Matrix with pointwise modulus applied.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PointwiseModulus")]
        public static Matrix<double> PointwiseModulus(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> dividend,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> divisor
            )
        {
            return dividend.PointwiseModulus(divisor);
        }

        /// <summary>
        /// Pointwise multiplication of two matrices, i.e. for $A = (a_{ij})_{ij}, B = (b_{ij})_{ij}$ the result is 
        /// $(a_{ij}b_{ij})_{ij}$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PointwiseMultiply"/>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns>The result of the pointwise multiplication.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PointwiseMultiply")]
        public static Matrix<double> PointwiseMultiply(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix2
            )
        {
            return matrix1.PointwiseMultiply(matrix2);
        }

        /// <summary>
        /// Raises each entry to the given power.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PointwisePower"/>
        /// <param name="matrix"></param>
        /// <param name="exponent"></param>
        /// <returns>A matrix with each entry raised to the given power.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PointwisePower")]
        public static Matrix<double> PointwisePower(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double exponent
            )
        {
            return matrix.PointwisePower(exponent);
        }

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#PointwisePower"/>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns>Pointwise remainder (% operator), where the result has the sign of the dividend.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.PointwiseRemainder")]
        public static Matrix<double> PointwiseRemainder(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> dividend,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> divisor
            )
        {
            return dividend.PointwiseRemainder(divisor);
        }

        /// <summary>
        /// For a square matrix $A$ and the exponent $k$ calculate $A^k$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Power"/>
        /// <param name="matrix">$A$</param>
        /// <param name="exponent">$k$</param>
        /// <returns>$A^k$</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Power")]
        public static Matrix<double> Power(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int exponent
            )
        {
            return matrix.Power(exponent);
        }

        /// <summary>
        /// For a $m \times n$ matrix $M$ with $m \geq n$ the QR-factorization is a pair of an orthogonal matrix $Q$ and an
        /// upper triangular matrix $R$ such that $M = QR$. This module fails if $M$ is not $m \times n$ with $m \geq n$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/QR%601.htm"/>
        /// <param name="mat">The matrix you want to factorize</param>
        /// <returns>Returns the matrices $Q$, $R$ and the factorization object for further use.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.QR")]
        public static Tuple<Matrix<double>, Matrix<double>, QR<double>> QR(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat
            )
        {
            QR<double> qr = mat.QR();
            return new Tuple<Matrix<double>, Matrix<double>, QR<double>>(qr.Q, qr.R, qr);
        }

        /// <summary>
        /// Calculates the (effective numerical) rank using an SVD.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Rank"/>
        /// <param name="matrix"></param>
        /// <returns>The (effective numerical) rank of the matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Rank")]
        public static int Rank(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Rank();
        }


        /// <summary>
        /// Reduces all column vectors by applying a function between two of them, until only a single vector is left.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ReduceColumns")]
        public static Vector<double> Reducecolumns(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Func<Vector<double>, Vector<double>, Vector<double>> f
            )
        {
            return matrix.ReduceColumns(f);
        }

        /// <summary>
        /// Reduces all row vectors by applying a function between two of them, until only a single vector is left.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ReduceRows")]
        public static Vector<double> Reducerows(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Func<Vector<double>, Vector<double>, Vector<double>> f
            )
        {
            return matrix.ReduceRows(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Remainder"/>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Remainder")]
        public static Matrix<double> Remainder(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> dividend,
            [InputPin(PropertyMode = PropertyMode.Allow)] double divisor
            )
        {
            return dividend.Remainder(divisor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#RemainderByThis"/>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.RemainderByThis")]
        public static Matrix<double> RemainderByThis(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> dividend,
            [InputPin(PropertyMode = PropertyMode.Allow)] double divisor
            )
        {
            return dividend.RemainderByThis(divisor);
        }

        /// <summary>
        /// Removes the indicated column from the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#RemoveColumn"/>
        /// <param name="matrix"></param>
        /// <param name="columnIndex"></param>
        /// <returns>The matrix with the column removed.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.RemoveColumn")]
        public static Matrix<double> RemoveColumn(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int columnIndex
            )
        {
            return matrix.RemoveColumn(columnIndex);
        }

        /// <summary>
        /// Removes the indicated row from the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#RemoveRow"/>
        /// <param name="matrix"></param>
        /// <param name="rowIndex"></param>
        /// <returns>The matrix with the row removed.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.RemoveRow")]
        public static Matrix<double> RemoveRow(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int rowIndex
            )
        {
            return matrix.RemoveRow(rowIndex);
        }

        /// <summary>
        /// Returns the requested row elements in a vector. If no values for columnIndex or length 
        /// are supplied (i.e. blank field or disconnected), maximal values will be assumed (i.e. all remaining
        /// columns or from the beginning)
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Row"/>
        /// <param name="matrix">The matrix to select the values from.</param>
        /// <param name="index">The row to copy elements from.</param>
        /// <param name="columnIndex">The column to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A vector containing the requested elements.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Row")]
        public static Vector<double> Row(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int index,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? columnIndex = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            return matrix.Row(index, columnIndex ?? 0, length ?? (matrix.ColumnCount - (columnIndex ?? 0)));
        }

        /// <summary>
        /// Calculates the absolute value sum of each row vector.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#RowAbsoluteSums"/>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The absolute value sums of the row vectors.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.RowAbsoluteSums")]
        public static Vector<double> RowAbsoluteSums(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.RowAbsoluteSums();
        }

        /// <summary>
        /// Returns the number of rows of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#RowCount"/>
        /// <param name="matrix"></param>
        /// <returns>The number of </returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.RowCount")]
        public static int RowCount(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.RowCount;
        }

        /// <summary>
        /// Calculates the value sum of each row vector.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#RowSums"/>
        /// <param name="matrix"></param>
        /// <returns>The sum of each row vector as a vector.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.RowSums")]
        public static Vector<double> RowSums(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.RowSums();
        }

        /// <summary>
        /// Calculates the $p$-norm $\|\cdot\|_p$ for earch row vector, for $1 \leq p &lt; \infty$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#RowNorms"/>
        /// <param name="matrix">The matrix for which you want to calculate the $p$-norms</param>
        /// <param name="p">$p$</param>
        /// <returns>A vector containing the $p$-norm of each rows.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.RowNorms")]
        public static Vector<double> RowNorms(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double p
            )
        {
            return matrix.RowNorms(p);
        }

        /// <summary>
        /// Copies the values of the given vector to the specified sub-column. If length and row index are unset (i.e. disconnected
        /// or blank field) maximal values are assumed.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#SetColumn"/>
        /// <param name="matrix"></param>
        /// <param name="columnIndex"></param>
        /// <param name="column"></param>
        /// <param name="rowIndex"></param>
        /// <param name="length"></param>
        /// <returns>Matrix with subcolumn set to given vector.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.SetColumn")]
        public static Matrix<double> SetColumn(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int columnIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] Vector<double> column,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? rowIndex = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            matrix.SetColumn(columnIndex, rowIndex ?? 0, length ?? (matrix.RowCount - (rowIndex ?? 0)), column);
            return matrix;
        }


        /// <summary>
        /// Set the diagonal of the matrix to the given vector.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#SetDiagonal"/>
        /// <param name="matrix"></param>
        /// <param name="diagonal"></param>
        /// <returns>Matrix with the diagonal set to the given vector.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.SetDiagonal")]
        public static Matrix<double> SetDiagonal(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Vector<double> diagonal
            )
        {
            matrix.SetDiagonal(diagonal);
            return matrix;
        }

        /// <summary>
        /// Copies the values of the given vector to the specified sub-row. If length and column index are unset (i.e. disconnected
        /// or bolumnnk field) maximal values are assumed.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#SetRow"/>
        /// <param name="matrix"></param>
        /// <param name="rowIndex"></param>
        /// <param name="row"></param>
        /// <param name="columnIndex"></param>
        /// <param name="length"></param>
        /// <returns>Matrix with subrow set to given vector.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.SetRow")]
        public static Matrix<double> SetRow(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int rowIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] Vector<double> row,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? columnIndex = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? length = null
            )
        {
            matrix.SetRow(rowIndex, columnIndex ?? 0, length ?? (matrix.ColumnCount - (columnIndex ?? 0)), row);
            return matrix;
        }


        /// <summary>
        /// Copies the values of a given matrix into a region of the other matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#SetSubMatrix"/>
        /// <param name="matrix">The matrix to copy into</param>
        /// <param name="submatrix">The matrix to copy from</param>
        /// <param name="rowIndex">The rowIndex in the target matrix</param>
        /// <param name="columnIndex">The columnIndex in the target matrix</param>
        /// <param name="sourceRowIndex">Row to start from in submatrix, leave disconnected for maximal value</param>
        /// <param name="sourceColumnIndex">Column to start from in submatrix, leave disconnected for maximal value</param>
        /// <param name="rowCount">Number of rows to copy, leave disconnected/blank for maximal value</param>
        /// <param name="columnCount">Number of columns to copy, leave disconnected/blank for maximal value</param>
        /// <returns>A matrix with a submatrix set to the given matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.SetSubMatrix")]
        public static Matrix<double> SetSubMatrix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> submatrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int rowIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] int columnIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? sourceRowIndex = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? sourceColumnIndex = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? rowCount = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] int? columnCount = null
            )
        {
            int effectiveSourceRowIndex = sourceRowIndex ?? 0;
            int effectiveSourceColumnIndex = sourceColumnIndex ?? 0;
            int effectiveRowCount = rowCount ?? (submatrix.RowCount - effectiveSourceRowIndex);
            int effectiveColumnCount = columnCount ?? (submatrix.ColumnCount - effectiveSourceColumnIndex);
            matrix.SetSubMatrix(
                rowIndex, effectiveSourceRowIndex, effectiveRowCount,
                columnIndex, effectiveSourceColumnIndex, effectiveColumnCount,
                submatrix);
            return matrix;
        }

        /// <summary>
        /// Solves a system of linear equations of the form $AX = B$ for given $A, B$. Uses QR- or LU-decomposition.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Solve"/>
        /// <param name="lhs">The matrix A</param>
        /// <param name="rhs">The matrix B</param>
        /// <returns>The solution $X$</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Solve")]
        public static Matrix<double> Solve(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> lhs,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> rhs
            )
        {
            return lhs.Solve(rhs);
        }

        /// <summary>
        /// Stacks one matrix on top of the other.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Stack"/>
        /// <param name="upper">This matrix goes to the top.</param>
        /// <param name="lower">This matrix goes below the other.</param>
        /// <returns>A matrix containing the stacked matrices.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Stack")]
        public static Matrix<double> Stack(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> upper,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> lower
            )
        {
            upper.Stack(lower);
            return upper;
        }

        /// <summary>
        /// Returns the strictly lower triangle matrix of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#StrictlyLowerTriangle"/>
        /// <param name="matrix"></param>
        /// <returns>The strictly lower triangle matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.StrictlyLowerTriangle")]
        public static Matrix<double> StrictlyLowerTriangle(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.StrictlyLowerTriangle();
        }

        /// <summary>
        /// Returns the strictly upper triangle matrix of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#StrictlyUpperTriangle"/>
        /// <param name="matrix"></param>
        /// <returns>The strictly upper triangle matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.StrictlyUpperTriangle")]
        public static Matrix<double> StrictlyUpperTriangle(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.StrictlyUpperTriangle();
        }

        /// <summary>
        /// Creates a submatrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#SubMatrix"/>
        /// <param name="matrix"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="columnCount"></param>
        /// <returns>The submatrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.SubMatrix")]
        public static Matrix<double> SubMatrix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] int rowIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] int columnIndex,
            [InputPin(PropertyMode = PropertyMode.Allow)] int rowCount,
            [InputPin(PropertyMode = PropertyMode.Allow)] int columnCount
            )
        {
            return matrix.SubMatrix(rowIndex, rowCount, columnIndex, columnCount);
        }

        /// <summary>
        /// Subtracts the second matrix from the first matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Subtract"/>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns>The difference of the two matrices.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Subtract")]
        public static Matrix<double> Subtract(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix2
        )
        {
            return matrix1.Subtract(matrix2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#SubtractFrom"/>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.SubtractFrom")]
        public static Matrix<double> SubtractFrom(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] double scalar
            )
        {
            return matrix.SubtractFrom(scalar);
        }

        /// <summary>
        /// If $M$ is a $n\times m$ matrix, then $M = U \Sigma V^t$ where $U$ is a $m\times m$ unitary matrix, $\Sigma$ is 
        /// $m\times n$ diagonal matrix with nonnegative reals on the diagonal and $V$ is an $n\times n$ unitary matrix, called
        /// the singular value decomposition of the matrix $M$, with singular values being the values on the diagonal of $\Sigma$.
        /// 
        /// Returns the matrices $U$, $\Sigma$ and $V^t$, as well as a vector containing the singular values $\Sigma_{ii}$ in ascending order
        /// and a Factorization object for further use.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/Svd%601.htm"/>
        /// <param name="mat"></param>
        /// <returns>Returns the matrices $U$, $\Sigma$ and $V^t$, as well as a vector containing the singular values $\Sigma_{ii}$ in ascending order
        /// and a Factorization object for further use.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Svd")]
        public static Tuple<Matrix<double>, Matrix<double>, Matrix<double>, Vector<double>, Svd<double>> Svd(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> mat
            )
        {
            Svd<double> svd = mat.Svd();
            return new Tuple<Matrix<double>, Matrix<double>, Matrix<double>, Vector<double>, Svd<double>>(
                svd.U, svd.W, svd.VT, svd.S, svd);
        }

        /// <summary>
        /// Returns this matrix as a multidimensional array.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ToArray"/>
        /// <param name="matrix"></param>
        /// <returns>An multidimensional array contianing the values of this matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ToArray")]
        public static Double[,] ToArray(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ToArray();
        }

        /// <summary>
        /// Returns this matrix as an array with the data laid out column-wise.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ToColumnWiseArray"/>
        /// <param name="matrix"></param>
        /// <returns>An array contianing the values of this matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ToColumnMajorArray")]
        public static Double[] ToColumnWiseArray(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ToColumnMajorArray();
        }

        /// <summary>
        /// Returns a string that summarizes the content of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ToMatrixString"/>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ToMatrixString")]
        public static string ToMatrixString(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ToMatrixString();
        }

        /// <summary>
        /// Returns this matrix as an array with the data laid out row-wise.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ToRowWiseArray"/>
        /// <param name="matrix"></param>
        /// <returns>An array contianing the values of this matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ToRowMajorArray")]
        public static Double[] ToRowWiseArray(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ToRowMajorArray();
        }

        /// <summary>
        /// Returns a string that summarizes the content of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ToString"/>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ToString")]
        public static string ToString(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ToString();
        }

        /// <summary>
        /// Returns a string that describes the type, dimensions and shape of this matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#ToTypeString"/>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.ToTypeString")]
        public static string ToTypeString(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.ToTypeString();
        }


        /// <summary>
        /// Computes the trace of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Trace"/>
        /// <param name="matrix"></param>
        /// <returns>The trace of the matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Trace")]
        public static double Trace(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Trace();
        }

        /// <summary>
        /// Transposes the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#Transpose"/>
        /// <param name="matrix"></param>
        /// <returns>The transpose of the matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.Transpose")]
        public static Matrix<double> Transpose(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.Transpose();
        }

        /// <summary>
        /// Multiplies the first matrix with the transpose of the other matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#TransposeAndMultiply"/>
        /// <param name="matrix"></param>
        /// <param name="transposedMatrix"></param>
        /// <returns>The result of the multiplication</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.TransposeAndMultiply")]
        public static Matrix<double> TransposeAndMultiply(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> transposedMatrix
            )
        {
            return matrix.TransposeAndMultiply(transposedMatrix);
        }

        /// <summary>
        /// Transposes the first matrix and multiplies it with the second.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#TransposeThisAndMultiply"/>
        /// <param name="transposedMatrix"></param>
        /// <param name="matrix"></param>
        /// <returns>The result of the multiplication.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.TransposeThisAndMultiply")]
        public static Matrix<double> TransposeThisAndMultiply(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> transposedMatrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return transposedMatrix.TransposeThisAndMultiply(matrix);
        }

        /// <summary>
        /// Returns the upper triangle of the matrix.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Matrix.htm#UpperTriangle"/>
        /// <param name="matrix"></param>
        /// <returns>The upper triangle of the matrix.</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Matrix.UpperTriangle")]
        public static Matrix<double> UpperTriangle(
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> matrix
            )
        {
            return matrix.UpperTriangle();
        }

        /// <summary>
        /// Uses a factorization object like a QR- or LU-decomposition (or anything implementing the ISolver-Interface)
        /// of a matrix $A$
        /// to solve a system of linear equations of the form $AX = B$, for a given $B$. In particular this solves
        /// systems of the form $Ax = b$ for vectors $x$ and $b$.
        /// </summary>
        /// <reference href="http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/ISolver%601.htm" />
        /// <param name="lhs">The solver corresponding to the matrix $A$</param>
        /// <param name="rhs">A matrix $B$ corresponding to the right hand side of $AX = B$</param>
        /// <returns>The solution $X$</returns>
        [StaticModule(ModuleType = "MathNet.Numerics.LinearAlgebra.Factorization.Solve")]
        public static Matrix<double> Solve(
            [InputPin(PropertyMode = PropertyMode.Allow)] ISolver<double> lhs,
            [InputPin(PropertyMode = PropertyMode.Allow)] Matrix<double> rhs
            )
        {
            return lhs.Solve(rhs);
        }

    }
}
