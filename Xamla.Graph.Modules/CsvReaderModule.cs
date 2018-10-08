using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Types.Sequence;
using Xamla.Utilities.Csv.Import;

namespace Xamla.Graph.Modules
{
    public enum CsvReaderEncoding
    {
        Auto,
        ASCII,
        Unicode,
        UTF8,
        UTF32
    }

    [Module(ModuleType = "Xamla.IO.CsvReader", Description = "This module reads the data of a csv file and returns an item as array to every data entity.")]
    public class CsvReaderModule
        : StaticMethodModule
    {
        static Dictionary<CsvReaderEncoding, Encoding> EncodingsMapping = new Dictionary<CsvReaderEncoding, Encoding>
        {
            { CsvReaderEncoding.Auto, null },
            { CsvReaderEncoding.ASCII, Encoding.ASCII },
            { CsvReaderEncoding.Unicode, Encoding.Unicode },
            { CsvReaderEncoding.UTF8, Encoding.UTF8 },
            { CsvReaderEncoding.UTF32, Encoding.UTF32 }
        };

        public CsvReaderModule(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "DataItem", Description = "Data item as array.")]
        public static ISequence<string[]> Read(
            [InputPin(Name = "Path", Description = "Path to the imported csv file", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText, ResolvePath=true)]
                string path,
            [InputPin(Name = "Encoding", Description = "Encoding of the file", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.DropDown)]
                CsvReaderEncoding encoding = CsvReaderEncoding.Auto,
            [InputPin(Name = "SkipFirstLine", Description = "Skip the first line of the file", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.CheckBox)]
                bool skipFirstLine = false,
            [InputPin(Name = "SkipEmptyLines", Description = "Skip lines without text", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.CheckBox)]
                bool skipEmptyLines = true,
            [InputPin(Name = "Delimiters", Description = "Characters which are used as delimiters between two data fields", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.SingleLineText)]
                string delimiters = ";",
            [InputPin(Name = "CommentSymbol", Description = "Leading symbols for comments", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.SingleLineText)]
                string commentSymbol = "#",
            [InputPin(Name = "StringQuoting", Description = "Characters which are used to quote a string", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.SingleLineText)]
                string stringQuoting = "'",
            [InputPin(Name = "MultilineStrings", Description = "A quoted string contains line breaks", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.CheckBox)]
                bool multilineStrings = false,
            [InputPin(Name = "Escaping", Description = "Use character escaping", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.CheckBox)]
                bool escaping = false,
            [InputPin(Name = "TwoQuotationMarksEscape", Description = "Two quotation charactes marks an escaping", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.SingleLineText)]
                bool twoQuotationMarksEscape = false
        )
        {
            var csvImportSettings = new CsvImportSettings
            {
                CommentSymbol = commentSymbol,
                Delimiters = delimiters,
                Encoding = EncodingsMapping[encoding] == null ? null : EncodingsMapping[encoding].WebName,
                Escaping = escaping,
                MultilineStrings = multilineStrings,
                SkipEmptyLines = skipEmptyLines,
                SkipFirstLine = skipFirstLine,
                StringQuoting = stringQuoting,
                TwoQuotationMarksEscape = twoQuotationMarksEscape
            };

            return Sequence.Using(
                () => File.Open(path, FileMode.Open, FileAccess.Read),
                fileStream => CsvImporter.ImportLines(fileStream, csvImportSettings).ToSequence()
            );
        }
    }
}
