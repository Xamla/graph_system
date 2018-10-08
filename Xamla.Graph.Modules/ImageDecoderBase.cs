using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    public abstract class ImageDecoderBase
        : ModuleBase
    {
        GenericInputPin readablePin;
        GenericOutputPin imagePin;

        public ImageDecoderBase(IGraphRuntime runtime)
            : base(runtime)
        {
            this.readablePin = this.AddInputPin("Readable", PinDataTypeFactory.Create<IReadable>(), PropertyMode.Never);
            this.imagePin = this.AddOutputPin("Image", PinDataTypeFactory.Create<IImageBuffer>());
        }

        public IInputPin Readablepin
        {
            get { return readablePin; }
        }

        public IOutputPin ImagePin
        {
            get { return imagePin; }
        }

        protected abstract IImageBuffer Decode(IReadable input);

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var readable = (IReadable)inputs[0];

            var result = Decode(readable);

            return Task.FromResult(new object[] { result });
        }
    }
}
