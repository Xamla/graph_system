using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (maxLength < 3)
                throw new ArgumentOutOfRangeException("maxLength must be greater or equal to 3.");

            if (value.Length <= maxLength)
                return value;

            return value.Remove(maxLength - 3) + "...";
        }

        public static string StripAnsiEscapes(this string value) =>
            Regex.Replace(value, @"\x1b(\[.*?[@-~]|\].*?(\x07|\x1b\\))", string.Empty);     // '(' + CSI + '.*?' + CMD + '|' + OSC + '.*?' + '(' + ST + '|' + BEL + ')' + ')'
    }
}
