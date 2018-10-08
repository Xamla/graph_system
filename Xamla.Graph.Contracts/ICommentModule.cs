using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph
{
    public interface ICommentModule
        : IModule
    {
        string Comment { get; set; }
        Int2 Size { get; set; }
        string Color { get; set; }
        INode RefersTo { get; set; }
    }
}
