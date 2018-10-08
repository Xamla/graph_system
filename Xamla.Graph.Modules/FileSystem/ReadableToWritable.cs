using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Modules.FileSystem
{
    [Module(ModuleType = "Xamla.IO.ReadableToWritable")]
    public class ReadableToWritable
        : ModuleBase
    {
        GenericInputPin readablePin;
        GenericOutputPin writablePin;

        public ReadableToWritable(IGraphRuntime runtime)
            : base(runtime)
        {
            this.readablePin = this.AddInputPin("Readable", PinDataTypeFactory.Create<IReadable>(), PropertyMode.Never);
            this.writablePin = this.AddOutputPin("Writable", PinDataTypeFactory.Create<IWritable>());
        }

        public IInputPin ReadablePin
        {
            get { return readablePin; }
        }

        public IOutputPin WritablePin
        {
            get { return writablePin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var readable = (IReadable)inputs[0];
            IWritable result = new ReadableWritableAdapter(readable);

            return Task.FromResult(new object[] { result });
        }
    }
}
