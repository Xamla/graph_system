using System;
using System.Collections.Generic;
using System.Text;

namespace Xamla.Utilities.Strings
{
    public class StringMacroMap : IStringMacroMap
    {
        private readonly Dictionary<string, IStringMacroResolver> resolverTable;
        private bool enableEnvironmentVars;

        public StringMacroMap(IEqualityComparer<string> stringComparer)
        {
            resolverTable = new Dictionary<string, IStringMacroResolver>(stringComparer);
            enableEnvironmentVars = true;
        }

        public StringMacroMap()
            : this(StringComparer.OrdinalIgnoreCase)
        {
        }

        private bool LookupVariable(string name, string parameter, out string value)
        {
            if (resolverTable.TryGetValue(name, out IStringMacroResolver stringMacroResolver) && stringMacroResolver.Resolve(name, parameter, out value))
                return true;
            value = null;
            return false;
        }

        private string ExpandInternal(string input)
        {
            if (input == null)
                return null;

            StringBuilder output = new StringBuilder(input.Length);

            int pos = 0;
            while (pos < input.Length)
            {
                int start = input.IndexOf("$(", pos);
                int end = (start != -1) ? input.IndexOf(')', start + 2) : start;
                if (end > start)
                {
                    output.Append(input.Substring(pos, start - pos));

                    string name = input.Substring(start + 2, end - start - 2);
                    string parameter = null;
                    int paramSep = name.IndexOf(':');
                    if (paramSep > 0 && paramSep < name.Length - 1)
                    {
                        parameter = name.Substring(paramSep + 1);
                        name = name.Substring(0, paramSep);
                    }
                    string value;
                    if (LookupVariable(name, parameter, out value))
                    {
                        if (!resolverTable.Comparer.Equals(name, value)) // Expand variables recursively
                            value = Expand(value);
                        output.Append(value);
                        pos = end + 1;
                    }
                    else
                    {
                        output.Append(input.Substring(start, 2));
                        pos = start + 2;
                    }
                }
                else if (pos != 0)
                {
                    output.Append(input.Substring(pos));
                    return output.ToString();
                }
                else
                {
                    return input;
                }
            }
            return output.ToString();
        }

        #region IStringMacroMap Members

        public IStringMacroResolver GetDynamicMapping(string name)
        {
            resolverTable.TryGetValue(name, out IStringMacroResolver stringMacroResolver);
            return stringMacroResolver;
        }

        public void AddStaticMapping(string name, string value, bool generalizable)
        {
            resolverTable.Add(name, new StaticMacroResolver(value, generalizable));
        }

        public void AddDynamicMapping(string name, StringMacroResolver stringMacroResolver, bool generalizable)
        {
            resolverTable.Add(name, new DelegateMacroResolver(stringMacroResolver, generalizable));
        }

        public void AddDynamicMapping(string name, IStringMacroResolver stringMacroResolver)
        {
            resolverTable.Add(name, stringMacroResolver);
        }

        public void RemoveMapping(string name)
        {
            resolverTable.Remove(name);
        }

        public bool EnableEnvironmentVars
        {
            get { return enableEnvironmentVars; }
            set { enableEnvironmentVars = value; }
        }

        public string Expand(string input, bool environmentVars)
        {
            string output = ExpandInternal(input);
            if (environmentVars)
                return Environment.ExpandEnvironmentVariables(output);
            return output;
        }

        public string Expand(string input)
        {
            string output = ExpandInternal(input);
            if (enableEnvironmentVars && output != null)
                return Environment.ExpandEnvironmentVariables(output);
            return output;
        }

        public string Generalize(string input)
        {
            // we only use static map (dynamic seems to be too volatile)
            foreach (KeyValuePair<string, IStringMacroResolver> mapping in resolverTable)
            {
                IStringMacroResolver resolver = mapping.Value;
                if (!resolver.IsGeneralizable)
                    continue;

                string value;
                if (resolver.Resolve(mapping.Key, null, out value))
                    input = input.Replace(value, "$(" + mapping.Key + ")");
            }
            return input;
        }

        public bool IsExpandable(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            int start = input.IndexOf("$(");
            if (start == -1)
                return false;
            start += 3;
            if (start >= input.Length)
                return false;
            int end = input.IndexOf(')', start);
            return end != -1;
        }

        #endregion
    }
}
