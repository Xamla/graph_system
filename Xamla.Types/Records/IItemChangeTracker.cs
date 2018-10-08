using System;
using System.Collections.Generic;
using System.Text;

namespace Xamla.Types.Records
{
    public enum ItemNotificationType
    {
        Insert,
        Update,
        Delete
    }

    public struct ItemRevision
    {
        public ItemId Id;
        public int? Revision;

        public ItemRevision(ItemId id, int? revision)
        {
            Id = id;
            Revision = revision;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Id, Revision.HasValue ? Revision.ToString() : "*");
        }
    }

    public class ItemNotification
    {
        public ItemNotificationType Notification { get; set; }
        public IList<ItemRevision> Revisions { get; set; }

        public override string ToString()
        {
            return string.Format("{0} of {1}", this.Notification, GetItemsDebugString());
        }

        string GetItemsDebugString()
        {
            if (this.Revisions == null)
                return "<null>";

            var sb = new StringBuilder();
            for (int i = 0; i < this.Revisions.Count && i < 100; ++i)
            {
                var item = this.Revisions[i];
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(item.ToString());
            }

            if (this.Revisions.Count > 100)
                sb.AppendFormat("and {0} more...", this.Revisions.Count - 100);

            return sb.ToString();
        }
    }

    public interface IItemChangeTracker
    {
        IObservable<ItemNotification> WhenItemNotification { get; }

        void OnItemsInserted(IEnumerable<ItemRevision> ids);
        void OnItemsUpdated(IEnumerable<ItemRevision> ids);
        void OnItemsDeleted(IEnumerable<ItemRevision> ids);
    }

    public static class ItemChangeTrackerExtensions
    {
        public static void OnItemsInserted(this IItemChangeTracker tracker, params ItemRevision[] ids)
        {
            tracker.OnItemsInserted(ids);
        }

        public static void OnItemsUpdated(this IItemChangeTracker tracker, params ItemRevision[] ids)
        {
            tracker.OnItemsUpdated(ids);
        }

        public static void OnItemsDeleted(this IItemChangeTracker tracker, params ItemRevision[] ids)
        {
            tracker.OnItemsDeleted(ids);
        }
    }
}
