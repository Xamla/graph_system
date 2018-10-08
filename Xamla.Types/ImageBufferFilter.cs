using System;
using System.Collections.Generic;
using System.Linq;
using Xamla.Utilities;

namespace Xamla.Types
{
    public static partial class I
    {
        static Dictionary<Type, PixelFormat> typeFormatMap = new Dictionary<Type, PixelFormat>
        {
            { typeof(Rgb24), PixelFormat.Rgb24 },
            { typeof(Rgba32), PixelFormat.Rgba32 },
            { typeof(Rgb48), PixelFormat.Rgb48 }
        };

        public static I<float> Multiply(this M<double> m, I<float> image, PixelFormat? targetFormat = null)
        {
            var a = image.Data;

            if (a.Rank != 3)        // y, x, channel
                throw new ArgumentException("Array must be three dimensional.");

            if (a.Dimension[2] < m.Columns)
                throw new ArgumentException("Third dimension of input array must have at least m.Columns elements.");

            if (!targetFormat.HasValue)
            {
                if (m.Columns == a.Dimension[2])
                    targetFormat = image.Format;
                else
                    throw new Exception("Target pixel format has to specified when channel count is changed.");
            }

            var r = new A<float>(a.Dimension[0], a.Dimension[1], m.Rows);
            var ranges = targetFormat.Value.ChannelRanges;

            for (int y = 0; y < r.Dimension[0]; ++y)
            {
                for (int x = 0; x < r.Dimension[1]; ++x)
                {
                    for (int i = 0; i < m.Rows; ++i)
                    {
                        double sum = 0;
                        for (int k = 0; k < m.Columns; ++k)
                        {
                            sum += m[i, k] * a[y, x, k];
                        }
                        r[y, x, i] = (float)ranges[i].Clamp(sum);
                    }
                }
            }

            return new I<float>(targetFormat.Value, r);
        }

