using System;
using System.Globalization;
using System.IO;
using System.Text;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.IO.MToWritable", Description = "This module converts matrix to a writeable, in csv format.")]
    public class MToWritable
        : SingleInstanceMethodModule
    {
        public MToWritable(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "Writeable", Description = "Writable.")]
        public IWritable Write(
            [InputPin(Name = "M", Description = "source Matrix", PropertyMode = PropertyMode.Allow)]
                M m,
            [InputPin(Name = "Delimiter", Description = "Character which should be used as delimiter between two data fields", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText)]
                string delimiter = ",",
            [InputPin(Name = "Headline", Description = "Write a Headline", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.SingleLineText)]
                string[] headline = null
        )
        {
            return Writable.Create(async (fileStream, cancel) =>
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    var sb = new StringBuilder();
                    if (headline != null)
                    {
                        for (int i = 0; i < headline.Length; ++i)
                        {
                            if (i > 0)
                                sb.Append(delimiter);

                            var s = headline[i];
                            s = s.Replace("\"", "\"\"");        // duplicate quotation marks " -> ""
                            sb.Append('"');     // start of string
                            sb.Append(s);
                            sb.Append('"');     // end of string
                        }

                        await writer.WriteLineAsync(sb.ToString());
                        sb.Clear();
                    }

                    var a = m.UnderlyingArray;
                    for (int i = 0; i < a.Dimension[0]; i++)
                    {
                        if ((i % 100) == 0)
                            cancel.ThrowIfCancellationRequested();

                        for (int j = 0; j < a.Dimension[1]; j++)
                        {
                            if (j > 0)
                                sb.Append(delimiter);
                            sb.Append(Convert.ToString(a.GetValue(i, j), CultureInfo.InvariantCulture));
                        }

                        await writer.WriteLineAsync(sb.ToString());      // we always add a line break - even when writing the last line of the CSV file
                        sb.Clear();
                    }

                    await writer.FlushAsync();
                }
            });
        }
    }
}
