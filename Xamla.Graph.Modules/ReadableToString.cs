using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.IO.ReadableToString")]
    public class ReadableToString
        : ModuleBase
    {
        GenericInputPin readablePin;
        GenericOutputPin stringPin;

        public ReadableToString(IGraphRuntime runtime)
            : base(runtime)
        {
            this.readablePin = AddInputPin("Readable", PinDataTypeFactory.Create<IReadable>(), PropertyMode.Allow);
            this.stringPin = AddOutputPin("String", PinDataTypeFactory.Create<string>());
        }

        public IOutputPin StringPin
        {
            get { return stringPin; }
        }

        public IInputPin ReadablePin
        {
            get { return readablePin; }
        }

        private static string ToString(IReadable readable)
        {
            using (var sr = new StreamReader(readable.Open()))
            {
                return sr.ReadToEnd();
            }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var readable = (IReadable)inputs[0];

            var result = ToString(readable);

            return Task.FromResult(new object[] { result });
        }
    }
}
