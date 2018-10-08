using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public interface ISchemaCache
        : ISchemaProvider
    {
        void AddProvider(ISchemaProvider provider);
        void RemoveProvider(ISchemaProvider provider);
        void Reload();
        void Flush();
    }
}
