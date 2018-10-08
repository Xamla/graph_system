using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Xamla.Types.Records
{
    public class CompositeSchemaChangeTracker
        : ISchemaChangeTracker
    {
        List<Tuple<ISchemaChangeTracker, IDisposable>> trackers = new List<Tuple<ISchemaChangeTracker, IDisposable>>();
        Subject<SchemaNotification> whenSchemaNotification = new Subject<SchemaNotification>();

        public void Add(ISchemaChangeTracker tracker)
        {
            var subscription = tracker.WhenSchemaNotification.Subscribe(whenSchemaNotification);
            trackers.Add(Tuple.Create(tracker, subscription));
        }

        public void Remove(ISchemaChangeTracker tracker)
        {
            int i = trackers.FindIndex(x => x.Item1 == tracker);
            if (i >= 0)
            {
                var t = trackers[i];
                trackers.RemoveAt(i);
                t.Item2.Dispose();
            }
        }

        public IObservable<SchemaNotification> WhenSchemaNotification
        {
            get { return whenSchemaNotification; }
        }

        public void OnSchemaInserted(Schema schema)
        {
            foreach (var t in trackers)
                t.Item1.OnSchemaInserted(schema);
        }

        public void OnSchemaDeleted(Schema schema)
        {
            foreach (var t in trackers)
                t.Item1.OnSchemaDeleted(schema);
        }
    }
}
