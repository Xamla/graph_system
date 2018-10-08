using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Models
{
    public class CommentModuleModel
        : NodeModel
    {
        public CommentModuleModel()
            : base("Comment")
        {
        }

        public string Comment { get; set; }
        public Int2 Position { get; set; }
        public Int2 Size { get; set; }
        public string Color { get; set; }
        public DisplayMode DisplayMode { get; set; }
        public NodeModel RefersTo { get; set; }
    }
}
