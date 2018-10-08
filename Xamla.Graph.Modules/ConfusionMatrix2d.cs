using System;
using System.Linq;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.ML.ConfusionMatrix2d", Description = "This modules calculates a confusion matrix. Assumes that labels are $0$ or $1$. If not use ConfusionMatrix")]
    public class ConfusionMatrix2d
        : SingleInstanceMethodModule
    {
        public ConfusionMatrix2d(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "ConfusionMatrix", Description = "Calculated matrix.")]
        public M<int> Calculate(
            [InputPin(Name = "predictedLabel", Description = "Vector of predicted labels", PropertyMode = PropertyMode.Allow)] V<double> predictedLabel,
            [InputPin(Name = "actualLabel", Description = "Vector of actual labels", PropertyMode = PropertyMode.Allow)] V<double> actualLabel
            )
        {
            return CreateConfusionMatrix2d(predictedLabel, actualLabel);
        }

        // calculate 2x2 confusionmatrix, given V<double> predictedLabel, V<double> actualLabel in {0,1}^n
        //public static M<int> createConfusionMatrix2d(V<double> predictedLabel, V<double> actualLabel)
        //{
        //    var confusion = new M<int>(2, 2);
        //    for(var i = 0; i < predictedLabel.Count(); i++)
        //    {
        //        confusion[(int) predictedLabel[i],(int) actualLabel[i]]++;
        //    }
        //    return confusion;
        //}

        public static M<int> CreateConfusionMatrix2d(V<double> predictedLabel, V<double> actualLabel, V<double> labels = null)
        {
            if (labels == null)
            {
                labels = new V<double>(new double[] { 0, 1 });
            }

            var confusion = new M<int>(2, 2);
            for (var i = 0; i < predictedLabel.Count(); i++)
            {
                var pos1 = (int)labels.IndexOf(predictedLabel[i]);
                var pos2 = (int)labels.IndexOf(actualLabel[i]);
                confusion[pos1, pos2]++;
            }
            return confusion;
        }
    }

    [Module(ModuleType = "Xamla.ML.ConfusionMatrix2dMetric", Description = "This modules calculates some metrices for a given confusion matrix.")]
    public class ConfusionMatrix2dMetric
        : SingleInstanceMethodModule
    {
        public ConfusionMatrix2dMetric(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "Metric", Description = "Value of calculated metric.")]
        public double Calculate(
            [InputPin(Name = "M", Description = "Confusion matrix", PropertyMode = PropertyMode.Allow)] M<int> confusion,
            [InputPin(Name = "Metric", Description = "One of the following metricies:", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText)] Metric metric
        )
        {
            return metricFac(metric, (M<int>)confusion);
        }

        public enum Metric
        {
            sensitivity = 0,
            specificity = 1,
            precision = 2,
            negativePredictiveValue = 3,
            fallOut = 4,
            falseDiscoveryRate = 5,
            missRate = 6,
            accuracy = 7,
            f1Score = 8,
            matthewsCorrelationCoefficient = 9
        }

        public static double metricFac(Metric metric, M<int> confusion)
        {
            double value = 0;
            switch ((int)metric)
            {
                case 0:
                    value = Sensitivity(confusion);
                    break;
                case 1:
                    value = Specificity(confusion);
                    break;
                case 2:
                    value = Precision(confusion);
                    break;
                case 3:
                    value = NegativePredictiveValue(confusion);
                    break;
                case 4:
                    value = FallOut(confusion);
                    break;
                case 5:
                    value = FalseDiscoveryRate(confusion);
                    break;
                case 6:
                    value = MissRate(confusion);
                    break;
                case 7:
                    value = Accuracy(confusion);
                    break;
                case 8:
                    value = F1Score(confusion);
                    break;
                case 9:
                    value = MatthewsCorrelationCoefficient(confusion);
                    break;
            }

            return value;
        }

        // list of metricies packaged in enum?
        private static double Sensitivity(M<int> m) // or called recall
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return truePositive / (truePositive + falseNegative);
        }

        private static double Specificity(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return trueNegative / (trueNegative + falsePositive);
        }

        private static double Precision(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return truePositive / (truePositive + falsePositive);
        }

        private static double NegativePredictiveValue(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return trueNegative / (trueNegative + falseNegative);
        }

        private static double FallOut(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return falsePositive / (falsePositive + trueNegative);
        }

        private static double FalseDiscoveryRate(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return falsePositive / (falsePositive + truePositive);
        }

        private static double MissRate(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return falseNegative / (falseNegative + truePositive);
        }

        private static double Accuracy(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return (truePositive + trueNegative) / (truePositive + falsePositive + trueNegative + falseNegative);
        }

        private static double F1Score(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            return (2 * truePositive) / (2 * truePositive + falsePositive + falseNegative);
        }

        private static double MatthewsCorrelationCoefficient(M<int> m)
        {
            double truePositive = m[0, 0];
            double falsePositive = m[0, 1];
            double falseNegative = m[1, 0];
            double trueNegative = m[1, 1];
            double positive = truePositive + falsePositive;
            double negative = trueNegative + falseNegative;
            double numerator = truePositive * trueNegative - falsePositive * falseNegative;
            double denominator = Math.Sqrt(positive * (truePositive + falseNegative) * (trueNegative + falsePositive) * negative);
            return numerator / denominator;
        }
    }
}
