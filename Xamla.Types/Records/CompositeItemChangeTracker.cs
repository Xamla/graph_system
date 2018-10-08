using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Xamla.Types.Records
{
    public class CompositeItemChangeTracker
        : IItemChangeTracker
    {
        List<Tuple<IItemChangeTracker, IDisposable>> trackers = new List<Tuple<IItemChangeTracker, IDisposable>>();
        Subject<ItemNotification> whenRecordNotification = new Subject<ItemNotification>();

        public void Add(IItemChangeTracker tracker)
        {
            var subscription = tracker.WhenItemNotification.Subscribe(whenRecordNotification);
            trackers.Add(Tuple.Create(tracker, subscription));
        }

        public void Remove(IItemChangeTracker tracker)
        {
            int i = trackers.FindIndex(x => x.Item1 == tracker);
            if (i >= 0)
            {
                var t = trackers[i];
                trackers.RemoveAt(i);
                t.Item2.Dispose();
            }
        }

        public IObservable<ItemNotification> WhenItemNotification
        {
            get { return whenRecordNotification; }
        }

        public void OnItemsInserted(IEnumerable<ItemRevision> ids)
        {
            foreach (var t in trackers)
                t.Item1.OnItemsInserted(ids);
        }

        public void OnItemsUpdated(IEnumerable<ItemRevision> ids)
        {
            foreach (var t in trackers)
                t.Item1.OnItemsUpdated(ids);
        }

        public void OnItemsDeleted(IEnumerable<ItemRevision> ids)
        {
            foreach (var t in trackers)
                t.Item1.OnItemsDeleted(ids);
        }
    }
}
