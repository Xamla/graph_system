using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public class SchemaCache
        : ISchemaCache
        , IDisposable
    {
        Dictionary<int, Schema> schemaMap;
        Dictionary<string, Schema> schemaByName;

        List<ISchemaProvider> schemaProviders;
        IDisposable whenSchemaChangedSubscription;

        private ILogger Logger { get; } = Logging.CreateLogger<SchemaCache>();

        public SchemaCache(IObservable<SchemaNotification> whenSchemaChanged)
        {
            this.schemaProviders = new List<ISchemaProvider>();
            this.schemaMap = new Dictionary<int, Schema>();
            this.schemaByName = new Dictionary<string, Schema>();

            if (whenSchemaChanged != null)
                whenSchemaChangedSubscription = whenSchemaChanged.Subscribe(OnSchemaChanged);
            this.Flush();
        }

        void OnSchemaChanged(SchemaNotification notification)
        {
            switch (notification.Notification)
            {
                case SchemaNotificationType.Insert:
                    {
                        lock (this)
                        {
                            if (schemaMap.ContainsKey(notification.SchemaId))
                                return;
                        }

                        try
                        {
                            var schema = schemaProviders.Select(x => x.GetSchemaById(notification.SchemaId)).FirstOrDefault(x => x != null);
                            if (schema != null)
                                AddSchema(schema);
                        }
                        catch (Exception e)
                        {
                            this.Logger.LogWarning(e, "Fetching inserted schema (id: {0}, name: '{1}') failed: '{2}'", notification.SchemaId, notification.SchemaName, e.Message);
                        }
                    }
                    break;

                case SchemaNotificationType.Delete:
                    {
                        lock (this)
                        {
                            if (!schemaMap.ContainsKey(notification.SchemaId))
                                return;
                        }

                        RemoveSchema(notification.SchemaId, notification.SchemaName);
                    }
                    break;
            }
        }

        public bool TryGetSchemaByName(string name, out Schema schema)
        {
            if (!schemaByName.TryGetValue(name, out schema) && !Schema.BuiltIn.TryGetSchemaByName(name, out schema))
            {
                // ensure that schema is really missing by slow synchronous provider fetch
                foreach (var provider in schemaProviders)
                {
                    if (provider.TryGetSchemaByName(name, out schema))
                    {
                        AddSchema(schema);
                        break;
                    }
                }
            }

            if (schema == null)
                return false;

            return true;
        }

        public bool TryGetSchemaById(int schemaId, out Schema schema)
        {
            if (!schemaMap.TryGetValue(schemaId, out schema) && !Schema.BuiltIn.TryGetSchemaById(schemaId, out schema))
            {
                // ensure that schema is really missing by slow synchronous provider fetch
                foreach (var provider in schemaProviders)
                {
                    if (provider.TryGetSchemaById(schemaId, out schema))
                    {
                        AddSchema(schema);
                        break;
                    }
                }
            }

            if (schema == null)
                return false;
           
            return true;
        }

        public Schema GetSchemaById(int schemaId)
        {
            Schema schema;
            if (!TryGetSchemaById(schemaId, out schema))
                throw new SchemaNotFoundException(schemaId);

            return schema;
        }

        public Schema GetSchemaByName(string schemaName)
        {
            Schema schema;
            if (!TryGetSchemaByName(schemaName, out schema))
                throw new SchemaNotFoundException(schemaName);

            return schema;
        }

        void AddSchema(Schema schema)
        {
            lock (this)
            {
                schemaByName[schema.Name] = schema;
                schemaMap[schema.Id] = schema;
            }
        }

        void RemoveSchema(int id, string name)
        {
            lock (this)
            {
                schemaByName.Remove(name);
                schemaMap.Remove(id);
            }
        }

        void RemoveSchema(Schema schema)
        {
            RemoveSchema(schema.Id, schema.Name);
        }

        void FetchAll(ISchemaProvider provider)
        {
            lock (this)
            {
                foreach (var s in provider.GetAll())
                    this.AddSchema(s);
            }
        }

        public IEnumerable<Schema> GetAll()
        {
            lock (this)
            {
                return this.schemaProviders.SelectMany(x => x.GetAll()).Distinct().ToArray();
            }
        }

        public void AddProvider(ISchemaProvider provider)
        {
            lock (this)
            {
                schemaProviders.Add(provider);

                try
                {
                    FetchAll(provider);
                }
                catch (Exception)
                {
                }
            }
        }

        public void RemoveProvider(ISchemaProvider provider)
        {
            lock (this)
            {
                schemaProviders.Remove(provider);
                Reload();
            }
        }

        public void Reload()
        {
            lock (this)
            {
                // The this-lock is held while fetching results. 
                // Incoming add and remove notifications on other threads
                // will be blocked until the fetch operation completes.

                this.Flush();
                foreach (var p in this.schemaProviders)
                    FetchAll(p);
            }
        }

        public void Flush()
        {
            lock (this)
            {
                schemaMap.Clear();
                schemaByName.Clear();
            }
        }

        public void Dispose()
        {
            if (whenSchemaChangedSubscription != null)
            {
                whenSchemaChangedSubscription.Dispose();
                whenSchemaChangedSubscription = null;
            }
        }
    }
}
