using System.Collections.Generic;
using System.Diagnostics;

namespace Xamla.Utilities
{
    public static class ProcessHelper
    {
        public enum Concatenation
        {
            None,
            Prepend,
            Append
        }

        public static void MergeEnvironmentVariables(IReadOnlyDictionary<string, string> source, Concatenation concatenate, IDictionary<string, string> target)
        {
            foreach(var (key, value) in source)
            {
                MergeEnvironmentVariables(key, value, concatenate, target);
            }
        }

        public static void MergeEnvironmentVariables(string key, IReadOnlyList<string> source, Concatenation concatenate, IDictionary<string, string> target)
        {
            foreach(var value in source)
            {
                MergeEnvironmentVariables(key, value, concatenate, target);
            }
        }

        public static void MergeEnvironmentVariables(string key, string value, Concatenation concatenate, IDictionary<string, string> target)
        {
            if (target.ContainsKey(key))
            {
                if (concatenate == Concatenation.Append)
                {
                    value = $"{target[key]}:{value}";
                }
                else if (concatenate == Concatenation.Prepend)
                {
                    value = $"{value}:{target[key]}";
                }
                target.Remove(key);
            }

            target.Add(key, value);
        }
    }
}
