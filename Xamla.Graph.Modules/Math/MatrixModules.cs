using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Types;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules
{
    public static class MatrixModules
    {
        /// <summary>
        /// Multiplies matrix a with matrix b.
        /// </summary>
        /// <param name="a">A matrix</param>
        /// <param name="b">A matrix</param>
        /// <returns>The resulting matrix.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Multiply")]
        public static M<double> Multiply(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> a,
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> b
        )
        {
            return a.Multiply(b);
        }

        /// <summary>
        /// Scale a matrix with a real number, i.e. multiply each entry with it.
        /// </summary>
        /// <param name="matrix">The matrix to be scaled.</param>
        /// <param name="factor">The factor</param>
        /// <returns>The matrix scaled with the factor.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Scale")]
        public static M<double> Scale(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Default)] double factor
        )
        {
            return matrix.Map(x => factor * x);
        }

        /// <summary>
        /// Returns $r$-th row of the matrix $m$
        /// </summary>
        /// <param name="matrix">The matrix we want to select a row from</param>
        /// <param name="rowIndex">The index of the row</param>
        /// <returns>The corresponding row vector</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.GetRow")]
        public static V<double> GetRow(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Default)] int rowIndex
        )
        {
            return matrix.GetRow(rowIndex);
        }

        /// <summary>
        /// Returns $r$-th column of the matrix $m$
        /// </summary>
        /// <param name="matrix">The matrix we want to select a column from</param>
        /// <param name="columnIndex">The index of the column</param>
        /// <returns>The corresponding column vector</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.GetColumn")]
        public static V<double> GetColumn(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Default)] int columnIndex
        )
        {
            return matrix.GetColumn(columnIndex);
        }

        /// <summary>
        /// Transpose the matrix.
        /// </summary>
        /// <param name="matrix">The matrix you want to take the transpose of.</param>
        /// <returns>The transposed matrix.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Transpose")]
        public static M<double> Transpose(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix
        )
        {
            return matrix.Transpose();
        }

        /// <summary>
        /// Calculate the trace of a matrix, i.e. the sum of of the entries on the main diagonal.
        /// </summary>
        /// <param name="matrix">The matrix we want to calculate the trace of.</param>
        /// <returns>The trace of a matrix.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Trace")]
        public static double Trace(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix
        )
        {
            if (matrix.Rows != matrix.Columns)
            {
                throw new ArgumentException("The trace is only defined for square matrices.");
            }

            else
            {
                double trace = 0;
                for (int i = 0; i < matrix.Rows; i++)
                {
                    trace += matrix.UnderlyingArray[i, i];
                }
                return trace;
            }

        }

        /// <summary>
        /// Get the minimal entry in a matrix or vector.
        /// </summary>
        /// <param name="matrix">The matrix or vector we want to get the minimal entry from. Note that vectors of type V automatically get casted to M.</param>
        /// <returns>
        /// <return name="minValue">The minimal element of the matrix or vector.</return>
        /// <return name="index">Int2 value containing the index (Y = Row, X = Column) of the first element which has minimal value.</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Min")]
        public static Tuple<double, Int2> Min(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix
        )
        {
            var pos = new Int2();
            double v = double.MaxValue;

            for (int i = 0; i < matrix.Rows; ++i)
            {
                for (int j = 0; j < matrix.Columns; ++j)
                {
                    if (matrix[i, j] < v)
                    {
                        v = matrix[i, j];
                        pos = new Int2(j, i);
                    }
                }
            }

            return Tuple.Create(v, pos);
        }

        /// <summary>
        /// Get the maximal entry in a matrix or vector.
        /// </summary>
        /// <param name="matrix">The matrix or vector we want to get the maximal entry from. Note that vectors of type V automatically get casted to M.</param>
        /// <returns>
        /// <return name="maxValue">The maximal elementes of the matrix or vector.</return>
        /// <return name="index">Int2 value containing the index (Y = Row, X = Column) of the first element which has maximal value.</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Max")]
        public static Tuple<double, Int2> Max(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix
        )
        {
            var pos = new Int2();
            double v = double.MinValue;

            for (int i = 0; i < matrix.Rows; ++i)
            {
                for (int j = 0; j < matrix.Columns; ++j)
                {
                    if (matrix[i, j] > v)
                    {
                        v = matrix[i, j];
                        pos = new Int2(j, i);
                    }
                }
            }

            return Tuple.Create(v, pos);
        }

        /// <summary>
        /// The scalar or dot product of two vectors, i.e. for
        /// $v = (v_1, \dots, v_n)$, $w = (w_1, \dots, w_n)$
        /// the dot product is defined as $\langle v, w \rangle = \sum_{i=1}^n v_i\cdot w_i$.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">A second vector of the same length.</param>
        /// <returns>The dot product of these vectors.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.DotProduct")]
        public static double DotProduct(
            [InputPin(PropertyMode = PropertyMode.Allow)] V<double> vector1,
            [InputPin(PropertyMode = PropertyMode.Allow)] V<double> vector2
        )
        {
            return vector1.Dot(vector2);
        }

        /// <summary>
        /// Normalize a vector to unit length w.r.t. the usual euclidean norm.
        /// </summary>
        /// <param name="vector">The vector to be normalized.</param>
        /// <returns>A normalized vector.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Normalize")]
        public static V<double> Normalize(
            [InputPin(PropertyMode = PropertyMode.Allow)] V<double> vector
        )
        {
            return vector.Normalize();
        }

        /// <summary>
        /// Clip each entry of the matrix $(m_{ij})$ such that $min \leq m_{ij} \leq max$ for all $i, j$. If not $max \geq min$ the default operation sets $max = min$, resulting in a matrix which has $min$ in every entry.
        /// Leave min or max empty, if you only want one sided boundaries.
        /// </summary>
        /// <param name="matrix">A matrix</param>
        /// <param name="min">The minimum for each entry; leave empty if you do not want to establish a lower boundary</param>
        /// <param name="max">The maximum for each entry; leave empty if you do not want to establish an upper boundary</param>
        /// <returns></returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Clip")]
        public static M<double> Clip(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Default)] double? min = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double? max = null
        )
        {
            int rows = matrix.Rows;
            int columns = matrix.Columns;
            //return M.Generate<double>(rows, columns, (i, j) => Range.Clamp(matrix[i, j], min??double.MinValue, max??double.MaxValue));
            return matrix.Map(x => Range.Clamp(x, min ?? double.MinValue, max ?? double.MaxValue));
        }

        /// <summary>
        /// Return a submatrix in block form.
        /// </summary>
        /// <param name="matrix">The base matrix, say $(m_{ij})$.</param>
        /// <param name="firstRow">The first row index $i_1$</param>
        /// <param name="firstColumn">The first column index $j_1$</param>
        /// <param name="lastRow">The last row index $i_2$</param>
        /// <param name="lastColumn">The last column index $j_2$</param>
        /// <returns>$(m_{ij})_{i_1 \leq i \leq i_2, ~ j_1 \leq j \leq j_2}$</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.SubMatrix")]
        public static M<double> SubMatrix(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Default)] int firstRow,
            [InputPin(PropertyMode = PropertyMode.Default)] int firstColumn,
            [InputPin(PropertyMode = PropertyMode.Default)] int lastRow,
            [InputPin(PropertyMode = PropertyMode.Default)] int lastColumn
        )
        {
            if (firstRow < 0) { firstRow = 0; }
            if (firstColumn < 0) { firstColumn = 0; }
            if (lastRow > matrix.Rows) { lastRow = matrix.Rows; }
            if (lastColumn > matrix.Columns) { lastColumn = matrix.Columns; }

            /*
             * In case of lastRow < firstRow or the same for columns, I think the right way
             * to handle this situation would be to raise an exception. However, I guess
             * users would appreciate a more fault tolerant approach, so we swap the corresponding
             * values.
             */

            if (lastRow < firstRow)
            {
                int t = lastRow;
                lastRow = firstRow;
                firstRow = t;
            }

            if (lastColumn < firstColumn)
            {
                int t = lastColumn;
                lastColumn = firstColumn;
                firstColumn = t;
            }

            return M.Generate<double>(
                lastRow - firstRow,
                lastColumn - firstColumn,
                (i, j) => matrix[i + firstRow, j + firstColumn]
                );
        }

        /// <summary>
        /// Generate a matrix filled with values from a sequence.
        /// </summary>
        /// <param name="seq">The sequence providing the values.</param>
        /// <param name="rows">The number of rows of the resulting matrix.</param>
        /// <param name="columns">The number of columns of the resulting matrix.</param>
        /// <returns>A matrix filled with values from a sequence.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.FromSequence")]
        public static M<double> GenerateMatrixFromSequence(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<double> seq,
            [InputPin(PropertyMode = PropertyMode.Default)] int rows,
            [InputPin(PropertyMode = PropertyMode.Default)] int columns
        )
        {
            var m = new M<double>(rows, columns);

            using (var enumerator = seq.GetEnumerator())
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (!enumerator.MoveNext())
                            return m;

                        m[i, j] = enumerator.Current;
                    }
                }
            }

            return m;
        }

        /// <summary>
        /// Replace all entries in a matrix by lowerValue or upperValue (default 0 and 1), if they are below or above
        /// the specified threshhold respectively.
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <param name="threshhold">The threshhold value</param>
        /// <param name="lowerValue">The value each entry below the threshhold gets replaced with (default 0)</param>
        /// <param name="upperValue">The value each entry above the threshhold gets replaced with (default 1)</param>
        /// <returns>A matrix with all values replaced.</returns>
        [StaticModule(ModuleType = "Xamla.Math.Matrix.Threshhold")]
        public static M<double> BinaryMatrix(
            [InputPin(PropertyMode = PropertyMode.Allow)] M<double> matrix,
            [InputPin(PropertyMode = PropertyMode.Default)] double threshhold,
            [InputPin(PropertyMode = PropertyMode.Default)] double lowerValue = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double upperValue = 1
        )
        {
            return matrix.Map(x => x < threshhold ? lowerValue : upperValue);
        }
    }
}
