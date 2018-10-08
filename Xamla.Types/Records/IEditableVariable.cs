using System;

namespace Xamla.Types.Records
{
    public interface IEditableVariable
        : IEditable
    {
        Schema DataSchema { get; set; }
        Cursor DataCursor { get; set; }
    }
}
