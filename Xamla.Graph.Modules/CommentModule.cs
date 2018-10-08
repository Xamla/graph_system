using System;
using System.Threading.Tasks;
using Xamla.Graph.Models;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Graph.Comment")]
    public class CommentModule
        : ModuleBase
        , ICommentModule
    {
        string comment;
        Int2 size;
        string color;
        INode refersTo;

        public CommentModule(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Comment, DisplayMode.Expanded)
        {
        }

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                if (value == comment)
                    return;

                comment = value;
                MarkModified();
            }
        }

        public Int2 Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value == size)
                    return;

                size = value;
                MarkModified();
            }
        }

        public string Color
        {
            get
            {
                return color;
            }
            set
            {
                ValidateHexColor(value);

                if (value == color)
                    return;

                color = value;
                MarkModified();
            }
        }

        private static void ValidateHexColor(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (!value.StartsWith("#"))
                throw new Exception("Hexcolor has to start with #.");

            value = value.Substring(1);

            if (value.Length != 6)
                throw new ArgumentOutOfRangeException("Hexcolor needs exactly 6 characters.");

            Convert.ToInt32(value, 16);
        }

        public INode RefersTo
        {
            get
            {
                return refersTo;
            }
            set
            {
                if (value == refersTo)
                    return;

                refersTo = value;
                MarkModified();
            }
        }

        public override NodeModel ToModel(int? depthLimit)
        {
            if (depthLimit.HasValue && depthLimit < 0)
                return null;

            return new CommentModuleModel
            {
                Id = this.Id,
                Trace = this.Trace,
                ObjectId = this.ObjectId,
                Comment = this.Comment,
                Position = this.Position,
                Color = this.Color,
                Size = this.Size,
                DisplayMode = this.Display,
                RefersTo = this.RefersTo == null ? null : new NodeModel("Reference") { Id = this.RefersTo.Id, Trace = this.RefersTo.Trace, ObjectId = this.RefersTo.ObjectId }
            };
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            return null;
        }
    }
}