        private static void ScaleChannelRangeInplace(this I<float> image, int channel, double lowIn, double highIn, double lowOut, double highOut, double gamma)
        {
            var range = image.Format.ChannelRanges[channel];
            double d = highIn - lowIn;
            double f = highOut - lowOut;
            if (d == 0)
                d = 1;
            if (gamma == 1)
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        // Values below lowIn and above highIn are clipped,
                        // i.e. values below lowIn are mapped to lowOut, and values above highIn are mapped to highOut
                        image[y, x, channel] = (float)(range.Clamp((image[y, x, channel] - lowIn) / d * f + lowOut));
                    }
                }
            }
            else
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        // Values below lowIn and above highIn are clipped,
                        // i.e. values below lowIn are mapped to lowOut, and values above highIn are mapped to highOut
                        image[y, x, channel] = (float)(range.Clamp(Math.Pow((image[y, x, channel] - lowIn) / d, gamma) * f + lowOut));
                    }
                }
            }
        }

        public static I<float> ScaleRanges(this I<float> image, Range<double>[] inRange, Range<double>[] outRange, double[] gamma = null)
        {
            I<float> result = image.Clone();
            result.format = new PixelFormat(result.format.PixelType, result.format.PixelChannels, result.format.ElementType, outRange, result.format.ColorSpace);

            if (gamma == null)
            {
                gamma = new double[image.Channels];
                for (int i = 0; i < image.Channels; ++i)
                    gamma[i] = 1.0;
            }

            var format = image.Format;

            // For Gray, Lab, Luv, and Yuv images only color channel 0 is adjusted.
            if (
                (format.PixelType.Equals(PixelType.F32C1) && (format.PixelChannels.Equals(PixelChannels.Gray) || format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (format.PixelType.Equals(PixelType.F32C3) && (format.PixelChannels.Equals(PixelChannels.Lab)
                || format.PixelChannels.Equals(PixelChannels.Luv)
                || format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                if (inRange[0].Low > inRange[0].High || outRange[0].Low > outRange[0].High)
                    throw new ArgumentException("InRange.Low must not be greater than inRange.High and outRange.Low must not be greater than outRange.High.");
                if (gamma[0] <= 0)
                    throw new ArgumentException("Gamma value has to be greater than 0.");

                result.ScaleChannelRangeInplace(0, inRange[0].Low, inRange[0].High, outRange[0].Low, outRange[0].High, gamma[0]);
            }

            // For Rgb and Xyz images all color channels are adjusted.
            else if (
                (format.PixelType.Equals(PixelType.F32C3) && (format.PixelChannels.Equals(PixelChannels.Rgb)
                                                            || format.PixelChannels.Equals(PixelChannels.Bgr)
                                                            || format.PixelChannels.Equals(PixelChannels.Xyz)
                                                            || format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (format.PixelType.Equals(PixelType.F32C4) && (format.PixelChannels.Equals(PixelChannels.Rgbx) || format.PixelChannels.Equals(PixelChannels.Bgrx)))
            )
            {
                if (inRange[0].Low > inRange[0].High || inRange[1].Low > inRange[1].High || inRange[2].Low > inRange[2].High
                    || outRange[0].Low > outRange[0].High || outRange[1].Low > outRange[1].High || outRange[2].Low > outRange[2].High)
                    throw new ArgumentException("InRange.Low must not be greater than inRange.High and outRange.Low must not be greater than outRange.High.");
                if (gamma[0] <= 0 || gamma[1] <= 0 || gamma[2] <= 0)
                    throw new ArgumentException("Gamma values have to be greater than 0.");

                result.ScaleChannelRangeInplace(0, inRange[0].Low, inRange[0].High, outRange[0].Low, outRange[0].High, gamma[0]);
                result.ScaleChannelRangeInplace(1, inRange[1].Low, inRange[1].High, outRange[1].Low, outRange[1].High, gamma[1]);
                result.ScaleChannelRangeInplace(2, inRange[2].Low, inRange[2].High, outRange[2].Low, outRange[2].High, gamma[2]);
            }

            // For Hsv images only color channel 2 is adjusted.
            else if (
                format.PixelType.Equals(PixelType.F32C3) && format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                if (inRange[2].Low > inRange[2].High || outRange[2].Low > outRange[2].High)
                    throw new ArgumentException("InRange.Low must not be greater than inRange.High and outRange.Low must not be greater than outRange.High.");
                if (gamma[2] <= 0)
                    throw new ArgumentException("Gamma value has to be greater than 0.");

                result.ScaleChannelRangeInplace(2, inRange[2].Low, inRange[2].High, outRange[2].Low, outRange[2].High, gamma[2]);
            }

            else
            {
                throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
            }

            return result;
        }

        public static int[] CreateChannelHistogram(this I<byte> image, int channel)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (channel < 0 || channel >= image.Format.ChannelCount)
                throw new ArgumentOutOfRangeException("Image does not have a channel with the specified index.", "channel");

            int[] histogram = new int[256];

            int step = image.Channels;
            var buffer = image.Data.Buffer;

            for (int i = channel; i < buffer.Length; i += step)
            {
                histogram[buffer[i]] += 1;
            }

            return histogram;
        }

        public static int[] CreateChannelHistogram(this I<float> image, int bins, int channel)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (bins <= 0)
                throw new ArgumentOutOfRangeException("Histogram bin count must be a positive integer.", "bins");

            if (channel < 0 || channel >= image.Format.ChannelCount)
                throw new ArgumentOutOfRangeException("Image does not have a channel with the specified index.", "channel");

            int[] histogram = new int[bins];

            int step = image.Channels;
            var buffer = image.Data.Buffer;

            var inR = image.Format.ChannelRanges[channel];
            var outR = Range.Create(0, bins - 1);

            for (int i = channel; i < buffer.Length; i += step)
            {
                double v = buffer[i];
                int index = (int)((v - inR.Low) / (inR.High - inR.Low) * bins);
                histogram[outR.Clamp(index)] += 1;
            }

            return histogram;
        }

        public static int[] CreateChannelHistogramOfRect(this I<float> image, int startRow, int startCol, int height, int width, int bins, int channel)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (bins <= 0)
                throw new ArgumentOutOfRangeException("Histogram bin count must be a positive integer.", "bins");

            if (channel < 0 || channel >= image.Format.ChannelCount)
                throw new ArgumentOutOfRangeException("Image does not have a channel with the specified index.", "channel");

            int[] histogram = new int[bins];

            var inR = image.Format.ChannelRanges[channel];
            var outR = Range.Create(0, bins - 1);

            for (int y = startRow; y < startRow + height; ++y)
            {
                for (int x = startCol; x < startCol + width; ++x)
                {
                    var v = image[y, x, channel];
                    int index = (int)((v - inR.Low) / (inR.High - inR.Low) * bins);
                    histogram[outR.Clamp(index)] += 1;
                }
            }

            return histogram;
        }

        public static int[][] CreateHistogram(this I<float> image, int bins)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (bins <= 0)
                throw new ArgumentOutOfRangeException("Histogram bin count must be a positive integer.", "bins");

            int channelCount = image.Format.ChannelCount;
            int[][] channelHistograms = new int[image.Format.ChannelCount][];
            for (int i = 0; i < image.Format.ChannelCount; ++i)
                channelHistograms[i] = new int[bins];

            var outR = Range.Create(0, bins - 1);
            var buffer = image.Data.Buffer;
            for (int i = 0; i < buffer.Length; ++i)
            {
                int c = i % channelCount;                       // current channel
                var inR = image.Format.ChannelRanges[c];        // channel range
                double v = buffer[i];                           // current channel value
                int index = (int)((v - inR.Low) / (inR.High - inR.Low) * bins);
                channelHistograms[c][outR.Clamp(index)] += 1;
            }

            return channelHistograms;
        }

        public static Range<double> GetHistogramBinRange(this I<float> image, int channel, int bins, int binIndex)
        {
            var r = image.Format.ChannelRanges[channel];
            double low = (r.High - r.Low) * binIndex / bins;
            double high = (r.High - r.Low) * (binIndex + 1) / bins;
            return Range.Create(r.Clamp(low), r.Clamp(high));
        }

        public static I<float> AutoSaturate(this I<float> image, double saturationFactor = 0.01, int bins = 500)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (!Range.Create(0, 0.5).Contains(saturationFactor))
                throw new ArgumentOutOfRangeException("Saturation factor must lie in the interval [0, 0.5].");

            var channelHistograms = CreateHistogram(image, bins);

            int nthElement = Math.Max(1, (int)(image.PixelCount * saturationFactor));
            var sourceRanges = new Range<double>[image.Channels];
            for (int i = 0; i < image.Channels; ++i)
            {
                var histogram = channelHistograms[i];

                int lowIndex = histogram.Scan(0, (a, x) => a + x).TakeWhile(x => x < nthElement).Count();
                int highIndex = histogram.Length - 1 - histogram.Reverse().Scan(0, (a, x) => a + x).TakeWhile(x => x < nthElement).Count();

                double low = image.GetHistogramBinRange(i, bins, lowIndex).Low;
                double high = image.GetHistogramBinRange(i, bins, highIndex).High;

                if (low < high)
                    sourceRanges[i] = Range.Create(low, high);
                else
                    sourceRanges[i] = image.Format.ChannelRanges[i];
            }

            return image.ScaleRanges(sourceRanges, image.Format.ChannelRanges);
        }

        public static I<float> Quantization(this I<float> image, double[] stepSize)
        {
            var result = image.CloneEmpty();
            for (int i = 0; i < image.Channels; ++i)
            {
                var r = image.Format.ChannelRanges[i];
                double rangeWidth = r.High - r.Low;
                double stretch = (rangeWidth + stepSize[i]) / (rangeWidth * stepSize[i]);

                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        var c = image[y, x, i];
                        result[y, x, i] = (float)(r.Clamp(Math.Floor((c - r.Low) * stretch) * stepSize[i] + r.Low));
                    }
                }
            }
            return result;
        }

        public static I<float> HistogramEqualization(this I<float> image, int bins = 255)
        {
            var format = image.Format;

            // For Rgb and Xyz images the histograms of all color channels are equalized.
            if (
                format.PixelType.Equals(PixelType.F32C3) && (format.PixelChannels.Equals(PixelChannels.Rgb)
                                                            || format.PixelChannels.Equals(PixelChannels.Xyz)
                                                            || format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                var result = image.CloneEmpty();

                // Calculate the histogram of the input image
                // (i.e. calculate the occurrence of each R, G, and B (X, Y, and Z) intensity values of the input image):
                var hist = image.CreateHistogram(bins);
                var cdf = new int[format.ChannelCount][];
                var newIntensityValues = new double[format.ChannelCount][];

                // Calculate the cumulative distribution probability:
                for (int i = 0; i < format.ChannelCount; ++i)
                {
                    var r = format.ChannelRanges[i];
                    cdf[i] = new int[bins];
                    newIntensityValues[i] = new double[bins];

                    cdf[i][0] = hist[i][0];
                    newIntensityValues[i][0] = (double)cdf[i][0] / image.PixelCount * (r.High - r.Low) - r.Low;
                    for (int j = 1; j < bins; ++j)
                    {
                        cdf[i][j] = cdf[i][j - 1] + hist[i][j];
                        newIntensityValues[i][j] = (double)cdf[i][j] / image.PixelCount * (r.High - r.Low) - r.Low;
                    }

                    for (int y = 0; y < image.Height; ++y)
                    {
                        for (int x = 0; x < image.Width; ++x)
                        {
                            var c = image[y, x, i];
                            int bin = (int)((c - r.Low) / (r.High - r.Low) * (bins - 1));
                            result[y, x, i] = (float)r.Clamp(newIntensityValues[i][bin]);
                        }
                    }
                }
                return result;
            }

            // For Gray, Lab, Luv and Yuv images the histogram of only the first color channel is equalized.
            else if (
                (format.PixelType.Equals(PixelType.F32C1) && (format.PixelChannels.Equals(PixelChannels.Gray)
                                                            || format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (format.PixelType.Equals(PixelType.F32C3) && (format.PixelChannels.Equals(PixelChannels.Lab)
                                                            || format.PixelChannels.Equals(PixelChannels.Luv)
                                                            || format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                return image.EqualizeSingleChannel(0, bins);
            }

            // For Hsv images the histogram of only the third color channel is equalized.
            else if (
                format.PixelType.Equals(PixelType.F32C3) && format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                return image.EqualizeSingleChannel(2, bins);
            }

            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        public static I<float> EqualizeSingleChannel(this I<float> image, int channel, int bins)
        {
            var result = image.Clone();

            // Calculate the histogram of the input image
            // (i.e. calculate the occurrence of the Gray (L) (Y) intensity values of the input image):
            var hist = image.CreateChannelHistogram(bins, channel);
            var newIntensityValues = new double[bins];

            // Calculate the cumulative distribution probability:
            var r = image.Format.ChannelRanges[channel];
            var cdf = new int[bins];

            cdf[0] = hist[0];
            newIntensityValues[0] = (double)cdf[0] / image.PixelCount * (r.High - r.Low) - r.Low;
            for (int j = 1; j < bins; ++j)
            {
                cdf[j] = cdf[j - 1] + hist[j];
                newIntensityValues[j] = (double)cdf[j] / image.PixelCount * (r.High - r.Low) - r.Low;
            }

            for (int y = 0; y < image.Height; ++y)
            {
                for (int x = 0; x < image.Width; ++x)
                {
                    var c = image[y, x, channel];
                    int bin = (int)((c - r.Low) / (r.High - r.Low) * (bins - 1));
                    result[y, x, channel] = (float)r.Clamp(newIntensityValues[bin]);
                }
            }

            return result;
        }

        public static I<float> AdaptiveHistogramEqualization(this I<float> image, int bins, int tilesY, int tilesX, double normalizedClipLimit = 0.01)
        {
            var format = image.Format;

            // Check if the image needs to be padded.
            // Padding occurs when image dimensions are not divisible by the selected number of tiles
            var colDiv = image.Width % tilesX;
            var rowDiv = image.Height % tilesY;
            int[] tileDim = new int[2];
            int padCol = 0, padRow = 0;
            if (colDiv != 0)
            {
                tileDim[0] = (int)Math.Floor((double)image.Width / tilesX) + 1;
                padCol = tileDim[0] * tilesX - image.Width;
            }
            else
                tileDim[0] = image.Width / tilesX;
            if (rowDiv != 0)
            {
                tileDim[1] = (int)Math.Floor((double)image.Height / tilesY) + 1;
                padRow = tileDim[1] * tilesY - image.Height;
            }
            else
                tileDim[1] = image.Height / tilesY;

            int padRowPre = (int)Math.Floor(padRow / 2.0);
            int padRowPost = (int)Math.Ceiling(padRow / 2.0);
            int padColPre = (int)Math.Floor(padCol / 2.0);
            int padColPost = (int)Math.Ceiling(padCol / 2.0);
            I<float> paddedImage = image.AddBorder(padRowPre, padColPre, padRowPost, padColPost, BorderMode.Reflect);

            // Compute actual clip limit from the normalized value entered by the user
            // Maximum value of normClipLimit=1 results in standard AHE, i.e. no clipping.
            // The minimum value minClipLimit would uniformly distribute the image pixels across
            // the entire histogram, which would result in the lowest possible contrast value.
            int pixelPerTile = tileDim[0] * tileDim[1];
            int minClipLimit = (int)Math.Ceiling((double)pixelPerTile / bins);
            int clipLimit = minClipLimit + (int)Math.Round((double)normalizedClipLimit * (pixelPerTile - minClipLimit));

            // For Gray, Lab, Luv and Yuv images the histogram of only the first color channel is equalized.
            if (
                (format.PixelType.Equals(PixelType.F32C1) && (format.PixelChannels.Equals(PixelChannels.Gray)
                                                            || format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (format.PixelType.Equals(PixelType.F32C3) && (format.PixelChannels.Equals(PixelChannels.Lab)
                || format.PixelChannels.Equals(PixelChannels.Luv)
                || format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                // Calculate the equalized histogram (only for channel 0) for each tile,
                // i.e. calculate the histogram for each tile, clip it, equalize it and
                // store it in the array "equalizedHist" of equalized tile histograms.
                float[][] equalizedHist = new float[tilesX * tilesY][];
                int counter = 0;
                for (int row = 0; row < tilesY * tileDim[1]; row += tileDim[1])
                {
                    for (int col = 0; col < tilesX * tileDim[0]; col += tileDim[0])
                    {
                        equalizedHist[counter]
                            = paddedImage.CalcEqualizedChannelHistogramOfRect(row, col, tileDim[1], tileDim[0], bins, clipLimit, 0);
                        counter += 1;
                    }
                }

                // Assign each image pixel with its new intensity values gained from a bilinear
                // interpolation between equalized histogram values of neighboring tiles.
                // (only for channel 0)
                I<float> equalizedImage = image.Clone();
                int w = tileDim[0], h = tileDim[1], width = equalizedImage.Width, height = equalizedImage.Height;
                int nTw = tilesX; // number of tiles in width
                int nTh = tilesY; // number of tiles in height

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        // Calculate indices of tiles belonging to the interpolation.
                        int x0 = (int)Math.Floor(Math.Max(0, (x - 0.5 * w) / w));
                        int x1 = Math.Min((int)Math.Floor((x + 0.5 * w) / w), nTw - 1);
                        //int x1 = Math.Min(x0 + 1, nTw - 1);
                        int y0 = (int)Math.Floor(Math.Max(0, (y - 0.5 * h) / h));
                        int y1 = Math.Min((int)Math.Floor((y + 0.5 * h) / h), nTh - 1);
                        //int y1 = Math.Min(y0 + 1, nTh - 1);
                        int i00 = y0 * nTw + x0;
                        int i10 = y0 * nTw + x1;
                        int i01 = y1 * nTw + x0;
                        int i11 = y1 * nTw + x1;

                        // Calculate factors for the interpolation in x- and y-direction.
                        double fx = ((x + 0.5 * w) % w) / w;
                        double fy = ((y + 0.5 * h) % h) / h;

                        // Calculate index of bin, in which image intensity value of pixel (x,y) falls.
                        // (only for channel 0)
                        var r = format.ChannelRanges[0];
                        var c = image[y, x, 0];
                        int bin = (int)((c - r.Low) / (r.High - r.Low) * (bins - 1));

                        // Calculate new intensity value of image pixel (x,y) via bilinear interpolation
                        // between equalized histogram values of the four neighboring tiles determined above.
                        // (only for channel 0)
                        equalizedImage[y, x, 0]
                            = (float)Interpolate(equalizedHist[i00][bin], equalizedHist[i10][bin],
                            equalizedHist[i01][bin], equalizedHist[i11][bin], fx, fy);
                    }
                }
                return equalizedImage;
            }

            // For Rgb and Xyz images the histograms of all color channels are equalized.
            else if (
                format.PixelType.Equals(PixelType.F32C3) && (format.PixelChannels.Equals(PixelChannels.Rgb)
                                                            || format.PixelChannels.Equals(PixelChannels.Xyz)
                                                            || format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                // Calculate the equalized histogram for each tile, i.e. calculate the histogram
                // for each tile, clip it, equalize it and store it in the array "equalizedHist"
                // of equalized tile histograms.
                float[][][] equalizedHist = new float[tilesX * tilesY][][];
                int counter = 0;
                for (int row = 0; row < tilesY * tileDim[1]; row += tileDim[1])
                {
                    for (int col = 0; col < tilesX * tileDim[0]; col += tileDim[0])
                    {
                        equalizedHist[counter] = new float[3][];
                        equalizedHist[counter][0]
                            = paddedImage.CalcEqualizedChannelHistogramOfRect(row, col, tileDim[1], tileDim[0], bins, clipLimit, 0);
                        equalizedHist[counter][1]
                            = paddedImage.CalcEqualizedChannelHistogramOfRect(row, col, tileDim[1], tileDim[0], bins, clipLimit, 1);
                        equalizedHist[counter][2]
                            = paddedImage.CalcEqualizedChannelHistogramOfRect(row, col, tileDim[1], tileDim[0], bins, clipLimit, 2);
                        counter += 1;
                    }
                }

                // Assign each image pixel with its new intensity values gained from a bilinear
                // interpolation between equalized histogram values of neighboring tiles.
                I<float> equalizedImage = image.Clone();
                int w = tileDim[0], h = tileDim[1], width = equalizedImage.Width, height = equalizedImage.Height;
                int nTw = tilesX; // number of tiles in width
                int nTh = tilesY; // number of tiles in height

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        // Calculate indices of tiles belonging to the interpolation.
                        int x0 = (int)Math.Floor(Math.Max(0, (x - 0.5 * w) / w));
                        int x1 = Math.Min((int)Math.Floor((x - 0.5 * w) / w + 1), nTw - 1);
                        //int x1 = Math.Min(x0 + 1, nTw - 1);
                        int y0 = (int)Math.Floor(Math.Max(0, (y - 0.5 * h) / h));
                        int y1 = Math.Min((int)Math.Floor((y - 0.5 * h) / h + 1), nTh - 1);
                        //int y1 = Math.Min(y0 + 1, nTh - 1);
                        int i00 = y0 * nTw + x0;
                        int i10 = y0 * nTw + x1;
                        int i01 = y1 * nTw + x0;
                        int i11 = y1 * nTw + x1;

                        // Calculate factors for the interpolation in x- and y-direction.
                        double fx = ((x + 0.5 * w) % w) / w;
                        double fy = ((y + 0.5 * h) % h) / h;

                        for (int z = 0; z < image.Channels; ++z)
                        {
                            // Calculate index of bin, in which image intensity value of pixel (x,y,z) falls.
                            var r = format.ChannelRanges[z];
                            var c = image[y, x, z];
                            int bin = (int)((c - r.Low) / (r.High - r.Low) * (bins - 1));

                            // Calculate new intensity value of image pixel (x,y,z) via bilinear interpolation
                            // between equalized histogram values of the four neighboring tiles determined above.
                            equalizedImage[y, x, z]
                                = (float)Interpolate(equalizedHist[i00][z][bin], equalizedHist[i10][z][bin],
                                equalizedHist[i01][z][bin], equalizedHist[i11][z][bin], fx, fy);
                        }
                    }
                }
                return equalizedImage;
            }

            // For Hsv images the histogram of only the third color channel is equalized.
            else if (
                format.PixelType.Equals(PixelType.F32C3) && format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                // Calculate the equalized histogram (only for channel 2) for each tile,
                // i.e. calculate the histogram for each tile, clip it, equalize it and
                // store it in the array "equalizedHist" of equalized tile histograms.
                float[][] equalizedHist = new float[tilesX * tilesY][];
                int counter = 0;
                for (int row = 0; row < tilesY * tileDim[1]; row += tileDim[1])
                {
                    for (int col = 0; col < tilesX * tileDim[0]; col += tileDim[0])
                    {
                        equalizedHist[counter]
                            = paddedImage.CalcEqualizedChannelHistogramOfRect(row, col, tileDim[1], tileDim[0], bins, clipLimit, 2);
                        counter += 1;
                    }
                }

                // Assign each image pixel with its new intensity values gained from a bilinear
                // interpolation between equalized histogram values of neighboring tiles.
                // (only for channel 2)
                I<float> equalizedImage = image.Clone();
                int w = tileDim[0], h = tileDim[1], width = equalizedImage.Width, height = equalizedImage.Height;
                int nTw = tilesX; // number of tiles in width
                int nTh = tilesY; // number of tiles in height

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        // Calculate indices of tiles belonging to the interpolation.
                        int x0 = (int)Math.Floor(Math.Max(0, (x - 0.5 * w) / w));
                        int x1 = Math.Min((int)Math.Floor((x - 0.5 * w) / w + 1), nTw - 1);
                        //int x1 = Math.Min(x0 + 1, nTw - 1);
                        int y0 = (int)Math.Floor(Math.Max(0, (y - 0.5 * h) / h));
                        int y1 = Math.Min((int)Math.Floor((y - 0.5 * h) / h + 1), nTh - 1);
                        //int y1 = Math.Min(y0 + 1, nTh - 1);
                        int i00 = y0 * nTw + x0;
                        int i10 = y0 * nTw + x1;
                        int i01 = y1 * nTw + x0;
                        int i11 = y1 * nTw + x1;

                        // Calculate factors for the interpolation in x- and y-direction.
                        double fx = ((x + 0.5 * w) % w) / w;
                        double fy = ((y + 0.5 * h) % h) / h;

                        // Calculate index of bin, in which image intensity value of pixel (x,y) falls.
                        // (only for channel 2)
                        var r = format.ChannelRanges[2];
                        var c = image[y, x, 2];
                        int bin = (int)((c - r.Low) / (r.High - r.Low) * (bins - 1));

                        // Calculate new intensity value of image pixel (x,y) via bilinear interpolation
                        // between equalized histogram values of the four neighboring tiles determined above.
                        // (only for channel 2)
                        equalizedImage[y, x, 2]
                            = (float)Interpolate(equalizedHist[i00][bin], equalizedHist[i10][bin],
                            equalizedHist[i01][bin], equalizedHist[i11][bin], fx, fy);
                    }
                }
                return equalizedImage;
            }

            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        // Calculate clipped and equalized histogram of specified range and channel of input image.
        public static float[] CalcEqualizedChannelHistogramOfRect(this I<float> image, int startRow, int startCol, int height, int width, int bins, int clipLimit, int channel)
        {
            int[] hist = new int[bins];
            int[] cdf = new int[bins];
            float[] equalizedHist = new float[bins];

            hist = image.CreateChannelHistogramOfRect(startRow, startCol, height, width, bins, channel);
            hist.ClipChannelHistogram(bins, clipLimit);

            var r = image.Format.ChannelRanges[channel];
            int pixelCount = height * width;
            cdf[0] = hist[0];
            equalizedHist[0] = (float)((double)cdf[0] / pixelCount * (r.High - r.Low) - r.Low);
            for (int j = 1; j < bins; ++j)
            {
                cdf[j] = cdf[j - 1] + hist[j];
                equalizedHist[j] = (float)((double)cdf[j] / pixelCount * (r.High - r.Low) - r.Low);
            }
            return equalizedHist;
        }

        // This function clips a channel histogram according to the clipLimit
        // and redistributes clipped pixels accross bins below the clipLimit
        public static void ClipChannelHistogram(this int[] hist, int bins, int clipLimit)
        {
            // Total number of pixels overflowing clip limit in each bin.
            int totalExcess = 0;
            for (int i = 0; i < bins; ++i)
                totalExcess += Math.Max(hist[i] - clipLimit, 0);
            // Clip the histogram and redistribute the excess pixels in each bin.
            int avgBinIncr = (int)Math.Floor((double)totalExcess / bins);
            int upperLimit = clipLimit - avgBinIncr; // Bins larger than this will be set to clipLimit.
            // Put multiple pixels into the obvious place first.
            for (int i = 0; i < bins; ++i)
            {
                if (hist[i] > clipLimit)
                    hist[i] = clipLimit;
                else if (hist[i] > upperLimit) // high bin count
                {
                    totalExcess = totalExcess - (clipLimit - hist[i]);
                    hist[i] = clipLimit;
                }
                else
                {
                    totalExcess = totalExcess - avgBinIncr;
                    hist[i] = hist[i] + avgBinIncr;

                }
            }
            // This loops redistributes the remaining pixels, one pixel at a time
            int k = 0;
            while (totalExcess != 0)
            {
                // Keep increasing the step as fewer and fewer pixels remain for
                // the redistribution (spread them evenly)
                int stepSize = Math.Max((int)Math.Floor((double)bins / totalExcess), 1);
                for (int m = k; m < bins; m += stepSize)
                {
                    if (hist[m] < clipLimit)
                    {
                        hist[m] = hist[m] + 1;
                        totalExcess = totalExcess - 1; // reduce excess
                        if (totalExcess == 0)
                            break;
                    }
                }
                k = k + 1; // prevent from always placing the pixels in bin 0
                if (k >= bins) // start over if bins was reached
                    k = 1;
            }
        }

        // Bilinear interpolation.
        public static double Interpolate(double x0y0, double x1y0, double x0y1, double x1y1, double fx, double fy)
        {
            return (1.0 - fx) * (1.0 - fy) * x0y0 + fx * (1.0 - fy) * x1y0 + (1.0 - fx) * fy * x0y1 + fx * fy * x1y1;
        }

        public static float[,] Calc2DGausskernel(int length, double weight)
        {
            if (weight < 0.0)
                throw new ArgumentException("Weight has to be greater than zero.");

            if (length % 2 == 0) length -= 1;
            float[,] kernel = new float[length, length];
            int kernelRadius = length / 2;
            double c = 2.0 * weight * weight;
            double d = 1.0 / (c * Math.PI);
            double sumTotal = 0.0;

            for (int filterY = -kernelRadius; filterY <= kernelRadius; ++filterY)
            {
                for (int filterX = -kernelRadius; filterX <= kernelRadius; ++filterX)
                {
                    double arg = (filterX * filterX + filterY * filterY) / c;
                    kernel[filterY + kernelRadius, filterX + kernelRadius] = (float)(d * Math.Exp(-arg));
                    sumTotal += kernel[filterY + kernelRadius, filterX + kernelRadius];
                }
            }
            for (int y = 0; y < length; ++y)
            {
                for (int x = 0; x < length; ++x)
                {
                    kernel[y, x] = (float)(kernel[y, x] / sumTotal);
                }
            }
            return kernel;
        }

        public static float[] Calc1DGausskernel(int length, double weight)
        {
            if (weight < 0.0)
                throw new ArgumentException("Weight has to be greater than zero.");

            if (length % 2 == 0) length -= 1;
            float[] kernel = new float[length];
            int kernelRadius = length / 2;
            double c = 2.0 * weight * weight;
            double d = 1.0 / (c * Math.PI);
            double sumTotal = 0.0;

            for (int i = -kernelRadius; i <= kernelRadius; ++i)
            {
                double arg = (i * i) / c;
                kernel[i + kernelRadius] = (float)(d * Math.Exp(-arg));
                sumTotal += kernel[i + kernelRadius];
            }
            for (int i = 0; i < length; ++i)
            {
                kernel[i] = (float)(kernel[i] / sumTotal);
            }
            return kernel;
        }

        public static I<float> GaussianBlur(this I<float> source, int matrixSize, double weight)
        {
            float[] kernel1D = Calc1DGausskernel(matrixSize, weight);
            I<float> result = source.Clone();
            I<float> tmp = source.Clone();
            int filterOffset = (int)(matrixSize - 1) / 2;
            var yRange = Range.Create(0, source.Height - 1);
            var xRange = Range.Create(0, source.Width - 1);

            // For Rgb and Xyz images the difference of Gaussians for all channels are calculated.
            if (
                source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Rgb)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Xyz)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    //double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                    // Filtering along y-direction
                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            double pixelValue = 0.0;
                            for (int filterY = -filterOffset; filterY <= filterOffset; ++filterY)
                            {
                                int calcYOffset = yRange.Clamp(y + filterY);
                                pixelValue += source[calcYOffset, x, z]
                                            * kernel1D[filterY + filterOffset];
                            }
                            tmp[y, x, z] = (float)pixelValue;
                        }
                    }
                    // Filtering along x-direction
                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            double pixelValue = 0.0;
                            for (int filterX = -filterOffset; filterX <= filterOffset; ++filterX)
                            {
                                int calcXOffset = xRange.Clamp(x + filterX);
                                pixelValue += tmp[y, calcXOffset, z]
                                            * kernel1D[filterX + filterOffset];
                            }
                            //result[y, x, z] = (float)r.Clamp(source[y, x, z] - pixelValue + rMiddle);
                            result[y, x, z] = (float)r.Clamp(pixelValue); // Gaussian blur
                        }
                    }
                }
                return result;
            }

            // For Gray, Lab, Luv and Yuv images the difference of Gaussians for only the first color channel is calculated.
            else if (
                (source.Format.PixelType.Equals(PixelType.F32C1) && (source.Format.PixelChannels.Equals(PixelChannels.Gray)
                                                                    || source.Format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Lab)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Luv)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                var r = source.Format.ChannelRanges[0];
                //double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                // Filtering along y-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterY = -filterOffset; filterY <= filterOffset; ++filterY)
                        {
                            int calcYOffset = yRange.Clamp(y + filterY);
                            pixelValue += source[calcYOffset, x, 0]
                                        * kernel1D[filterY + filterOffset];
                        }
                        tmp[y, x, 0] = (float)pixelValue;
                    }
                }
                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterX = -filterOffset; filterX <= filterOffset; ++filterX)
                        {
                            int calcXOffset = xRange.Clamp(x + filterX);
                            pixelValue += tmp[y, calcXOffset, 0]
                                        * kernel1D[filterX + filterOffset];
                        }
                        //result[y, x, 0] = (float)r.Clamp(source[y, x, 0] - pixelValue + rMiddle);
                        result[y, x, 0] = (float)r.Clamp(pixelValue); // Gaussian blur
                    }
                }
                return result;
            }
            // For Hsv images the difference of Gaussians for only the third color channel is calculated.
            else if (
                source.Format.PixelType.Equals(PixelType.F32C3) && source.Format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                var r = source.Format.ChannelRanges[2];
                //double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                // Filtering along y-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterY = -filterOffset; filterY <= filterOffset; ++filterY)
                        {
                            int calcYOffset = yRange.Clamp(y + filterY);
                            pixelValue += source[calcYOffset, x, 2]
                                        * kernel1D[filterY + filterOffset];
                        }
                        tmp[y, x, 2] = (float)pixelValue;
                    }
                }
                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterX = -filterOffset; filterX <= filterOffset; ++filterX)
                        {
                            int calcXOffset = xRange.Clamp(x + filterX);
                            pixelValue += tmp[y, calcXOffset, 2]
                                        * kernel1D[filterX + filterOffset];
                        }
                        //result[y, x, 2] = (float)r.Clamp(source[y, x, 2] - pixelValue + rMiddle);
                        result[y, x, 2] = (float)r.Clamp(pixelValue); // Gaussian blur
                    }
                }
                return result;
            }
            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        public static I<float> FastGaussianBlur(this I<float> source, double weight)
        {
            I<float> tmp = source.CloneEmpty();
            I<float> result = source.CloneEmpty();
            double input = 0.0, output = 0.0, out1 = 0.0, out2 = 0.0, out3 = 0.0;

            double q = 0.0;
            if (weight > 2.5)
                q = 0.98711 * weight - 0.96330;
            else if (weight <= 2.5 && weight >= 0.5)
                q = 3.97156 - 4.14554 * Math.Sqrt(1.0 - 0.26891 * weight);
            else
                throw new ArgumentException("Weight has to be >= 0.5.");

            double q2 = q * q;
            double q3 = q2 * q;
            double b0 = 1.57825 + 2.44413 * q + 1.4281 * q2 + 0.422205 * q3;
            double b1 = 2.44413 * q + 2.85619 * q2 + 1.26661 * q3;
            double b2 = -1.4281 * q2 - 1.26661 * q3;
            double b3 = 0.422205 * q3;

            double B = 1.0 - (b1 + b2 + b3) / b0;

            // For Rgb and Xyz images the division of Gaussians for all channels are calculated.
            if (
                source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Rgb)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Xyz)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    //double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                    // Filtering along x-direction
                    for (int y = 0; y < source.Height; ++y)
                    {
                        // Forward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int x = 0; x < source.Width; ++x)
                        {
                            input = source[y, x, z];
                            // Replication at the boundary
                            if (x == 0) { out1 = input; out2 = input; out3 = input; }
                            if (x == 1) { out2 = input; out3 = input; }
                            if (x == 2) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            tmp[y, x, z] = (float)output;
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                        // Backward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int x = source.Width - 1; x >= 0; --x)
                        {
                            input = tmp[y, x, z];
                            // Replication at the boundary
                            if (x == source.Width - 1) { out1 = input; out2 = input; out3 = input; }
                            if (x == source.Width - 2) { out2 = input; out3 = input; }
                            if (x == source.Width - 3) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            result[y, x, z] = (float)output;
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                    }
                    // Filtering along y-direction
                    for (int x = 0; x < source.Width; ++x)
                    {
                        // Forward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int y = 0; y < source.Height; ++y)
                        {
                            input = result[y, x, z];
                            // Replication at the boundary
                            if (y == 0) { out1 = input; out2 = input; out3 = input; }
                            if (y == 1) { out2 = input; out3 = input; }
                            if (y == 2) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            tmp[y, x, z] = (float)output;
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                        // Backward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int y = source.Height - 1; y >= 0; --y)
                        {
                            input = tmp[y, x, z];
                            // Replication at the boundary
                            if (y == source.Height - 1) { out1 = input; out2 = input; out3 = input; }
                            if (y == source.Height - 2) { out2 = input; out3 = input; }
                            if (y == source.Height - 3) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            //result[y, x, z] = (float)r.Clamp(source[y, x, z] - output + rMiddle);
                            result[y, x, z] = (float)r.Clamp(output); // Gaussian blur
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                    }
                }
                return result;
            }
            // For Gray, Lab, Luv and Yuv images the difference of Gaussians for only the first color channel is calculated.
            else if (
                (source.Format.PixelType.Equals(PixelType.F32C1) && (source.Format.PixelChannels.Equals(PixelChannels.Gray)
                                                                    || source.Format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Lab)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Luv)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                var r = source.Format.ChannelRanges[0];
                //double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = 0; x < source.Width; ++x)
                    {
                        input = source[y, x, 0];
                        // Replication at the boundary
                        if (x == 0) { out1 = input; out2 = input; out3 = input; }
                        if (x == 1) { out2 = input; out3 = input; }
                        if (x == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 0] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = source.Width - 1; x >= 0; --x)
                    {
                        input = tmp[y, x, 0];
                        // Replication at the boundary
                        if (x == source.Width - 1) { out1 = input; out2 = input; out3 = input; }
                        if (x == source.Width - 2) { out2 = input; out3 = input; }
                        if (x == source.Width - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        result[y, x, 0] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                // Filtering along y-direction
                for (int x = 0; x < source.Width; ++x)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = 0; y < source.Height; ++y)
                    {
                        input = result[y, x, 0];
                        // Replication at the boundary
                        if (y == 0) { out1 = input; out2 = input; out3 = input; }
                        if (y == 1) { out2 = input; out3 = input; }
                        if (y == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 0] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = source.Height - 1; y >= 0; --y)
                    {
                        input = tmp[y, x, 0];
                        // Replication at the boundary
                        if (y == source.Height - 1) { out1 = input; out2 = input; out3 = input; }
                        if (y == source.Height - 2) { out2 = input; out3 = input; }
                        if (y == source.Height - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        //result[y, x, 0] = (float)r.Clamp(source[y, x, 0] - output + rMiddle);
                        result[y, x, 0] = (float)r.Clamp(output); // Gaussian blur
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                return result;
            }
            // For Hsv images the difference of Gaussians for only the third color channel is calculated.
            else if (
                source.Format.PixelType.Equals(PixelType.F32C3) && source.Format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                var r = source.Format.ChannelRanges[2];
                //double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = 0; x < source.Width; ++x)
                    {
                        input = source[y, x, 2];
                        // Replication at the boundary
                        if (x == 0) { out1 = input; out2 = input; out3 = input; }
                        if (x == 1) { out2 = input; out3 = input; }
                        if (x == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 2] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = source.Width - 1; x >= 0; --x)
                    {
                        input = tmp[y, x, 2];
                        // Replication at the boundary
                        if (x == source.Width - 1) { out1 = input; out2 = input; out3 = input; }
                        if (x == source.Width - 2) { out2 = input; out3 = input; }
                        if (x == source.Width - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        result[y, x, 2] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                // Filtering along y-direction
                for (int x = 0; x < source.Width; ++x)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = 0; y < source.Height; ++y)
                    {
                        input = result[y, x, 2];
                        // Replication at the boundary
                        if (y == 0) { out1 = input; out2 = input; out3 = input; }
                        if (y == 1) { out2 = input; out3 = input; }
                        if (y == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 2] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = source.Height - 1; y >= 0; --y)
                    {
                        input = tmp[y, x, 2];
                        // Replication at the boundary
                        if (y == source.Height - 1) { out1 = input; out2 = input; out3 = input; }
                        if (y == source.Height - 2) { out2 = input; out3 = input; }
                        if (y == source.Height - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        //result[y, x, 2] = (float)r.Clamp(source[y, x, 2] - output + rMiddle);
                        result[y, x, 2] = (float)r.Clamp(output); // Gaussian blur
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                return result;
            }
            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        public static I<float> DifferenceOfGaussianFilter(this I<float> source, int matrixSize, double weight1, double weight2 = 0.0)
        {
            if (weight2 != 0.0 && weight2 <= weight1)
                throw new ArgumentException("Weight2 has to be greater than weight1.");

            I<float> gaussianBlur1 = source.GaussianBlur(matrixSize, weight1);
            I<float> gaussianBlur2 = source.CloneEmpty();
            if (weight2 != 0.0)
                gaussianBlur2 = source.GaussianBlur(matrixSize, weight2);
            I<float> result = source.CloneEmpty();

            // For Rgb and Xyz images the difference of Gaussians for all channels are calculated.
            if (
                source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Rgb)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Xyz)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            if (weight2 == 0.0)
                                result[y, x, z] = (float)r.Clamp(source[y, x, z] - gaussianBlur1[y, x, z] + rMiddle);
                            else
                                result[y, x, z] = (float)r.Clamp(gaussianBlur1[y, x, z] - gaussianBlur2[y, x, z] + rMiddle);
                        }
                    }
                }
                return result;
            }
            // For Gray, Lab, Luv and Yuv images the difference of Gaussians for only the first color channel is calculated.
            else if (
                (source.Format.PixelType.Equals(PixelType.F32C1) && (source.Format.PixelChannels.Equals(PixelChannels.Gray)
                                                                    || source.Format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Lab)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Luv)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                var r = source.Format.ChannelRanges[0];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        if (weight2 == 0.0)
                            result[y, x, 0] = (float)r.Clamp(source[y, x, 0] - gaussianBlur1[y, x, 0] + rMiddle);
                        else
                            result[y, x, 0] = (float)r.Clamp(gaussianBlur1[y, x, 0] - gaussianBlur2[y, x, 0] + rMiddle);
                    }
                }
                return result;
            }
            // For Hsv images the difference of Gaussians for only the third color channel is calculated.
            else if (
                source.Format.PixelType.Equals(PixelType.F32C3) && source.Format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                var r = source.Format.ChannelRanges[2];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        if (weight2 == 0.0)
                            result[y, x, 2] = (float)r.Clamp(source[y, x, 2] - gaussianBlur1[y, x, 2] + rMiddle);
                        else
                            result[y, x, 2] = (float)r.Clamp(gaussianBlur1[y, x, 2] - gaussianBlur2[y, x, 2] + rMiddle);
                    }
                }
                return result;
            }
            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        public static I<float> FastDoGFilter(this I<float> source, double weight1, double weight2 = 0.0)
        {
            if (weight2 != 0.0 && weight2 <= weight1)
                throw new ArgumentException("Weight2 has to be greater than weight1.");

            I<float> gaussianBlur1 = source.FastGaussianBlur(weight1);
            I<float> gaussianBlur2 = source.CloneEmpty();
            if (weight2 != 0.0)
                gaussianBlur2 = source.FastGaussianBlur(weight2);
            I<float> result = source.CloneEmpty();

            // For Rgb and Xyz images the difference of Gaussians for all channels are calculated.
            if (
                source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Rgb)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Xyz)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            if (weight2 == 0.0)
                                result[y, x, z] = (float)r.Clamp(source[y, x, z] - gaussianBlur1[y, x, z] + rMiddle);
                            else
                                result[y, x, z] = (float)r.Clamp(gaussianBlur1[y, x, z] - gaussianBlur2[y, x, z] + rMiddle);
                        }
                    }
                }
                return result;
            }
            // For Gray, Lab, Luv and Yuv images the difference of Gaussians for only the first color channel is calculated.
            else if (
                (source.Format.PixelType.Equals(PixelType.F32C1) && (source.Format.PixelChannels.Equals(PixelChannels.Gray)
                                                                    || source.Format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Lab)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Luv)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                var r = source.Format.ChannelRanges[0];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        if (weight2 == 0.0)
                            result[y, x, 0] = (float)r.Clamp(source[y, x, 0] - gaussianBlur1[y, x, 0] + rMiddle);
                        else
                            result[y, x, 0] = (float)r.Clamp(gaussianBlur1[y, x, 0] - gaussianBlur2[y, x, 0] + rMiddle);
                    }
                }
                return result;
            }
            // For Hsv images the difference of Gaussians for only the third color channel is calculated.
            else if (
                source.Format.PixelType.Equals(PixelType.F32C3) && source.Format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                var r = source.Format.ChannelRanges[2];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;

                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        if (weight2 == 0.0)
                            result[y, x, 2] = (float)r.Clamp(source[y, x, 2] - gaussianBlur1[y, x, 2] + rMiddle);
                        else
                            result[y, x, 2] = (float)r.Clamp(gaussianBlur1[y, x, 2] - gaussianBlur2[y, x, 2] + rMiddle);
                    }
                }
                return result;
            }
            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        public static I<float> DivisionOfGaussianFilter(this I<float> source, int matrixSize, double weight, double scaling)
        {
            float[] kernel1D = Calc1DGausskernel(matrixSize, weight);
            I<float> denominator = new I<float>(source.Format, source.Height, source.Width, source.Channels);
            I<float> result = source.CloneEmpty();
            I<float> tmp = source.CloneEmpty();
            int filterOffset = (int)(matrixSize - 1) / 2;
            var yRange = Range.Create(0, source.Height - 1);
            var xRange = Range.Create(0, source.Width - 1);
            double[] mean = new double[source.Channels];

            // For Rgb and Xyz images the division of Gaussians for all channels are calculated.
            if (
                source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Rgb)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Xyz)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                    mean[z] = 0.0;

                    // Filtering along y-direction
                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            double pixelValue = 0.0;
                            for (int filterY = -filterOffset; filterY <= filterOffset; ++filterY)
                            {
                                int calcYOffset = yRange.Clamp(y + filterY);

                                pixelValue += (source[calcYOffset, x, z] - rMiddle) * (source[calcYOffset, x, z] - rMiddle)
                                            * kernel1D[filterY + filterOffset];
                            }
                            tmp[y, x, z] = (float)pixelValue;
                        }
                    }
                    // Filtering along x-direction
                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            double pixelValue = 0.0;
                            for (int filterX = -filterOffset; filterX <= filterOffset; ++filterX)
                            {
                                int calcXOffset = xRange.Clamp(x + filterX);
                                pixelValue += tmp[y, calcXOffset, z] * kernel1D[filterX + filterOffset];
                            }
                            denominator[y, x, z] = (float)Math.Sqrt(pixelValue);
                            mean[z] += denominator[y, x, z];
                        }
                    }
                }
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                    mean[z] /= (source.Height * source.Width);
                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            result[y, x, z] = (float)r.Clamp((source[y, x, z] - rMiddle) / (Math.Max(mean[z], denominator[y, x, z]) * scaling) + rMiddle);
                        }
                    }
                }
                return result;
            }

            // For Gray, Lab, Luv and Yuv images the division of Gaussians for only the first color channel is calculated.
            else if (
                (source.Format.PixelType.Equals(PixelType.F32C1)
                && (source.Format.PixelChannels.Equals(PixelChannels.Gray) || source.Format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Lab)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Luv)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                var r = source.Format.ChannelRanges[0];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                mean[0] = 0.0;

                // Filtering along y-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterY = -filterOffset; filterY <= filterOffset; ++filterY)
                        {
                            int calcYOffset = yRange.Clamp(y + filterY);
                            pixelValue += (source[calcYOffset, x, 0] - rMiddle) * (source[calcYOffset, x, 0] - rMiddle)
                                        * kernel1D[filterY + filterOffset];
                        }
                        tmp[y, x, 0] = (float)pixelValue;
                    }
                }
                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterX = -filterOffset; filterX <= filterOffset; ++filterX)
                        {
                            int calcXOffset = xRange.Clamp(x + filterX);
                            pixelValue += tmp[y, calcXOffset, 0] * kernel1D[filterX + filterOffset];
                        }
                        denominator[y, x, 0] = (float)Math.Sqrt(pixelValue);
                        mean[0] += denominator[y, x, 0];
                    }
                }
                mean[0] /= (source.Height * source.Width);
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        result[y, x, 0] = (float)r.Clamp((source[y, x, 0] - rMiddle) / (Math.Max(mean[0], denominator[y, x, 0]) * scaling) + rMiddle);
                    }
                }
                return result;
            }
            // For Hsv images the division of Gaussians for only the third color channel is calculated.
            else if (
                source.Format.PixelType.Equals(PixelType.F32C3) && source.Format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                var r = source.Format.ChannelRanges[2];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                mean[2] = 0.0;

                // Filtering along y-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterY = -filterOffset; filterY <= filterOffset; ++filterY)
                        {
                            int calcYOffset = yRange.Clamp(y + filterY);
                            pixelValue += (source[calcYOffset, x, 2] - rMiddle) * (source[calcYOffset, x, 2] - rMiddle)
                                        * kernel1D[filterY + filterOffset];
                        }
                        tmp[y, x, 2] = (float)pixelValue;
                    }
                }
                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        double pixelValue = 0.0;
                        for (int filterX = -filterOffset; filterX <= filterOffset; ++filterX)
                        {
                            int calcXOffset = xRange.Clamp(x + filterX);
                            pixelValue += tmp[y, calcXOffset, 2] * kernel1D[filterX + filterOffset];
                        }
                        denominator[y, x, 2] = (float)Math.Sqrt(pixelValue);
                        mean[2] += denominator[y, x, 2];
                    }
                }
                mean[2] /= (source.Height * source.Width);
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        result[y, x, 2] = (float)r.Clamp((source[y, x, 2] - rMiddle) / (Math.Max(mean[2], denominator[y, x, 2]) * scaling) + rMiddle);
                    }
                }
                return result;
            }
            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        public static I<float> FastDivisionFilter(this I<float> source, double weight, double scaling)
        {
            I<float> tmp = source.CloneEmpty();
            I<float> result = source.CloneEmpty();
            I<float> denominator = source.Clone();
            double[] mean = new double[source.Channels];
            double input = 0.0, output = 0.0, out1 = 0.0, out2 = 0.0, out3 = 0.0;

            double q = 0.0;
            if (weight > 2.5)
                q = 0.98711 * weight - 0.96330;
            else if (weight <= 2.5 && weight >= 0.5)
                q = 3.97156 - 4.14554 * Math.Sqrt(1.0 - 0.26891 * weight);
            else
                throw new ArgumentException("Weight has to be >= 0.5.");

            double q2 = q * q;
            double q3 = q2 * q;
            double b0 = 1.57825 + 2.44413 * q + 1.4281 * q2 + 0.422205 * q3;
            double b1 = 2.44413 * q + 2.85619 * q2 + 1.26661 * q3;
            double b2 = -1.4281 * q2 - 1.26661 * q3;
            double b3 = 0.422205 * q3;

            double B = 1.0 - (b1 + b2 + b3) / b0;

            // For Rgb and Xyz images the division of Gaussians for all channels are calculated.
            if (
                source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Rgb)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Xyz)
                                                                || source.Format.PixelChannels.Equals(PixelChannels.Unknown))
            )
            {
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                    mean[z] = 0.0;

                    // Filtering along x-direction
                    for (int y = 0; y < source.Height; ++y)
                    {
                        // Forward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int x = 0; x < source.Width; ++x)
                        {
                            input = (source[y, x, z] - rMiddle) * (source[y, x, z] - rMiddle);
                            // Replication at the boundary
                            if (x == 0) { out1 = input; out2 = input; out3 = input; }
                            if (x == 1) { out2 = input; out3 = input; }
                            if (x == 2) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            tmp[y, x, z] = (float)output;
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                        // Backward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int x = source.Width - 1; x >= 0; --x)
                        {
                            input = tmp[y, x, z];
                            // Replication at the boundary
                            if (x == source.Width - 1) { out1 = input; out2 = input; out3 = input; }
                            if (x == source.Width - 2) { out2 = input; out3 = input; }
                            if (x == source.Width - 3) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            result[y, x, z] = (float)output;
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                    }
                    // Filtering along y-direction
                    for (int x = 0; x < source.Width; ++x)
                    {
                        // Forward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int y = 0; y < source.Height; ++y)
                        {
                            input = result[y, x, z];
                            // Replication at the boundary
                            if (y == 0) { out1 = input; out2 = input; out3 = input; }
                            if (y == 1) { out2 = input; out3 = input; }
                            if (y == 2) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            tmp[y, x, z] = (float)output;
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                        // Backward filtering
                        input = output = out1 = out2 = out3 = 0.0;
                        for (int y = source.Height - 1; y >= 0; --y)
                        {
                            input = tmp[y, x, z];
                            // Replication at the boundary
                            if (y == source.Height - 1) { out1 = input; out2 = input; out3 = input; }
                            if (y == source.Height - 2) { out2 = input; out3 = input; }
                            if (y == source.Height - 3) { out3 = input; }
                            output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                            denominator[y, x, z] = (float)Math.Sqrt(output < 0 ? 0 : output);
                            mean[z] += denominator[y, x, z];
                            out3 = out2;
                            out2 = out1;
                            out1 = output;
                        }
                    }
                }
                for (int z = 0; z < source.Channels; ++z)
                {
                    var r = source.Format.ChannelRanges[z];
                    double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                    mean[z] /= (source.Height * source.Width);
                    for (int y = 0; y < source.Height; ++y)
                    {
                        for (int x = 0; x < source.Width; ++x)
                        {
                            result[y, x, z] = (float)r.Clamp((source[y, x, z] - rMiddle) / (Math.Max(mean[z], denominator[y, x, z]) * scaling) + rMiddle);
                        }
                    }
                }
                return result;
            }
            // For Gray, Lab, Luv and Yuv images the division of Gaussians for only the first color channel is calculated.
            else if (
                (source.Format.PixelType.Equals(PixelType.F32C1) && (source.Format.PixelChannels.Equals(PixelChannels.Gray)
                                                                    || source.Format.PixelChannels.Equals(PixelChannels.Unknown)))
                || (source.Format.PixelType.Equals(PixelType.F32C3) && (source.Format.PixelChannels.Equals(PixelChannels.Lab)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Luv)
                                                                        || source.Format.PixelChannels.Equals(PixelChannels.Yuv)))
            )
            {
                var r = source.Format.ChannelRanges[0];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                mean[0] = 0.0;

                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = 0; x < source.Width; ++x)
                    {
                        input = (source[y, x, 0] - rMiddle) * (source[y, x, 0] - rMiddle);
                        // Replication at the boundary
                        if (x == 0) { out1 = input; out2 = input; out3 = input; }
                        if (x == 1) { out2 = input; out3 = input; }
                        if (x == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 0] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = source.Width - 1; x >= 0; --x)
                    {
                        input = tmp[y, x, 0];
                        // Replication at the boundary
                        if (x == source.Width - 1) { out1 = input; out2 = input; out3 = input; }
                        if (x == source.Width - 2) { out2 = input; out3 = input; }
                        if (x == source.Width - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        result[y, x, 0] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                // Filtering along y-direction
                for (int x = 0; x < source.Width; ++x)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = 0; y < source.Height; ++y)
                    {
                        input = result[y, x, 0];
                        // Replication at the boundary
                        if (y == 0) { out1 = input; out2 = input; out3 = input; }
                        if (y == 1) { out2 = input; out3 = input; }
                        if (y == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 0] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = source.Height - 1; y >= 0; --y)
                    {
                        input = tmp[y, x, 0];
                        // Replication at the boundary
                        if (y == source.Height - 1) { out1 = input; out2 = input; out3 = input; }
                        if (y == source.Height - 2) { out2 = input; out3 = input; }
                        if (y == source.Height - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        denominator[y, x, 0] = (float)Math.Sqrt(output < 0 ? 0 : output);
                        mean[0] += denominator[y, x, 0];
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                mean[0] /= (source.Height * source.Width);
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        result[y, x, 0] = (float)r.Clamp((source[y, x, 0] - rMiddle) / (Math.Max(mean[0], denominator[y, x, 0]) * scaling) + rMiddle);
                    }
                }
                return result;
            }
            // For Hsv images the division of Gaussians for only the third color channel is calculated.
            else if (
                source.Format.PixelType.Equals(PixelType.F32C3) && source.Format.PixelChannels.Equals(PixelChannels.Hsv)
            )
            {
                var r = source.Format.ChannelRanges[2];
                double rMiddle = r.Low + (r.High - r.Low) / 2.0;
                mean[2] = 0.0;

                // Filtering along x-direction
                for (int y = 0; y < source.Height; ++y)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = 0; x < source.Width; ++x)
                    {
                        input = (source[y, x, 2] - rMiddle) * (source[y, x, 2] - rMiddle);
                        // Replication at the boundary
                        if (x == 0) { out1 = input; out2 = input; out3 = input; }
                        if (x == 1) { out2 = input; out3 = input; }
                        if (x == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 2] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int x = source.Width - 1; x >= 0; --x)
                    {
                        input = tmp[y, x, 2];
                        // Replication at the boundary
                        if (x == source.Width - 1) { out1 = input; out2 = input; out3 = input; }
                        if (x == source.Width - 2) { out2 = input; out3 = input; }
                        if (x == source.Width - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        result[y, x, 2] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                // Filtering along y-direction
                for (int x = 0; x < source.Width; ++x)
                {
                    // Forward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = 0; y < source.Height; ++y)
                    {
                        input = result[y, x, 2];
                        // Replication at the boundary
                        if (y == 0) { out1 = input; out2 = input; out3 = input; }
                        if (y == 1) { out2 = input; out3 = input; }
                        if (y == 2) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        tmp[y, x, 2] = (float)output;
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                    // Backward filtering
                    input = output = out1 = out2 = out3 = 0.0;
                    for (int y = source.Height - 1; y >= 0; --y)
                    {
                        input = tmp[y, x, 2];
                        // Replication at the boundary
                        if (y == source.Height - 1) { out1 = input; out2 = input; out3 = input; }
                        if (y == source.Height - 2) { out2 = input; out3 = input; }
                        if (y == source.Height - 3) { out3 = input; }
                        output = B * input + (b1 * out1 + b2 * out2 + b3 * out3) / b0;
                        denominator[y, x, 2] = (float)Math.Sqrt(output < 0 ? 0 : output);
                        mean[2] += denominator[y, x, 2];
                        out3 = out2;
                        out2 = out1;
                        out1 = output;
                    }
                }
                mean[2] /= (source.Height * source.Width);
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        result[y, x, 2] = (float)r.Clamp((source[y, x, 2] - rMiddle) / (Math.Max(mean[2], denominator[y, x, 2]) * scaling) + rMiddle);
                    }
                }
                return result;
            }
            throw new Exception("Image format has to be GrayF32, LabF32, LuvF32, YuvF32, RgbF32, XyzF32, or HsvF32.");
        }

        public static I<float> CenteringAroundMean(this I<float> source)
        {
            I<float> result = source.CloneEmpty();
            double[] mean = new double[3];
            for (int z = 0; z < source.Channels; ++z)
            {
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        mean[z] += source[y, x, z];
                    }
                }
                mean[z] /= (source.Height * source.Width);
                var r = source.Format.ChannelRanges[z];
                double meanValOfRange = r.Low + (r.High - r.Low) / 2.0;
                for (int y = 0; y < source.Height; ++y)
                {
                    for (int x = 0; x < source.Width; ++x)
                    {
                        result[y, x, z] = (float)r.Clamp(source[y, x, z] + (meanValOfRange - mean[z]));
                    }
                }
            }
            return result;
        }

        public static I<float> Difference(this I<float> image1, I<float> image2)
        {
            if (image1.Format.ChannelCount != image1.Format.ChannelCount || image1.Height != image2.Height || image1.Width != image2.Width)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<float> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges.Select(x => new Range<float>((float)x.Low, (float)x.High)).ToArray();
            for (int y = 0; y < image1.Height; ++y)
            {
                for (int x = 0; x < image1.Width; ++x)
                {
                    for (int z = 0; z < image1.Channels; ++z)
                        result[y, x, z] = ranges[z].Clamp(image1[y, x, z] - image2[y, x, z]);
                }
            }
            return result;
        }

        public static I<float> Add(this I<float> image1, I<float> image2)
        {
            if (image1.Format.ChannelCount != image1.Format.ChannelCount || image1.Height != image2.Height || image1.Width != image2.Width)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<float> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges.Select(x => new Range<float>((float)x.Low, (float)x.High)).ToArray();
            for (int y = 0; y < image1.Height; ++y)
            {
                for (int x = 0; x < image1.Width; ++x)
                {
                    for (int z = 0; z < image1.Channels; ++z)
                        result[y, x, z] = ranges[z].Clamp(image1[y, x, z] + image2[y, x, z]);
                }
            }
            return result;
        }

        public static I<float> Multiply(this I<float> image1, I<float> image2)
        {
            if (image1.Format.ChannelCount != image1.Format.ChannelCount || image1.Height != image2.Height || image1.Width != image2.Width)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<float> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges.Select(x => new Range<float>((float)x.Low, (float)x.High)).ToArray();
            for (int y = 0; y < image1.Height; ++y)
            {
                for (int x = 0; x < image1.Width; ++x)
                {
                    for (int z = 0; z < image1.Channels; ++z)
                        result[y, x, z] = ranges[z].Clamp(image1[y, x, z] * image2[y, x, z]);
                }
            }
            return result;
        }

        public static I<float> Divide(this I<float> image1, I<float> image2)
        {
            if (image1.Format.ChannelCount != image1.Format.ChannelCount || image1.Height != image2.Height || image1.Width != image2.Width)
                throw new ArgumentException("Image1 and image2 have to be of same format and dimensions.");

            I<float> result = image1.CloneEmpty();
            var ranges = image1.Format.ChannelRanges.Select(x => new Range<float>((float)x.Low, (float)x.High)).ToArray();
            for (int y = 0; y < image1.Height; ++y)
            {
                for (int x = 0; x < image1.Width; ++x)
                {
                    for (int z = 0; z < image1.Channels; ++z)
                    {
                        result[y, x, z] = ranges[z].Clamp(image1[y, x, z] / image2[y, x, z]);
                    }
                }
            }
            return result;
        }

        // Subdivide image into tiles. (numTiles.X/numTiles[1] = number of tiles per column/row)
        public static I<float>[] Subdivide(this I<float> image, Int2 numTiles)
        {
            // Check if the image needs to be padded.
            // Padding occurs when image dimensions are not divisible by the selected number of tiles
            var colDiv = image.Width % numTiles.X;
            var rowDiv = image.Height % numTiles.Y;
            int[] tileDim = new int[2]; // width and height of each tile
            int padCol = 0, padRow = 0;
            if (colDiv != 0)
            {
                tileDim[0] = (int)Math.Floor((double)image.Width / numTiles.X) + 1;
                padCol = tileDim[0] * numTiles.X - image.Width;
            }
            else
                tileDim[0] = image.Width / numTiles.X;
            if (rowDiv != 0)
            {
                tileDim[1] = (int)Math.Floor((double)image.Height / numTiles.Y) + 1;
                padRow = tileDim[1] * numTiles.Y - image.Height;
            }
            else
                tileDim[1] = image.Height / numTiles.Y;

            int padRowPre = (int)Math.Floor(padRow / 2.0);
            int padRowPost = (int)Math.Ceiling(padRow / 2.0);
            int padColPre = (int)Math.Floor(padCol / 2.0);
            int padColPost = (int)Math.Ceiling(padCol / 2.0);
            I<float> paddedImage = image.AddBorder(padRowPre, padColPre, padRowPost, padColPost, BorderMode.Reflect);

            I<float>[] tiles = new I<float>[numTiles.X * numTiles.Y];
            int counter = 0;
            for (int row = 0; row < numTiles.Y * tileDim[1]; row += tileDim[1])
            {
                for (int col = 0; col < numTiles.X * tileDim[0]; col += tileDim[0])
                {
                    tiles[counter] = new I<float>(paddedImage.Format, tileDim[1], tileDim[0], paddedImage.Channels);
                    for (int y = 0; y < tileDim[1]; ++y)
                    {
                        for (int x = 0; x < tileDim[0]; ++x)
                        {
                            for (int z = 0; z < image.Channels; ++z)
                                tiles[counter][y, x, z] = paddedImage[row + y, col + x, z];
                        }
                    }
                    counter += 1;
                }
            }
            return tiles;
        }

        public static I<float>[] Subdivide(this I<float> image, Int2 winSize, Int2 winStride)
        {
            var numCols = (image.Width - (winSize.X - winStride.X)) / winStride.X;
            var numRows = (image.Height - (winSize.Y - winStride.Y)) / winStride.Y;
            var windows = new I<float>[numCols * numRows];

            var i = 0;
            for (int row = 0; row <= image.Height - winSize.Y; row += winStride.Y)
            {
                for (int col = 0; col <= image.Width - winSize.X; col += winStride.X)
                {
                    var rect = new IntRect(col, row, winSize.X, winSize.Y);
                    windows[i] = image.Crop(rect);
                    i++;
                }
            }

            return windows;
        }

        // Recombine tiles into image.
        public static I<float> Recombine(this I<float>[] tiles, int numTilesPerCol, int numTilesPerRow)
        {
            var format = tiles[0].Format; // format of each tile
            int height = tiles[0].Height; // height of each tile
            int width = tiles[0].Width;   // width of each tile
            int channels = tiles[0].Channels; // number of channels of each tile
            I<float> result = new I<float>(format, height * numTilesPerRow, width * numTilesPerCol, channels);

            for (int i = 0; i < numTilesPerRow; ++i)
            {
                for (int j = 0; j < numTilesPerCol; ++j)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            for (int z = 0; z < channels; ++z)
                            {
                                result[y + i * height, x + j * width, z] = tiles[i * numTilesPerCol + j][y, x, z];
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static I<T> SetChannel<T>(this I<T> image, I<T> channelImage, int channel)
        {
            if (image.Channels < channel)
                throw new ArgumentException("Wrong channel selected.", "channel");

            if (channelImage.Channels > 1)
                throw new ArgumentException("ChannelImage has to few channels.", "channelImage");

            for (int y = 0; y < image.Height; ++y)
            {
                for (int x = 0; x < image.Width; ++x)
                {
                    image[y, x, channel] = channelImage[y, x];
                }
            }

            return image;
        }

        public static I<T> GetChannel<T>(this I<T> image, int channel)
        {
            var targetFormat = new PixelFormat(
                (PixelType)(((int)image.Format.ChannelType) | 1),
                PixelChannels.Gray,
                image.Format.ElementType,
                new Range<double>[] { image.Format.ChannelRanges[channel] },
                image.Format.ColorSpace
            );

            var r = new I<T>(targetFormat, image.Height, image.Width);
            for (int y = 0; y < image.Height; ++y)
            {
                for (int x = 0; x < image.Width; ++x)
                {
                    r[y, x] = image[y, x, channel];
                }
            }
            return r;
        }

        public static I<float> Crop(this I<float> image, IntRect rect)
        {
            var output = new I<float>(image.Format, rect.Height, rect.Width);
            for (int dy = 0, sy = rect.Top; dy < rect.Height; ++dy, ++sy)
            {
                if (sy < 0 || sy >= image.Height)
                    continue;

                for (int dx = 0, sx = rect.Left; dx < rect.Width; ++dx, ++sx)
                {
                    if (dx < 0 || dx >= image.Width)
                        continue;

                    for (int c = 0; c < image.Channels; ++c)
                        output[dy, dx, c] = image[sy, sx, c];
                }
            }
            return output;
        }
    }
}
