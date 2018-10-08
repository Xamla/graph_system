using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Xamla.Graph.MethodModule;
using Xamla.Types.Converters;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.OpenCv
{
    [Module(ModuleType = "OpenCv.LoadImage", Description = "Loads an image from a file.", ReferenceUrl = "https://docs.opencv.org/3.3.0/d4/da8/group__imgcodecs.html#ga288b8b3da0892bd651fce07b3bbd3a56")]
    [ModuleTypeAlias("OpenCv.imread", IncludeInCatalog = true)]
    public class LoadImage
        : SingleInstanceMethodModule
    {
        public LoadImage(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, null, new OpenCvPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public Mat Load(
            [InputPin(PropertyMode = PropertyMode.Default, Description = "Name of file to be loaded.", Editor = WellKnownEditors.SingleLineText, ResolvePath = true)] string fileName = "astronaut.png",
            [InputPin(PropertyMode = PropertyMode.Default)] ImreadModes colorType = ImreadModes.AnyColor
        )
        {
            return new Mat(fileName, colorType);
        }
    }

    [Module(ModuleType = "OpenCv.ViewMat")]
    public class ViewMat
        : SingleInstanceMethodModule
    {
        public ViewMat(IGraphRuntime runtime)
            : base(runtime)
        {
            this.PreviewGenerator = new OpenCvPreviewGenerator(runtime) { IgnoreEmptyOutputs = true };
        }

        [ModuleMethod]
        public void View(
            [InputPin(PropertyMode = PropertyMode.Never)] Mat image
        )
        {
            this.PreviewGenerator.SetOutputs(image);
        }
    }

    [Module(ModuleType = "OpenCv.SaveImage", Description = "Saves an image to a specified file.", ReferenceUrl = "https://docs.opencv.org/3.3.0/d4/da8/group__imgcodecs.html#gabbc7ef1aa2edfaa87772f1202d67e0ce")]
    [ModuleTypeAlias("OpenCv.imwrite", IncludeInCatalog = true)]
    public class SaveImage
        : SingleInstanceMethodModule
    {
        public SaveImage(IGraphRuntime runtime)
            : base(runtime)
        {
            this.PreviewGenerator = new OpenCvPreviewGenerator(runtime) { IgnoreEmptyOutputs = true };
            EnableVirtualOutputPin();
        }

        [ModuleMethod]
        [OutputPin(Name = "outputFileName", Description = "Full path to the file that was written.")]
        public string Save(
            [InputPin(PropertyMode = PropertyMode.Never)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true, Description = "Path of the file to write. The file type is indirectly specified by the file extension (e.g. \".png\", \".jpg\").")] string path = "output.png",
            [InputPin(PropertyMode = PropertyMode.Default, Description = "Option to make the file name unique by appending a GUID to it.")] bool useUniqueFileName = false,
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Text")] int[] parameters = null
        )
        {
            this.PreviewGenerator.SetOutputs(image);

            var directory = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);

            if (useUniqueFileName)
                fileName = fileName + Guid.NewGuid().ToString("N");

            fileName = Path.Combine(directory, fileName + extension);

            if (extension != ".txt")
            {
                image.SaveImage(fileName, parameters);
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    for (int i = 0; i < image.Rows; i++)
                    {
                        for (int j = 0; j < image.Cols; j++)
                        {
                            sw.WriteLine(image.At<float>(i, j));
                        }
                    }
                    sw.Close();
                }
            }

            return fileName;
        }
    }

    public enum MatDepth
    {
        CV_8U = MatType.CV_8U,
        CV_8S = MatType.CV_8S,
        CV_16U = MatType.CV_16U,
        CV_16S = MatType.CV_16S,
        CV_32S = MatType.CV_32S,
        CV_32F = MatType.CV_32F,
        CV_64F = MatType.CV_64F,
        CV_USRTYPE1 = MatType.CV_USRTYPE1
    }

    [Module(ModuleType = "OpenCv.Core.ConvertTo")]
    public class ConvertMatType
        : SingleInstanceMethodModule
    {
        public ConvertMatType(IGraphRuntime runtime)
            : base(runtime)
        {
            this.PreviewGenerator = new OpenCvPreviewGenerator(runtime);
        }

        [ModuleMethod]
        public Mat Convert(
            [InputPin(PropertyMode = PropertyMode.Never)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] MatDepth depth,
            [InputPin(PropertyMode = PropertyMode.Default)] int channels)
        {
            MatType type = MatType.MakeType((int)depth, channels);
            var result = new Mat(image.Size(), type);
            image.ConvertTo(result, type);
            return result;
        }
    }

    [Module(ModuleType = "OpenCv.FromMat")]
    public class FromMat
        : SingleInstanceMethodModule
    {
        public FromMat(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Collapsed)
        {
        }

        public enum DestinationType
        {
            Point2fArray,
            PointArray
        }

        [ModuleMethod]
        public object Convert(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mat,
            [InputPin(PropertyMode = PropertyMode.Default)] DestinationType type = DestinationType.Point2fArray
            )
        {
            switch (type)
            {
                case DestinationType.PointArray:
                    return this.Runtime.TypeConverters.GetConverter<Mat, Point[]>().Convert(mat);
                case DestinationType.Point2fArray:
                    return this.Runtime.TypeConverters.GetConverter<Mat, Point2f[]>().Convert(mat);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    [Module(ModuleType = "OpenCv.ToMat")]
    public class ToMat
        : SingleInstanceMethodModule
    {
        public ToMat(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Collapsed)
        {
        }

        [ModuleMethod]
        public Mat Convert(
            [InputPin(PropertyMode = PropertyMode.Allow)] Array array,
            [InputPin(PropertyMode = PropertyMode.Default)] MatDepth depth
            )
        {
            var arrayType = array.GetType();

            var dimensions = arrayType.GetArrayRank();
            if (dimensions == 1)
            {
                var type = MatType.MakeType((int)depth, 1);
                var m = new Mat(1, array.Length, type, array.Cast<float>().ToArray());
                return m;
            }

            throw new NotImplementedException(string.Format("Cannot convert '{0}' to 'Mat'", arrayType));
        }
    }


    [Module(ModuleType = "OpenCv.SlidingWindow")]
    public class SlidingWindow
        : SingleInstanceMethodModule
    {
        public SlidingWindow(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        public ISequence<Mat> SlidingWindowMethod(
            Mat image,
            [InputPin(DefaultValue = "32, 32", PropertyMode = PropertyMode.Default)] Size windowSize,
            [InputPin(DefaultValue = "32, 32", PropertyMode = PropertyMode.Default)] Size stride
        )
        {
            return SlidingWindowProc(image, windowSize, stride).ToSequence();
        }


        public IEnumerable<Mat> SlidingWindowProc(
            Mat image,
            Size windowSize,
            Size stride
        )
        {
            if (windowSize.Width < 0 || windowSize.Height < 0)
                throw new ArgumentException("Window size must be non negative.", "windowSize");

            if (stride.Width <= 0 || stride.Height <= 0)
                throw new ArgumentException("Stride must be positive.", "stride");

            for (int row = 0; row <= image.Height - windowSize.Height; row += stride.Height)
            {
                for (int col = 0; col <= image.Width - windowSize.Width; col += stride.Width)
                {
                    var rect = new Rect(col, row, windowSize.Width, windowSize.Height);
                    yield return image.SubMat(rect);
                }
            }
        }
    }

    [Module(ModuleType = "OpenCv.HogSlidingWindow", Description = "Extracts a own array for each descriptor in the hog values. Each descriptor will be build by concating NxM blocks.")]
    public class HogSlidingWindow
        : SingleInstanceMethodModule
    {
        public HogSlidingWindow(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        public Tuple<ISequence<float[]>, ISequence<Rect>> Slide(
            [InputPin(Description = "", PropertyMode = PropertyMode.Never)] HOGDescriptor hogDescriptor,
            [InputPin(Description = "The values calculated by the Hog computation of a full image", PropertyMode = PropertyMode.Allow)] float[] values,
            [InputPin(Description = "", PropertyMode = PropertyMode.Default)] Size cellSize,
            [InputPin(Description = "How many blocks are describing one Feature-Vector.", PropertyMode = PropertyMode.Default)] Size blockCount
        )
        {
            return SlideInternal(values, hogDescriptor, cellSize, blockCount);
        }

        Tuple<ISequence<float[]>, ISequence<Rect>> SlideInternal(
            float[] values,
            HOGDescriptor hogDescriptor,
            Size cellSize,
            Size blockCount
        )
        {
            List<float[]> descriptors = new List<float[]>();
            List<Rect> descriptorWindows = new List<Rect>();

            var width = hogDescriptor.WinSize.Width;
            var height = hogDescriptor.WinSize.Height;

            var blockSize = hogDescriptor.BlockSize;
            var blockStride = hogDescriptor.BlockStride;

            // how many blocks(total) per row / col
            var blocksX = 1 + ((width - blockSize.Width) / blockStride.Width);
            var blocksY = 1 + ((height - blockSize.Height) / blockStride.Height);

            // how many cells per block
            var cellsX = blockSize.Width / cellSize.Width;
            var cellsY = blockSize.Height / cellSize.Height;

            var blockHistogramSize = cellsX * cellsY * hogDescriptor.Nbins;
            int descriptorSize = blockCount.Width * blockCount.Height * blockHistogramSize;

            var xGap = blocksX % blockCount.Width;
            var yGap = blocksY % blockCount.Height;

            var xPixels = blockSize.Width + ((blockCount.Width - 1) * blockStride.Width);
            var yPixels = blockSize.Height + ((blockCount.Height - 1) * blockStride.Height);

            int index = 0;

            // over all blocks (total)
            for (int blockX = 0; blockX < blocksX - xGap; blockX += blockCount.Width)
            {
                for (int blockY = 0; blockY < blocksY - yGap; blockY += blockCount.Height)
                {
                    var descriptor = new float[descriptorSize];
                    var descriptorWindow = new Rect(blockX * blockStride.Width, blockY * blockStride.Height, xPixels, yPixels);

                    int i = 0;
                    index = 0;
                    // over all blocks (per descriptor)
                    for (int currentBlockX = blockX; currentBlockX < blockX + blockCount.Width; currentBlockX++)
                    {
                        var blockXPos = currentBlockX * blocksY * blockHistogramSize;

                        for (int currentBlockY = blockY; currentBlockY < blockY + blockCount.Height; currentBlockY++)
                        {
                            var blockYPos = currentBlockY * blockHistogramSize;

                            // over all cells (per block)
                            for (int currentCellX = 0; currentCellX < cellsX; currentCellX++)
                            {
                                var cellXPos = currentCellX * cellsY * hogDescriptor.Nbins;

                                for (int currentCellY = 0; currentCellY < cellsY; currentCellY++)
                                {
                                    var cellYPos = currentCellY * hogDescriptor.Nbins;

                                    // over all bins
                                    for (int bin = 0; bin < hogDescriptor.Nbins; bin++)
                                    {
                                        // jump to X-BlockPos
                                        index = blockXPos;

                                        // jump to Y-BlockPos
                                        index += blockYPos;

                                        // jump to X-CellPos
                                        index += cellXPos;

                                        // jump to Y-CellPos
                                        index += cellYPos;

                                        // jump to Bin
                                        index += bin;

                                        try
                                        {
                                            descriptor[i++] = values[index];
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                    descriptors.Add(descriptor);
                    descriptorWindows.Add(descriptorWindow);
                }
            }
            return Tuple.Create(descriptors.ToSequence(), descriptorWindows.ToSequence());
        }
    }

    [Module(ModuleType = "OpenCv.ImagePyramid", Description = "")]
    public class ImagePyramid
        : SingleInstanceMethodModule
    {
        const float defaultScale = 1.4142135623730950488016887242097F; //sqrt(2)
        public ImagePyramid(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        public ISequence<Mat> PyrDown(
            [InputPin(Description = "", PropertyMode = PropertyMode.Never)] Mat image,
            [InputPin(Description = "", PropertyMode = PropertyMode.Default, DefaultValue = "1, 1")] Size minSize,
            [InputPin(Description = "", PropertyMode = PropertyMode.Default)] float scale = defaultScale,
            [InputPin(Description = "", PropertyMode = PropertyMode.Default)] InterpolationFlags filter = InterpolationFlags.Lanczos4
            )
        {
            if (scale <= 0)
                throw new ArgumentException("'scale' must be positive.", "scale");

            if (minSize.Width <= 0 || minSize.Height <= 0)
                throw new ArgumentException("'minSize' must be positive.", "minSize");

            return PyrDownInternal(image, minSize, scale, filter).ToSequence();
        }

        IEnumerable<Mat> PyrDownInternal(
            Mat image,
            Size minSize,
            float scale,
            InterpolationFlags filter)
        {
            var sizes = new List<Size>();
            float width = image.Width;
            float height = image.Height;
            for (; ; )
            {
                width /= scale;
                height /= scale;

                if (width < minSize.Width || height < minSize.Height)
                    break;

                sizes.Add(new Size(width, height));
            }

            yield return image;
            foreach (var size in sizes.Distinct())
            {
                yield return image.Resize(size, 0, 0, filter);
            }
        }
    }

    [Module(ModuleType = "OpenCv.Skeleton", Description = "")]
    public class Skeleton
        : SingleInstanceMethodModule
    {
        public Skeleton(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        public Mat MakeSkeleton(
            [InputPin(Description = "", PropertyMode = PropertyMode.Never)] Mat image
        )
        {
            if (image.Channels() != 1)
                throw new Exception("Input image should be a single channel image.");

            Mat img = image.Clone();
            Mat skel = Mat.Zeros(img.Size(), MatType.CV_8UC1);
            Mat temp;
            Mat eroded;

            Mat element = OpenCvWrapper.GetStructuringElement(new Size(3, 3), MorphShapes.Cross);

            bool done;
            do
            {
                eroded = img.Erode(element);
                temp = eroded.Dilate(element);
                temp = OpenCvWrapper.Subtract(img, temp);
                skel = OpenCvWrapper.BitwiseOr(skel, temp);
                eroded.CopyTo(img);

                done = (OpenCvWrapper.CountNonZero(img) == 0);
            } while (!done);

            return skel;
        }
    }

    [Module(ModuleType = "OpenCv.EquTriangle", Description = "")]
    public class EquTriangle
        : SingleInstanceMethodModule
    {
        public EquTriangle(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        public Tuple<Point, Point, Point, Point, Point, Point> Evaluate(LineSegmentPoint[] lines, int width, int heigth)
        {
            var triangles = new List<Tuple<Point, Point, Point, double>>();

            for (int i = 0; i < lines.Length; ++i)
            {
                var l1 = lines[i];
                for (int j = i + 1; j < lines.Length; ++j)
                {
                    var l2 = lines[j];
                    var p12 = l1.LineIntersection(l2);

                    if (p12.HasValue
                        && p12.Value.X >= 0 && p12.Value.X <= width
                        && p12.Value.Y >= 0 && p12.Value.Y <= heigth)
                    {
                        for (int k = j + 1; k < lines.Length; ++k)
                        {
                            var l3 = lines[k];
                            var p23 = l3.LineIntersection(l2);
                            var p13 = l3.LineIntersection(l1);

                            if (p23.HasValue
                                && p23.Value.X >= 0 && p23.Value.X <= width
                                && p23.Value.Y >= 0 && p23.Value.Y <= heigth
                                && p13.HasValue
                                && p13.Value.X >= 0 && p13.Value.X <= width
                                && p13.Value.Y >= 0 && p13.Value.Y <= heigth)
                            {
                                var p1 = p12.Value;
                                var p2 = p13.Value;
                                var p3 = p23.Value;

                                var d1 = p1.DistanceTo(p2);
                                var d2 = p1.DistanceTo(p3);
                                var d3 = p2.DistanceTo(p3);

                                var normalizeFactor = Math.Max(Math.Max(d1, d2), d3);
                                if (normalizeFactor == 0)
                                    continue;

                                d1 = d1 / normalizeFactor;
                                d2 = d2 / normalizeFactor;
                                d3 = d3 / normalizeFactor;

                                var diff1 = Math.Abs(d1 - d2);
                                var diff2 = Math.Abs(d1 - d3);
                                var diff3 = Math.Abs(d2 - d3);

                                triangles.Add(Tuple.Create(new Point { X = p1.X, Y = p1.Y }, new Point { X = p2.X, Y = p2.Y }, new Point { X = p3.X, Y = p3.Y }, diff1 + diff2 + diff3));
                            }
                        }
                    }
                }
            }

            triangles = triangles.Distinct().ToList();
            triangles.Sort((x1, x2) => x1.Item4.CompareTo(x2.Item4));
            var triangle = triangles.First();

            List<Point> points = new List<Point> { triangle.Item1, triangle.Item2, triangle.Item3 };
            points.Sort((x1, x2) => x1.Y.CompareTo(x2.Y));
            var up = points.First();

            points.Sort((x1, x2) => x1.X.CompareTo(x2.X));
            var left = points.First();
            var right = points.Last();

            var leftTranslated = left;
            var rightTranslated = new Point(right.X, left.Y);
            var upTranslated = new Point((left.X + (right.X - left.X) / 2), up.Y);

            var d = leftTranslated.DistanceTo(rightTranslated);
            var distance = upTranslated.DistanceTo(leftTranslated);

            // solve Equation:  d = (upTranslated + translatationVector).DistanceTo(leftTranslated)
            // d = sqrt((uX - lX)^2 + (uY - t - lY)^2)
            var uX = upTranslated.X;
            var lX = leftTranslated.X;
            var uY = upTranslated.Y;
            var lY = leftTranslated.Y;

            var t = Math.Sqrt(d * d - ((uX - lX) * (uX - lX))) + uY - lY;

            if (d > distance)
            {
                upTranslated = new Point(upTranslated.X, upTranslated.Y - t);
            }
            else
            {
                upTranslated = new Point(upTranslated.X, upTranslated.Y + t);
            }

            distance = upTranslated.DistanceTo(leftTranslated);

            return Tuple.Create(left, up, right, leftTranslated, upTranslated, rightTranslated);
        }
    }

    [Module(ModuleType = "OpenCv.Core.RandU")]
    public class RandomByteImage
        : SingleInstanceMethodModule
    {
        public RandomByteImage(IGraphRuntime runtime)
            : base(runtime)
        {
            this.PreviewGenerator = new OpenCvPreviewGenerator(runtime);
        }

        [ModuleMethod]
        public Mat Generate(
            [InputPin(PropertyMode = PropertyMode.Default)] int width,
            [InputPin(PropertyMode = PropertyMode.Default)] int height,
            [InputPin(PropertyMode = PropertyMode.Default)] int channels = 1)
        {
            var image = new Mat(height, width, MatType.MakeType(MatType.CV_8U, channels));

            if (channels == 1)
                image.Randu(0, 256);
            else if (channels == 3)
                image.Randu(new Scalar(0, 0, 0), new Scalar(256, 256, 256));
            else if (channels == 4)
                image.Randu(new Scalar(0, 0, 0, 0), new Scalar(256, 256, 256, 256));
            else
                throw new Exception("Not supported channel-count. Expected count: 1, 3 or 4");

            return image;
        }
    }

    [Module(ModuleType = "OpenCv.DrawTemplateMatching", Description = "Draws a rectangle generated by template matching")]
    public class DrawTemplateMatching
      : SingleInstanceMethodModule
    {
        public DrawTemplateMatching(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded, null, new OpenCvPreviewGenerator(runtime))
        {
        }

        [ModuleMethod]
        public Mat Load(
            [InputPin(PropertyMode = PropertyMode.Never)] Mat source,
            [InputPin(PropertyMode = PropertyMode.Never)] Mat sourceTemplate,
            Point matchLocation,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] int thickness = 3)
        {
            var destination = source.Clone();
            var p2 = new Point(matchLocation.X + sourceTemplate.Cols, matchLocation.Y + sourceTemplate.Rows);
            Cv2.Rectangle(destination, matchLocation, p2, color, thickness);
            return destination;
        }
    }
}
