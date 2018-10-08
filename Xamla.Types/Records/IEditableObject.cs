namespace Xamla.Types.Records
{
    public interface IEditableObject
        : IEditable
    {
        IEditable GetField(string name);
        bool TryGetField(string name, out IEditable result);
        IEditable GetField(int index);
    }
}
