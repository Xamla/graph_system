using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamla.Types.Records
{
    public class MemorySchemaRegistry
        : ISchemaRegistry
    {
        Dictionary<int, Schema> schemaById = new Dictionary<int, Schema>();
        Dictionary<string, Schema> schemaByName = new Dictionary<string, Schema>();
        int nextSchemaId = 5000;

        public MemorySchemaRegistry()
        {
        }

        public void Add(Schema schema)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            lock (this)
            {
                if (schema.Id < 0)
                {
                    schema.Id = ++nextSchemaId;
                }
                else
                {
                    if (schema.Id < 1000)
                    {
                        throw new ArgumentException("ID of schema to be added must not be in reserved interval for built-in types [0, 999].", "schema");
                    }

                    if (schemaById.ContainsKey(schema.Id))
                    {
                        throw new ArgumentException("ID of schema to be added is already in use.", "schema");
                    }

                    if (nextSchemaId < schema.Id)
                        nextSchemaId = schema.Id + 1;
                }

                schemaById.Add(schema.Id, schema);
                schemaByName[schema.Name] = schema;
            }
        }

        public int Count
        {
            get { return schemaById.Count; }
        }

        public bool Remove(Schema schema)
        {
            if (schema == null)
                return false;

            lock (this)
            {
                Schema found;
                if (!schemaById.TryGetValue(schema.Id, out found) || !object.ReferenceEquals(found, schema))
                    return false;
                   
                // remvove from id-table
                schemaById.Remove(found.Id);

                // try to remove from name-table
                Schema found2;
                if (schemaByName.TryGetValue(found.Name, out found2) && object.ReferenceEquals(found, found2))
                    schemaByName.Remove(found2.Name);
            }

            return true;
        }

        public Schema GetSchemaById(int schemaId)
        {
            Schema result;
            if (!TryGetSchemaById(schemaId, out result))
                throw new SchemaNotFoundException(schemaId);
            return result; 
        }

        public Schema GetSchemaByName(string schemaName)
        {
            Schema result;
            if (!TryGetSchemaByName(schemaName, out result))
                throw new SchemaNotFoundException(schemaName);
            return result;
        }

        public IEnumerable<Schema> GetAll()
        {
            lock (this)
            {
                return schemaById.Values.ToList();	// return isolation copy
            }
        }

        public bool TryGetSchemaById(int schemaId, out Schema result)
        {
            if (schemaId >= 0 && schemaId <= BuiltInSchemas.HIGHEST_ID)
            {
                if (Schema.BuiltIn.TryGetSchemaById(schemaId, out result))
                    return true;
            }

            lock (this)
            {
                return schemaById.TryGetValue(schemaId, out result);
            }
        }

        public bool TryGetSchemaByName(string schemaName, out Schema result)
        {
            lock (this)
            {
                return schemaByName.TryGetValue(schemaName, out result) || Schema.BuiltIn.TryGetSchemaByName(schemaName, out result);
            }
        }

        public void Clear()
        {
            this.schemaById.Clear();
            this.schemaByName.Clear();
            this.nextSchemaId = 5000;
        }

        public bool Contains(Schema item)
        {
            if (item == null)
                return false;

            Schema found;
            return schemaById.TryGetValue(item.Id, out found) && object.ReferenceEquals(found, item);
        }

        public void CopyTo(Schema[] array, int arrayIndex)
        {
            this.schemaById.Values.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<Schema> GetEnumerator()
        {
            return this.schemaById.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
