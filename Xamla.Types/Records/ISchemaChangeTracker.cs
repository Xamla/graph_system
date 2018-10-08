using System;

namespace Xamla.Types.Records
{
    public enum SchemaNotificationType
    {
        Insert,
        Delete
    }

    public class SchemaNotification
    {
        public SchemaNotificationType Notification { get; set; }
        public int SchemaId { get; set; }
        public string SchemaName { get; set; }
    }

    public interface ISchemaChangeTracker
    {
        IObservable<SchemaNotification> WhenSchemaNotification { get; }

        void OnSchemaInserted(Schema schema);
        void OnSchemaDeleted(Schema schema);
    }
}
