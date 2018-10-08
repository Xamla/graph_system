using System;
using System.Linq;
using Xamla.Graph.MethodModule;
using Xamla.Types;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.ML.KFoldcrossValidation", Description = "Randomizes and splits an input training set.")]
    public class KFoldcrossValidation
        : SingleInstanceMethodModule
    {
        public KFoldcrossValidation(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "testing", Description = "testing1 = training samples, testing2 = trainging labels, testing3 = testing samples, testing4 = testing labels.")]
        public Tuple<ISequence<M<double>>, ISequence<M<double>>, ISequence<V<double>>, ISequence<V<double>>> CrossValidation(
            [InputPin(Name = "samples", Description = "Matrix of samples", PropertyMode = PropertyMode.Allow)] M<double> samples,
            [InputPin(Name = "labels", Description = "Vector of labels", PropertyMode = PropertyMode.Allow)] V<double> labels,
            [InputPin(Name = "k-fold", Description = "In how many subsets should be divided", PropertyMode = PropertyMode.Default)] int k
            )
        {
            var rowCount = samples.Rows;
            var columnCount = samples.Columns;

            if (rowCount == 0 || columnCount == 0)
                return Tuple.Create(Sequence.Empty<M<double>>(), Sequence.Empty<M<double>>(), Sequence.Empty<V<double>>(), Sequence.Empty<V<double>>());

            // change if samples are always in form: ISequence<V<double>>
            V<double>[] rows = new V<double>[rowCount];
            for (var i = 0; i < rowCount; i++)
                rows[i] = samples.GetRow(i);

            // shuffle samples and labels (in the same way)
            Random rnd = new Random();
            var tuple = Shuffle(rnd, rows, labels);
            rows = tuple.Item1;
            var shuffelLabels = tuple.Item2;

            // create subsamples
            var reminder = (int)rows.Length % k;
            var floor = (int)Math.Floor((double)rows.Count() / k);
            V<double>[] subLabels = CreateSubLabels(k, shuffelLabels);
            M<double>[] subSamples = CreateSubSamples(columnCount, k, rows);

            // create testing/training samples
            M<double>[] trainingSamples = new M<double>[k];
            V<double>[] trainingLabels = new V<double>[k];

            for (var i = 0; i < k; i++)
            {
                int actLength;
                if (i < reminder)
                    actLength = (reminder - 1) * (floor + 1) + (k - reminder) * floor;
                else
                    actLength = reminder * (floor + 1) + (k - reminder - 1) * floor;

                double[] varS = new double[actLength * columnCount];
                double[] varL = new double[actLength];

                // the first rowCount % k blocks have one element more
                var positionS = 0;
                var positionL = 0;
                for (var j = 0; j < reminder; j++)
                {
                    if (j != i)
                    {
                        subSamples[j].UnderlyingArray.Buffer.CopyTo(varS, positionS);
                        positionS += (floor + 1) * columnCount;
                        subLabels[j].UnderlyingArray.Buffer.CopyTo(varL, positionL);
                        positionL += floor + 1;
                    }
                }

                // the last k - rowCount % k blocks not
                for (var j = reminder; j < k; j++)
                {
                    if (j != i)
                    {
                        subSamples[j].UnderlyingArray.Buffer.CopyTo(varS, positionS);
                        positionS += floor * columnCount;
                        subLabels[j].UnderlyingArray.Buffer.CopyTo(varL, positionL);
                        positionL += floor;
                    }
                }

                // reshape the double[] array into a matrix
                var aS = A<double>.FromArray(varS, actLength, columnCount);
                trainingSamples[i] = (M<double>)M.FromArray(aS);
                var aL = A<double>.FromArray(varL, actLength);
                trainingLabels[i] = (V<double>)V.FromArray(aL);
            }

            return Tuple.Create(trainingSamples.ToSequence(), subSamples.ToSequence(), trainingLabels.ToSequence(), subLabels.ToSequence());
        }

        private static M<double>[] CreateSubSamples(int column, int k, V<double>[] rows)
        {
            M<double>[] subSamples = new M<double>[k];
            var reminder = (int)rows.Length % k;
            var floor = (int)Math.Floor((double)rows.Count() / k);
            for (var i = 0; i < reminder; i++)
            {
                var matrix = M.Generate<double>(floor + 1, column, (m, n) => (double)rows[i * (floor + 1) + m][n]);
                subSamples[i] = matrix;
            }
            for (var i = reminder; i < k; i++)
            {
                var matrix = M.Generate<double>(floor, column, (m, n) => (double)rows[i * floor + m][n]);
                subSamples[i] = matrix;
            }
            return subSamples;
        }

        private static V<double>[] CreateSubLabels(int k, V<double> labels)
        {
            V<double>[] subLabels = new V<double>[k];
            var reminder = labels.Rows % k;
            var floor = (int)Math.Floor((double)labels.Rows / k);
            for (var i = 0; i < reminder; i++)
            {
                var matrix = V.Generate<double>((m) => (double)labels[i * (floor + 1) + m], (floor + 1));
                subLabels[i] = matrix;
            }
            for (var i = reminder; i < k; i++)
            {
                var matrix = V.Generate<double>((m) => (double)labels[i * floor + m], floor);
                subLabels[i] = matrix;
            }
            return subLabels;
        }

        private static Tuple<V<double>[], V<double>> Shuffle(Random rnd, V<double>[] rows, V<double> labels)
        {
            for (int i = rows.Length; i > 1; i--)
            {
                int j = rnd.Next(i); // 0 <= j <= i-1
                var tmp = rows[j];
                rows[j] = rows[i - 1];
                rows[i - 1] = tmp;
                var tmp2 = labels[j];
                labels[j] = labels[i - 1];
                labels[i - 1] = tmp2;
            }
            return Tuple.Create(rows, labels);
        }
    }
}
