using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamla.Graph
{
    public class ModuleException
        : Exception
    {
        public ModuleException(string moduleTrace, string moduleObjectId, Exception innerException)
            : base(string.Format("Module '{0}': {1}", moduleTrace, innerException.Message), innerException)
        {
            this.ModuleTrace = moduleTrace;
            this.ModuleObjectId = moduleObjectId;
            this.InnerExceptions = new ReadOnlyCollection<Exception>(new Exception[] { innerException });
        }

        public ModuleException(string moduleTrace, string moduleObjectId, params Exception[] innerExceptions)
            : this(moduleTrace, moduleObjectId, (IList<Exception>)innerExceptions)
        {
        }

        public ModuleException(string moduleTrace, string moduleObjectId, IEnumerable<Exception> innerExceptions)
            : this(moduleTrace, moduleObjectId, innerExceptions as IList<Exception> ?? new List<Exception>(innerExceptions))
        {
        }

        public ModuleException(string moduleTrace, string moduleObjectId, IList<Exception> innerExceptions)
            : base(string.Format("Module '{0}': {1} exceptions", moduleTrace, innerExceptions.Count, innerExceptions.Count > 0 ? innerExceptions[0] : null))
        {
            this.ModuleTrace = moduleTrace;
            this.ModuleObjectId = moduleObjectId;
            this.InnerExceptions = new ReadOnlyCollection<Exception>(innerExceptions);
        }

        public string ModuleObjectId
        {
            get;
            private set;
        }

        public string ModuleTrace
        {
            get;
            private set;
        }

        public IReadOnlyCollection<Exception> InnerExceptions
        {
            get;
            private set;
        }
    }
}
