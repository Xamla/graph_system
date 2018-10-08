using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph
{
    public interface IDependencyDownloader
    {
        //void Download(SqlHierarchyId container);
        IContainerDependency Download(Uri source);
    }
}
