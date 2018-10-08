namespace Xamla.Utilities.Strings
{
    public interface IStringMacroMap
    {
        /// <summary>
        /// Returns the IStringMacroResolver with the specified name.
        /// </summary>
        /// <param name="name">Name of the IStringMacroResolver to return.</param>
        /// <returns>IStringMacroResolver or null.</returns>
        IStringMacroResolver GetDynamicMapping(string name);

        /// <summary>
        /// Adds a static name to value mapping.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="generalizable"></param>
        void AddStaticMapping(string name, string value, bool generalizable);

        /// <summary>
        /// Adds an IStringMacroResolver for the given name with the given generalizable parameter.
        /// </summary>
        /// <param name="name">Name to resolve with the IStringMacroResolver.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="generalizable">The parameter for the resolver.</param>
        void AddDynamicMapping(string name, StringMacroResolver resolver, bool generalizable);

        /// <summary>
        /// Adds an IStringMacroResolver for the given name.
        /// </summary>
        /// <param name="name">Name to resolve with the IStringMacroResolver.</param>
        /// <param name="resolver">The resolver.</param>
        void AddDynamicMapping(string name, IStringMacroResolver resolver);

        /// <summary>
        /// Removes the mapping for the name.
        /// </summary>
        /// <param name="name">Name to remove from the mapping table.</param>
        void RemoveMapping(string name);

        /// <summary>
        /// Decides, if environment variables are resolved after internal replacements.
        /// </summary>
        bool EnableEnvironmentVars { get; set; }

        /// <summary>
        /// Expands the string. The decision too also expand
        /// environment variables depends on EnableEnvironmentVars.
        /// Environment variables are expanded after the macro expansion.
        /// </summary>
        /// <param name="input">The string to expand.</param>
        /// <returns>The expanded string.</returns>
        string Expand(string input);

        /// <summary>
        /// Expands the string and if environmentVars is true also
        /// the environment variables.
        /// Environment variables are expanded after the macro expansion.
        /// </summary>
        /// <param name="input">The string to expand.</param>
        /// <param name="environmentVars">True if the environment variables should be expanded.</param>
        /// <returns>The expanded string.</returns>
        string Expand(string input, bool environmentVars);

        /// <summary>
        /// Tries to replace all elements of the string, that are the destination
        /// of a mapping with their name. Only generalizable resolvers are used for this.
        /// <example>If you have got a resolver that resolves $(TestId) to 10 and a
        /// string "Test 10" than the result of this method is the string "Test $(TestId). </example>
        /// </summary>
        /// <param name="input">The string to generalize.</param>
        /// <returns>The generalized string.</returns>
        string Generalize(string input);

        /// <summary>
        /// Checks if the string contains at least one part of the form
        /// $(*). It does not check, if this variable is actually resolvable!
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <returns>True if the string contains a variable, no matter if resolvable or not.</returns>
        bool IsExpandable(string input);
    }
}
