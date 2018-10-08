using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public class SchemaNotFoundException
        : XamlaException
    {
        public SchemaNotFoundException(int schemaId)
            : base(string.Format("Schema not found: {0}", schemaId), HttpStatusCode.NotFound, XamlaError.SchemaNotFoundById)
        {
        }

        public SchemaNotFoundException(string schemaName)
            : base(string.Format("Schema not found: {0}", schemaName), HttpStatusCode.NotFound, XamlaError.SchemaNotFoundByName)
        {
        }
    }

    public interface ISchemaProvider
    {
        /// <summary>Loads a single item schema by ID.</summary>
        /// <param name="schemaId">ID of schema to load</param>
        /// <returns>Schema found</returns>
        /// <exception cref="SchemaNotFoundException">Thrown if schema is not found.</exception>
        Schema GetSchemaById(int schemaId);

        /// <summary>Loads a single item schema by name.</summary>
        /// <param name="schemaId">Name of schema to load</param>
        /// <returns>Schema found</returns>
        /// <exception cref="SchemaNotFoundException">Thrown if schema is not found.</exception>
        Schema GetSchemaByName(string schemaName);

        bool TryGetSchemaById(int schemaId, out Schema result);
        bool TryGetSchemaByName(string schemaName, out Schema result);

        IEnumerable<Schema> GetAll();
    }

    public interface ISchemaRepository
        : ICollection<Schema>
    {
    }

    public interface ISchemaRegistry
        : ISchemaRepository
        , ISchemaProvider
    {
    }
}
