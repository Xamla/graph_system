using System;
using System.Text.RegularExpressions;

namespace Xamla.Utilities
{
    public static class Base64Url
    {
        static readonly Regex base64UrlCharsetCheck = new Regex(@"^[A-Za-z0-9\-_=]+$");

        public static bool IsValid(string token)
        {
            if (token != null)
            {
                if ((token.Length % 4) != 1 && base64UrlCharsetCheck.IsMatch(token))
                    return true;
            }
            return false;
        }

        public static byte[] Parse(string token)
        {
            if (!IsValid(token))
            {
                throw new ArgumentException("The specified string is not a valid base64url string.", "token");
            }

            token = token.Replace('-', '+').Replace('_', '/');
            var base64 = token.PadRight(token.Length + (4 - token.Length % 4) % 4, '=');
            return Convert.FromBase64String(base64);
        }

        public static string ToBase64Url(this byte[] buffer)
        {
            return System.Convert.ToBase64String(buffer).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static string ToBase64(this byte[] buffer)
        {
            return System.Convert.ToBase64String(buffer);
        }
    }
}
