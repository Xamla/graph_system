using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.IO.StringToReadable")]
    public class StringToReadable
        : ModuleBase
    {
        GenericInputPin stringPin;
        GenericOutputPin readablePin;

        public StringToReadable(IGraphRuntime runtime)
            : base(runtime)
        {
            this.stringPin = AddInputPin("String", PinDataTypeFactory.Create<string>(), PropertyMode.Allow);
            this.readablePin = AddOutputPin("Readable", PinDataTypeFactory.Create<IReadable>());
        }

        public IInputPin StringPin
        {
            get { return stringPin; }
        }

        public IOutputPin ReadablePin
        {
            get { return readablePin; }
        }

        private static IReadable ToReadable(string text)
        {
            return Readable.Create(() => new MemoryStream(Encoding.UTF8.GetBytes(text)));
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var text = (string)inputs[0];

            var result = ToReadable(text);

            return Task.FromResult(new object[] { result });
        }
    }
}
