using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities.Csv.Import
{
    public class CsvReaderSettings
    {
        public CsvReaderSettings()
        {
            this.SkipEmptyLines = true;
            this.MultilineStrings = true;
            this.MaxTokenLength = 1024 * 1024;
            this.StartOfComment = "#";
            this.Delimiters = new[] { ',', ';' };
            this.Quotes = new[] { '\"', '\'' };
            this.TwoQuotationMarkEscaping = true;
        }

        public bool SkipFirstLine { get; set; }
        public bool SkipEmptyLines { get; set; }
        public char[] Quotes { get; set; }
        public char[] Delimiters { get; set; }
        public string StartOfComment { get; set; }
        public bool EscapedStrings { get; set; }
        public bool TwoQuotationMarkEscaping { get; set; }
        public bool MultilineStrings { get; set; }
        public int? MaxTokenLength { get; set; }
    }

    public class CsvReader
    {
        const char BACKSLASH = '\\';

        static readonly Dictionary<char, char> ESCAPINGS = new Dictionary<char, char>
        {
            { '\'', '\x0027' },
            { '\"', '\x0022' },
            { '\\', '\x005c' },
            { '0', '\x0000' },
            { 'a', '\x0007' },
            { 'b', '\x0008' },
            { 'f', '\x000c' },
            { 'n', '\x000a' },
            { 'r', '\x000d' },
            { 't', '\x0009' },
            { 'v', '\x000b' },
        };

        CsvReaderSettings settings;
        List<string> currentRow;
        StringBuilder token;
        TextReader reader;
        char current;
        bool comment;
        
        public CsvReader()
            : this(new CsvReaderSettings())
        {
        }

        public CsvReader(CsvReaderSettings settings)
        {
            this.settings = settings;
        }

        public CsvReaderSettings Settings
        {
            get { return settings; }
        }

        void AddToken()
        {
            currentRow.Add(token.ToString());
            token.Clear();
        }

        bool IsDelimiter(char c)
        {
            return settings.Delimiters != null && settings.Delimiters.Contains(c);
        }

        bool IsQuotationMark(char c)
        {
            return settings.Quotes != null && settings.Quotes.Contains(c);
        }

        public IEnumerable<string[]> Read(TextReader input)
        {
            reader = input;
            token = new StringBuilder();
            currentRow = null;
            comment = false;
            bool firstLine = true;

            while (MoveNext())
            {
                if (currentRow == null)
                {
                    currentRow = new List<string>();
                }

                var c = current;

                if (!comment)
                {
                    if (IsDelimiter(c))
                    {
                        AddToken();
                    }
                    else if (IsQuotationMark(c) && (token.Length == 0 || string.IsNullOrWhiteSpace(token.ToString())))
                    {
                        // string-literal parsing starts only when the quotation marks are the first non-whitespace characters of a field
                        token.Clear();
                        ReadString(c);
                    }
                    else if (c != '\r' && c != '\n')
                    {
                        AppendToken(c);

                        if (CompareBack(settings.StartOfComment))
                        {
                            token.Length = token.Length - settings.StartOfComment.Length;
                            comment = true;
                        }
                    }
                }

                if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && reader.Peek() == '\n')
                        reader.Read();  // skip newline char

                    // finish line
                    if (token.Length > 0)
                        AddToken();

                    if ((currentRow.Count > 0 || !settings.SkipEmptyLines) && (!firstLine || !settings.SkipFirstLine))
                        yield return currentRow.ToArray();

                    firstLine = false;
                    comment = false;
                    currentRow = null;
                }
            }

            // complete last line (end of stream case)

            if (token.Length > 0)
                AddToken();
                
            if (currentRow != null)
            {
                if ((currentRow.Count > 0 || !settings.SkipEmptyLines) && (!firstLine || !settings.SkipFirstLine))
                    yield return currentRow.ToArray();
            }
        }

        bool MoveNext()
        {
            int c = reader.Read();
            if (c < 0)
                return false;

            current = (char)c;
            return true;
        }

        char Current
        {
            get { return current; }
        }

        bool IsNextChar(char value)
        {
            int c = reader.Peek();
            if (c < 0)
                return false;

            return c == value;
        }
        
        void AppendToken(char c)
        {
            token.Append(c);
            if (settings.MaxTokenLength < token.Length)
                throw new IndexOutOfRangeException("Token too long.");
        }

        public bool CompareBack(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || token.Length < value.Length)
            {
                return false;
            }

            for (int i = value.Length - 1, j = token.Length - 1; i >= 0; i--, j--)
            {
                if (value[i] != token[j])
                {
                    return false;
                }
            }

            return true;
        }

        private void ReadString(char quotationMark)
        {
            while (MoveNext())
            {
                if (!settings.MultilineStrings && (current == '\r' || current == '\n'))
                {
                    // reached end of line before quotation char was seen
                    break;
                }
                if (current == quotationMark)
                {
                    if (settings.TwoQuotationMarkEscaping && IsNextChar(quotationMark))
                    {
                        reader.Read();
                        AppendToken(quotationMark);
                    }
                    else
                        break;
                }
                else if (settings.EscapedStrings && current == BACKSLASH)
                {
                    if (!ReadEscapedCharacter())
                    {
                        break;
                    }
                }
                else
                {
                    AppendToken(current);
                }
            }
        }

        private bool ReadEscapedCharacter()
        {
            if (!MoveNext())
                return false;

            char replacement;
            if (ESCAPINGS.TryGetValue(current, out replacement))
            {
                AppendToken(replacement);
            }
            else
            {
                AppendToken(BACKSLASH);
                AppendToken(current);
            }

            return true;
        }
    }
}
