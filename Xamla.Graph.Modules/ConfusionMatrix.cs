using System.Linq;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.ML.ConfusionMatrix", Description = "This modules calculates a confusion matrix.")]
    public class ConfusionMatrix
        : SingleInstanceMethodModule
    {
        public ConfusionMatrix(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "ConfusionMatrix", Description = "Calculated matrix.")]
        public M<int> Calculate(
            [InputPin(Name = "predictedLabel", Description = "Vector of predicted labels", PropertyMode = PropertyMode.Allow)] V<double> predictedLabel,
            [InputPin(Name = "actualLabel", Description = "Vector of actual labels", PropertyMode = PropertyMode.Allow)] V<double> actualLabel,
            [InputPin(Name = "labels", Description = "Vector of all labels, the order for the confusion Matrix; must contain all labels", PropertyMode = PropertyMode.Allow)] V<double> labels
        )
        {
            return CreateConfusionMatrix(predictedLabel, actualLabel, labels);
        }

        // calculate mxm confusionmatrix, given V<double> predictedLabel, V<double> actualLabel in double^n, V<double> Labels in double^k
        public static M<int> CreateConfusionMatrix(V<double> predictedLabel, V<double> actualLabel, V<double> labels)
        {
            var confusion = new M<int>(labels.Count(), labels.Count());
            for (var j = 0; j < predictedLabel.Count(); j++)
            {
                var pos1 = labels.IndexOf(predictedLabel[j]);
                var pos2 = labels.IndexOf(actualLabel[j]);
                confusion[pos1, pos2] += 1;
            }
            return confusion;
        }

        public static M<int> CreateConfusionMatrixForLabels(V<double> predictedLabel, V<double> actualLabel, V<double> labels)
        {
            var modPredictedLabel = predictedLabel.Where(value => labels.IndexOf(value) != -1);
            var modActualLabel = actualLabel.Where(value => labels.IndexOf(value) != -1);

            return CreateConfusionMatrix(predictedLabel, actualLabel, labels);
        }
    }

    [Module(ModuleType = "Xamla.ML.ConfusionMatrixMetric", Description = "This modules calculates some metrices for a given confusion matrix.")]
    public class ConfusionMatrixMetric
        : SingleInstanceMethodModule
    {
        public ConfusionMatrixMetric(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "Metric", Description = "Value of calculated metric.")]
        public double Calculate(
            [InputPin(Name = "M", Description = "Confusion matrix.", PropertyMode = PropertyMode.Allow)] M<int> confusionM,
            [InputPin(Name = "Metric", Description = "Metrix. _MicroAverage and _MacroAverage differ in how the average is calculated.", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText)] Metric metric
        )
        {
            var value = PrecisionMicroAverage((M<int>)confusionM);
            return value;
        }

        public static int All(M<int> confusionM)
        {
            return confusionM.UnderlyingArray.Sum();
        }

        public static int Positive(M<int> confusionM, int label)
        {
            int count = 0;
            for (var i = 0; i < confusionM.Rows; i++)
            {
                count += confusionM[i, label];
            }
            return count;
        }

        public static int Negative(M<int> confusionM, int label)
        {
            return All(confusionM) - Positive(confusionM, label);
        }

        public static int TruePositive(M<int> confusionM, int label)
        {
            return confusionM[label, label];
        }

        public static int FalseNegative(M<int> confusionM, int label)
        {
            return Positive(confusionM, label) - TruePositive(confusionM, label);
        }

        public static int FalsePositive(M<int> confusionM, int label)
        {
            int count = 0;
            for (var i = 0; i < confusionM.Rows; i++)
            {
                if (i != label)
                    count += confusionM[label, i];
            }
            return count;
        }

        public static int TrueNegative(M<int> confusionM, int label)
        {
            return Negative(confusionM, label) - FalsePositive(confusionM, label);
        }

        public enum Metric
        {
            PrecisionMicroAverage = 0,
            AccuracyAverage = 1,
            PrecisionMacroAverage = 2,
            RecallMacroAverage = 3,
            F1ScoreMacroAverage = 4,
            ErrorRate = 5,
            RecallMicroAverage = 6,
            F1ScoreMicroAverage = 7
        }

        public static double MetricFac(Metric metric, M<int> confusion)
        {
            double value = 0;
            switch ((int)metric)
            {
                case 0:
                    value = PrecisionMicroAverage(confusion);
                    break;
                case 1:
                    value = AccuracyAverage(confusion);
                    break;
                case 2:
                    value = PrecisionMacroAverage(confusion);
                    break;
                case 3:
                    value = RecallMacroAverage(confusion);
                    break;
                case 4:
                    value = F1ScoreMacroAverage(confusion);
                    break;
                case 5:
                    value = ErrorRate(confusion);
                    break;
            }

            return value;
        }

        private static double PrecisionMicroAverage(M<int> m)
        {
            double numerator = 0;
            double denominator = All(m); ; // what if denominator stays 0?
            for (var i = 0; i < m.Rows; i++)
            {
                numerator += TruePositive(m, i);
            }
            return numerator / denominator;
        }

        private static double AccuracyAverage(M<int> m)
        {
            double numerator = 0;
            double denominator = All(m); // what if denominator stays 0?
            for (var i = 0; i < m.Rows; i++)
            {
                numerator += TruePositive(m, i) + TrueNegative(m, i);
            }
            return (numerator / denominator) / m.Rows;
        }

        private static double PrecisionMacroAverage(M<int> m)
        {
            double numerator = 0;
            double denominator = m.Rows; // what if denominator stays 0?
            for (var i = 0; i < m.Rows; i++)
            {
                numerator += TruePositive(m, i) / (TruePositive(m, i) + FalsePositive(m, i));
            }
            return numerator / denominator;
        }

        private static double RecallMacroAverage(M<int> m)
        {
            double numerator = 0;
            double denominator = m.Rows; // what if denominator stays 0?
            for (var i = 0; i < m.Rows; i++)
            {
                numerator += TruePositive(m, i) / (TruePositive(m, i) + FalseNegative(m, i));
            }
            return numerator / denominator;
        }

        private static double F1ScoreMacroAverage(M<int> m)
        {
            var precision = PrecisionMacroAverage(m);
            var recall = RecallMacroAverage(m);

            return (2 * precision * recall) / (precision + recall);
        }

        private static double ErrorRate(M<int> m)
        {
            double numerator = 0;
            double denominator = All(m);
            for (var i = 0; i < m.Rows; i++)
            {
                numerator += FalsePositive(m, i) + FalseNegative(m, i);
            }
            return (numerator / denominator) / m.Rows;
        }

        private static double RecallMicroAverage(M<int> m)
        {
            double numerator = 0;
            double denominator = 0;
            for (var i = 0; i < m.Rows; i++)
            {
                numerator += TruePositive(m, i);
                denominator += TruePositive(m, i) + FalsePositive(m, i);
            }
            return numerator / denominator;
        }

        private static double F1ScoreMicroAverage(M<int> m)
        {
            double numerator = 2 * PrecisionMicroAverage(m) * RecallMicroAverage(m);
            double denominator = PrecisionMicroAverage(m) + RecallMicroAverage(m);

            return numerator / denominator;
        }
    }

    [Module(ModuleType = "Xamla.ML.Extract2DConfusionMatrix", Description = "This modules extracts a 2D confusion matrix for a given multidimensional confusion matrix and a given index.")]
    class Extract2DConfusionMatrix
        : SingleInstanceMethodModule
    {
        public Extract2DConfusionMatrix(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "2dConfusionMatrix", Description = "2d confusion matrix for given label.")]
        public M<int> Extract(
            [InputPin(Name = "M", Description = "confusion matrix", PropertyMode = PropertyMode.Allow)] M<int> confusionMulti,
            [InputPin(Name = "index", Description = "index of label", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText)] int label
        )
        {
            var confusion = new M<int>(2, 2);
            confusion[0, 0] = ConfusionMatrixMetric.TruePositive(confusionMulti, label);
            confusion[1, 0] = ConfusionMatrixMetric.Positive(confusionMulti, label) - confusion[0, 0];
            confusion[1, 0] = ConfusionMatrixMetric.Negative(confusionMulti, label) - confusionMulti[0, 0];
            confusion[1, 1] = ConfusionMatrixMetric.All(confusionMulti) - confusion[1, 0];
            return confusion;
        }
    }
}
