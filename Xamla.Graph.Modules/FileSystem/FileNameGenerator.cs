using System;
using System.Globalization;
using System.IO;
using Xamla.Graph.MethodModule;


namespace Xamla.Graph.Modules.FileSystem
{
    /// <summary>
    /// FileName Generator class
    /// </summary>
    [Module(ModuleType = "Xamla.IO.FileNameGenerator", Description = "Autogenerate file name strings")]
    public class FileNameGenerator
        : SingleInstanceMethodModule
    {
        long counter;

        /// <summary>
        ///
        /// </summary>
        /// <param name="runtime"></param>
        public FileNameGenerator(IGraphRuntime runtime)
            : base(runtime)
        {
            this.counter = 1;
        }

        /// <summary>
        /// On started of graph reset the counter to one
        /// </summary>
        protected override void OnStart()
        {
            counter = (long)this.Properties.GetProperty("sequenceStartValue").Value;
        }

        /// <summary>
        /// Enum with all possible modes of the file name generator
        /// </summary>

        public enum FileNameGeneratorMode
        {
            /// <summary>
            /// Generator creates a string with the current date (year, month, day)
            /// and add it between the prefixed and subsequent string.
            /// </summary>
            Date,
            /// <summary>
            /// Generator creates a string with the current time (hours, minutes, seconds, milliseconds)
            /// and add it between the prefixed and subsequent string.
            /// </summary>
            Time,
            /// <summary>
            /// Generator creates a string with the current date followed by the current time
            /// and add it between the prefixed and subsequent string.
            /// </summary>
            Date_Time,
            /// <summary>
            /// Generator creates a string with a simple numeric sequence starting with 1
            /// add add it between the prefixed and subsequent string.
            /// </summary>
            Sequence,

            /// <summary>
            /// Generator creates a string with a global unique idendentifier.
            /// </summary>
            Guid
        };

        /// <summary>
        /// Autogenerate file name strings
        /// </summary>
        /// <param name="generatorMode">filename generator mode, see details by hover over dropdown items</param>
        /// <param name="prefix">string which is added in front of the generated string</param>
        /// <param name="postfix">string which is add subsequent to the generated string</param>
        /// <param name="sequenceStartValue">when sequence mode is active sequence start with this value</param>
        /// <param name="useUtcTime">If checkbox is selected Utc time is used instead of local time</param>
        /// <returns>
        /// <return name="joinedString">concatenated string of the prefixed, generated and subsequent string</return>
        /// </returns>
        [ModuleMethod]
        [OutputPin(Name = "fileName", Description = "Concatenated string of the prefixed, generated and subsequent string")]
        public string GenerateFileName(
           [InputPin(PropertyMode = PropertyMode.Default)] string prefix = "output",
           [InputPin(PropertyMode = PropertyMode.Default)] string postfix = "",
           [InputPin(PropertyMode = PropertyMode.Default)] FileNameGeneratorMode generatorMode = FileNameGeneratorMode.Sequence,
           [InputPin(PropertyMode = PropertyMode.Default)] long sequenceStartValue = 1,
           [InputPin(PropertyMode = PropertyMode.Default)] bool useUtcTime = false
        )
        {
            string generatedString = string.Empty;

            Func<string, string> getDateTime;
            if (useUtcTime)
            {
                getDateTime = format => DateTime.UtcNow.ToString(format, CultureInfo.InvariantCulture);
            }
            else
            {
                getDateTime = format => DateTime.Now.ToString(format, CultureInfo.InvariantCulture);
            }

            switch (generatorMode)
            {
                case FileNameGeneratorMode.Date:
                    generatedString = getDateTime("yyyyMMdd");
                    break;

                case FileNameGeneratorMode.Time:
                    generatedString = getDateTime("HHmmss.fff");
                    break;

                case FileNameGeneratorMode.Date_Time:
                    generatedString = getDateTime("yyyyMMdd_HHmmss.fff");
                    break;

                case FileNameGeneratorMode.Sequence:
                    generatedString = counter.ToString("D8");
                    counter++;
                    break;

                case FileNameGeneratorMode.Guid:
                    generatedString = Guid.NewGuid().ToString("N");
                    break;

                default:
                    throw new NotImplementedException("A mode was selected which is not implemented in the FileNameGenerator module.");
            }
            return string.Concat(prefix, generatedString, postfix);
        }
    }
}
