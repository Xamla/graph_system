using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Types.Records
{
    public static class DataBaseError
    {
        // DatabaseErrors                   14000  - 14999

    }

    public class ItemMapping
    {
        public ItemId SourceId { get; set; }
        public ItemId DestinationId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("SourceId: {0}; DestinationId: {1}; Name: '{2}';", SourceId, DestinationId, Name);
        }
    }

    public class LatestSnapshot
    {
        public ItemId FileId { get; set; }
        public ItemId SnapshotId { get; set; }
        public string SnapshotName { get; set; }

        public override string ToString()
        {
            return string.Format("FileId: {0}; SnapshotId: {1}; SnapshotName: '{2}'", FileId, SnapshotId, SnapshotName);
        }
    }

    public struct ItemName
    {
        ItemId id;
        string name;
        ItemFlags flags;

        public ItemId Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public ItemFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public ItemName(ItemId id, string name, ItemFlags flags)
        {
            this.id = id;
            this.name = name;
            this.flags = flags;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}; Name: '{1}'; Flags: {2};", Id, Name, Flags);
        }

        public override int GetHashCode()
        {
            return HashHelper.CombineHashCode(Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name ?? string.Empty) : -1, Id);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemName))
                return false;

            var other = (ItemName)obj;
            return id.Equals(other.id)
                && flags.Equals(other.flags)
                && StringComparer.OrdinalIgnoreCase.Equals(this.name, other.name);
        }
    }

    public struct ItemDataSize
    {
        public long Count { get; set; }
        public long DataSize { get; set; }

        public override string ToString()
        {
            return string.Format("Count: {0}; DataSize: {1} bytes;", Count, DataSize);
        }
    }

    public class ItemNotFoundException
        : XamlaException
    {
        public ItemNotFoundException(ItemId itemId)
            : base(string.Format("Item '{0}' not found.", itemId), HttpStatusCode.NotFound, XamlaError.ItemNotFound)
        {
            this.ItemId = itemId;
        }

        public ItemNotFoundException(ItemId parentId, string childName)
            : base(string.Format("Child with name '{0}' of parent item '{1}' not found.", childName, parentId), HttpStatusCode.NotFound, XamlaError.ItemNotFound)
        {
            this.ItemId = parentId;
            this.Name = Name;
        }

        public ItemId ItemId
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }
    }

    public interface IItemProvider
    {
        bool ReadOnly { get; }

        // sync
        T GetItem<T>(ItemId id) where T : Item;
        T GetChildByName<T>(ItemId parentId, string name) where T : Item;
        T TryGetItem<T>(ItemId id) where T : Item;
        T TryGetChildByName<T>(ItemId parentId, string name) where T : Item;

        //async
        Task<T> GetItemAsync<T>(ItemId id) where T : Item;
        Task<T> GetChildByNameAsync<T>(ItemId parentId, string name) where T : Item;
        Task<string> GetPathAsync(ItemId id, ItemId rootId = default(ItemId));
        Task<T> GetItemByPathAsync<T>(ItemId parentId, string path) where T : Item;

        Task<T> TryGetItemAsync<T>(ItemId id) where T : Item;
        Task<T> TryGetChildByNameAsync<T>(ItemId parentId, string name) where T : Item;
        Task<T> TryGetItemByPathAsync<T>(ItemId parentId, string path) where T : Item;

        Task<IList<T>> GetChildrenAsync<T>(ItemId parentId, int? count = null) where T : Item;
        Task<IList<T>> GetChildrenByNameLikeAsync<T>(ItemId parentId, string pattern, int? count = null) where T : Item;
        Task<IList<T>> GetChildrenAscendingAsync<T>(ItemId parentId, int count, int parentDistance = 1, ItemId after = default(ItemId)) where T : Item;
        Task<IList<T>> GetChildrenDescendingAsync<T>(ItemId parentId, int count, int parentDistance = 1, ItemId before = default(ItemId)) where T : Item;

        Task<IList<T>> GetDescendantsAsync<T>(ItemId parentId, int depth, int? count = null, bool flat = false) where T : Item;
        Task<IList<T>> GetDescendantsAscendingAsync<T>(ItemId parentId, int count, ItemId after = default(ItemId)) where T : Item;
        Task<IList<T>> GetDescendantsDescendingAsync<T>(ItemId parentId, int count, ItemId before = default(ItemId)) where T : Item;

        Task<IList<ItemLink>> LinkQueryAsync(ItemId fromItemId, ItemId toItemId, int? linkType = null);
        Task<IList<ItemLink>> LinkQueryFromAsync(ItemId fromItemId, int? linkType = null);
        Task<IList<ItemLink>> LinkQueryToAsync(ItemId toItemId, int? linkType = null);
        Task<IList<ItemLink>> LinkQueryAllToDescendantsAsync(ItemId targetsParentId, int? linkType = null, ItemId fromItemId = default(ItemId));
        Task<IList<ItemLink>> LinkQueryAllFromDescendantsAsync(ItemId sourcesParentId, int? linkType = null, ItemId toItemId = default(ItemId));
        Task<IList<ItemId>> LinkCheckManyAsync(IEnumerable<ItemId> fromIds, int? linkType = null);

        // TODO: Add Update, AppenChild, Clone, Exists etc. operations, 
    }
}
