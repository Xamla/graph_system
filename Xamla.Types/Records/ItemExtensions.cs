using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public static class ItemExtensions
    {
        public static Task<string> GetPathAsync(this Item item, ItemId rootId = default(ItemId))
        {
            return item.Provider.GetPathAsync(item.Id, rootId);
        }

        public static Task<T> TryGetParentAsync<T>(this Item item) where T : Item
        {
            return item.Provider.TryGetItemAsync<T>(item.Id.GetAncestor(1));
        }

        public static async Task<T> GetParentAsync<T>(this Item item) where T : Item
        {
            var parent = await TryGetParentAsync<T>(item).ConfigureAwait(false);
            if (parent == null)
                throw new Exception(string.Format("Parent of item '{0}' was not found.", item.Id));
            return parent;
        }

        public static Task<T> TryGetChildByNameAsync<T>(this Item item, string name) where T : Item
        {
            return item.Provider.TryGetChildByNameAsync<T>(item.Id, name);
        }

        public static Task<T> GetChildByNameAsync<T>(this Item item, string name) where T : Item
        {
            return item.Provider.GetChildByNameAsync<T>(item.Id, name);
        }

        public static Task<IList<T>> GetSiblingsBeforeAsync<T>(this Item item, int count) where T : Item
        {
            return item.Provider.GetChildrenDescendingAsync<T>(item.Id.GetAncestor(1), count, 1, item.Id);
        }

        public static Task<IList<T>> GetSiblingsAfterAsync<T>(this Item item, int count) where T : Item
        {
            return item.Provider.GetChildrenAscendingAsync<T>(item.Id.GetAncestor(1), count, 1, item.Id);
        }

        public static Task<IList<T>> GetChildrenAsync<T>(this Item item, int? count = null) where T : Item
        {
            return item.Provider.GetChildrenAsync<T>(item.Id, count);
        }

        public static Task<IList<T>> GetChildrenAscendingAsync<T>(this Item item, int count, int parentDistance = 1, ItemId after = default(ItemId)) where T : Item
        {
            return item.Provider.GetChildrenAscendingAsync<T>(item.Id, count, parentDistance, after);
        }

        public static Task<IList<T>> GetChildrenDescendingAsync<T>(this Item item, int count, int parentDistance = 1, ItemId before = default(ItemId)) where T : Item
        {
            return item.Provider.GetChildrenDescendingAsync<T>(item.Id, count, parentDistance, before);
        }

        public static Task<IList<T>> GetDescendantsAsync<T>(this Item item, int depth, int? count = null, bool flat = false) where T : Item
        {
            return item.Provider.GetDescendantsAsync<T>(item.Id, depth, count, flat);
        }

        public static Task<IList<T>> GetDescendantsAscendingAsync<T>(this Item item, int count, ItemId after = default(ItemId)) where T : Item
        {
            return item.Provider.GetDescendantsAscendingAsync<T>(item.Id, count, after);
        }

        public static Task<IList<T>> GetDescendantsDescendingAsync<T>(this Item item, int count, ItemId before = default(ItemId)) where T : Item
        {
            return item.Provider.GetDescendantsDescendingAsync<T>(item.Id, count, before);
        }
    }
}
