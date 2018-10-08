using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xamla.Utilities.Csv.Import
{
    public class CsvImporter
    {
        public static List<string[]> ImportLinesPreview(Stream stream, CsvImportSettings settings, int numLines)
        {
            return ImportLines(stream, settings).Take(numLines).ToList();
        }

        public static IEnumerable<string[]> ImportLines(Stream stream, CsvImportSettings settings)
        {
            TextReader input;
            if (string.IsNullOrWhiteSpace(settings.Encoding))
                input = new StreamReader(stream);
            else
                input = new StreamReader(stream, Encoding.GetEncoding(settings.Encoding));

            var csvReaderSettings = new CsvReaderSettings
            {
                MaxTokenLength = 1024 * 1024,
                SkipFirstLine = settings.SkipFirstLine,
                SkipEmptyLines = settings.SkipEmptyLines,
                Quotes = settings.StringQuoting != null ? settings.StringQuoting.ToCharArray() : null,
                Delimiters = settings.Delimiters != null ? settings.Delimiters.ToCharArray() : null,
                StartOfComment =  settings.CommentSymbol,
                EscapedStrings = settings.Escaping,
                TwoQuotationMarkEscaping = settings.TwoQuotationMarksEscape,
                MultilineStrings = settings.MultilineStrings
            };

            var csvReader = new CsvReader(csvReaderSettings);
            return csvReader.Read(input);
        }
    }
}
