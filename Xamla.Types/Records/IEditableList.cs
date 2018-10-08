using System.Collections.Generic;

namespace Xamla.Types.Records
{
    public interface IEditableList
        : IEditable
        , IEnumerable<IEditable>
    {
        Schema ItemSchema { get; }

        IEditable Add();
        IEditable GetItem(int index);

        IEditable InsertAt(int index);
        void RemoveAt(int index);
        void Clear();

        int Capacity { get; set; }
    }
}
