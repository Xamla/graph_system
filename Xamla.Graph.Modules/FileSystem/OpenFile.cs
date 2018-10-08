using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.Xml;
using Xamla.Types;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.FileSystem
{
    [Module(ModuleType = "Xamla.IO.OpenFile")]
    public class OpenFile
        : ModuleBase
    {
        class FileReadable
            : IReadable
        {
            IFileSystem fs;
            string path;

            public FileReadable(IFileSystem fs, string path)
            {
                this.fs = fs;
                this.path = path;
            }

            public Stream Open()
            {
                return fs.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        readonly GenericInputPin path;
        readonly GenericOutputPin readable;

        public OpenFile(IGraphRuntime runtime)
            : base(runtime)
        {
            this.path = this.AddInputPin("path", PinDataTypeFactory.Create<string>(string.Empty, WellKnownEditors.FileSelector), PropertyMode.Default, flags: PinFlags.ResolvePath);
            this.readable = this.AddOutputPin("readable", PinDataTypeFactory.Create<IReadable>());
        }

        public IInputPin PathPin
        {
            get { return path; }
        }

        public string Path
        {
            get { return this.properties.Get<string>(path.Id); }
            set { this.Properties.Set(path.Id, value); }
        }

        public IOutputPin ReadablePin
        {
            get { return readable; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, System.Threading.CancellationToken cancel)
        {
            var path = this.Graph.ResolvePath((string)inputs[0]);
            var result = CreateReadable(path);

            return Task.FromResult(new object[] { result });
        }

        IReadable CreateReadable(string path)
        {
            return new FileReadable(this.Runtime.FileSystem, path);
        }
    }
}
