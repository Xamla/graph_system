using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;
using Xamla.Utilities.Csv.Import;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Graph.Controls.VectorInput", Description = @"Vector input field, to produce a V<double>, with comma seperation: 'x1, x2, ... xn' -> [x1, x2, ..., xn]")]
    public class VectorInput
        : ControlModuleBase
    {
        public VectorInput(IGraphRuntime runtime)
            : base(runtime, null, PinDataTypeFactory.Create<string>(), PinDataTypeFactory.Create<V<double>>(), WellKnownEditors.MultiLineText)
        {
        }

        public string Value
        {
            get { return properties.Get<string>(this.value.Id); }
            set { properties.Set(this.value.Id, value, true); }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var csvSettings = new CsvReaderSettings
            {
                Delimiters = new char[] { ',' },
                SkipEmptyLines = false,
                SkipFirstLine = false,
                MaxTokenLength = 1024 * 1024,
                StartOfComment = ""
            };
            return Task.FromResult(new object[] { Transform(properties.Get<string>(this.value.Id), csvSettings) });
        }

        private static V<double> Transform(string text, CsvReaderSettings csvReaderSettings)
        {
            using (var sr = new StringReader(text))
            {
                var csvReader = new CsvReader(csvReaderSettings);
                var splitted = csvReader.Read(sr).ToArray();
                var rows = splitted[0].Length;

                var v = V<double>.Generate<double>((row) =>
                {
                    double value;
                    if (row < rows && double.TryParse(splitted[0][row], NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        return value;
                    return 0;
                }, rows);
                return v;
            }
        }
    }
}
