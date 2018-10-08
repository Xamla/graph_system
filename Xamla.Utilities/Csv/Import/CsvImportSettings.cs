namespace Xamla.Utilities.Csv.Import
{
    public class CsvImportSettings
    {
        public CsvImportSettings()
        {
            this.CommentSymbol = "#";
            this.StringQuoting = "\"'";
            this.Delimiters = ",";
            this.TwoQuotationMarksEscape = true;
            this.MultilineStrings = true;
            this.SkipEmptyLines = true;
        }

        public string Encoding { get; set; }
        public bool SkipFirstLine { get; set; }
        public bool SkipEmptyLines { get; set; }
        public string CommentSymbol { get; set; }
        public string Delimiters { get; set; }
        public string StringQuoting { get; set; }
        public bool Escaping { get; set; }
        public bool TwoQuotationMarksEscape { get; set; }
        public bool MultilineStrings { get; set; }
    }
}
