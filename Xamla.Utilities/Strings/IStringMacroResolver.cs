namespace Xamla.Utilities.Strings
{
    public interface IStringMacroResolver
    {
        bool IsGeneralizable { get; }
        bool Resolve(string name, string parameter, out string value);
    }

    public delegate bool StringMacroResolver(string name, string parameter, out string value);
}
