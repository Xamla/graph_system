using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Xamla.Graph.MethodModule;
using Xamla.Types;
using Xamla.Utilities.Csv.Import;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.IO.ReadableToM", Description = "This module parses a readable to M, in csv format.")]
    public class ReadableToM
        : StaticMethodModule
    {
        public ReadableToM(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "M", Description = "Matrix M.")]
        public static M<double> Read(
            [InputPin(Name = "Readable", Description = "readable", PropertyMode = PropertyMode.Allow)]
                IReadable readable,
            [InputPin(Name = "Delimiter", Description = "Delimiter", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText)]
                string delimiter = ",",
            [InputPin(Name = "SkipEmptyLines", Description = "Should empty lines be skipped?", PropertyMode = PropertyMode.Default)]
                bool skipEmptyLines = true,
            [InputPin(Name = "SkipFirstLine", Description = "Should the first line be skipped?", PropertyMode = PropertyMode.Default)]
                bool skipFirstLine = false,
            [InputPin(Name = "MaxTokenLength", Description = "Maximal length of a token.", PropertyMode = PropertyMode.Default)]
                int? maxTokenLength = 1024 * 1024,
            [InputPin(Name = "StartOfComment", Description = "Start of a comment line", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText)]
                string startOfComment = "#"
        )
        {
            var settings = new CsvReaderSettings
            {
                Delimiters = delimiter.ToCharArray(),
                SkipEmptyLines = skipEmptyLines,
                SkipFirstLine = skipFirstLine,
                MaxTokenLength = maxTokenLength,
                StartOfComment = startOfComment,
            };

            return Transform(readable, settings);
        }

        private static M<double> Transform(IReadable readable, CsvReaderSettings csvReaderSettings)
        {
            using (var sr = new StreamReader(readable.Open()))
            {
                var csvReader = new CsvReader(csvReaderSettings);
                var splitted = csvReader.Read(sr).ToArray();
                var rows = splitted.Length;

                int maxCols = 0;
                if (rows > 0)
                {
                    maxCols = splitted[0].Length;
                    for (var i = 1; i < rows; i++)
                    {
                        if (splitted[i].Length > maxCols)
                            maxCols = splitted[i].Length;
                    }
                }

                return M.Generate<double>(rows, maxCols, (row, col) =>
                {
                    double value;
                    if (row < rows && col < splitted[row].Length && double.TryParse(splitted[row][col], NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        return value;
                    return 0;
                });
            }
        }
    }
}
