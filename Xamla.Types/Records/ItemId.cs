using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xamla.Types.Records
{
    public interface INullable
    {
        bool IsNull { get; }
    }

    public class ItemIdException
        : Exception
    {
        public ItemIdException(string message)
            : base(message)
        {
        }
    }

    public struct ItemId
        : INullable
    {
        public static ItemId Null
        {
            get { throw new NotImplementedException(); }
        }

        public static ItemId GetRoot()
        {
            throw new NotImplementedException();
        }

        public static ItemId Parse(string text)
        {
            throw new NotImplementedException();
        }

        public bool IsNull
        {
            get { throw new NotImplementedException(); }
        }

        public ItemId GetAncestor(int n)
        {
            throw new NotImplementedException();
        }

        public ItemId GetDescendant(ItemId child1, ItemId child2)
        {
            throw new NotImplementedException();
        }

        public int GetLevel()
        {
            throw new NotImplementedException();
        }

        public bool IsDescendantOf(ItemId parent)
        {
            throw new NotImplementedException();
        }

        public ItemId GetReparentedValue(ItemId oldRoot, ItemId newRoot)
        {
            throw new NotImplementedException();
        }

        public void Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
