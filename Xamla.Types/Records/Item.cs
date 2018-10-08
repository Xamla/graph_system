using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamla.Types.Records
{
    public class ItemFile
    {
        public ItemFile()
        {
        }

        public ItemFile(string uri)
        {
            this.Uri = uri;
        }

        public ItemFile(string uri, Guid id, long size)
        {
            this.Uri = uri;
            this.Id = id;
            this.Size = size;
        }

        public Guid Id { get; set; }
        public string Uri { get; set; }
        public long Size { get; set; }

        public override string ToString()
        {
            return string.Format("Uri: '{0}', Size: {1}, Id: {2}", this.Uri, this.Size, this.Id);
        }
    }

    public class ItemLink
    {
        public ItemId FromId { get; set; }
        public ItemId ToId { get; set; }
        public int LinkType { get; set; }
        public double Weight { get; set; }
    }

    public class ItemStatistics
    {
        public int Children { get; set; }
        public int Subtree { get; set; }
        public long StorageSize { get; set; }
        public long SubtreeStorageSize { get; set; }

        public override string ToString()
        {
            return string.Format("Children: {0}, Subtree: {1}, StorageSize: {2}, SubtreeStorageSize: {3}", this.Children, this.Subtree, this.StorageSize, this.SubtreeStorageSize);
        }
    }

    public class Item
    {
        public ItemId Id { get; set; }
        public ItemId? OriginalItemId { get; set; }

        private ItemFlags flags = ItemFlags.CountAsChild | ItemFlags.Propagate | ItemFlags.AutoStorageSize | ItemFlags.Cachable;
        public ItemFlags Flags
        {
            get { return flags; }
            set { this.flags = value; }
        }

        public int Revision { get; set; }
        public string Name { get; set; }
        public IList<ItemFile> Files { get; set; }
        public IList<ItemLink> Links { get; set; }
        public ItemStatistics Statistics { get; set; }
        public IList<Item> Children { get; set; }

        public static Item TryFindChild(Item item, ItemId id)
        {
            if (item.Children != null)
            {
                foreach (var x in item.Children)
                {
                    if (x.Id.Equals(id))
                        return x;

                    var found = TryFindChild(x, id);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public Item TryFindChild(ItemId id)
        {
            return TryFindChild(this, id);
        }

        public Item TryFindChildRelative(params int[] descendantIds)
        {
            return TryFindChild(this, this.Id.Append(descendantIds));
        }

        public Item TryFindChildRelative(ItemId id)
        {
            return TryFindChild(this, this.Id.Append(id));
        }

        public override string ToString()
        {
            return string.Format("Item '{0}'", this.Id);
        }

        private IItemProvider provider;

        [NonRecordField]
        public IItemProvider Provider
        {
            get { return provider; }
            set
            {
                provider = value;
                if (this.Children != null)
                {
                    foreach (var child in this.Children)
                        child.Provider = value;
                }
            }
        }

        public void ModifyFlags(ItemFlags add, ItemFlags remove)
        {
            this.Flags |= add;
            this.Flags &= ~remove;
        }

        public void SetFlags(ItemFlags flags, bool value = true)
        {
            this.Flags = value ? this.Flags | flags : this.Flags & ~flags;
        }

        public bool HasFlags(ItemFlags flags)
        {
            return (this.Flags & flags) == flags;
        }

        public bool HasFlagsAny(ItemFlags flags)
        {
            return (this.Flags & flags) != 0;
        }

        [NonRecordField]
        public ItemId OriginalId
        {
            get { return this.OriginalItemId ?? this.Id; }
        }

        [NonRecordField]
        public bool Cachable
        {
            get { return HasFlags(ItemFlags.Cachable); }
            set { SetFlags(ItemFlags.Cachable, value); }
        }

        [NonRecordField]
        public bool IsContainer
        {
            get { return HasFlags(ItemFlags.IsContainer); }
            set { SetFlags(ItemFlags.IsContainer, value); }
        }

        [NonRecordField]
        public bool CountAsChild
        {
            get { return HasFlags(ItemFlags.CountAsChild); }
            set { SetFlags(ItemFlags.CountAsChild, value); }
        }

        [NonRecordField]
        public bool Propagate
        {
            get { return HasFlags(ItemFlags.Propagate); }
            set { SetFlags(ItemFlags.Propagate, value); }
        }

        [NonRecordField]
        public bool Hidden
        {
            get { return HasFlags(ItemFlags.Hidden); }
            set { SetFlags(ItemFlags.Hidden, value); }
        }

        [NonRecordField]
        public bool Modified
        {
            get { return HasFlags(ItemFlags.Modified); }
            set { SetFlags(ItemFlags.Modified, value); }
        }
    }

    public class Item<T>
        : Item
    {
        public T Data { get; set; }

        public Item<T2> Cast<T2>()
        {
            return new Item<T2>
            {
                Id = this.Id,
                OriginalItemId = this.OriginalItemId,
                Flags = this.Flags,
                Revision = this.Revision,
                Name = this.Name,
                Files = this.Files,
                Links = this.Links,
                Statistics = this.Statistics,
                Children = this.Children,
                Data = (T2)(object)this.Data
            };
        }
    }

    [Record(Version = 1, SchemaBaseName = "Xamla.Folder")]
    public class Folder
        : Item
    {
        public Folder()
        {
            this.Flags &= ~ItemFlags.CountAsChild;
            this.Flags |= ItemFlags.IsContainer;
        }

        public string Type { get; set; }

        [RecordField(Nullable = true, MaxLength = 32 * 1024)]
        public string Description { get; set; }
    }
}
