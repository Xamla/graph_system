using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public interface IEditable
        : ICursor
    {
        IEditableFactory Factory { get; }

        void Set(object value);

        new bool IsNull { get; set; }
        int SerializedSize { get; }
        void WriteTo(BinaryWriter writer);

        void Freeze();
        bool Frozen { get; }

        IEditable CloneAsEditable(bool frozen = false);
    }

    public interface IEditableConvertible
    {
        IEditable ToEditable(IEditableFactory factory);
    }

    public class FrozenObjectReadonlyException
        : InvalidOperationException
    {
        public FrozenObjectReadonlyException()
            : base("Editable is frozen and cannot be modified.")
        {
        }
    }

    public class OperationRequiresFrozenObjectException
        : InvalidOperationException
    {
        public OperationRequiresFrozenObjectException()
            : base("The operation can only be performed on objects in frozen state.")
        {
        }
    }

    public static class EditableExtensions
    {
        public static byte[] ToByteArray(this IEditable source, bool freeze = true)
        {
            if (!source.Frozen)
            {
                if (freeze)
                    source.Freeze();
                else
                    source = source.CloneAsEditable(true);
            }

            var buffer = new byte[source.SerializedSize];
            source.WriteTo(new MemoryStream(buffer));
            return buffer;
        }

        public static IEditableVariable ToVariable(this IEditable source, bool nullable = true)
        {
            return source.Factory.CreateVariable(source, nullable);
        }

        public static dynamic ToDynamic(this IEditable source)
        {
            return new DynamicEditable(source);
        }

        public static IEditable GetField(this IEditableObject source, ItemLayout itemField)
        {
            return source.GetField((int)itemField);
        }

        public static Dictionary<string, ICursor> BackupValues(this ICursor source, params string[] fieldPaths)
        {
            var saved = new Dictionary<string, ICursor>();

            foreach (var path in fieldPaths)
            {
                var value = source.NavigateTo(path);
                saved.Add(path, value.CloneAsCursor());
            }

            return saved;
        }

        public static void RestoreValues(this IEditable destination, Dictionary<string, IEditable> values)
        {
            foreach (var entry in values)
            {
                var value = (IEditable)destination.NavigateTo(entry.Key);
                value.Set(entry.Value);
            }
        }

        public static Cursor ToCursor(this IEditable source, bool freeze = true)
        {
            if (!source.Frozen)
            {
                if (freeze)
                    source.Freeze();
                else
                    source = source.CloneAsEditable(true);
            }

            return new Cursor(source.SchemaProvider, source.Schema, new ArraySegment<byte>(source.ToByteArray()));
        }
    }
}
