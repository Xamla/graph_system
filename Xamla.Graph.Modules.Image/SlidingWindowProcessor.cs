using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    [Module(ModuleType = "Xamla.Image.SlidingWindowProcessor")]
    public class SlidingWindowProcessor
        : SingleInstanceMethodModule
    {
        CropImage cropProcessor;

        public SlidingWindowProcessor(IGraphRuntime runtime)
            : base(runtime)
        {
            this.cropProcessor = new CropImage(runtime);
        }

        [ModuleMethod]
        public IImageBuffer[] SlidingWindow(IImageBuffer image, Int2 winSize, Int2 winStride)
        {
            var numCols = (image.Width - (winSize.X - winStride.X)) / winStride.X;
            var numRows = (image.Height - (winSize.Y - winStride.Y)) / winStride.Y;
            var windows = new IImageBuffer[numCols * numRows];

            var i = 0;
            for (int row = 0; row <= image.Height - winSize.Y; row += winStride.Y)
            {
                for (int col = 0; col <= image.Width - winSize.X; col += winStride.X)
                {
                    var rect = new IntRect(col, row, col + winSize.X, row + winSize.Y);
                    windows[i] = cropProcessor.Crop(image, rect);
                    i++;
                }
            }

            return windows;
        }
    }
}
