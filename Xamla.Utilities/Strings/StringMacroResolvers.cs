namespace Xamla.Utilities.Strings
{
    /// <summary>
    /// Generic class for single function-based string replacement
    /// </summary>
    internal class DelegateMacroResolver : IStringMacroResolver
    {
        private readonly StringMacroResolver resolver;
        private readonly bool generalizable;

        public DelegateMacroResolver(StringMacroResolver resolver, bool generalizable)
        {
            this.resolver = resolver;
            this.generalizable = generalizable;
        }

        #region IStringMacroResolver Members

        public bool IsGeneralizable
        {
            get { return generalizable; }
        }

        /// <summary>
        /// Resolve string by call of delegate function resolver(<paramref name="name"/>, <paramref name="parameter"/>, out <paramref name="value"/>)
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="value">The result</param>
        /// <returns></returns>
        public bool Resolve(string name, string parameter, out string value)
        {
            return resolver(name, parameter, out value);
        }
        #endregion
    }

    internal class StaticMacroResolver : IStringMacroResolver
    {
        private readonly string value;
        private readonly bool generalizable;

        public StaticMacroResolver(string value, bool generalizable)
        {
            this.value = value;
            this.generalizable = generalizable;
        }

        #region IStringMacroResolver Members

        public bool IsGeneralizable
        {
            get { return generalizable; }
        }

        public bool Resolve(string name, string parameter, out string value)
        {
            value = this.value;
            return true;
        }

        #endregion
    }
}
