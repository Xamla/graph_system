using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.OpenCv
{
    /// <summary>
    ///
    /// </summary>
    public class OpenCvWrapper
    {
        /// <summary>
        /// Performs a point-in-contour test.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#pointpolygontest"/>
        /// <param name="contour">Input contour.</param>
        /// <param name="pt">Point tested against the contour.</param>
        /// <param name="measureDist">If true, the function estimates the signed distance from the point to the nearest contour edge. Otherwise, the function only checks if the point is inside a contour or not.</param>
        /// <returns>
        /// <return name="result">If measure true, the function returns the signed distance from the point to the nearest contour edge. Otherwise, the function only checks if the point is inside a contour or not.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.PointPolygonTest")]
        public static Double PointPolygonTest(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Point> contour,
            [InputPin(PropertyMode = PropertyMode.Allow)] Point2f pt,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean measureDist)
        {
            return Cv2.PointPolygonTest(contour, pt, measureDist);
        }

        /// <summary>
        /// The function is used to detect translational shifts that occur between two images. The operation takes advantage of the Fourier shift theorem for detecting the translational shift in the frequency domain. It can be used for fast image registration as well as motion estimation. For more information please see http://en.wikipedia.org/wiki/Phase_correlation .
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/motion_analysis_and_object_tracking.html#phasecorrelate"/>
        /// <param name="src1">Source floating point array (CV_32FC1 or CV_64FC1)</param>
        /// <param name="src2">Source floating point array (CV_32FC1 or CV_64FC1)</param>
        /// <param name="window">Floating point array with windowing coefficients to reduce edge effects (optional).</param>
        /// <returns>
        /// <return name="pt">detected phase shift (sub-pixel) between the two arrays.</return>
        /// <return name="response">Signal power within the $5\times 5$ centroid around the peak, between 0 and 1 (optional).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.PhaseCorrelateRes")]
        public static Tuple<Point2d, Double> PhaseCorrelateRes(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat window = null)
        {
            Double response;
            var pt = Cv2.PhaseCorrelateRes(src1, src2, window == null ? new Mat() : window, out response);
            return Tuple.Create(pt, response);
        }

        /// <summary>
        /// Computes a Hanning window coefficients in two dimensions.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/motion_analysis_and_object_tracking.html#createhanningwindow"/>
        /// <param name="winSize">The window size specifications</param>
        /// <param name="dst"></param>
        /// <returns>
        /// <return name="dst">Hann coefficients.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CreateHanningWindow", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat CreateHanningWindow(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "3, 3")] Size winSize,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst = null
            )
        {
            var dstOutput = dst == null ? new Mat() : dst.Clone();
            Cv2.CreateHanningWindow(dstOutput, winSize, MatType.CV_64FC1);
            return dstOutput;
        }

        /// <summary>
        /// Applies a fixed-level threshold to each array element.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#threshold"/>
        /// <param name="src">Input array (single-channel, 8-bit or 32-bit floating point).</param>
        /// <param name="thresh">Threshold value.</param>
        /// <param name="maxval">Maximum value to use with the THRESH_BINARY and THRESH_BINARY_INV thresholding types.</param>
        /// <param name="type">Thresholding type.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// <return name="thresholdValue">The computed threshold value when $type == OTSU$.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Threshold", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Double> Threshold(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Double thresh = 32,
            [InputPin(PropertyMode = PropertyMode.Default)] Double maxval = 128,
            [InputPin(PropertyMode = PropertyMode.Default)] ThresholdTypes type = ThresholdTypes.Binary)
        {
            var dst = new Mat();
            var t = Cv2.Threshold(src, dst, thresh, maxval, type);
            return Tuple.Create(dst, t);
        }

        /// <summary>
        /// Applies an adaptive threshold to an array.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#adaptivethreshold"/>
        /// <param name="src">Source 8-bit single-channel image.</param>
        /// <param name="adaptiveMethod">Adaptive thresholding algorithm to use, ADAPTIVE_THRESH_MEAN_C or ADAPTIVE_THRESH_GAUSSIAN_C .</param>
        /// <param name="thresholdType">Thresholding type that must be either THRESH_BINARY or THRESH_BINARY_INV .</param>
        /// <param name="blockSize">Size of a pixel neighborhood that is used to calculate a threshold value for the pixel: 3, 5, 7, and so on.</param>
        /// <param name="c">Constant subtracted from the mean or weighted mean.
        /// Normally, it is positive but may be zero or negative as well.</param>
        /// <param name="maxValue">Non-zero value assigned to the pixels for which the condition is satisfied.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.AdaptiveThreshold", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat AdaptiveThreshold(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] AdaptiveThresholdTypes adaptiveMethod = AdaptiveThresholdTypes.GaussianC,
            [InputPin(PropertyMode = PropertyMode.Default)] ThresholdTypes thresholdType = ThresholdTypes.Binary,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 blockSize = 5,
            [InputPin(PropertyMode = PropertyMode.Default)] Double c = 5,
            [InputPin(PropertyMode = PropertyMode.Default)] Double maxValue = 255
        )
        {
            var dst = new Mat();
            Cv2.AdaptiveThreshold(src, dst, maxValue, adaptiveMethod, thresholdType, blockSize, c);
            return dst;
        }

        /// <summary>
        /// Blurs an image and downsamples it.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#pyrdown"/>
        /// <param name="src">Input image.</param>
        /// <param name="dstSize">Size of the output image; by default, it is computed as $Size((img.cols+1)/2$.</param>
        /// <param name="borderType">Pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.PyrDown", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat PyrDown(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Size? dstSize = null,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.PyrDown(src, dst, dstSize, borderType);
            return dst;
        }

        /// <summary>
        /// Upsamples an image and then blurs it.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#pyrup"/>
        /// <param name="src">Input image.</param>
        /// <param name="dstSize">Size of the output image; by default, it is computed as $Size(img.cols \cdot 2, (img.rows \cdot 2)$.</param>
        /// <param name="borderType">Pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.PyrUp", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat PyrUp(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Size? dstSize = null,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.PyrUp(src, dst, dstSize, borderType);
            return dst;
        }

        /// <summary>
        /// Calculates a histogram of a set of arrays.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/histograms.html#calchist"/>
        /// <param name="images">Source arrays. They all should have the same depth, CV_8U or CV_32F , and the same size. Each of them can have an arbitrary number of channels.</param>
        /// <param name="channels">List of the $dims$ channels used to compute the histogram. The first array channels are numerated from $0$ to $images[0].channels()-1$,
        /// the second array channels are counted from $images[0].channels()$ to $images[0].channels() + images[1].channels()-1$, and so on.</param>
        /// <param name="dims">Histogram dimensionality that must be positive and not greater than CV_MAX_DIMS (equal to 32 in the current OpenCV version).</param>
        /// <param name="histSize">Array of histogram sizes in each dimension.</param>
        /// <param name="ranges">Array of the dims arrays of the histogram bin boundaries in each dimension. When the histogram is uniform ($uniform=true$),
        /// then for each dimension $i$ it is enough to specify the lower (inclusive) boundary  $L_0$ of the 0-th histogram bin and the upper (exclusive) boundary
        /// $U_{\text{histSize}[i]-1}$ for the last histogram bin $histSize[i]-1$. That is, in case of a uniform histogram each of $ranges[i]$ is an array of 2 elements.
        /// When the histogram is not uniform ($uniform=false$), then each of $ranges[i]$ contains
        /// $histSize[i]+1$ elements: $L_0, U_0=L_1, U_1=L_2, ..., U_{\text{histSize[i]}-2}=L_{\text{histSize[i]}-1}, U_{\text{histSize[i]}-1}$.
        /// The array elements, that are not between $L_0$ and $U_{\text{histSize[i]}-1}$, are not counted in the histogram.</param>
        /// <param name="mask">Optional mask. If the matrix is not empty, it must be an 8-bit array of the same size as $images[i]$. The non-zero mask elements mark the array.</param>
        /// <param name="uniform">Flag indicating whether the histogram is uniform or not (see above).</param>
        /// <returns>
        /// <return name="hist">Output histogram, which is a dense or sparse $dims$-dimensional array.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CalcHist")]
        public static Mat CalcHist(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat[] images,
            [InputPin(PropertyMode = PropertyMode.Allow)] Int32[] channels,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dims,
            [InputPin(PropertyMode = PropertyMode.Allow)] Int32[] histSize,
            [InputPin(PropertyMode = PropertyMode.Allow)] Rangef[] ranges,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean uniform = true)
        {
            var hist = new Mat();
            Cv2.CalcHist(images, channels, mask == null ? new Mat() : mask, hist, dims, histSize, ranges, uniform, false);
            return hist;
        }

        /// <summary>
        /// Calculates a histogram for a array
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/histograms.html#calchist"/>
        /// <param name="image">Source array. CV_8U or CV_32F.</param>
        /// <param name="ranges">Array of the dims arrays of the histogram bin boundaries in each dimension. When the histogram is uniform ($uniform=true$),
        /// then for each dimension $i$ it is enough to specify the lower (inclusive) boundary  $L_0$ of the 0-th histogram bin and the upper (exclusive) boundary
        /// $U_{\text{histSize}[i]-1}$ for the last histogram bin $histSize[i]-1$. That is, in case of a uniform histogram each of $ranges[i]$ is an array of 2 elements.
        /// When the histogram is not uniform ($uniform=false$), then each of $ranges[i]$ contains
        /// $histSize[i]+1$ elements: $L_0, U_0=L_1, U_1=L_2, ..., U_{\text{histSize[i]}-2}=L_{\text{histSize[i]}-1}, U_{\text{histSize[i]}-1}$.
        /// The array elements, that are not between $L_0$ and $U_{\text{histSize[i]}-1}$, are not counted in the histogram.</param>
        /// <param name="channel">channel</param>
        /// <param name="dim">Histogram dimensionality that must be positive and not greater than CV_MAX_DIMS (equal to 32 in the current OpenCV version).</param>
        /// <param name="histSize">Histogram size.</param>
        /// <param name="mask">Optional mask.</param>
        /// <param name="uniform">Flag indicating whether the histogram is uniform or not (see above).</param>
        /// <returns>
        /// <return name="hist">Output histogram, which is a dense or sparse $dims$-dimensional array.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CalcSingleHist")]
        public static Mat CalcSingleHist(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] Rangef[] ranges,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 channel = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dim = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 histSize = 255,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean uniform = true
            )
        {
            var hist = new Mat();
            Mat[] images = { image };
            Int32[] channels = { channel };
            Int32[] histSizes = { histSize };
            Cv2.CalcHist(images, channels, mask == null ? new Mat() : mask, hist, dim, histSizes, ranges, uniform, false);
            return hist;
        }

        /// <summary>
        /// Computes the joint dense histogram for a set of images.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/histograms.html#calcbackproject"/>
        /// <param name="images">Source arrays. They all should have the same depth, CV_8U or CV_32F , and the same size. Each of them can have an arbitrary number of channels.</param>
        /// <param name="channels">The list of channels used to compute the back projection. The number of channels must match the histogram dimensionality. The first array channels are numerated from $0$ to $images[0].channels()-1$ , the second array channels are counted from $images[0].channels() to images[0].channels() + images[1].channels()-1$, and so on.</param>
        /// <param name="hist">Input histogram that can be dense or sparse.</param>
        /// <param name="ranges"> Array of arrays of the histogram bin boundaries in each dimension. See $calcHist()$.</param>
        /// <param name="uniform">Flag indicating whether the histogram is uniform or not (see above).</param>
        /// <returns>
        /// <return name="backProject">Destination back projection array that is a single-channel array of the same size and depth as $images[0]$.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CalcBackProject", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat CalcBackProject(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat[] images,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32[] channels,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat hist,
            [InputPin(PropertyMode = PropertyMode.Allow)] Rangef[] ranges,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean uniform = true)
        {
            var backProject = new Mat();
            Cv2.CalcBackProject(images, channels, hist, backProject, ranges, uniform);
            return backProject;
        }

        /// <summary>
        /// Compares two histograms stored in dense arrays
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/histograms.html#comparehist"/>
        /// <param name="h1">The first compared histogram</param>
        /// <param name="h2">The second compared histogram of the same size as h1</param>
        /// <param name="method">The comparison method</param>
        /// <returns>
        /// <return name="dist">Distance between input histograms.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CompareHist")]
        public static Double CompareHist(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat h1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat h2,
            [InputPin(PropertyMode = PropertyMode.Default)] HistCompMethods method)
        {
            return Cv2.CompareHist(h1, h2, method);
        }

        /// <summary>
        /// Normalizes the grayscale image brightness and contrast by normalizing its histogram.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/histograms.html#equalizehist"/>
        /// <param name="src">The source 8-bit single channel image.</param>
        /// <returns>
        /// <return name="dst">Equalized image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.EqualizeHist", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat EqualizeHist(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src)
        {
            var dst = new Mat();
            Cv2.EqualizeHist(src, dst);
            return dst;
        }

        /// <summary>
        /// Computes the “minimal work” distance between two weighted point configurations.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/histograms.html#emd"/>
        /// <param name="signature1">First signature, a  $\text{size1}\times \text{dims}+1$ floating-point matrix. Each row stores the point weight followed by the point coordinates. The matrix is allowed to have a single column (weights only) if the user-defined cost matrix is used.</param>
        /// <param name="signature2">Second signature of the same format as signature1 , though the number of rows may be different. The total weights may be different. In this case an extra “dummy” point is added to either signature1 or signature2 </param>
        /// <param name="distType">Used metric. CV_DIST_L1, CV_DIST_L2 , and CV_DIST_C stand for one of the standard metrics. CV_DIST_USER means that a pre-calculated cost matrix cost is used.</param>
        /// <param name="cost">User-defined  $\text{size1}\times \text{size2}$ cost matrix. Also, if a cost matrix is used, lower boundary lowerBound cannot be calculated because it needs a metric function.</param>
        /// <param name="lowerBound">If the calculated distance between mass centers is greater or equal to lowerBound (it means that the signatures are far enough), the function does not calculate EMD. In any case lowerBound is set to the calculated distance between mass centers on return. Thus, if you want to calculate both distance between mass centers and EMD, lowerBound should be set to 0.</param>
        /// <returns>
        /// <return name="flow"></return>
        /// <return name="lowerBound">lower boundary of a distance between the two signatures that is a distance between mass centers.</return>
        /// <return name="response"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.EMD")]
        public static Tuple<Mat, Single, Single> EMD(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat signature1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat signature2,
            [InputPin(PropertyMode = PropertyMode.Default)] DistanceTypes distType,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat cost = null,
            [InputPin(PropertyMode = PropertyMode.Default)] float lowerBound = 0)
        {
            var flow = new Mat();
            float lowerBoundOut = lowerBound;
            var response = Cv2.EMD(signature1, signature2, distType, cost == null ? new Mat() : cost, out lowerBoundOut, flow);
            return Tuple.Create(flow, lowerBoundOut, response);
        }

        /// <summary>
        /// Performs a marker-based image segmentation using the watershed algorithm.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#watershed"/>
        /// <param name="image">Input 8-bit 3-channel image.</param>
        /// <param name="markers">Input 32-bit single-channel image (map) of markers.
        /// It should have the same size as image.</param>
        /// <returns>
        /// <return name="markers">Output 32-bit single-channel image (map) of markers.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Watershed", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Watershed(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat markers)
        {
            var dst = markers.Clone();
            Cv2.Watershed(image, dst);
            return dst;
        }

        /// <summary>
        /// Performs initial step of meanshift segmentation of an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#pyrmeanshiftfiltering"/>
        /// <param name="src">The source 8-bit, 3-channel image.</param>
        /// <param name="sp">The spatial window radius.</param>
        /// <param name="sr">The color window radius.</param>
        /// <param name="maxLevel">Maximum level of the pyramid for the segmentation.</param>
        /// <param name="termcrit">Termination criteria: when to stop meanshift iterations.</param>
        /// <returns>
        /// <return name="dst">The destination image of the same format and the same size as the source.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.PyrMeanShiftFiltering", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat PyrMeanShiftFiltering(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Double sp,
            [InputPin(PropertyMode = PropertyMode.Default)] Double sr,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxLevel = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] TermCriteria? termcrit = null)
        {
            var dst = new Mat();
            Cv2.PyrMeanShiftFiltering(src, dst, sp, sr, maxLevel, termcrit);
            return dst;
        }

        ////TODO: delete? not working
        ///// <summary>
        ///// Segments the image using GrabCut algorithm
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#grabcut"/>
        ///// <param name="img">Input 8-bit 3-channel image.</param>
        ///// <param name="mask">Input 8-bit single-channel mask.
        ///// The mask is initialized by the function when mode is set to GC_INIT_WITH_RECT.
        ///// Its elements may have Cv2.GC_BGD / Cv2.GC_FGD / Cv2.GC_PR_BGD / Cv2.GC_PR_FGD</param>
        ///// <param name="rect">ROI containing a segmented object. The pixels outside of the ROI are
        ///// marked as "obvious background". The parameter is only used when mode==GC_INIT_WITH_RECT.</param>
        ///// <param name="bgdModel">Temporary array for the background model. Do not modify it while you are processing the same image.</param>
        ///// <param name="fgdModel">Temporary arrays for the foreground model. Do not modify it while you are processing the same image.</param>
        ///// <param name="iterCount">Number of iterations the algorithm should make before returning the result.
        ///// Note that the result can be refined with further calls with mode==GC_INIT_WITH_MASK or mode==GC_EVAL .</param>
        ///// <param name="mode">Operation mode that could be one of GrabCutFlag value.</param>
        ///// <returns>
        ///// <return name="mask">Output 8-bit single-channel mask.</return>
        ///// <return name="bgdModel">Background model</return>
        ///// <return name="fgdModel">Foreground model</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.ImgProc.GrabCut", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Tuple<Mat, Mat, Mat> GrabCut(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Rect rect,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat bgdModel,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat fgdModel,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Int32 iterCount,
        //    [InputPin(PropertyMode = PropertyMode.Default)] GrabCutFlag mode)
        //{
        //    var maskOutput = new Mat(); // mask.Clone();
        //    var bgdModelOutput = new Mat();// bgdModel.Clone();
        //    var fgdModelOutput = new Mat();// fgdModel.Clone();
        //    Cv2.GrabCut(img, maskOutput, rect, bgdModelOutput, fgdModelOutput, iterCount, mode);
        //    return Tuple.Create(maskOutput, bgdModelOutput, fgdModelOutput); //TODO
        //}


        /// <summary>
        /// builds the discrete Voronoi diagram
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#distancetransform"/>
        /// <param name="src">8-bit, single-channel (binary) source image.</param>
        /// <param name="distanceType"> Type of distance. It can be CV_DIST_L1, CV_DIST_L2 , or CV_DIST_C .</param>
        /// <param name="maskSize">Size of the distance transform mask. It can be 3, 5, or CV_DIST_MASK_PRECISE (the latter option is only supported by the first function). In case of the CV_DIST_L1 or CV_DIST_C distance type, the parameter is forced to 3 because a  3\times 3 mask gives the same result as  5\times 5 or any larger aperture.</param>
        /// <param name="labelType">Type of the label array to build. If labelType==DIST_LABEL_CCOMP then each connected component of zeros in src (as well as all the non-zero pixels closest to the connected component) will be assigned the same label. If labelType==DIST_LABEL_PIXEL then each zero pixel (and all the non-zero pixels closest to it) gets its own label.</param>
        /// <returns>
        /// <return name="dst">Output image with calculated distances. It is a 32-bit floating-point, single-channel image of the same size as src </return>
        /// <return name="labels">Optional output 2D array of labels (the discrete Voronoi diagram). It has the type CV_32SC1 and the same size as src . See the details below.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.DistanceTransformWithLabels")]
        public static Tuple<Mat, Mat> DistanceTransformWithLabels(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] DistanceTypes distanceType,
            [InputPin(PropertyMode = PropertyMode.Default)] DistanceMaskSize maskSize = DistanceMaskSize.Mask3,
            [InputPin(PropertyMode = PropertyMode.Default)] DistanceTransformLabelTypes labelType = DistanceTransformLabelTypes.CComp
            )
        {
            var dst = new Mat();
            var labels = new Mat();
            Cv2.DistanceTransformWithLabels(src, dst, labels, distanceType, maskSize, labelType);
            return Tuple.Create(dst, labels);
        }


        /// <summary>
        /// computes the distance transform map
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#distancetransform"/>
        /// <param name="src">8-bit, single-channel (binary) source image.</param>
        /// <param name="distanceType"> Type of distance. It can be CV_DIST_L1, CV_DIST_L2 , or CV_DIST_C .</param>
        /// <param name="maskSize">Size of the distance transform mask. It can be 3, 5, or CV_DIST_MASK_PRECISE (the latter option is only supported by the first function). In case of the CV_DIST_L1 or CV_DIST_C distance type, the parameter is forced to 3 because a  3\times 3 mask gives the same result as  5\times 5 or any larger aperture.</param>
        /// <returns>
        /// <return name="dst">Output image with calculated distances. It is a 32-bit floating-point, single-channel image of the same size as src </return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.DistanceTransform", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DistanceTransform(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] DistanceTypes distanceType,
            [InputPin(PropertyMode = PropertyMode.Default)] DistanceMaskSize maskSize
            )
        {
            var dst = new Mat();
            Cv2.DistanceTransform(src, dst, distanceType, maskSize);
            return dst;
        }

        /// <summary>
        /// Fills a connected component with the given color.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#floodfill"/>
        /// <param name="image">Input 1- or 3-channel, 8-bit, or floating-point image.</param>
        /// <param name="seedPoint">Starting point.</param>
        /// <param name="newVal">New value of the repainted domain pixels.</param>
        /// <param name="loDiff">Maximal lower brightness/color difference between the currently
        /// observed pixel and one of its neighbors belonging to the component, or a seed pixel
        /// being added to the component.</param>
        /// <param name="upDiff">Maximal upper brightness/color difference between the currently
        /// observed pixel and one of its neighbors belonging to the component, or a seed pixel
        /// being added to the component.</param>
        /// <param name="flags">Operation flags. Lower bits contain a connectivity value,
        /// 4 (default) or 8, used within the function. Connectivity determines which
        /// neighbors of a pixel are considered. </param>
        /// <returns>
        /// <return name="img">Output 1- or 3-channel, 8-bit, or floating-point image.</return>
        /// <return name="int">Number of repainted pixels.</return>
        /// <return name="rect">Minimum bounding rectangle of the repainted domain.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.FloodFill", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Int32, Rect> FloodFill(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] Point seedPoint,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "255, 0, 0, 0")] Scalar newVal,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? loDiff = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? upDiff = null,//5, 10, 10, 5",
            [InputPin(PropertyMode = PropertyMode.Default)] FloodFillFlags flags = FloodFillFlags.Link4)
        {
            var rect = new Rect();
            var dst = image.Clone();
            Int32 tuu = Cv2.FloodFill(dst, seedPoint, newVal, out rect, loDiff, upDiff, flags);
            return Tuple.Create(dst, tuu, rect);
        }

        /// <summary>
        /// Converts image from one color space to another
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#cvtcolor"/>
        /// <param name="src">The input image, 8-bit unsigned, 16-bit unsigned or single-precision floating-point.</param>
        /// <param name="code">The color space conversion code.</param>
        /// <param name="dstCn">The number of channels in the output image; if the parameter is 0, the number of the channels will be derived automatically from input image and the code</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CvtColor", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat CvtColor(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] ColorConversionCodes code = ColorConversionCodes.RGB2GRAY,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dstCn = 1
            )
        {
            var dst = new Mat();
            Cv2.CvtColor(src, dst, code, dstCn);
            return dst;
        }

        /// <summary>
        /// Calculates all of the moments up to the third order of a polygon or rasterized shape.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#moments"/>
        /// <param name="array"> Raster image (single-channel, 8-bit or floating-point 2D array) or an array ($1 \times N$ or $N \times 1$) of 2D points ($Point$ or $Point2f$).</param>
        /// <param name="binaryImage">If it is true, all non-zero image pixels are treated as 1’s. The parameter is used for images only.</param>
        /// <returns>
        /// <return name="moments">Output moments.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Moments")]
        public static Moments Moments(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat array,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean binaryImage = false)
        {
            return Cv2.Moments(array, binaryImage);
        }

        // TODO: as converter
        ///// <summary>
        ///// moments to CvHuMoments
        ///// </summary>
        ///// <param name="moments"></param>
        ///// <returns>
        ///// <return name="cvHuMoments"></return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.ImgProc.HuMoments", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static CvHuMoments HuMoments(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Moments moments
        //    )
        //{
        //    var huMoments = new CvHuMoments();
        //    var cvMoments = (CvMoments)moments;
        //    cvMoments.GetHuMoments(out huMoments);
        //    return huMoments;
        //}

        /// <summary>
        /// Computes the proximity map for the raster template and the image where the template is searched for
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/object_detection.html#matchtemplate"/>
        /// <param name="image">Image where the search is running; should be 8-bit or 32-bit floating-point.</param>
        /// <param name="templ">Searched template; must be not greater than the source image and have the same data type.</param>
        /// <param name="method">Specifies the comparison method.</param>
        /// <returns>
        /// <return name="result">A map of comparison results; will be single-channel 32-bit floating-point.
        /// If image is $W\times H$ and templ is $w \times h$ then result will be $(W-w+1) \times (H-h+1)$.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.MatchTemplate", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat MatchTemplate(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat templ,
            [InputPin(PropertyMode = PropertyMode.Default)] TemplateMatchModes method = TemplateMatchModes.CCoeff)
        {
            var result = new Mat();
            Cv2.MatchTemplate(image, templ, result, method);
            return result;
        }

        /// <summary>
        /// Finds contours in a binary image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#findcontours"/>
        /// <param name="image">Source, an 8-bit single-channel image. Non-zero pixels are treated as 1’s. Zero pixels remain 0’s, so the image is treated as binary . You can use compare() , inRange() , threshold() , adaptiveThreshold() , Canny() , and others to create a binary image out of a grayscale or color one. The function modifies the image while extracting the contours.</param>
        /// <param name="mode"></param>
        /// <param name="method"></param>
        /// <param name="offset"></param>
        /// <returns>
        /// <return name="image">Output image.</return>
        /// <return name="contours">Detected contours. Each contour is stored as a vector of points.</return>
        /// <return name="hierachy">Output vector, containing information about the image topology. It has as many elements as the number of contours. For each i-th contour contours[i] , the elements hierarchy[i][0] , hiearchy[i][1] , hiearchy[i][2] , and hiearchy[i][3] are set to 0-based indices in contours of the next and previous contours at the same hierarchical level, the first child contour and the parent contour, respectively. If for the contour i there are no next, previous, parent, or nested contours, the corresponding elements of hierarchy[i] will be negative.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.FindContours")]
        public static Tuple<Mat, Mat[], Mat> FindContours(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] RetrievalModes mode = RetrievalModes.CComp,
            [InputPin(PropertyMode = PropertyMode.Default)] ContourApproximationModes method = ContourApproximationModes.ApproxSimple,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? offset = null)
        {
            var imageOutput = image.Clone();
            Mat[] contours;
            var hiearchy = new Mat();
            Cv2.FindContours(imageOutput, out contours, hiearchy, mode, method, offset);
            return Tuple.Create(imageOutput, contours, hiearchy);
        }


        /// <summary>
        /// Draws contours outlines or filled contours.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#drawcontours"/>
        /// <param name="image">Input Image.</param>
        /// <param name="contours">All the input contours. Each contour is stored as a point vector.</param>
        /// <param name="color">Color of the contours. If any part of the Scalar is negativ, a random color will be used.</param>
        /// <param name="contourIdx">Parameter indicating a contour to draw. If it is negative, all the contours are drawn.</param>
        /// <param name="thickness">Thickness of lines the contours are drawn with. If it is negative (for example, $thickness=CV_{FILLED}$), the contour interiors are drawn.</param>
        /// <param name="lineType">Line connectivity. See $line()$ for details.</param>
        /// <param name="hierarchy">Optional information about hierarchy. It is only needed if you want to draw only some of the contours (see $maxLevel$).</param>
        /// <param name="maxLevel">Maximal level for drawn contours. If it is 0, only the specified contour is drawn. If it is 1, the function draws the contour(s) and all the nested contours. If it is 2, the function draws the contours, all the nested contours, all the nested-to-nested contours, and so on. This parameter is only taken into account when there is $hierarchy$ available.</param>
        /// <param name="offset">Optional contour shift parameter. Shift all the drawn contours by the specified  $\text{offset}=(dx,dy)$ .</param>
        /// <returns>
        /// <return name="dst">Output Image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.DrawContours", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DrawContours(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat[] contours,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 contourIdx = -1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 thickness = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] LineTypes lineType = LineTypes.Link8,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat hierarchy = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxLevel = Int32.MaxValue,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? offset = null)
        {
            var imageOutput = image.Clone();
            if (imageOutput.Type().Channels == 1)
                imageOutput = image.CvtColor(ColorConversionCodes.GRAY2BGR);

            Scalar c = color;
            var rndColor = c.Val0 < 0 || c.Val1 < 0 || c.Val2 < 0 || c.Val3 < 0;
            if (rndColor)
                c = Scalar.RandomColor();

            if (contourIdx < 0)
            {
                for (int i = 0; i < contours.Count(); i++)
                {
                    if (rndColor)
                        c = Scalar.RandomColor();

                    Cv2.DrawContours(imageOutput, contours, i, c, thickness, lineType, hierarchy, 0, offset);
                }
            }
            else
            {
                Cv2.DrawContours(imageOutput, contours, contourIdx, c, thickness, lineType, hierarchy, maxLevel, offset);
            }
            return imageOutput;
        }

        /// <summary>
        /// Approximates contour or a curve using Douglas-Peucker algorithm
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#approxpolydp"/>
        /// <param name="curve">The polygon or curve to approximate.
        /// Must be $1 \times N$ or $N \times 1$ matrix of type CV_32SC2 or CV_32FC2.</param>
        /// <param name="epsilon">Specifies the approximation accuracy.
        /// This is the maximum distance between the original curve and its approximation.</param>
        /// <param name="closed">The result of the approximation;
        /// The type should match the type of the input curve</param>
        /// <returns>
        /// <return name="approxCurve">The result of the approximation;
        /// The type should match the type of the input curve</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.ApproxPolyDP", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat ApproxPolyDP(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat curve,
            [InputPin(PropertyMode = PropertyMode.Default)] Double epsilon = 1e-5,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean closed = false)
        {
            var approxCurve = new Mat();
            Cv2.ApproxPolyDP(curve, approxCurve, epsilon, closed);
            return approxCurve;
        }

        /// <summary>
        /// Calculates a contour perimeter or a curve length.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#arclength"/>
        /// <param name="curve">Input vector of 2D points, stored in Mat.</param>
        /// <param name="closed">Flag indicating whether the curve is closed or not.</param>
        /// <returns>
        /// <return name="arc">a curve length or a closed contour perimeter.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.ArcLength")]
        public static Double ArcLength(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat curve,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean closed = false)
        {
            return Cv2.ArcLength(curve, closed);
        }

        /// <summary>
        /// Calculates the up-right bounding rectangle of a point set.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html?highlight=arclength#boundingrect"/>
        /// <param name="curve"> Input 2D point set, stored in Mat.</param>
        /// <returns>
        /// <return name="rect">minimal up-right bounding rectangle for the specified point set.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.BoundingRect")]
        public static Rect BoundingRect(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat curve
            )
        {
            return Cv2.BoundingRect(curve);
        }

        /// <summary>
        /// Calculates a contour area.
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html?#contourarea"></reference>
        /// <param name="contour">Input vector of 2D points (contour vertices), stored in Mat.</param>
        /// <param name="oriented">Oriented area flag. If it is true, the function returns a signed area value, depending on the contour orientation (clockwise or counter-clockwise). Using this feature you can determine orientation of a contour by taking the sign of an area. By default, the parameter is false, which means that the absolute value is returned.</param>
        /// <returns>
        /// <return name="area">Area of the contour.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.ContourArea")]
        public static Double ContourArea(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat contour,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean oriented = false
            )
        {
            return Cv2.ContourArea(contour, oriented);
        }

        /// <summary>
        /// Finds a rotated rectangle of the minimum area enclosing the input 2D point set.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#minarearect"/>
        /// <param name="points">Input vector of 2D points, stored in Mat.</param>
        /// <returns>
        /// <return name="boundingRect">The minimum-area bounding rectangle (possibly rotated) for a specified point set.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.MinAreaRect")]
        public static RotatedRect MinAreaRect(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat points)
        {
            return Cv2.MinAreaRect(points);
        }

        /// <summary>
        /// Finds a rotated rectangle of the minimum area enclosing the input 2D point set.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#minenclosingcircle"/>
        /// <param name="points">Input vector of 2D points, stored in Mat. </param>
        /// <returns>
        /// <return name="center">Center point of enclosing circle.</return>
        /// <return name="radius">Radius of enclosing circle.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.MinEnclosingCircle")]
        public static Tuple<Point2f, Single> MinEnclosingCircle(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat points)
        {
            Point2f center;
            Single radius;
            Cv2.MinEnclosingCircle(points, out center, out radius);
            return Tuple.Create(center, radius);
        }

        /// <summary>
        /// Compares two shapes. All three implemented methods use the Hu invariants.
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#matchshapes" ></reference>
        /// <param name="contour1">First contour or grayscale image.</param>
        /// <param name="contour2">Second contour or grayscale image.</param>
        /// <param name="method">Comparison method: CV_CONTOURS_MATCH_I1 , CV_CONTOURS_MATCH_I2 or CV_CONTOURS_MATCH_I3 (see the details below).</param>
        /// <returns>
        /// <return name="distance">Distance between the contours</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.MatchShapes")]
        public static Double MatchShapes(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat contour1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat contour2,
            [InputPin(PropertyMode = PropertyMode.Default)] ShapeMatchModes method = ShapeMatchModes.I1
            )
        {
            return Cv2.MatchShapes(contour1, contour2, method, 0);
        }

        /// <summary>
        /// Computes convex hull for a set of 2D points.
        /// </summary>
        /// <referene href="http://docs.opencv.org/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#convexhull"/>
        /// <param name="points">The input 2D point set, represented by CV_32SC2 or CV_32FC2 matrix</param>
        /// <param name="clockwise">If true, the output convex hull will be oriented clockwise,
        /// otherwise it will be oriented counter-clockwise. Here, the usual screen coordinate
        /// system is assumed - the origin is at the top-left corner, x axis is oriented to the right,
        /// and y axis is oriented downwards.</param>
        /// <param name="returnPoints"></param>
        /// <returns>
        /// <return name="hull">The output convex hull. It is a vector of points that form the
        /// hull (must have the same type as the input points).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.ConvexHull", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat ConvexHull(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat points,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean clockwise = false,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean returnPoints = true
            )
        {
            var hull = new Mat();
            Cv2.ConvexHull(points, hull, clockwise, returnPoints);
            return hull;
        }

        //TODO neccessary?

        /// <summary>
        /// Computes convex hull for a set of 2D points.
        /// </summary>
        /// <param name="points">The input 2D point set, represented by CV_32SC2 or CV_32FC2 matrix</param>
        /// <param name="clockwise">If true, the output convex hull will be oriented clockwise, otherwise it
        ///     will be oriented counter-clockwise. Here, the usual screen coordinate system
        ///     is assumed - the origin is at the top-left corner, x axis is oriented to
        ///     the right, and y axis is oriented downwards.</param>
        /// <returns>
        /// <return name="indicies">The output convex hull. It is a vector of 0-based point indices of the hull
        ///     points in the original array (since the set of convex hull points is a subset
        ///     of the original point set</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.ConvexHullIndices")]
        public static Int32[] ConvexHullIndices(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Point> points,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean clockwise = false
            )
        {
            return Cv2.ConvexHullIndices(points, clockwise);
        }

        /// <summary>
        /// Computes the contour convexity defects
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#convexitydefects"/>
        /// <param name="contour">Input contour.</param>
        /// <param name="convexHull">Convex hull obtained using convexHull() that
        /// should contain indices of the contour points that make the hull.</param>
        /// <returns>
        /// <return name="convexityDefects">
        /// The output vector of convexity defects.
        /// Each convexity defect is represented as 4-element integer vector
        /// (a.k.a. cv::Vec4i): (start_index, end_index, farthest_pt_index, fixpt_depth),
        /// where indices are 0-based indices in the original contour of the convexity defect beginning,
        /// end and the farthest point, and fixpt_depth is fixed-point approximation
        /// (with 8 fractional bits) of the distance between the farthest contour point and the hull.
        /// That is, to get the floating-point value of the depth will be fixpt_depth/256.0.
        /// </return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.ConvexityDefects", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat ConvexityDefects(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat contour,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat convexHull
            )
        {
            var convexityDefects = new Mat();
            Cv2.ConvexityDefects(contour, convexHull, convexityDefects);
            return convexityDefects;
        }


        /// <summary>
        /// Tests a contour convexity.
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#iscontourconvex"/>
        /// <param name="contour">Input vector of 2D points</param>
        /// <returns>
        /// <return name="convex">True if countour is convex.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.IsContourConvex")]
        public static Boolean IsContourConvex(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat contour
            )
        {
            return Cv2.IsContourConvex(contour);
        }

        // TODO found no docu

        /// <summary>
        /// finds intersection of two convex polygons
        /// </summary>
        /// <param name="p1">First convex polgons.</param>
        /// <param name="p2">Second convex polgons.</param>
        /// <param name="handleNested"></param>
        /// <returns>
        /// <return name="intersection">Resulting convex polygon.</return>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.IntersectConvexConvex", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Single> IntersectConvexConvex(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat p1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat p2,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean handleNested = true)
        {
            var p12 = new Mat();
            var result = Cv2.IntersectConvexConvex(p1, p2, p12, handleNested);
            return Tuple.Create(p12, result);
        }

        /// <summary>
        /// Fits an ellipse around a set of 2D points.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#fitellipse"/>
        /// <param name="points">Input 2D point set, stored in Mat.</param>
        /// <returns>
        /// <return name="elipse">Elipse as a rotated Rectangle.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.FitEllipse")]
        public static RotatedRect FitEllipse(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat points
            )
        {
            return Cv2.FitEllipse(points);
        }

        /// <summary>
        /// Fits line to the set of 2D points using M-estimator algorithm
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/structural_analysis_and_shape_descriptors.html#fitline"/>
        /// <param name="points">Input vector of 2D or 3D points</param>
        /// <param name="distType">Distance used by the M-estimator</param>
        /// <param name="param">Numerical parameter ($C$) for some types of distances.
        /// If it is $0$, an optimal value is chosen.</param>
        /// <param name="reps">Sufficient accuracy for the radius
        /// (distance between the coordinate origin and the line).</param>
        /// <param name="aeps">Sufficient accuracy for the angle.
        /// $0.01$ would be a good default value for reps and aeps.</param>
        /// <returns>
        /// <return name="line">Output line parameters.
        /// In case of 2D fitting, it is a vector of 4 elements
        /// (like Vec4f) - (vx, vy, x0, y0), where (vx, vy) is a normalized vector
        /// collinear to the line and (x0, y0) is a point on the line.
        /// In case of 3D fitting, it should be a vector of 6 elements
        /// (like Vec6f) - (vx, vy, vz, x0, y0, z0), where (vx, vy, vz) is a
        /// normalized vector collinear to the line and (x0, y0, z0) is a point on the line.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.FitLine", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FitLine(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat points,
            [InputPin(PropertyMode = PropertyMode.Default)] DistanceTypes distType = DistanceTypes.C,
            [InputPin(PropertyMode = PropertyMode.Default)] Double param = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Double reps = 0.01,
            [InputPin(PropertyMode = PropertyMode.Default)] Double aeps = 0.01
            )
        {
            var line = new Mat();
            Cv2.FitLine(points, line, distType, param, reps, aeps);
            return line;
        }

        /// <summary>
        /// Returns a structuring element of the specified size and shape for morphological operations.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#getstructuringelement"/>
        /// <param name="ksize">Size of the structuring element.</param>
        /// <param name="shape">Element shape that could be one of the following: MORPH_RECT, MORPH_ELLIPSE, MORPH_CROSS, CV_SHAPE_CUSTOM</param>
        /// <returns>
        /// <return name="structuring">The structuring element.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.GetStructuringElement", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat GetStructuringElement(
            [InputPin(PropertyMode = PropertyMode.Allow, DefaultValue = "3, 3")] Size ksize,
            [InputPin(PropertyMode = PropertyMode.Default)] MorphShapes shape = MorphShapes.Rect)
        {
            return Cv2.GetStructuringElement(shape, ksize);
        }

        /// <summary>
        /// Forms a border around the image
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/imgproc/doc/filtering.html"/>
        /// <param name="src">The source image</param>
        /// <param name="top">Specify how much pixels in each direction from the source image rectangle
        ///     one needs to extrapolate</param>
        /// <param name="bottom">Specify how much pixels in each direction from the source image rectangle
        ///     one needs to extrapolate</param>
        /// <param name="left">Specify how much pixels in each direction from the source image rectangle
        ///     one needs to extrapolate</param>
        /// <param name="right">Specify how much pixels in each direction from the source image rectangle
        ///     one needs to extrapolate</param>
        /// <param name="borderType">The border type</param>
        /// <param name="value">The border value if $borderType == Constant$</param>
        /// <returns>
        /// <return name="dst"> The destination image; will have the same type as src and the size $Size(src.cols+left+right,
        ///     src.rows+top+bottom)$</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CopyMakeBorder", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat CopyMakeBorder(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] int top,
            [InputPin(PropertyMode = PropertyMode.Default)] int bottom,
            [InputPin(PropertyMode = PropertyMode.Default)] int left,
            [InputPin(PropertyMode = PropertyMode.Default)] int right,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? value = null
            )
        {
            var dst = new Mat();
            Cv2.CopyMakeBorder(src, dst, top, bottom, left, right, borderType, value);
            return dst;
        }


        /// <summary>
        /// Smoothes image using median filter.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#medianblur"/>
        /// <param name="src">The source 1-, 3- or 4-channel image.
        /// When ksize is 3 or 5, the image depth should be CV_8U , CV_16U or CV_32F.
        /// For larger aperture sizes it can only be CV_8U.</param>
        /// <param name="ksize">The aperture linear size. It must be odd and more than 1, i.e. 3, 5, 7 ...</param>
        /// <returns>
        /// <return name="dst">The blurred image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.MedianBlur", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat MedianBlur(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 ksize = 5)
        {
            var dst = new Mat();
            Cv2.MedianBlur(src, dst, ksize);
            return dst;
        }

        /// <summary>
        /// Blurs an image using a Gaussian filter.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#gaussianblur"/>
        /// <param name="src">Input image; the image can have any number of channels, which are processed independently,
        /// but the depth should be CV_8U, CV_16U, CV_16S, CV_32F or CV_64F.</param>
        /// <param name="ksize">Gaussian kernel size. $ksize.width$ and $ksize.height$ can differ but they both must be positive and odd.
        /// Or, they can be zero’s and then they are computed from sigma*.</param>
        /// <param name="sigmaX">Gaussian kernel standard deviation in X direction.</param>
        /// <param name="sigmaY">Gaussian kernel standard deviation in Y direction; if sigmaY is zero, it is set to be equal to sigmaX,
        /// if both sigmas are zeros, they are computed from ksize.width and ksize.height,
        /// respectively (see getGaussianKernel() for details); to fully control the result
        /// regardless of possible future modifications of all this semantics, it is recommended to specify all of ksize, sigmaX, and sigmaY.</param>
        /// <param name="borderType">Pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">The blurred image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.GaussianBlur", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat GaussianBlur(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "3, 3")] Size ksize,
            [InputPin(PropertyMode = PropertyMode.Default)] Double sigmaX = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Double sigmaY = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.GaussianBlur(src, dst, ksize, sigmaX, sigmaY, borderType);
            return dst;
        }

        /// <summary>
        /// Applies bilateral filter to the image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#bilateralfilter"/>
        /// <param name="src">The source 8-bit or floating-point, 1-channel or 3-channel image</param>
        /// <param name="d">The diameter of each pixel neighborhood, that is used during filtering.
        /// If it is non-positive, it's computed from sigmaSpace.</param>
        /// <param name="sigmaColor">Filter sigma in the color space.
        /// Larger value of the parameter means that farther colors within the pixel neighborhood
        /// will be mixed together, resulting in larger areas of semi-equal color.</param>
        /// <param name="sigmaSpace">Filter sigma in the coordinate space.
        /// Larger value of the parameter means that farther pixels will influence each other
        /// (as long as their colors are close enough; see sigmaColor). Then $d\gt0$ , it specifies
        /// the neighborhood size regardless of sigmaSpace, otherwise d is proportional to sigmaSpace.</param>
        /// <param name="borderType">Pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.BilateralFilter", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat BilateralFilter(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 d = 5,
            [InputPin(PropertyMode = PropertyMode.Default)] Double sigmaColor = 150,
            [InputPin(PropertyMode = PropertyMode.Default)] Double sigmaSpace = 150,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.BilateralFilter(src, dst, d, sigmaColor, sigmaSpace, borderType);
            return dst;
        }

        /// <summary>
        /// Smoothes image using box filter.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#boxfilter"/>
        /// <param name="src">The source image.</param>
        /// <param name="ksize">The smoothing kernel size.</param>
        /// <param name="anchor">The anchor point. The default value $Point(-1,-1)$ means that the anchor is at the kernel center.</param>
        /// <param name="normalize">Indicates, whether the kernel is normalized by its area or not.</param>
        /// <param name="borderType">The border mode used to extrapolate pixels outside of the image.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.BoxFilter", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat BoxFilter(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "3, 3")] Size ksize,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? anchor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean normalize = true,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.BoxFilter(src, dst, new MatType(-1), ksize, anchor, normalize, borderType);
            return dst;
        }

        /// <summary>
        /// Smoothes image using normalized box filter
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#blur"/>
        /// <param name="src">The source image.</param>
        /// <param name="ksize">The smoothing kernel size.</param>
        /// <param name="anchor">The anchor point. The default value $Point(-1,-1)$ means that the anchor is at the kernel center.</param>
        /// <param name="borderType">The border mode used to extrapolate pixels outside of the image.</param>
        /// <returns>
        /// <return name="dst">The blurred image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Blur", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Blur(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "3, 3")] Size ksize,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? anchor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.Blur(src, dst, ksize, anchor, borderType);
            return dst;
        }

        //TODO  <param name="ddepth">The desired depth of the destination image. If it is negative, it will be the same as src.depth()</param>

        /// <summary>
        /// Convolves an image with the kernel
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#filter2d"/>
        /// <param name="src">The source image</param>
        /// <param name="kernel">Convolution kernel (or rather a correlation kernel),
        /// a single-channel floating point matrix. If you want to apply different kernels to
        /// different channels, split the image into separate color planes using split() and process them individually</param>
        /// <param name="anchor">The anchor of the kernel that indicates the relative position of
        /// a filtered point within the kernel. The anchor should lie within the kernel.
        /// The special default value (-1,-1) means that the anchor is at the kernel center</param>
        /// <param name="delta">The optional value added to the filtered pixels before storing them in dst</param>
        /// <param name="borderType">The pixel extrapolation method</param>
        /// <returns>
        /// <return name="dst">The destination image. It will have the same size and the same number of channels as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Filter2D", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Filter2D(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            //[InputPin(PropertyMode = PropertyMode.Allow)] MatType ddepth,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat kernel,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? anchor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Double delta = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.Filter2D(src, dst, new MatType(-1), kernel, anchor, delta, borderType);
            return dst;
        }

        /// <summary>
        /// Applies separable linear filter to an image
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#sepfilter2d"/>
        /// <param name="src">The source image</param>
        /// <param name="kernelX">The coefficients for filtering each row</param>
        /// <param name="kernelY">The coefficients for filtering each column</param>
        /// <param name="anchor">The anchor position within the kernel; The default value (-1, 1) means that the anchor is at the kernel center</param>
        /// <param name="delta">The value added to the filtered results before storing them</param>
        /// <param name="borderType">The pixel extrapolation method</param>
        /// <returns>
        /// <return name="dst">The destination image; will have the same size and the same number of channels as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.SepFilter2D", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat SepFilter2D(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            //[InputPin(PropertyMode = PropertyMode.Allow)] MatType ddepth,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat kernelX,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat kernelY,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? anchor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Double delta = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.SepFilter2D(src, dst, new MatType(-1), kernelX, kernelY, anchor, delta, borderType);
            return dst;
        }

        /// <summary>
        /// Calculates the first, second, third or mixed image derivatives using an extended Sobel operator.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#sobel"/>
        /// <param name="src">The source image.</param>
        /// <param name="xorder">Order of the derivative x.</param>
        /// <param name="yorder">Order of the derivative y.</param>
        /// <param name="ksize">Size of the extended Sobel kernel, must be 1, 3, 5 or 7</param>
        /// <param name="scale">The optional scale factor for the computed derivative values (by default, no scaling is applied.</param>
        /// <param name="delta">The optional delta value, added to the results prior to storing them in dst.</param>
        /// <param name="borderType">The pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Sobel", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Sobel(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            //[InputPin(PropertyMode = PropertyMode.Default)] MatrixType ddepth,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 xorder = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 yorder = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 ksize = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Double scale = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Double delta = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.Sobel(src, dst, new MatType(-1), xorder, yorder, ksize, scale, delta, borderType);
            return dst;
        }

        //TODO://// <param name="ddepth">The output image depth.</param>

        /// <summary>
        /// Calculates the first x- or y- image derivative using Scharr operator.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#scharr"/>
        /// <param name="src">Input image.</param>
        /// <param name="xorder">Order of the derivative x.</param>
        /// <param name="yorder">Order of the derivative y.</param>
        /// <param name="scale">The optional scale factor for the computed derivative values (by default, no scaling is applied.</param>
        /// <param name="delta">The optional delta value, added to the results prior to storing them in dst.</param>
        /// <param name="borderType">The pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Scharr", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Scharr(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            //[InputPin(PropertyMode = PropertyMode.Default)] MatType ddepth,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 xorder = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 yorder = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Double scale = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Double delta = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.Scharr(src, dst, new MatType(-1), xorder, yorder, scale, delta, borderType);
            return dst;
        }

        //TODO://// <param name="ddepth">The desired depth of the output image.</param>

        /// <summary>
        /// Calculates the Laplacian of an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#laplacian"/>
        /// <param name="src">Input image.</param>
        /// <param name="ksize">The aperture size used to compute the second-derivative filters.</param>
        /// <param name="scale">The optional scale factor for the computed Laplacian values (by default, no scaling is applied.</param>
        /// <param name="delta">The optional delta value, added to the results prior to storing them in dst.</param>
        /// <param name="borderType">The pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Laplacian", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Laplacian(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            //[InputPin(PropertyMode = PropertyMode.Default)] MatType ddepth,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 ksize = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Double scale = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Double delta = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.Laplacian(src, dst, new MatType(-1), ksize, scale, delta, borderType);
            return dst;
        }

        /// <summary>
        /// Finds edges in an image using Canny algorithm.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#canny"/>
        /// <param name="src">Single-channel 8-bit input image.</param>
        /// <param name="threshold1">The first threshold for the hysteresis procedure.</param>
        /// <param name="threshold2">The second threshold for the hysteresis procedure.</param>
        /// <param name="apertureSize">Aperture size for the Sobel operator. [By default this is ApertureSize.Size3]</param>
        /// <param name="L2gradient">Indicates, whether the more accurate L2 norm should be used to compute the image gradient magnitude (true), or a faster default L1 norm is enough (false). [By default this is false]</param>
        /// <returns>
        /// <return name="edges">The output edge map. It will have the same size and the same type as image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Canny", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Canny(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Double threshold1 = 32,
            [InputPin(PropertyMode = PropertyMode.Default)] Double threshold2 = 64,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 apertureSize = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean L2gradient = false)
        {
            var edges = new Mat();
            Cv2.Canny(src, edges, threshold1, threshold2, apertureSize, L2gradient);
            return edges;
        }


        /// <summary>
        /// computes both eigenvalues and the eigenvectors of 2x2 derivative covariation matrix  at each pixel. The output is stored as 6-channel matrix.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#cornereigenvalsandvecs"/>
        /// <param name="src">single-channel 8-bit or floating-point image.</param>
        /// <param name="blockSize">Neighborhood size.</param>
        /// <param name="ksize">Aperture parameter for the Sobel() operator.</param>
        /// <param name="borderType">Pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CornerEigenValsAndVecs")]
        public static Mat CornerEigenValsAndVecs(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 blockSize = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 ksize = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.CornerEigenValsAndVecs(src, dst, blockSize, ksize, borderType);
            return dst;
        }


        /// <summary>
        /// computes another complex cornerness criteria at each pixel
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#precornerdetect"/>
        /// <param name="src"> Source single-channel 8-bit of floating-point image.</param>
        /// <param name="ksize"> Aperture size of the Sobel() .</param>
        /// <param name="borderType">Pixel extrapolation method.</param>
        /// <returns>
        /// <return name="dst">Output image that has the type CV_32F and the same size as src .</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.PreCornerDetect", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat PreCornerDetect(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 ksize = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Default)
        {
            var dst = new Mat();
            Cv2.PreCornerDetect(src, dst, ksize, borderType);
            return dst;
        }

        /// <summary>
        /// adjusts the corner locations with sub-pixel accuracy to maximize the certain cornerness criteria
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#cornersubpix"/>
        /// <param name="image">Input image.</param>
        /// <param name="inputCorners">Initial coordinates of the input corners and refined coordinates provided for output.</param>
        /// <param name="winSize">Half of the side length of the search window.</param>
        /// <param name="zeroZone">Half of the size of the dead region in the middle of the search zone
        /// over which the summation in the formula below is not done. It is used sometimes to avoid possible singularities
        /// of the autocorrelation matrix. The value of (-1,-1) indicates that there is no such a size.</param>
        /// <param name="epsilon">Specifies the termcriteria.</param>
        /// <param name="max_iter">Specifies the termcriteria.</param>
        /// <param name="type">Criteria for termination of the iterative process of corner refinement.
        /// That is, the process of corner position refinement stops either after criteria.maxCount iterations
        /// or when the corner position moves by less than criteria.epsilon on some iteration.</param>
        /// <returns>
        /// <return name="location">The corner location.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.CornerSubPix", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Point2f[] CornerSubPix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Point2f> inputCorners,
            [InputPin(PropertyMode = PropertyMode.Default)] Size winSize,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "-1, -1")] Size zeroZone,
            [InputPin(PropertyMode = PropertyMode.Default)] int max_iter,
            [InputPin(PropertyMode = PropertyMode.Default)] double epsilon,
            [InputPin(PropertyMode = PropertyMode.Default)] CriteriaType type = CriteriaType.Eps
        )
        {
            var criteria = new TermCriteria(type, max_iter, epsilon);
            return Cv2.CornerSubPix(image, inputCorners, winSize, zeroZone, criteria);
        }

        /// <summary>
        /// finds the strong enough corners where the cornerMinEigenVal() or cornerHarris() report the local maxima
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#goodfeaturestotrack"/>
        /// <param name="src">Input 8-bit or floating-point 32-bit, single-channel image.</param>
        /// <param name="maxCorners">Maximum number of corners to return. If there are more corners than are found,
        /// the strongest of them is returned.</param>
        /// <param name="qualityLevel">Parameter characterizing the minimal accepted quality of image corners.
        /// The parameter value is multiplied by the best corner quality measure, which is the minimal eigenvalue
        /// or the Harris function response (see cornerHarris() ). The corners with the quality measure less than
        /// the product are rejected. For example, if the best corner has the quality measure = 1500, and the qualityLevel=0.01,
        /// then all the corners with the quality measure less than 15 are rejected.</param>
        /// <param name="minDistance">Minimum possible Euclidean distance between the returned corners.</param>
        /// <param name="blockSize">Size of an average block for computing a derivative covariation matrix over each pixel neighborhood.</param>
        /// <param name="useHarrisDetector">Parameter indicating whether to use a Harris detector</param>
        /// <param name="k">Free parameter of the Harris detector.</param>
        /// <param name="mask">Optional region of interest. If the image is not empty
        /// (it needs to have the type CV_8UC1 and the same size as image ), it specifies the region
        /// in which the corners are detected.</param>
        /// <returns>
        /// <return name="corners">Output vector of detected corners.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.GoodFeaturesToTrack", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Point2f[] GoodFeaturesToTrack(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxCorners,
            [InputPin(PropertyMode = PropertyMode.Default)] Double qualityLevel,
            [InputPin(PropertyMode = PropertyMode.Default)] Double minDistance,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 blockSize,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean useHarrisDetector,
            [InputPin(PropertyMode = PropertyMode.Default)] Double k,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null
        )
        {
            return Cv2.GoodFeaturesToTrack(src, maxCorners, qualityLevel, minDistance, mask == null ? new Mat() : mask, blockSize, useHarrisDetector, k);
        }

        /// <summary>
        /// Finds lines in a binary image using standard Hough transform.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#houghlines"/>
        /// <param name="image">The 8-bit, single-channel, binary source image. The image may be modified by the function</param>
        /// <param name="rho">Distance resolution of the accumulator in pixels</param>
        /// <param name="theta">Angle resolution of the accumulator in radians</param>
        /// <param name="threshold">The accumulator threshold parameter. Only those lines are returned that get enough votes ( &gt; threshold )</param>
        /// <param name="srn">For the multi-scale Hough transform it is the divisor for the distance resolution rho. [By default this is 0]</param>
        /// <param name="stn">For the multi-scale Hough transform it is the divisor for the distance resolution theta. [By default this is 0]</param>
        /// <returns>
        /// <return name="lineSegement">The output vector of lines. Each line is represented by a two-element vector (rho, theta) .
        /// rho is the distance from the coordinate origin (0,0) (top-left corner of the image) and theta is the line rotation angle in radians</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.HoughLines")]
        public static LineSegmentPoint[] HoughLines(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] Double rho = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Double theta = 0.007,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 threshold = 100,
            [InputPin(PropertyMode = PropertyMode.Default)] Double srn = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Double stn = 0
        )
        {
            IEnumerable<LineSegmentPolar> lines = Cv2.HoughLines(image, rho, theta, threshold, srn, stn);
            return lines.Select(x => x.ToSegmentPoint(1000)).ToArray();
        }


        /// <summary>
        /// draw some lines given by houghlines
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="lines">Houghlines.</param>
        /// <param name="color">Color. If one value is negative, color is random for each line.</param>
        /// <param name="thickness">The thickness of drawed lines.</param>
        /// <returns>
        /// <return name="image">Image with drawn lines.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.DrawCvLineSegmentPoint", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DrawCvLineSegmentPoint(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] LineSegmentPoint[] lines,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] int thickness = 1
            )
        {
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;

            Scalar c = color;
            var dst = image.Clone();
            foreach (var line in lines)
            {
                if (randomColor)
                    c = Scalar.RandomColor();
                Cv2.Line(dst, line.P1, line.P2, c, thickness);
            }
            return dst;
        }

        /// <summary>
        /// draw some circles given by houghcircles
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="circles">Houghcircles.</param>
        /// <param name="color">Color. If one value is negative, color is random for each circle.</param>
        /// <returns>
        /// <return name="image">Image with drawn circles.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.DrawCvCircleSegment", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DrawCvCircleSegment(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] CircleSegment[] circles,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar color
            )
        {
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;

            Scalar c = color;
            var dst = image.Clone();
            foreach (var circle in circles)
            {
                if (randomColor)
                    c = Scalar.RandomColor();

                Point center = new Point(circle.Center.X, circle.Center.Y);

                Cv2.Circle(dst, center, (int)Math.Round(circle.Radius), c);
            }
            return dst;
        }


        /// <summary>
        /// Finds lines segments in a binary image using probabilistic Hough transform.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#houghlinesp"/>
        /// <param name="image">8-bit, single-channel binary source image.</param>
        /// <param name="rho">Distance resolution of the accumulator in pixels</param>
        /// <param name="theta">Angle resolution of the accumulator in radians</param>
        /// <param name="threshold">The accumulator threshold parameter. Only those lines are returned that get enough votes ($\gt threshold$).</param>
        /// <param name="minLineLength">The minimum line length. Line segments shorter than that will be rejected. [By default this is 0]</param>
        /// <param name="maxLineGap">The maximum allowed gap between points on the same line to link them. [By default this is 0]</param>
        /// <returns>
        /// <return name="lines">The output lines. Each line is represented by a 4-element vector (x1, y1, x2, y2)</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.HoughLinesP")]
        public static LineSegmentPoint[] HoughLinesP(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] Double rho = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Double theta = 0.007,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 threshold = 100,
            [InputPin(PropertyMode = PropertyMode.Default)] Double minLineLength = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Double maxLineGap = 0
        )
        {
            var imageOutput = image.Clone();
            return Cv2.HoughLinesP(imageOutput, rho, theta, threshold, minLineLength, maxLineGap);
        }

        /// <summary>
        /// Finds circles in a grayscale image using a Hough transform.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/feature_detection.html#houghcircles"/>
        /// <param name="image">The 8-bit, single-channel, grayscale input image.</param>
        /// <param name="dp">The inverse ratio of the accumulator resolution to the image resolution. </param>
        /// <param name="minDist">Minimum distance between the centers of the detected circles. </param>
        /// <param name="param1">The first method-specific parameter. [By default this is 100]</param>
        /// <param name="param2">The second method-specific parameter. [By default this is 100]</param>
        /// <param name="minRadius">Minimum circle radius. [By default this is 0]</param>
        /// <param name="maxRadius">Maximum circle radius. [By default this is 0] </param>
        /// <returns>
        /// <return name="circles">The output vector found circles. Each vector is encoded as 3-element floating-point vector (x, y, radius)</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.HoughCircles")]
        public static CircleSegment[] HoughCircles(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            //[InputPin(PropertyMode = PropertyMode.Default)] HoughCirclesMethod method,
            [InputPin(PropertyMode = PropertyMode.Default)] Double dp = 10,
            [InputPin(PropertyMode = PropertyMode.Default)] Double minDist = 10,
            [InputPin(PropertyMode = PropertyMode.Default)] Double param1 = 100,
            [InputPin(PropertyMode = PropertyMode.Default)] Double param2 = 100,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 minRadius = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxRadius = 0
        )
        {
            return Cv2.HoughCircles(image, HoughMethods.Gradient, dp, minDist, param1, param2, minRadius, maxRadius);
        }

        /// <summary>
        /// Default borderValue for Dilate/Erode
        /// </summary>
        /// <returns>
        /// <return name="border">default border value.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.MorphologyDefaultBorderValue", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Scalar MorphologyDefaultBorderValue()
        {
            return Cv2.MorphologyDefaultBorderValue();
        }

        /// <summary>
        /// Dilates an image by using a specific structuring element.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#dilate"/>
        /// <param name="src">The source image.</param>
        /// <param name="element">The structuring element used for dilation. If element=null , a $3\times 3$ rectangular structuring element is used.</param>
        /// <param name="anchor">Position of the anchor within the element. The default value $(-1, -1)$ means that the anchor is at the element center.</param>
        /// <param name="iterations">The number of times dilation is applied. [By default this is 1]</param>
        /// <param name="borderType">The pixel extrapolation method. [By default this is BorderTypes.Constant]</param>
        /// <param name="borderValue">The border value in case of a constant border. The default value has a special meaning. [By default this is CvCpp.MorphologyDefaultBorderValue()]</param>
        /// <returns>
        /// <return name="dst">The destination image. It will have the same size and the same type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Dilate", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Dilate(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat element = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? anchor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 iterations = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Constant,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? borderValue = null)
        {
            var dst = new Mat();
            Cv2.Dilate(src, dst, element == null ? new Mat() : element, anchor, iterations, borderType, borderValue);
            return dst;
        }

        /// <summary>
        /// Erodes an image by using a specific structuring element.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#erode"/>
        /// <param name="src">The source image</param>
        /// <param name="element">The structuring element used for dilation. If element=null, a $3\times 3$ rectangular structuring element is used.</param>
        /// <param name="anchor">Position of the anchor within the element. The default value $(-1, -1)$ means that the anchor is at the element center.</param>
        /// <param name="iterations">The number of times erosion is applied.</param>
        /// <param name="borderType">The pixel extrapolation method.</param>
        /// <param name="borderValue">The border value in case of a constant border. The default value has a special meaning. [By default this is CvCpp.MorphologyDefaultBorderValue()]</param>
        /// <returns>
        /// <return name="dst">The destination image. It will have the same size and the same type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Erode", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Erode(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat element = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? anchor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 iterations = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Constant,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? borderValue = null)
        {
            var dst = new Mat();
            Cv2.Erode(src, dst, element == null ? new Mat() : element, anchor, iterations, borderType, borderValue);
            return dst;
        }

        /// <summary>
        /// Performs advanced morphological transformations.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/filtering.html#morphologyex"/>
        /// <param name="src">Source image.</param>
        /// <param name="element">Structuring element.</param>
        /// <param name="op">Type of morphological operation.</param>
        /// <param name="anchor">Position of the anchor within the element. The default value $(-1, -1)$ means that the anchor is at the element center.</param>
        /// <param name="iterations">Number of times erosion and dilation are applied. [By default this is 1]</param>
        /// <param name="borderType">The pixel extrapolation method. [By default this is BorderTypes.Constant]</param>
        /// <param name="borderValue">The border value in case of a constant border. The default value has a special meaning. [By default this is CvCpp.MorphologyDefaultBorderValue()]</param>
        /// <returns>
        /// <return name="dst">Destination image. It will have the same size and the same type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.MorphologyEx", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat MorphologyEx(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat element,
            [InputPin(PropertyMode = PropertyMode.Default)] MorphTypes op = MorphTypes.Close,
            [InputPin(PropertyMode = PropertyMode.Default)] Point? anchor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 iterations = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderType = BorderTypes.Constant,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? borderValue = null
        )
        {
            var dst = new Mat();
            Cv2.MorphologyEx(src, dst, op, element, anchor, iterations, borderType, borderValue);
            return dst;
        }

        /// <summary>
        /// Resizes an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#resize"/>
        /// <param name="src">Input image.</param>
        /// <param name="dsize">Output image size; if it equals zero, it is computed as:
        /// $dsize = Size(round(fx \times src.cols), round(fy \times src.rows))$
        /// Either $dsize$ or both $fx$ and $fy$ must be non-zero.</param>
        /// <param name="fx">Scale factor along the horizontal axis; when it equals 0,
        /// it is computed as: $(double)dsize.width/src.cols$.</param>
        /// <param name="fy">Scale factor along the vertical axis; when it equals 0,
        /// it is computed as: $(double)dsize.height/src.rows$.</param>
        /// <param name="interpolation">Interpolation method.</param>
        /// <returns>
        /// <return name="dst">Output image; it has the size dsize (when it is non-zero) or the size computed
        /// from src.size(), fx, and fy; the type of dst is the same as of src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Resize", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Resize(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "512, 512")] Size dsize,
            [InputPin(PropertyMode = PropertyMode.Default)] Double fx = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Double fy = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] InterpolationFlags interpolation = InterpolationFlags.Linear
        )
        {
            var dst = new Mat();
            Cv2.Resize(src, dst, dsize, fx, fy, interpolation);
            return dst;
        }

        /// <summary>
        /// Applies an affine transformation to an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#warpaffine"/>
        /// <param name="src">Input image.</param>
        /// <param name="m">$2 \times 3$ transformation matrix.</param>
        /// <param name="dsize">Size of the output image.</param>
        /// <param name="flags">Combination of interpolation methods and the optional flag
        /// WARP_INVERSE_MAP that means that M is the inverse transformation ($dst \gt src$) .</param>
        /// <param name="borderMode">pixel extrapolation method; when borderMode=BORDER_TRANSPARENT,
        /// it means that the pixels in the destination image corresponding to the "outliers"
        /// in the source image are not modified by the function.</param>
        /// <param name="borderValue">value used in case of a constant border; by default, it is 0.</param>
        /// <returns>
        /// <return name="dst">Output image that has the size dsize and the same type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.WarpAffine", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat WarpAffine(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat m,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "512, 512")] Size dsize,
            [InputPin(PropertyMode = PropertyMode.Default)] InterpolationFlags flags = InterpolationFlags.Linear,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderMode = BorderTypes.Constant,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? borderValue = null
        )
        {
            var dst = new Mat();
            Cv2.WarpAffine(src, dst, m, dsize, flags, borderMode, borderValue);
            return dst;
        }


        /// <summary>
        /// Calculates an affine transform from three pairs of the corresponding points.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#getaffinetransform"/>
        /// <param name="src">Coordinates of triangle vertices in the source image.</param>
        /// <param name="dst">Coordinates of the corresponding triangle vertices in the destination image.</param>
        /// <returns>
        /// <return name="affine">$2\times 3$ matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.GetAffineTransform", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat GetAffineTransform(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst
            )
        {
            return Cv2.GetAffineTransform(src, dst);
        }

        /// <summary>
        /// Applies a perspective transformation to an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#warpperspective"/>
        /// <param name="src">Input image.</param>
        /// <param name="m">$3\times 3$ transformation matrix.</param>
        /// <param name="dsize">Size of the output image.</param>
        /// <param name="flags">Combination of interpolation methods (INTER_LINEAR or INTER_NEAREST) and the optional flag WARP_INVERSE_MAP, that sets M as the inverse transformation ( \text{dst}\rightarrow\text{src} ).</param>
        /// <param name="borderMode">Pixel extrapolation method (BORDER_CONSTANT or BORDER_REPLICATE).</param>
        /// <param name="borderValue">Value used in case of a constant border; by default, it equals 0.</param>
        /// <returns>
        /// <return name="dst">Output image that has the size dsize and the same type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.WarpPerspective", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat WarpPerspective(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat m,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "3, 3")] Size dsize,
            [InputPin(PropertyMode = PropertyMode.Default)] InterpolationFlags flags = InterpolationFlags.Linear,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderMode = BorderTypes.Constant,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? borderValue = null
        )
        {
            var dst = new Mat(dsize, src.Type());
            Cv2.WarpPerspective(src, dst, m, dsize, flags, borderMode, borderValue);
            return dst;
        }

        /// <summary>
        /// Applies a generic geometrical transformation to an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#remap"/>
        /// <param name="src">Source image.</param>
        /// <param name="map1">The first map of either $(x,y)$ points or just x values having the type CV_16SC2, CV_32FC1, or CV_32FC2.</param>
        /// <param name="map2">The second map of y values having the type CV_16UC1, CV_32FC1, or none (empty map if map1 is $(x,y)$ points), respectively.</param>
        /// <param name="interpolation">Interpolation method. The method INTER_AREA is not supported by this function.</param>
        /// <param name="borderMode">Pixel extrapolation method. When borderMode=BORDER_TRANSPARENT,
        /// it means that the pixels in the destination image that corresponds to the "outliers" in
        /// the source image are not modified by the function.</param>
        /// <param name="borderValue">Value used in case of a constant border. By default, it is 0.</param>
        /// <returns>
        /// <return name="dst">Destination image. It has the same size as map1 and the same type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Remap", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Remap(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat map1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat map2,
            [InputPin(PropertyMode = PropertyMode.Default)] InterpolationFlags interpolation = InterpolationFlags.Linear,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes borderMode = BorderTypes.Constant,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? borderValue = null
        )
        {
            var dst = new Mat();
            Cv2.Remap(src, dst, map1, map2, interpolation, borderMode, borderValue);
            return dst;
        }

        /// <summary>
        /// Calculates an affine matrix of 2D rotation.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#getrotationmatrix2d"/>
        /// <param name="center">Center of the rotation in the source image.</param>
        /// <param name="angle">Rotation angle in degrees. Positive values mean counter-clockwise rotation (the coordinate origin is assumed to be the top-left corner).</param>
        /// <param name="scale">Isotropic scale factor.</param>
        /// <returns>
        /// <return name="mapMatrix">The output affine transformation, $2 \times 3$ floating-point matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.GetRotationMatrix2D")]
        public static Mat GetRotationMatrix2D(
            [InputPin(PropertyMode = PropertyMode.Default)] Point2f center,
            [InputPin(PropertyMode = PropertyMode.Default)] Double angle = 180,
            [InputPin(PropertyMode = PropertyMode.Default)] Double scale = 1
        )
        {
            return Cv2.GetRotationMatrix2D(center, angle, scale);
        }

        /// <summary>
        /// Inverts an affine transformation.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#invertaffinetransform"/>0
        /// <param name="m">Original affine transformation.</param>
        /// <returns>
        /// <return name="im">Output reverse affine transformation.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.InvertAffineTransform", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat InvertAffineTransform(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat m
        )
        {
            var im = new Mat();
            Cv2.InvertAffineTransform(m, im);
            return im;
        }

        /// <summary>
        /// Calculates a perspective transform from four pairs of the corresponding points.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#getperspectivetransform"/>
        /// <param name="src">Coordinates of quadrangle vertices in the source image.</param>
        /// <param name="dst">Coordinates of the corresponding quadrangle vertices in the destination image.</param>
        /// <returns>
        /// <return name="mat">Perspective matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.GetPerspectiveTransform", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat GetPerspectiveTransform(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst
        )
        {
            return Cv2.GetPerspectiveTransform(src, dst);
        }

        /// <summary>
        /// Retrieves a pixel rectangle from an image with sub-pixel accuracy.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/geometric_transformations.html#getrectsubpix"/>
        /// <param name="image">Source image.</param>
        /// <param name="patchSize">Size of the extracted patch.</param>
        /// <param name="center">Floating point coordinates of the center of the extracted rectangle
        /// within the source image. The center must be inside the image.</param>
        /// <param name="patchType">Depth of the extracted pixels. By default, they have the same depth as src.</param>
        /// <returns>
        /// <return name="patch">Extracted patch that has the size patchSize and the same number of channels as src .</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.GetRectSubPix", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat GetRectSubPix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "50, 50")] Size patchSize,
            [InputPin(PropertyMode = PropertyMode.Default)] Point2f center,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 patchType = -1
        )
        {
            var patch = new Mat();
            Cv2.GetRectSubPix(image, patchSize, center, patch, patchType);
            return patch;
        }

        /// <summary>
        /// Calculates the integral of an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/miscellaneous_transformations.html#integral"/>
        /// <param name="src">Input image as $W \times H$, 8-bit or floating-point (32f or 64f).</param>
        /// <param name="sdepth">Desired depth of the integral and the tilted integral images, CV_32S, CV_32F, or CV_64F.</param>
        /// <returns>
        /// <return name="sum">Integral image as $(W+1)\times (H+1)$, 32-bit integer or floating-point (32f or 64f).</return>
        /// <return name="sqsum">Integral image for squared pixel values; it is $(W+1)\times (H+1)$, double-precision floating-point (64f) array.</return>
        /// <return name="tilted">Integral for the image rotated by 45 degrees; it is $(W+1)\times (H+1)$ array with the same data type as sum.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Integral")]
        public static Tuple<Mat, Mat, Mat> Integral(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 sdepth = -1
        )
        {
            var sum = new Mat();
            var sqsum = new Mat();
            var tilted = new Mat();
            Cv2.Integral(src, sum, sqsum, tilted, sdepth);
            return Tuple.Create(sum, sqsum, tilted);
        }

        // TODO no Accumulator
        ///// <summary>
        ///// Adds an image to the accumulator.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/motion_analysis_and_object_tracking.html#accumulate"/>
        ///// <param name="src">Input image as 1- or 3-channel, 8-bit or 32-bit floating point.</param>
        ///// <param name="dst">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</param>
        ///// <param name="mask">Optional operation mask.</param>
        ///// <returns>
        ///// <return name="dst">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.ImgProc.Accumulate", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Mat Accumulate(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null
        //)
        //{
        //    var dstOutput = dst.Clone();
        //    Cv2.Accumulate(src, dstOutput, mask == null ? new Mat() : mask);
        //    return dstOutput;
        //}

        ///// <summary>
        ///// Adds the square of a source image to the accumulator.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/motion_analysis_and_object_tracking.html#accumulatesquare"/>
        ///// <param name="src">Input image as 1- or 3-channel, 8-bit or 32-bit floating point.</param>
        ///// <param name="dst">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</param>
        ///// <param name="mask">Optional operation mask.</param>
        ///// <returns>
        ///// <return name="dst">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.ImgProc.AccumulateSquare", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Mat AccumulateSquare(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null
        //)
        //{
        //    var dstOutput = dst.Clone();
        //    Cv2.AccumulateSquare(src, dstOutput, mask == null ? new Mat() : mask);
        //    return dstOutput;
        //}

        ///// <summary>
        ///// Adds the per-element product of two input images to the accumulator.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/motion_analysis_and_object_tracking.html#accumulateproduct"/>
        ///// <param name="src1">First input image, 1- or 3-channel, 8-bit or 32-bit floating point.</param>
        ///// <param name="src2">Second input image of the same type and the same size as src1</param>
        ///// <param name="accumulator">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</param>
        ///// <param name="mask">Optional operation mask.</param>
        ///// <returns>
        ///// <return name="dst">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.ImgProc.AccumulateProduct", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Mat AccumulateProduct(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat accumulator,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null
        //)
        //{
        //    var dstOutput = accumulator.Clone();
        //    Cv2.AccumulateProduct(src1, src2, dstOutput, mask == null ? new Mat() : mask);
        //    return dstOutput;
        //}

        ///// <summary>
        ///// Updates a running average.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/imgproc/doc/motion_analysis_and_object_tracking.html#accumulateweighted"/>
        ///// <param name="src">Input image as 1- or 3-channel, 8-bit or 32-bit floating point.</param>
        ///// <param name="dst">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</param>
        ///// <param name="alpha">Weight of the input image.</param>
        ///// <param name="mask">Optional operation mask.</param>
        ///// <returns>
        ///// <return name="dst">Accumulator image with the same number of channels as input image, 32-bit or 64-bit floating-point.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.ImgProc.AccumulateWeighted", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Mat AccumulateWeighted(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Double alpha,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null
        //)
        //{
        //    var dstOutput = dst.Clone();
        //    Cv2.AccumulateWeighted(src, dstOutput, alpha, mask == null ? new Mat() : mask);
        //    return dstOutput;
        //}

        /// <summary>
        /// restores the damaged image areas using one of the available intpainting algorithms
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/photo/doc/inpainting.html#inpaint"/>
        /// <param name="src">Input 8-bit 1-channel or 3-channel image.</param>
        /// <param name="inpaintMask"> Inpainting mask, 8-bit 1-channel image. Non-zero pixels indicate the area that needs to be inpainted.
        /// dst – Output image with the same size and type as src .</param>
        /// <param name="inpaintRadius">Radius of a circular neighborhood of each point inpainted that is considered by the algorithm.</param>
        /// <param name="flags">Inpainting method: INPAINT_NS Navier-Stokes based method or INPAINT_TELEA Method by Alexandru Telea.</param>
        /// <returns>
        /// <return name="dst">Output image with the same size and type as src .</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Photo.Inpaint", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Inpaint(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat inpaintMask,
            [InputPin(PropertyMode = PropertyMode.Default)] Double inpaintRadius,
            [InputPin(PropertyMode = PropertyMode.Default)] InpaintMethod flags
        )
        {
            var dst = new Mat();
            Cv2.Inpaint(src, inpaintMask, dst, inpaintRadius, flags);
            return dst;
        }

        /// <summary>
        /// Perform image denoising using Non-local Means Denoising algorithm
        /// with several computational optimizations. Noise expected to be a gaussian white noise
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/photo/doc/denoising.html#fastnlmeansdenoising"/>
        /// <param name="src">Input 8-bit 1-channel, 2-channel or 3-channel image.</param>
        /// <param name="h">
        /// Parameter regulating filter strength. Big h value perfectly removes noise but also removes image details,
        /// smaller h value preserves details but also preserves some noise</param>
        /// <param name="templateWindowSize">
        /// Size in pixels of the template patch that is used to compute weights. Should be odd. Recommended value 7 pixels</param>
        /// <param name="searchWindowSize">
        /// Size in pixels of the window that is used to compute weighted average for given pixel.
        /// Should be odd. Affect performance linearly: greater searchWindowsSize - greater denoising time. Recommended value 21 pixels</param>
        /// <returns>
        /// <return name="dst">Output image with the same size and type as src .</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Photo.FastNlMeansDenoising", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FastNlMeansDenoising(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Single h = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 templateWindowSize = 7,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 searchWindowSize = 21
        )
        {
            var dst = new Mat();
            Cv2.FastNlMeansDenoising(src, dst, h, templateWindowSize, searchWindowSize);
            return dst;
        }

        /// <summary>
        /// Modification of fastNlMeansDenoising function for colored images
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/photo/doc/denoising.html#fastnlmeansdenoisingcolored"/>
        /// <param name="src">Input 8-bit 3-channel image.</param>
        /// <param name="h">Parameter regulating filter strength for luminance component.
        /// Bigger h value perfectly removes noise but also removes image details, smaller h value preserves details but also preserves some noise</param>
        /// <param name="hColor">The same as h but for color components. For most images value equals 10 will be enough
        /// to remove colored noise and do not distort colors</param>
        /// <param name="templateWindowSize">
        /// Size in pixels of the template patch that is used to compute weights. Should be odd. Recommended value 7 pixels</param>
        /// <param name="searchWindowSize">
        /// Size in pixels of the window that is used to compute weighted average for given pixel. Should be odd.
        /// Affect performance linearly: greater searchWindowsSize - greater denoising time. Recommended value 21 pixels</param>
        /// <returns>
        /// <return name="dst">Output image with the same size and type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Photo.FastNlMeansDenoisingColored", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FastNlMeansDenoisingColored(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Single h = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Single hColor = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 templateWindowSize = 7,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 searchWindowSize = 21
        )
        {
            var dst = new Mat();
            Cv2.FastNlMeansDenoisingColored(src, dst, h, hColor, templateWindowSize, searchWindowSize);
            return dst;
        }

        /// <summary>
        /// Modification of fastNlMeansDenoising function for images sequence where consequtive images have been captured in small period of time. For example video. This version of the function is for grayscale images or for manual manipulation with colorspaces.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/photo/doc/denoising.html#fastnlmeansdenoisingmulti"/>
        /// <param name="srcImgs">Input 8-bit 1-channel, 2-channel or 3-channel images sequence. All images should have the same type and size.</param>
        /// <param name="imgToDenoiseIndex">Target image to denoise index in srcImgs sequence</param>
        /// <param name="temporalWindowSize">Number of surrounding images to use for target image denoising. Should be odd. Images from $imgToDenoiseIndex - temporalWindowSize / 2$ to $imgToDenoiseIndex - temporalWindowSize / 2$ from srcImgs will be used to denoise srcImgs[imgToDenoiseIndex] image.</param>
        /// <param name="h">Parameter regulating filter strength for luminance component. Bigger h value perfectly removes noise but also removes image details, smaller h value preserves details but also preserves some noise</param>
        /// <param name="templateWindowSize">Size in pixels of the template patch that is used to compute weights. Should be odd. Recommended value 7 pixels</param>
        /// <param name="searchWindowSize">Size in pixels of the window that is used to compute weighted average for given pixel. Should be odd. Affect performance linearly: greater searchWindowsSize - greater denoising time. Recommended value 21 pixels</param>
        /// <returns>
        /// <return name="dst">Output image with the same size and type as srcImgs images.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Photo.FastNlMeansDenoisingMulti", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FastNlMeansDenoisingMulti(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> srcImgs,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 imgToDenoiseIndex,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 temporalWindowSize,
            [InputPin(PropertyMode = PropertyMode.Default)] Single h = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 templateWindowSize = 7,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 searchWindowSize = 21
        )
        {
            var dst = new Mat();
            Cv2.FastNlMeansDenoisingMulti(srcImgs, dst, imgToDenoiseIndex, temporalWindowSize, h, templateWindowSize, searchWindowSize);
            return dst;
        }

        /// <summary>
        /// Modification of fastNlMeansDenoisingMulti function for colored images sequences
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/photo/doc/denoising.html#fastnlmeansdenoisingcoloredmulti"/>
        /// <param name="srcImgs">Input 8-bit 3-channel images sequence. All images should have the same type and size.</param>
        /// <param name="imgToDenoiseIndex">Target image to denoise index in srcImgs sequence</param>
        /// <param name="temporalWindowSize">Number of surrounding images to use for target image denoising. Should be odd. Images from $imgToDenoiseIndex - temporalWindowSize / 2$ to $imgToDenoiseIndex - temporalWindowSize / 2$ from srcImgs will be used to denoise srcImgs[imgToDenoiseIndex] image.</param>
        /// <param name="h">Parameter regulating filter strength for luminance component. Bigger h value perfectly removes noise but also removes image details, smaller h value preserves details but also preserves some noise.</param>
        /// <param name="hColor">The same as h but for color components.</param>
        /// <param name="templateWindowSize">Size in pixels of the template patch that is used to compute weights. Should be odd. Recommended value 7 pixels</param>
        /// <param name="searchWindowSize">Size in pixels of the window that is used to compute weighted average for given pixel. Should be odd. Affect performance linearly: greater searchWindowsSize - greater denoising time. Recommended value 21 pixels</param>
        /// <returns>
        /// <return name="dst">Output image with the same size and type as srcImgs images.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Photo.FastNlMeansDenoisingColoredMulti", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FastNlMeansDenoisingColoredMulti(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> srcImgs,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 imgToDenoiseIndex,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 temporalWindowSize,
            [InputPin(PropertyMode = PropertyMode.Default)] Single h = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Single hColor = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 templateWindowSize = 7,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 searchWindowSize = 21
        )
        {
            var dst = new Mat();
            Cv2.FastNlMeansDenoisingColoredMulti(srcImgs, dst, imgToDenoiseIndex, temporalWindowSize, h, hColor, templateWindowSize, searchWindowSize);
            return dst;
        }
        // TODO testing, because there may be side effects


        // TODO int scale Scale factor.
        //int iterations Iteration count.
        //double tau Asymptotic value of steepest descent method.
        //double lambda Weight parameter to balance data term and smoothness term.
        //double alpha Parameter of spacial distribution in Bilateral-TV.
        //int btvKernelSize Kernel size of Bilateral-TV filter.
        //int blurKernelSize Gaussian blur kernel size.
        //double blurSigma Gaussian blur sigma.
        //int temporalAreaRadius Radius of the temporal search area.
        //Ptr<DenseOpticalFlowExt> opticalFlow Dense optical flow algorithm.

        /// <summary>
        /// Create Bilateral TV-L1 Super Resolution with given FrameSource.
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/superres/doc/super_resolution.html#superres-createsuperresolution-btvl1"/>
        /// <param name="frameSource"></param>
        /// <returns>
        /// <return name="superResolution"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Superres.CreateSuperResolution_BTVL1", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static SuperResolution CreateSuperResolution_BTVL1(
            [InputPin(PropertyMode = PropertyMode.Always)] FrameSource frameSource
            )
        {
            var superResolution = Cv2.CreateSuperResolution_BTVL1();
            superResolution.SetInput(frameSource);
            return superResolution;
        }

        /// <summary>
        /// Process next frame from input and return output result.
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/superres/doc/super_resolution.html#superres-superresolution-nextframe"/>
        /// <param name="superResolution"></param>
        /// <returns>
        /// <return name="mat"></return>
        /// <return name="superResolution"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Superres.SuperResolutionNextFrame", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, SuperResolution> SuperResolutionNextFrame(
            [InputPin(PropertyMode = PropertyMode.Always)] SuperResolution superResolution
            )
        {
            var mat = new Mat();
            superResolution.NextFrame(mat);
            return Tuple.Create(mat, superResolution);
        }

        /// <summary>
        /// Finds a perspective transformation between two planes.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#findhomography"/>
        /// <param name="srcPoints">Coordinates of the points in the original plane, a matrix of the type CV_32FC2</param>
        /// <param name="dstPoints">Coordinates of the points in the target plane, a matrix of the type CV_32FC2</param>
        /// <param name="method">Method used to computed a homography matrix.</param>
        /// <param name="ransacReprojThreshold">Maximum allowed reprojection error to treat a point pair as an inlier (used in the RANSAC method only).</param>
        /// <returns>
        /// <return name="homography">perspective transformation.</return>
        /// <return name="mask">Output mask set by a robust method ( CV_RANSAC or CV_LMEDS ).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.FindHomography")]
        public static Tuple<Mat, Mat> FindHomography(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat srcPoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat dstPoints,
            [InputPin(PropertyMode = PropertyMode.Default)] HomographyMethods method = HomographyMethods.None,
            [InputPin(PropertyMode = PropertyMode.Default)] Double ransacReprojThreshold = 3
        )
        {
            var mask = new Mat();
            var homography = Cv2.FindHomography(srcPoints, dstPoints, method, ransacReprojThreshold, mask);
            return Tuple.Create(homography, mask);
        }

        /// <summary>
        /// Computes an RQ decomposition of 3x3 matrices.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#rqdecomp3x3"/>
        /// <param name="src">$3 \times 3$ input matrix.</param>
        /// <returns>
        /// <return name="eulerAngles">Euler angles.</return>
        /// <return name="mtxR">Output $3 \times 3$ upper-triangular matrix.</return>
        /// <return name="mtxQ">Output $3 \times 3$ orthogonal matrix.</return>
        /// <return name="qx">Optional output $3 \times 3$ rotation matrix around x-axis.</return>
        /// <return name="qy">Optional output $3 \times 3$ rotation matrix around y-axis.</return>
        /// <return name="qz">Optional output $3 \times 3$ rotation matrix around z-axis.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.RQDecomp3x3")]
        public static Tuple<Vec3d, Mat, Mat, Mat, Mat, Mat> RQDecomp3x3(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var mtxR = new Mat();
            var mtxQ = new Mat();
            var qx = new Mat();
            var qy = new Mat();
            var qz = new Mat();
            var eulerAngles = Cv2.RQDecomp3x3(src, mtxR, mtxQ, qx, qy, qz);
            return Tuple.Create(eulerAngles, mtxR, mtxQ, qx, qy, qz);
        }

        /// <summary>
        /// Decomposes a projection matrix into a rotation matrix and a camera matrix.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#decomposeprojectionmatrix"/>
        /// <param name="projMatrix">3x4 input projection matrix P.</param>
        /// <returns>
        /// <return name="cameraMatrix">Output 3x3 camera matrix K.</return>
        /// <return name="rotMatrix">Output 3x3 external rotation matrix R.</return>
        /// <return name="transVect">Output 4x1 translation vector T.</return>
        /// <return name="rotMatrixX">Optional 3x3 rotation matrix around x-axis.</return>
        /// <return name="rotMatrixY">Optional 3x3 rotation matrix around y-axis.</return>
        /// <return name="rotMatrixZ">Optional 3x3 rotation matrix around z-axis.</return>
        /// <return name="eulerAngles">Optional three-element vector containing three Euler angles of rotation in degrees.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.DecomposeProjectionMatrix")]
        public static Tuple<Mat, Mat, Mat, Mat, Mat, Mat, Mat> DecomposeProjectionMatrix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat projMatrix
        )
        {
            var cameraMatrix = new Mat();
            var rotMatrix = new Mat();
            var transVect = new Mat();
            var rotMatrixX = new Mat();
            var rotMatrixY = new Mat();
            var rotMatrixZ = new Mat();
            var eulerAngles = new Mat();
            Cv2.DecomposeProjectionMatrix(projMatrix, cameraMatrix, rotMatrix, transVect, rotMatrixX, rotMatrixY, rotMatrixZ, eulerAngles);
            return Tuple.Create(cameraMatrix, rotMatrix, transVect, rotMatrixX, rotMatrixY, rotMatrixZ, eulerAngles);
        }

        /// <summary>
        /// computes derivatives of the matrix product w.r.t each of the multiplied matrix coefficients
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#matmulderiv"/>
        /// <param name="a">First multiplied matrix.</param>
        /// <param name="b">Second multiplied matrix.</param>
        /// <returns>
        /// <return name="dABdA">First output derivative matrix d(A*B)/dA of size A.rows*B.cols X A.rows*A.cols .</return>
        /// <return name="dABdB">Second output derivative matrix d(A*B)/dB of size A.rows*B.cols X B.rows*B.cols .</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.MatMulDeriv")]
        public static Tuple<Mat, Mat> MatMulDeriv(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat a,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat b
        )
        {
            var dABdA = new Mat();
            var dABdB = new Mat();
            Cv2.MatMulDeriv(a, b, dABdA, dABdB);
            return Tuple.Create(dABdA, dABdB);
        }

        /// <summary>
        /// Combines two rotation-and-shift transformations.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#composert"/>
        /// <param name="rvec1">First rotation vector.</param>
        /// <param name="tvec1">First translation vector.</param>
        /// <param name="rvec2">Second rotation vector.</param>
        /// <param name="tvec2">Second translation vector.</param>
        /// <returns>
        /// <return name="rvec3">Output rotation vector of the superposition.</return>
        /// <return name="tvec3">Output translation vector of the superposition.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.ComposeRT")]
        public static Tuple<Mat, Mat> ComposeRT(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat rvec1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat tvec1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat rvec2,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat tvec2
        )
        {
            var rvec3 = new Mat();
            var tvec3 = new Mat();
            Cv2.ComposeRT(rvec1, tvec1, rvec2, tvec2, rvec3, tvec3);
            return Tuple.Create(rvec3, tvec3);
        }

        /// <summary>
        /// Projects 3D points to an image plane.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#projectpoints"/>
        /// <param name="objectPoints">Array of object points, $3\times N/N\times 3$ 1-channel or $1\times N/N\times 1$ 3-channel (or vector&lt;Point3f&gt;), where $N$ is the number of points in the view.</param>
        /// <param name="rvec">Rotation vector.</param>
        /// <param name="tvec">Translation vector.</param>
        /// <param name="cameraMatrix"> Camera matrix  $A = \vecthreethree{f_x}{0}{c_x}{0}{f_y}{c_y}{0}{0}{_1}$.</param>
        /// <param name="distCoeffs">nput vector of distortion coefficients  (k_1, k_2, p_1, p_2[, k_3[, k_4, k_5, k_6]]) of 4, 5, or 8 elements. If the vector is NULL/empty, the zero distortion coefficients are assumed.</param>
        /// <param name="aspectRatio">Optional “fixed aspect ratio” parameter. If the parameter is not 0, the function assumes that the aspect ratio (fx/fy) is fixed and correspondingly adjusts the jacobian matrix.</param>
        /// <returns>
        /// <return name="imagePoints">Output array of image points, 2xN/Nx2 1-channel or 1xN/Nx1 2-channel, or vector&lt;Point2f&gt;.</return>
        /// <return name="jacobian">Output $2N \times (10+&lt;numDistCoeffs&gt;) jacobian matrix of derivatives of image points with respect to components of the rotation vector, translation vector, focal lengths, coordinates of the principal point and the distortion coefficients. In the old interface different components of the jacobian are returned via different output parameters.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.ProjectPoints")]
        public static Tuple<Mat, Mat> ProjectPoints(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat objectPoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat rvec,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat tvec,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat cameraMatrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat distCoeffs,
            [InputPin(PropertyMode = PropertyMode.Default)] Double aspectRatio = 0
        )
        {
            var imagePoints = new Mat();
            var jacobian = new Mat();
            Cv2.ProjectPoints(objectPoints, rvec, tvec, cameraMatrix, distCoeffs, imagePoints, jacobian, aspectRatio);
            return Tuple.Create(imagePoints, jacobian);
        }

        /// <summary>
        /// Finds an object pose from 3D-2D point correspondences.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#solvepnp"/>
        /// <param name="objectPoints">Array of object points, $3\times N/N\times 3$ 1-channel or $1\times N/N\times 1$ 3-channel (or vector&lt;Point3f&gt;), where $N$ is the number of points in the view</param>
        /// <param name="imagePoints">Array of corresponding image points, $2\times N/N\times 2$ 1-channel or $1\times N/N\times 1$ 2-channel, where $N$ is the number of points. vector&lt;Point2f&gt; can be also passed here.</param>
        /// <param name="cameraMatrix">Input camera matrix  $A = \vecthreethree{fx}{0}{cx}{0}{fy}{cy}{0}{0}{1}$.</param>
        /// <param name="distCoeffs">Input vector of distortion coefficients  (k_1, k_2, p_1, p_2[, k_3[, k_4, k_5, k_6]]) of 4, 5, or 8 elements. If the vector is NULL/empty, the zero distortion coefficients are assumed.</param>
        /// <param name="rvec">Optional rotation vector (see Rodrigues() ) that, together with tvec , brings points from the model coordinate system to the camera coordinate system.</param>
        /// <param name="tvec">Optional translation vector.</param>
        /// <param name="useExtrinsicGuess">If true (1), the function uses the provided rvec and tvec values as initial approximations of the rotation and translation vectors, respectively, and further optimizes them.</param>
        /// <param name="flags">Method for solving a PnP problem.</param>
        /// <returns>
        /// <return name="rvec">Output rotation vector (see Rodrigues() ) that, together with tvec , brings points from the model coordinate system to the camera coordinate system.</return>
        /// <return name="tvec">Output translation vector.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.SolvePnP")]
        public static Tuple<Mat, Mat> SolvePnP(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat objectPoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat imagePoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat cameraMatrix,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat distCoeffs,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat rvec,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat tvec,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean useExtrinsicGuess = false,
            [InputPin(PropertyMode = PropertyMode.Default)] SolvePnPFlags flags = SolvePnPFlags.Iterative
        )
        {
            var rvecOutput = rvec.Clone();
            var tvecOutput = tvec.Clone();
            Cv2.SolvePnP(objectPoints, imagePoints, cameraMatrix, distCoeffs, rvecOutput, tvecOutput, useExtrinsicGuess, flags);
            return Tuple.Create(rvecOutput, tvecOutput);
        }

        ///// <summary>
        ///// computes the camera pose from a few 3D points and the corresponding projections. The outliers are possible.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#solvepnpransac"/>
        ///// <param name="objectPoints">Array of object points in the object coordinate space, $3\times N/N\times 3$ 1-channel or $1\times N/N\times 1$ 3-channel,
        ///// where N is the number of points.
        ///// <param name="imagePoints">Array of corresponding image points, 2xN/Nx2 1-channel or 1xN/Nx1 2-channel, where N is the number of points.</param>
        ///// <param name="cameraMatrix">Input $3\times 3$ camera matrix</param>
        ///// <param name="distCoeffs">Input vector of distortion coefficients (k_1, k_2, p_1, p_2[, k_3[, k_4, k_5, k_6]]) of 4, 5, or 8 elements.
        ///// If the vector is null, the zero distortion coefficients are assumed.</param>
        ///// <param name="rvec">Output rotation vector that, together with tvec , brings points from the model coordinate system
        ///// to the camera coordinate system.</param>
        ///// <param name="tvec">Output translation vector.</param>
        ///// <param name="useExtrinsicGuess">If true (1), the function uses the provided rvec and tvec values as initial approximations of the rotation and translation vectors, respectively, and further optimizes them.</param>
        ///// <param name="iterationsCount">Number of iterations.</param>
        ///// <param name="reprojectionError">Inlier threshold value used by the RANSAC procedure. The parameter value is the maximum allowed distance between the observed and computed point projections to consider it an inlier.</param>
        ///// <param name="minInliersCount">Number of inliers. If the algorithm at some stage finds more inliers than minInliersCount , it finishes.</param>
        ///// <param name="flags">Method for solving a PnP problem.</param>
        ///// <returns>
        ///// <return name="rvec">Output rotation vector (see Rodrigues() ) that, together with tvec , brings points from the model coordinate system to the camera coordinate system.</return>
        ///// <return name="tvec">Output translation vector.</return>
        ///// <return name="inliers">Output vector that contains indices of inliers in objectPoints and imagePoints .</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Calib3d.SolvePnPRansac", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        // public static Tuple<Mat, Mat, Mat> SolvePnPRansac(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat objectPoints,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat imagePoints,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat cameraMatrix,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat distCoeffs,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat rvec,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat tvec,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Boolean useExtrinsicGuess = false,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Int32 iterationsCount = 100,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Single reprojectionError = 8,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Int32 minInliersCount = 100,
        //    [InputPin(PropertyMode = PropertyMode.Default)] SolvePnPFlag flags = SolvePnPFlag.Iterative
        //)
        //{
        //    var inliers = new Mat();
        //    var rvecOutput = rvec.Clone();
        //    var tvecOutput = tvec.Clone();
        //    Cv2.SolvePnPRansac(objectPoints, imagePoints, cameraMatrix, distCoeffs, rvec, tvec, useExtrinsicGuess, iterationsCount, reprojectionError, minInliersCount, inliers, flags);
        //    return Tuple.Create(rvecOutput, tvecOutput, inliers);
        //}


        /// <summary>
        /// Finds an initial camera matrix from 3D-2D point correspondences.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#initcameramatrix2d"/>
        /// <param name="objectPoints"> Vector of vectors of the calibration pattern points in the calibration pattern coordinate space. In the old interface all the per-view vectors are concatenated.</param>
        /// <param name="imagePoints">Vector of vectors of the projections of the calibration pattern points. In the old interface all the per-view vectors are concatenated.</param>
        /// <param name="imageSize">Image size in pixels used to initialize the principal point.</param>
        /// <param name="aspectRatio">If it is zero or negative, both $f_x$ and $f_y$ are estimated independently. Otherwise, $f_x = f_y \cdot \text{aspectRatio}$ .</param>
        /// <returns>
        /// <return name="mat">Inital camera matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.InitCameraMatrix2D", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat InitCameraMatrix2D(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> objectPoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> imagePoints,
            [InputPin(PropertyMode = PropertyMode.Default)] Size imageSize,
            [InputPin(PropertyMode = PropertyMode.Default)] Double aspectRatio = 1
        )
        {
            return Cv2.InitCameraMatrix2D(objectPoints, imagePoints, imageSize, aspectRatio);
        }

        /// <summary>
        /// Finds the positions of internal corners of the chessboard.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#findchessboardcorners"/>
        /// <param name="image">Source chessboard view. It must be an 8-bit grayscale or color image.</param>
        /// <param name="patternSize">Number of inner corners per a chessboard row and column</param>
        /// <param name="flags">Various operation flags that can be zero or a combination.</param>
        /// <returns>
        /// <return name="corners">Output array of detected corners.</return>
        /// <return name="chessBoard">Returns true, if it could find all corners.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.FindChessboardCorners", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Boolean> FindChessboardCorners(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "8, 8")] Size patternSize,
            [InputPin(PropertyMode = PropertyMode.Default)] ChessboardFlags flags = ChessboardFlags.AdaptiveThresh
        )
        {
            var corners = new Mat();
            var chess = Cv2.FindChessboardCorners(image, patternSize, corners, flags);
            return Tuple.Create(corners, chess);
        }

        //TODO no docu found
        /// <summary>
        ///
        /// </summary>
        /// <param name="img"></param>
        /// <param name="regionSize"></param>
        /// <returns>
        /// <return name="mat"></return>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.Find4QuadCornerSubpix", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Boolean> Find4QuadCornerSubpix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Default)] Size regionSize
        )
        {
            var corners = new Mat();
            var result = Cv2.Find4QuadCornerSubpix(img, corners, regionSize);
            return Tuple.Create(corners, result);
        }

        /// <summary>
        /// Renders the detected chessboard corners.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#drawchessboardcorners"/>
        /// <param name="image">Input image. It must be an 8-bit color image.</param>
        /// <param name="patternSize">Number of inner corners per a chessboard row and column ($patternSize = cv::Size(points_per_row,points_per_column)$).</param>
        /// <param name="corners">Array of detected corners, the output of findChessboardCorners.</param>
        /// <param name="patternWasFound">Parameter indicating whether the complete board was found or not. The return value of findChessboardCorners() should be passed here.</param>
        /// <returns>
        /// <return name="image">Output image. It must be an 8-bit color image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.DrawChessboardCorners", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DrawChessboardCorners(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] Size patternSize,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat corners,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean patternWasFound
        )
        {
            var imageOutput = image.Clone();
            Cv2.DrawChessboardCorners(image, patternSize, corners, patternWasFound);
            return imageOutput;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="image"></param>
        /// <param name="patternSize"></param>
        /// <param name="flags"></param>
        /// <param name="blobDetector"></param>
        /// <returns>
        /// <return name="mat"></return>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.FindCirclesGrid", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Boolean> FindCirclesGrid(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] Size patternSize,
            [InputPin(PropertyMode = PropertyMode.Default)] FindCirclesGridFlags flags = FindCirclesGridFlags.SymmetricGrid,
            [InputPin(PropertyMode = PropertyMode.Default)] Feature2D blobDetector = null
        )
        {
            var centers = new Mat();
            var result = Cv2.FindCirclesGrid(image, patternSize, centers, flags, blobDetector);
            return Tuple.Create(centers, result);
        }

        /// <summary>
        /// Finds the camera intrinsic and extrinsic parameters from several views of a calibration pattern.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#calibratecamera"/>
        /// <param name="objectPoints">Is a vector of vectors of calibration pattern points in the calibration pattern coordinate space. The outer vector contains as many elements as the number of the pattern views. If the same calibration pattern is shown in each view and it is fully visible, all the vectors will be the same. Although, it is possible to use partially occluded patterns, or even different patterns in different views. Then, the vectors will be different. The points are 3D, but since they are in a pattern coordinate system, then, if the rig is planar, it may make sense to put the model to a XY coordinate plane so that Z-coordinate of each input object point is 0.</param>
        /// <param name="imagePoints">Is a vector of vectors of the projections of calibration pattern points. imagePoints.size() and objectPoints.size() and imagePoints[i].size() must be equal to objectPoints[i].size() for each i.</param>
        /// <param name="imageSize">Size of the image used only to initialize the intrinsic camera matrix.</param>
        /// <param name="cameraMatrix">Output $3\times 3$ floating-point camera matrix  $A = \vecthreethree{f_x}{0}{c_x}{0}{f_y}{c_y}{0}{0}{1}$. If CV_CALIB_USE_INTRINSIC_GUESS and/or CV_CALIB_FIX_ASPECT_RATIO are specified, some or all of fx, fy, cx, cy must be initialized before calling the function.</param>
        /// <param name="distCoeffs"></param>
        /// <param name="flags">Different flags that may be zero or a combination.</param>
        /// <param name="criteria">Termination criteria for the iterative optimization algorithm.</param>
        /// <returns>
        /// <return name="result">The final re-projection error.</return>
        /// <return name="rvecs">Output vector of rotation vectors (see Rodrigues() ) estimated for each pattern view. That is, each k-th rotation vector together with the corresponding k-th translation vector (see the next output parameter description) brings the calibration pattern from the model coordinate space (in which object points are specified) to the world coordinate space, that is, a real position of the calibration pattern in the k-th pattern view ($k=0.. M -1$).</return>
        /// <return name="tvecs">Output vector of translation vectors estimated for each pattern view.</return>
        /// <return name="cameraMatrix">Output $3\times 3$ floating-point camera matrix  $A = \vecthreethree{f_x}{0}{c_x}{0}{f_y}{c_y}{0}{0}{1}$. If CV_CALIB_USE_INTRINSIC_GUESS and/or CV_CALIB_FIX_ASPECT_RATIO are specified, some or all of fx, fy, cx, cy must be initialized before calling the function.</return>
        /// <return name="distCoeffs">Output vector of distortion coefficients (k_1, k_2, p_1, p_2[, k_3[, k_4, k_5, k_6]]) of 4, 5, or 8 elements.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.CalibrateCamera")]
        public static Tuple<Double, Mat[], Mat[], Mat, Mat> CalibrateCamera(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> objectPoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> imagePoints,
            [InputPin(PropertyMode = PropertyMode.Default)] Size imageSize,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat cameraMatrix = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat distCoeffs = null,
            [InputPin(PropertyMode = PropertyMode.Default)] CalibrationFlags flags = CalibrationFlags.None,
            [InputPin(PropertyMode = PropertyMode.Default)] TermCriteria? criteria = null
        )
        {
            Mat[] rvecs = new Mat[1];
            Mat[] tvecs = new Mat[1];
            var cameraMatrixOutput = cameraMatrix == null ? new Mat() : cameraMatrix.Clone();
            var distCoeffsOutput = distCoeffs == null ? new Mat() : distCoeffs.Clone();
            var result = Cv2.CalibrateCamera(objectPoints, imagePoints, imageSize, cameraMatrixOutput, distCoeffsOutput, out rvecs, out tvecs, flags, criteria);
            return Tuple.Create(result, rvecs, tvecs, cameraMatrixOutput, distCoeffsOutput);
        }

        /// <summary>
        /// Computes useful camera characteristics from the camera matrix.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#calibrationmatrixvalues"/>
        /// <param name="cameraMatrix">Input camera matrix that can be estimated by calibrateCamera() or stereoCalibrate() .</param>
        /// <param name="imageSize">Input image size in pixels.</param>
        /// <param name="apertureWidth">Physical width of the sensor.</param>
        /// <param name="apertureHeight">Physical height of the sensor.</param>
        /// <returns>
        /// <return name="fovx">field of view in degrees along the horizontal sensor axis.</return>
        /// <return name="fovy">field of view in degrees along the vertical sensor axis.</return>
        /// <return name="focalLength">Focal length of the lens in mm.</return>
        /// <return name="principalPoint">Principal point in pixels.</return>
        /// <return name="aspectRatio">$f_y/f_x$</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.CalibrationMatrixValues")]
        public static Tuple<Double, Double, Double, Point2d, Double> CalibrationMatrixValues(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat cameraMatrix,
            [InputPin(PropertyMode = PropertyMode.Default)] Size imageSize,
            [InputPin(PropertyMode = PropertyMode.Default)] Double apertureWidth,
            [InputPin(PropertyMode = PropertyMode.Default)] Double apertureHeight
        )
        {
            Double fovx, fovy, focalLength, aspectRatio;
            Point2d principalPoint;
            Cv2.CalibrationMatrixValues(cameraMatrix, imageSize, apertureWidth, apertureHeight, out fovx, out fovy, out focalLength, out principalPoint, out aspectRatio);
            return Tuple.Create(fovx, fovy, focalLength, principalPoint, aspectRatio);
        }

        /// <summary>
        /// Computes an optimal affine transformation between two 3D point sets.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html#estimateaffine3d"/>
        /// <param name="src">First input 3D point set.</param>
        /// <param name="dst">Second input 3D point set.</param>
        /// <param name="ransacThreshold">Maximum reprojection error in the RANSAC algorithm to consider a point as an inlier.</param>
        /// <param name="confidence">Confidence level, between 0 and 1, for the estimated transformation.
        /// Anything between 0.95 and 0.99 is usually good enough. Values too close to 1 can slow down the estimation significantly.
        /// Values lower than 0.8-0.9 can result in an incorrectly estimated transformation.</param>
        /// <returns>
        /// <return name="outVal">Output 3D affine transformation matrix 3 x 4 .</return>
        /// <return name="inliers">Output vector indicating which points are inliers.</return>
        /// <return name="ret"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Calib3d.EstimateAffine3D")]
        public static Tuple<Mat, Mat, Int32> EstimateAffine3D(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst,
            [InputPin(PropertyMode = PropertyMode.Default)] Double ransacThreshold = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] Double confidence = 0.99
        )
        {
            var outVal = new Mat();
            var inliers = new Mat();
            var ret = Cv2.EstimateAffine3D(src, dst, outVal, inliers, ransacThreshold, confidence);
            return Tuple.Create(outVal, inliers, ret);
        }

        /// <summary>
        /// Draw keypoints.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/drawing_function_of_keypoints_and_matches.html#drawkeypoints"/>
        /// <param name="image">Source image.</param>
        /// <param name="keypoints">Keypoints from the source image.</param>
        /// <param name="color">Color of keypoints.</param>
        /// <param name="flags">Flags setting drawing features. Possible flags bit values are defined by DrawMatchesFlags.</param>
        /// <returns>
        /// <return name="outImage">Output image. Its content depends on the flags value defining what is drawn in the output image. See possible flags bit values below.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.DrawKeypoints", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DrawKeypoints(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<KeyPoint> keypoints,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? color = null,
            [InputPin(PropertyMode = PropertyMode.Default)] DrawMatchesFlags flags = DrawMatchesFlags.Default
        )
        {
            var outImage = new Mat();
            Cv2.DrawKeypoints(image, keypoints, outImage, color, flags);
            return outImage;
        }

        /// <summary>
        /// Draws the found matches of keypoints from two images.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/drawing_function_of_keypoints_and_matches.html#drawmatches"/>
        /// <param name="img1">First source image.</param>
        /// <param name="keypoints1">Keypoints from the first source image.</param>
        /// <param name="img2">Second source image.</param>
        /// <param name="keypoints2">Keypoints from the second source image.</param>
        /// <param name="matches1To2">Matches from the first image to the second one, which means that keypoints1[i] has a corresponding point in keypoints2[matches[i]] .</param>
        /// <param name="matchColor">Color of matches (lines and connected keypoints). If matchColor==Scalar::all(-1) , the color is generated randomly.</param>
        /// <param name="singlePointColor">Color of single keypoints (circles), which means that keypoints do not have the matches. If singlePointColor==Scalar::all(-1) , the color is generated randomly.</param>
        /// <param name="matchesMask">Mask determining which matches are drawn. If the mask is empty, all matches are drawn.</param>
        /// <param name="flags">Flags setting drawing features. Possible flags bit values are defined by DrawMatchesFlags.</param>
        /// <returns>
        /// <return name="outImg">Output image. Its content depends on the flags value defining what is drawn in the output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.DrawMatches", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DrawMatches(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img1,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<KeyPoint> keypoints1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img2,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<KeyPoint> keypoints2,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<DMatch> matches1To2,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? matchColor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? singlePointColor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] IEnumerable<Byte> matchesMask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] DrawMatchesFlags flags = DrawMatchesFlags.Default
        )
        {
            var outImage = new Mat();
            Cv2.DrawMatches(img1, keypoints1, img2, keypoints2, matches1To2, outImage, matchColor, singlePointColor, matchesMask, flags);
            return outImage;
        }

        /// <summary>
        /// Draws the found matches of keypoints from two images.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/drawing_function_of_keypoints_and_matches.html#drawmatches"/>
        /// <param name="img1">First source image.</param>
        /// <param name="keypoints1">Keypoints from the first source image.</param>
        /// <param name="img2">Second source image.</param>
        /// <param name="keypoints2">Keypoints from the second source image.</param>
        /// <param name="matches1To2">Matches from the first image to the second one, which means that keypoints1[i] has a corresponding point in keypoints2[matches[i]] .</param>
        /// <param name="matchColor">Color of matches (lines and connected keypoints). If matchColor==Scalar::all(-1) , the color is generated randomly.</param>
        /// <param name="singlePointColor">Color of single keypoints (circles), which means that keypoints do not have the matches. If singlePointColor==Scalar::all(-1) , the color is generated randomly.</param>
        /// <param name="matchesMask">Mask determining which matches are drawn. If the mask is empty, all matches are drawn.</param>
        /// <param name="flags">Flags setting drawing features. Possible flags bit values are defined by DrawMatchesFlags.</param>
        /// <returns>
        /// <return name="outImg">Output image. Its content depends on the flags value defining what is drawn in the output image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.DrawManyMatches", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat DrawManyMatches(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img1,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<KeyPoint> keypoints1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img2,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<KeyPoint> keypoints2,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<IEnumerable<DMatch>> matches1To2,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? matchColor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? singlePointColor = null,
            [InputPin(PropertyMode = PropertyMode.Default)] IEnumerable<IEnumerable<Byte>> matchesMask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] DrawMatchesFlags flags = DrawMatchesFlags.Default
        )
        {
            var outImage = new Mat();
            Cv2.DrawMatches(img1, keypoints1, img2, keypoints2, matches1To2, outImage, matchColor, singlePointColor, matchesMask, flags);
            return outImage;
        }

        /// <summary>
        /// Module for extracting blobs from an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_feature_detectors.html#simpleblobdetector"/>
        /// <param name="thresholdStep">Convert the source image to binary images by applying thresholding with several thresholds from minThreshold (inclusive) to maxThreshold (exclusive) with distance thresholdStep between neighboring thresholds.</param>
        /// <param name="minThreshold">Convert the source image to binary images by applying thresholding with several thresholds from minThreshold (inclusive) to maxThreshold (exclusive) with distance thresholdStep between neighboring thresholds.</param>
        /// <param name="maxThreshold">Convert the source image to binary images by applying thresholding with several thresholds from minThreshold (inclusive) to maxThreshold (exclusive) with distance thresholdStep between neighboring thresholds.</param>
        /// <param name="minRepeatability"></param>
        /// <param name="minDistBetweenBlobs">Group centers from several binary images by their coordinates. Close centers form one group that corresponds to one blob, which is controlled by the minDistBetweenBlobs parameter.</param>
        /// <param name="filterByColor">This filter compares the intensity of a binary image at the center of a blob to blobColor. If they differ, the blob is filtered out. Use $blobColor = 0$ to extract dark blobs and $blobColor = 255$ to extract light blobs.</param>
        /// <param name="blobColor"></param>
        /// <param name="filterByArea">Extracted blobs have an area between minArea (inclusive) and maxArea (exclusive).</param>
        /// <param name="minArea"></param>
        /// <param name="maxArea"></param>
        /// <param name="filterByCircularity">Extracted blobs have circularity ($\frac{4*\pi*Area}{perimeter * perimeter}$) between minCircularity (inclusive) and maxCircularity (exclusive).</param>
        /// <param name="minCircularity"></param>
        /// <param name="maxCircularity"></param>
        /// <param name="filterByInertia">Extracted blobs have this ratio between minInertiaRatio (inclusive) and maxInertiaRatio (exclusive).</param>
        /// <param name="minInertiaRatio"></param>
        /// <param name="maxInertiaRatio"></param>
        /// <param name="filterByConvexity">Extracted blobs have convexity (area / area of blob convex hull) between minConvexity (inclusive) and maxConvexity (exclusive).</param>
        /// <param name="minConvexity"></param>
        /// <param name="maxConvexity"></param>
        /// <returns>
        /// <return name="blob">blob instance.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.SimpleBlob")]
        public static Feature2D SimpleBlob(
            [InputPin(PropertyMode = PropertyMode.Default)]  float thresholdStep = 10,
            [InputPin(PropertyMode = PropertyMode.Default)] float minThreshold = 50,
            [InputPin(PropertyMode = PropertyMode.Default)] float maxThreshold = 220,
            [InputPin(PropertyMode = PropertyMode.Default)] uint minRepeatability = 2,
            [InputPin(PropertyMode = PropertyMode.Default)] float minDistBetweenBlobs = 10,
            [InputPin(PropertyMode = PropertyMode.Default)] bool filterByColor = true,
            [InputPin(PropertyMode = PropertyMode.Default)] byte blobColor = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] bool filterByArea = true,
            [InputPin(PropertyMode = PropertyMode.Default)] float minArea = 25,
            [InputPin(PropertyMode = PropertyMode.Default)] float maxArea = 5000,
            [InputPin(PropertyMode = PropertyMode.Default)] bool filterByCircularity = false,
            [InputPin(PropertyMode = PropertyMode.Default)] float minCircularity = 0.8f,
            [InputPin(PropertyMode = PropertyMode.Default)] float maxCircularity = Single.MaxValue,
            [InputPin(PropertyMode = PropertyMode.Default)] bool filterByInertia = true,
            [InputPin(PropertyMode = PropertyMode.Default)] float minInertiaRatio = 0.1f,
            [InputPin(PropertyMode = PropertyMode.Default)] float maxInertiaRatio = Single.MaxValue,
            [InputPin(PropertyMode = PropertyMode.Default)] bool filterByConvexity = true,
            [InputPin(PropertyMode = PropertyMode.Default)] float minConvexity = 0.95f,
            [InputPin(PropertyMode = PropertyMode.Default)] float maxConvexity = Single.MaxValue
            )
        {
            var paras = new SimpleBlobDetector.Params();
            paras.ThresholdStep = thresholdStep;
            paras.MinThreshold = minThreshold;
            paras.MaxThreshold = maxThreshold;
            paras.MinRepeatability = minRepeatability;
            paras.MinDistBetweenBlobs = minDistBetweenBlobs;
            paras.FilterByColor = filterByColor;
            paras.BlobColor = blobColor;
            paras.FilterByArea = filterByArea;
            paras.MinArea = minArea;
            paras.MaxArea = maxArea;
            paras.FilterByCircularity = filterByCircularity;
            paras.MinCircularity = minCircularity;
            paras.MaxCircularity = maxCircularity;
            paras.FilterByInertia = filterByInertia;
            paras.MinInertiaRatio = minInertiaRatio;
            paras.MaxInertiaRatio = maxInertiaRatio;
            paras.FilterByConvexity = filterByConvexity;
            paras.MinConvexity = minConvexity;
            paras.MaxConvexity = maxConvexity;

            return SimpleBlobDetector.Create(paras);
        }

        /// <summary>
        /// Determines strong corners on an image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_feature_detectors.html#goodfeaturestotrackdetector"/>
        /// <param name="maxCorners">Maximum number of corners to return. If there are more corners than are found, the strongest of them is returned.</param>
        /// <param name="qualityLevel">Parameter characterizing the minimal accepted quality of image corners. The parameter value is multiplied by the best corner quality measure, which is the minimal eigenvalue (see cornerMinEigenVal() ) or the Harris function response (see cornerHarris() ).
        /// The corners with the quality measure less than the product are rejected. For example, if the best corner has the quality $measure = 1500$, and the $qualityLevel=0.01$, then all the corners with the quality measure less than 15 are rejected.</param>
        /// <param name="minDistance">Minimum possible Euclidean distance between the returned corners.</param>
        /// <param name="blockSize">Size of an average block for computing a derivative covariation matrix over each pixel neighborhood. See cornerEigenValsAndVecs() .</param>
        /// <param name="useHarrisDetector">Parameter indicating whether to use a Harris detector (see cornerHarris()) or cornerMinEigenVal().</param>
        /// <param name="k">Free parameter of the Harris detector.</param>
        /// <returns>
        /// <return name="gftt">Good features detector to track instance.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.Gftt")]
        public static Feature2D Gftt(
            [InputPin(PropertyMode = PropertyMode.Default)] int maxCorners = 1000,
            [InputPin(PropertyMode = PropertyMode.Default)] double qualityLevel = 0.01,
            [InputPin(PropertyMode = PropertyMode.Default)] double minDistance = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] int blockSize = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] bool useHarrisDetector = false,
            [InputPin(PropertyMode = PropertyMode.Default)] double k = 0.04
            )
        {
            return GFTTDetector.Create(maxCorners, qualityLevel, minDistance, blockSize, useHarrisDetector, k);
        }

        /// <summary>
        /// Maximally stable extremal region extractor.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/feature_detection_and_description.html#mser"/>
        /// <param name="delta"> delta, in the code, it compares $(size_{i}-size_{i-delta})/size_{i-delta}$</param>
        /// <param name="minArea">prune the area which smaller than min_area</param>
        /// <param name="maxArea">prune the area which bigger than max_area</param>
        /// <param name="maxVariation">prune the area have simliar size to its children</param>
        /// <param name="minDiversity">trace back to cut off mser with $diversity \le min_diversity$</param>
        /// <param name="maxEvolution">for color image, the evolution steps</param>
        /// <param name="areaThreshold">the area threshold to cause re-initialize</param>
        /// <param name="minMargin">ignore too small margin</param>
        /// <param name="edgeBlurSize">the aperture size for edge blur</param>
        /// <returns>
        /// <return name="mser">MSER feature detector instance.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.Mser")]
        public static Feature2D Mser(
            [InputPin(PropertyMode = PropertyMode.Default)] int delta = 5,
            [InputPin(PropertyMode = PropertyMode.Default)] int minArea = 60,
            [InputPin(PropertyMode = PropertyMode.Default)] int maxArea = 14400,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxVariation = 0.25,
            [InputPin(PropertyMode = PropertyMode.Default)] double minDiversity = 0.2,
            [InputPin(PropertyMode = PropertyMode.Default)] int maxEvolution = 200,
            [InputPin(PropertyMode = PropertyMode.Default)] double areaThreshold = 1.01,
            [InputPin(PropertyMode = PropertyMode.Default)] double minMargin = 0.003,
            [InputPin(PropertyMode = PropertyMode.Default)] int edgeBlurSize = 5
            )
        {
            return MSER.Create(delta, minArea, maxArea, maxVariation, minDiversity, maxEvolution, areaThreshold, minMargin, edgeBlurSize);
        }

        /// <summary>
        /// The ORB constructor.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/feature_detection_and_description.html#orb-orb"/>
        /// <param name="nFeatures">The maximum number of features to retain.</param>
        /// <param name="scaleFactor">Pyramid decimation ratio, greater than 1. $scaleFactor==2$ means the classical pyramid, where each next level has $4 \times$ less pixels than the previous, but such a big scale factor will degrade feature matching scores dramatically.
        /// On the other hand, too close to 1 scale factor will mean that to cover certain scale range you will need more pyramid levels and so the speed will suffer.</param>
        /// <param name="nLevels">The number of pyramid levels. The smallest level will have linear size equal to input_image_linear_size/pow(scaleFactor, nlevels).</param>
        /// <param name="edgeThreshold">This is size of the border where the features are not detected. It should roughly match the patchSize parameter.</param>
        /// <param name="firstLevel">It should be 0 in the current implementation.</param>
        /// <param name="wtaK">The number of points that produce each element of the oriented BRIEF descriptor. The default value 2 means the BRIEF where we take a random point pair and compare their brightnesses, so we get 0/1 response. Other possible values are 3 and 4.
        /// For example, 3 means that we take 3 random points (of course, those point coordinates are random, but they are generated from the pre-defined seed, so each element of BRIEF descriptor is computed deterministically from the pixel rectangle),
        /// find point of maximum brightness and output index of the winner (0, 1 or 2). Such output will occupy 2 bits, and therefore it will need a special variant of Hamming distance, denoted as NORM_HAMMING2 (2 bits per bin).
        /// When $WTA_K=4$, we take 4 random points to compute each bin (that will also occupy 2 bits with possible values 0, 1, 2 or 3).</param>
        /// <param name="scoreType">The default HARRIS_SCORE means that Harris algorithm is used to rank features (the score is written to KeyPoint::score and is used to retain best nfeatures features); FAST_SCORE is alternative value of the parameter that produces slightly less stable keypoints, but it is a little faster to compute.</param>
        /// <param name="patchSize">size of the patch used by the oriented BRIEF descriptor. Of course, on smaller pyramid layers the perceived image area covered by a feature will be larger.</param>
        /// <returns>
        /// <return name="orb">ORB feature detector instance.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.Orb")]
        public static Feature2D Orb(
            [InputPin(PropertyMode = PropertyMode.Default)] int nFeatures = 500,
            [InputPin(PropertyMode = PropertyMode.Default)] float scaleFactor = 1.2f,
            [InputPin(PropertyMode = PropertyMode.Default)] int nLevels = 8,
            [InputPin(PropertyMode = PropertyMode.Default)] int edgeThreshold = 31,
            [InputPin(PropertyMode = PropertyMode.Default)] int firstLevel = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] int wtaK = 2,
            [InputPin(PropertyMode = PropertyMode.Default)] ORBScore scoreType = ORBScore.Harris,
            [InputPin(PropertyMode = PropertyMode.Default)] int patchSize = 31
            )
        {
            return ORB.Create(nFeatures, scaleFactor, nLevels, edgeThreshold, firstLevel, wtaK, scoreType, patchSize);
        }


        ///// <summary>
        ///// The SURF extractor constructors.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/nonfree/doc/feature_detection.html#surf"/>
        ///// <param name="hessianThreshold">Threshold for hessian keypoint detector used in SURF.</param>
        ///// <param name="nOctaves"> Number of pyramid octaves the keypoint detector will use.</param>
        ///// <param name="nOctaveLayers">Number of octave layers within each octave.</param>
        ///// <param name="extended">Extended descriptor flag (true - use extended 128-element descriptors; false - use 64-element descriptors).</param>
        ///// <param name="upright">Up-right or rotated features flag (true - do not compute orientation of features; false - compute orientation).</param>
        ///// <returns>
        ///// <return name="surf">SURF feature detector instance.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Features2d.Surf")]
        // public static FeatureDetector Surf(
        //    [InputPin(PropertyMode = PropertyMode.Default)] int hessianThreshold = 400,
        //    [InputPin(PropertyMode = PropertyMode.Default)] int nOctaves = 4,
        //    [InputPin(PropertyMode = PropertyMode.Default)] int nOctaveLayers = 2,
        //    [InputPin(PropertyMode = PropertyMode.Default)] bool extended = true,
        //    [InputPin(PropertyMode = PropertyMode.Default)] bool upright = false
        //    )
        //{
        //    return new SURF(hessianThreshold, nOctaves, nOctaveLayers, extended, upright);
        //}

        /// <summary>
        /// Detects corners using the FAST algorithm
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/feature_detection_and_description.html#fast"/>
        /// <param name="threshold">threshold on difference between intensity of the central pixel and pixels of a circle around this pixel.</param>
        /// <param name="nonmaxSuppression">if true, non-maximum suppression is applied to detected corners (keypoints).</param>
        /// <returns>
        /// <return name="fast">FAST feature detector instance.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.Fast")]
        public static FastFeatureDetector Fast(
            [InputPin(PropertyMode = PropertyMode.Default)] int threshold = 10,
            [InputPin(PropertyMode = PropertyMode.Default)] bool nonmaxSuppression = true
            )
        {
            return FastFeatureDetector.Create(threshold, nonmaxSuppression);
        }

        ///// <summary>
        ///// The SIFT constructor. (feature detect)
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/nonfree/doc/feature_detection.html#sift-sift"/>
        ///// <param name="nFeatures">The number of best features to retain. The features are ranked by their scores (measured in SIFT algorithm as the local contrast)</param>
        ///// <param name="nOctaveLayers">The number of layers in each octave. 3 is the value used in D. Lowe paper. The number of octaves is computed automatically from the image resolution.</param>
        ///// <param name="contrastThreshold">The contrast threshold used to filter out weak features in semi-uniform (low-contrast) regions. The larger the threshold, the less features are produced by the detector.</param>
        ///// <param name="edgeThreshold">The threshold used to filter out edge-like features. Note that the its meaning is different from the contrastThreshold, i.e. the larger the edgeThreshold, the less features are filtered out (more features are retained).</param>
        ///// <param name="sigma">The sigma of the Gaussian applied to the input image at the octave #0. If your image is captured with a weak camera with soft lenses, you might want to reduce the number.</param>
        ///// <returns>
        ///// <return name="sift">SIFT feature detector instance.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Features2d.Sift")]
        // public static FeatureDetector Sift(
        //    [InputPin(PropertyMode = PropertyMode.Default)] int nFeatures = 0,
        //    [InputPin(PropertyMode = PropertyMode.Default)] int nOctaveLayers = 3,
        //    [InputPin(PropertyMode = PropertyMode.Default)] double contrastThreshold = 0.04,
        //    [InputPin(PropertyMode = PropertyMode.Default)] double edgeThreshold = 10,
        //    [InputPin(PropertyMode = PropertyMode.Default)] double sigma = 1.6
        //    )
        //{
        //    return new SIFT(nFeatures, nOctaveLayers, contrastThreshold, edgeThreshold, sigma);
        //}

        /// <summary>
        /// Brisk constructor
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/feature_detection_and_description.html#brisk-brisk"/>
        /// <param name="thresh">FAST/AGAST detection threshold score.</param>
        /// <param name="octaves">detection octaves. Use 0 to do single scale.</param>
        /// <param name="patternScale">apply this scale to the pattern used for sampling the neighbourhood of a keypoint.</param>
        /// <returns>
        /// <return name="brisk">BRISK feature detector instance.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.Brisk")]
        public static OpenCvSharp.BRISK Brisk(
            [InputPin(PropertyMode = PropertyMode.Default)] int thresh = 30,
            [InputPin(PropertyMode = PropertyMode.Default)] int octaves = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] float patternScale = 1.0f
            )
        {
            return OpenCvSharp.BRISK.Create(thresh, octaves, patternScale);
        }

        /// <summary>
        /// FREAK (Fast Retina Keypoint) keypoint descriptor.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/feature_detection_and_description.html#freak"/>
        /// <param name="orientationNormalized"></param>
        /// <param name="scaleNormalized"></param>
        /// <param name="patternScale"></param>
        /// <param name="nOctaves"></param>
        /// <param name="selectedPairs"></param>
        /// <returns>
        /// <return name="freak">Freak feature descriptor.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.Freak")]
        public static OpenCvSharp.XFeatures2D.FREAK Freak(
            [InputPin(PropertyMode = PropertyMode.Default)] bool orientationNormalized = true,
            [InputPin(PropertyMode = PropertyMode.Default)] bool scaleNormalized = true,
            [InputPin(PropertyMode = PropertyMode.Default)] float patternScale = 22f,
            [InputPin(PropertyMode = PropertyMode.Default)] int nOctaves = 4,
            [InputPin(PropertyMode = PropertyMode.Default)] IEnumerable<int> selectedPairs = null

            )
        {
            return OpenCvSharp.XFeatures2D.FREAK.Create(orientationNormalized, scaleNormalized, patternScale, nOctaves, selectedPairs);
        }

        /// <summary>
        /// Star detector constructor.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_feature_detectors.html?highlight=stardetector#starfeaturedetector"/>
        /// <param name="maxSize">maximum size of the features. The following values are supported: 4, 6, 8, 11, 12, 16, 22, 23, 32, 45, 46, 64, 90, 128. In the case of a different value the result is undefined.</param>
        /// <param name="responseThreshold">threshold for the approximated laplacian, used to eliminate weak features. The larger it is, the less features will be retrieved</param>
        /// <param name="lineThresholdProjected">another threshold for the laplacian to eliminate edges</param>
        /// <param name="lineThresholdBinarized">yet another threshold for the feature size to eliminate edges. The larger the 2nd threshold, the more points you get.</param>
        /// <param name="suppressNonmaxSize">If it is true, non-maximum supression is applied to detected corners (keypoints).</param>
        /// <returns>
        /// <return name="star">Star detector.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.StarDetector")]
        public static OpenCvSharp.XFeatures2D.StarDetector StarDetector(
            [InputPin(PropertyMode = PropertyMode.Default)] int maxSize = 45,
            [InputPin(PropertyMode = PropertyMode.Default)] int responseThreshold = 30,
            [InputPin(PropertyMode = PropertyMode.Default)] int lineThresholdProjected = 10,
            [InputPin(PropertyMode = PropertyMode.Default)] int lineThresholdBinarized = 8,
            [InputPin(PropertyMode = PropertyMode.Default)] int suppressNonmaxSize = 5
           )
        {
            return OpenCvSharp.XFeatures2D.StarDetector.Create(maxSize, responseThreshold, lineThresholdProjected, lineThresholdBinarized, suppressNonmaxSize);
        }


        /// <summary>
        /// detector using abstract interface
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_feature_detectors.html#featuredetector"/>
        /// <param name="image">Image.</param>
        /// <param name="featureDetector"></param>
        /// <param name="mask">Mask specifying where to look for keypoints (optional). It must be a 8-bit integer matrix with non-zero values in the region of interest.</param>
        /// <returns>
        /// <return name="keyPoints">The detected keypoints.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.FeatureDetector")]
        public static KeyPoint[] FeatureDetector(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] Feature2D featureDetector,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null
            )
        {
            return featureDetector.Detect(image, mask == null ? new Mat() : mask);
        }

        ///// <summary> // use select on sequence instead for arrays
        ///// detector using abstract interface
        ///// </summary>
        ///// <param name="image"></param>
        ///// <param name="featureDetector"></param>
        ///// <returns>
        ///// <return name="keyPoints">Key points.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Features2d.FeatureDetectorMultiple")]
        // public static KeyPoint[][] FeatureDetectorMultiple(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat[] image,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] FeatureDetector featureDetector
        //    )
        //{
        //    return featureDetector.Detect(image);
        //}

        /// <summary>
        /// Computes the descriptors for a set of keypoints detected in an image (first variant) or image set (second variant).
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/features2d/doc/common_interfaces_of_descriptor_extractors.html#descriptorextractor-compute"/>
        /// <param name="image">Image.</param>
        /// <param name="keypoints">Input collection of keypoints. Keypoints for which a descriptor cannot be computed are removed. Sometimes new keypoints can be added, for example: SIFT duplicates keypoint with several dominant orientations (for each orientation).</param>
        /// <param name="descriptorExtractor">Used feature detector method.</param>
        /// <returns>
        /// <return name="descriptors">Computed descriptors. In the second variant of the method descriptors[i] are descriptors computed for a keypoints[i]. Row j is the keypoints (or keypoints[i]) is the descriptor for keypoint j-th keypoint.</return>
        /// <return name="keypoints">Output collection of keypoints.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.DescriptorCompute")]
        public static Tuple<Mat, KeyPoint[]> DescriptorCompute(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Allow)] KeyPoint[] keypoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] Feature2D descriptorExtractor
            )
        {
            var descriptors = new Mat();
            var keypointsOutput = new KeyPoint[keypoints.Count()];
            keypoints.CopyTo(keypointsOutput, 0);
            var des = descriptorExtractor;
            des.Compute(image, ref keypointsOutput, descriptors);
            return Tuple.Create(descriptors, keypointsOutput);
        }

        /// <summary>
        /// Brute Force Matcher
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_descriptor_matchers.html#bfmatcher-bfmatcher"/>
        /// <param name="normtype">One of NORM_L1, NORM_L2, NORM_HAMMING, NORM_HAMMING2. L1 and L2 norms are preferable choices for SIFT and SURF descriptors, NORM_HAMMING should be used with ORB, BRISK and BRIEF, NORM_HAMMING2 should be used with ORB when $WTA_K==3$ or $4$ (see ORB::ORB constructor description).</param>
        /// <param name="crossCheck">If it is false, this is will be default BFMatcher behaviour when it finds the k nearest neighbors for each query descriptor. If $crossCheck==true$, then the knnMatch() method with $k=$1 will only return pairs $(i,j)$ such that for $i$-th query descriptor the $j$-th descriptor in the matcher’s collection is the nearest and vice versa, i.e. the BFMathcher will only return consistent pairs. Such technique usually produces best results with minimal number of outliers when there are enough matches. This is alternative to the ratio test, used by D. Lowe in SIFT paper.</param>
        /// <returns>
        /// <return name="bfMatcher">BF Matcher</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.BfMatcher")]
        public static DescriptorMatcher BfMatcher(
            [InputPin(PropertyMode = PropertyMode.Default)] NormTypes normtype = NormTypes.L2,
            [InputPin(PropertyMode = PropertyMode.Default)] bool crossCheck = false
            )
        {
            return new BFMatcher(normtype, crossCheck);
        }

        //TODO not working
        /// <summary>
        /// Flann-based descriptor matcher. This matcher trains flann::Index_ on a train descriptor collection and calls its nearest search methods to find the best matches. So, this matcher may be faster when matching a large train collection than the brute force matcher. FlannBasedMatcher does not support masking permissible matches of descriptor sets because flann::Index does not support this.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_descriptor_matchers.html#flannbasedmatcher"/>
        /// <returns>
        /// <return name="flannMatcher">Flann Matcher</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.FlannMatcher")]
        public static DescriptorMatcher FlannMatcher(
            )
        {
            return new FlannBasedMatcher();
        }

        /// <summary>
        /// Finds the best match for each descriptor from a query set.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_descriptor_matchers.html#descriptormatcher-match"/>
        /// <param name="descriptorMatcher">Descriptor matcher.</param>
        /// <param name="queryDescriptors">Query set of descriptors.</param>
        /// <param name="trainDescriptors">Train set of descriptors.</param>
        /// <param name="mask">Mask specifying permissible matches between an input query and train matrices of descriptors.</param>
        /// <returns>
        /// <return name="matches">Matches. If a query descriptor is masked out in mask , no match is added for this descriptor. So, matches size may be smaller than the query descriptors count.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.Match")]
        public static DMatch[] Match(
            [InputPin(PropertyMode = PropertyMode.Allow)] DescriptorMatcher descriptorMatcher,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat queryDescriptors,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat trainDescriptors,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null
            )
        {
            return descriptorMatcher.Match(queryDescriptors, trainDescriptors, mask == null ? new Mat() : mask);
        }

        /// <summary>
        /// Finds the k best matches for each descriptor from a query set.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_descriptor_matchers.html#descriptormatcher-knnmatch"/>
        /// <param name="descriptorMatcher">Descriptor matcher.</param>
        /// <param name="queryDescriptors">Query set of descriptors.</param>
        /// <param name="trainDescriptors">Train set of descriptors.</param>
        /// <param name="k">count of the best matches</param>
        /// <param name="mask">Mask specifying permissible matches between an input query and train matrices of descriptors.</param>
        /// <param name="compactResult">Parameter used when the mask (or masks) is not empty. If compactResult is false, the matches vector has the same size as queryDescriptors rows. If compactResult is true, the matches vector does not contain matches for fully masked-out query descriptors.</param>
        /// <returns>
        /// <return name="matches">Matches. Each matches[i] is k or less matches for the same query descriptor.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.KnnMatch")]
        public static DMatch[][] KnnMatch(
            [InputPin(PropertyMode = PropertyMode.Allow)] DescriptorMatcher descriptorMatcher,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat queryDescriptors,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat trainDescriptors,
            [InputPin(PropertyMode = PropertyMode.Default)] int k,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool compactResult = false
            )
        {
            return descriptorMatcher.KnnMatch(queryDescriptors, trainDescriptors, k, mask == null ? new Mat() : mask, compactResult);
        }

        /// <summary>
        /// For each query descriptor, finds the training descriptors not farther than the specified distance.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/features2d/doc/common_interfaces_of_descriptor_matchers.html#descriptormatcher-radiusmatch"/>
        /// <param name="descriptorMatcher">Descriptor matcher.</param>
        /// <param name="queryDescriptors">Query set of descriptors.</param>
        /// <param name="trainDescriptors">Train set of descriptors. This set is not added to the train descriptors collection stored in the class object.</param>
        /// <param name="maxDistance">Threshold for the distance between matched descriptors. Distance means here metric distance (e.g. Hamming distance), not the distance between coordinates (which is measured in Pixels)!</param>
        /// <param name="mask">Mask specifying permissible matches between an input query and train matrices of descriptors.</param>
        /// <param name="compactResult">Parameter used when the mask (or masks) is not empty. If compactResult is false, the matches vector has the same size as queryDescriptors rows. If compactResult is true, the matches vector does not contain matches for fully masked-out query descriptors.</param>
        /// <returns>
        /// <return name="matches">For each query descriptor, the methods find such training descriptors that the distance between the query descriptor and the training descriptor is equal or smaller than maxDistance. Found matches are returned in the distance increasing order.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.RadiusMatch")]
        public static DMatch[][] RadiusMatch(
            [InputPin(PropertyMode = PropertyMode.Allow)] DescriptorMatcher descriptorMatcher,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat queryDescriptors,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat trainDescriptors,
            [InputPin(PropertyMode = PropertyMode.Default)] float maxDistance,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool compactResult = false
            )
        {
            return descriptorMatcher.RadiusMatch(queryDescriptors, trainDescriptors, maxDistance, mask, compactResult);
        }

        /// <summary>
        /// Creates the HOG descriptor and detector.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/gpu/doc/object_detection.html?highlight=hog#gpu-hogdescriptor-hogdescriptor"/>
        /// <param name="winSize">Detection window size. Align to block size and block stride.</param>
        /// <param name="blockSize">Block size in pixels. Align to cell size. Only (16,16) is supported for now.</param>
        /// <param name="blockStride">Block stride. It must be a multiple of cell size.</param>
        /// <param name="cellSize">Cell size. Only (8, 8) is supported for now.</param>
        /// <param name="nbins">Number of bins. Only 9 bins per cell are supported for now.</param>
        /// <param name="derivAperture"></param>
        /// <param name="winSigma">Gaussian smoothing window parameter.</param>
        /// <param name="histogramNormType"></param>
        /// <param name="l2HysThreshold">L2-Hys normalization method shrinkage.</param>
        /// <param name="gammaCorrection">Flag to specify whether the gamma correction preprocessing is required or not.</param>
        /// <param name="nlevels">Maximum number of detection window increases.</param>
        /// <returns>
        /// <return name="hogDescriptor">Returns HOG descriptor and detector.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.HogDescriptor")]
        public static HOGDescriptor HogDescriptor(
            [InputPin(PropertyMode = PropertyMode.Default)] Size winSize,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "16, 16")] Size blockSize,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "8, 8")] Size blockStride,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "8, 8")] Size cellSize,
            [InputPin(PropertyMode = PropertyMode.Default)] int nbins = 9,
            [InputPin(PropertyMode = PropertyMode.Default)] int derivAperture = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double winSigma = -1,
            [InputPin(PropertyMode = PropertyMode.Default)] HistogramNormType histogramNormType = HistogramNormType.L2Hys,
            [InputPin(PropertyMode = PropertyMode.Default)] double l2HysThreshold = 0.2,
            [InputPin(PropertyMode = PropertyMode.Default)] bool gammaCorrection = true,
            [InputPin(PropertyMode = PropertyMode.Default)] int nlevels = 64
            )
        {
            return new HOGDescriptor(winSize, blockSize, blockStride, cellSize, nbins, derivAperture, winSigma, histogramNormType, l2HysThreshold, gammaCorrection, nlevels);
        }

        /// <summary>
        /// Sets coefficients for the linear SVM classifier.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/gpu/doc/object_detection.html?highlight=hog#gpu-hogdescriptor-setsvmdetector"/>
        /// <param name="hogDescriptor"></param>
        /// <param name="svmDetector">Coefficients</param>
        /// <returns>
        /// <return name="hogDescriptor"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.SetSvmDetector", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static HOGDescriptor SetSvmDetector(
            [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
            [InputPin(PropertyMode = PropertyMode.Allow)] float[] svmDetector
            )
        {
            var hogDescriptorOutput = hogDescriptor;
            hogDescriptorOutput.SetSVMDetector(svmDetector);
            return hogDescriptorOutput;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="img"></param>
        ///// <param name="locations"></param>
        ///// <param name="hogDescriptor"></param>
        ///// <param name="hitThreshold"></param>
        ///// <param name="winStride"></param>
        ///// <param name="padding"></param>
        ///// <returns>
        ///// <return name="flArr"></return>
        ///// <return name="cArr"></return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.ImgProc.HogDetectRoi", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        // public static Tuple<Point[], double[]> HogDetectRoi(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Point[] locations,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
        //    [InputPin(PropertyMode = PropertyMode.Default)] double hitThreshold = 0,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Size? winStride = null,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Size? padding = null
        //    )
        //{
        //    Size winStride0 = winStride.GetValueOrDefault(new Size());
        //    Size padding0 = padding.GetValueOrDefault(new Size());
        //    Point[] flArr = new Point[1];
        //    Double[] cArr = new double[1];
        //    hogDescriptor.DetectROI(img, locations, out flArr, out cArr, hitThreshold, winStride, padding);
        //    return Tuple.Create(flArr, cArr);
        //}



        /// <summary>
        /// Load new HOG descriptor.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/gpu/doc/object_detection.html?highlight=hog#gpu-cascadeclassifier-gpu-load"/>
        /// <param name="modelFile">Path to file.</param>
        /// <returns>
        /// <return name="hogDescriptor">New HOG descriptor.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.HogLoad")]
        public static HOGDescriptor HogLoad(
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string modelFile
            )
        {
            HOGDescriptor hogDescriptor = new HOGDescriptor();
            hogDescriptor.Load(modelFile);
            return hogDescriptor;
        }

        /// <summary>
        /// Returns coefficients of the classifier trained for people detection (for default window size).
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/gpu/doc/object_detection.html?highlight=hog#gpu-hogdescriptor-getdefaultpeopledetector"/>
        /// <returns>
        /// <return name="default">Default people detector.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.GetDefaultPeopleDetector")]
        public static float[] GetDefaultPeopleDetector(
            )
        {
            return HOGDescriptor.GetDefaultPeopleDetector();
        }

        /// <summary>
        ///  Returns coefficients of the classifier trained for people detection (for default window size).
        /// </summary>
        /// <returns>
        /// <return name="daimler">Daimler people detector.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.GetDaimlerPeopleDetector")]
        public static float[] GetDaimlerPeopleDetector(
            )
        {
            return HOGDescriptor.GetDaimlerPeopleDetector();
        }

        /// <summary>
        /// Saves a HOG model.
        /// </summary>
        /// <param name="hogDescriptor">The Hog model.</param>
        /// <param name="path">Path to file.</param>
        [StaticModule(ModuleType = "OpenCv.Features2d.HogSave")]
        public static void HogSave(
           [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
           [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path
           )
        {
            hogDescriptor.Save(path);
        }

        /// <summary>
        /// Read Alt HOG Model.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns>
        /// <return name="hogDescriptor">New HOG descriptor.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.HogReadAltModel", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static HOGDescriptor HogReadAltModel(
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path
            )
        {
            HOGDescriptor hogDescriptor = new HOGDescriptor();
            hogDescriptor.ReadALTModel(path);
            return hogDescriptor;
        }

        //// <param name="padding">Mock parameter to keep the CPU interface compatibility. It must be (0,0).</param>

        /// <summary>
        /// Performs object detection without a multi-scale window.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/gpu/doc/object_detection.html?highlight=hog#gpu-hogdescriptor-detect"/>
        /// <param name="hogDescriptor">HOG descriptor</param>
        /// <param name="img">Source image. CV_8UC1 and CV_8UC4 types are supported for now.</param>
        /// <param name="hitThreshold">Threshold for the distance between features and SVM classifying plane. Usually
        ///     it is 0 and should be specfied in the detector coefficients (as the last
        ///     free coefficient). But if the free coefficient is omitted (which is allowed),
        ///     you can specify it manually here.</param>
        /// <param name="winStride"> Window stride. It must be a multiple of block stride.</param>
        /// <param name="searchLocations"></param>
        /// <returns>
        /// <return name="points">Left-top corner points of detected objects boundaries.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.HogDetect")]
        public static Point[] HogDetect(
            [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Default)] double hitThreshold = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Size? winStride = null,
            //[InputPin(PropertyMode = PropertyMode.Default)] Size? padding = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Point[] searchLocations = null
            )
        {

            return hogDescriptor.Detect(img, hitThreshold, winStride, null, searchLocations);
        }

        /// <summary>
        /// Performs object detection with a multi-scale window.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/gpu/doc/object_detection.html?highlight=hog#gpu-hogdescriptor-detectmultiscale"/>
        /// <param name="hogDescriptor">HOG descriptor</param>
        /// <param name="img">Source image. CV_8UC1 and CV_8UC4 types are supported for now.</param>
        /// <param name="hitThreshold">Threshold for the distance between features and SVM classifying plane.</param>
        /// <param name="winStride"> Window stride. It must be a multiple of block stride.</param>
        /// <param name="scale"> Coefficient of the detection window increase.</param>
        /// <param name="groupThreshold">Coefficient to regulate the similarity threshold. When detected, some objects
        ///     can be covered by many rectangles. 0 means not to perform grouping.</param>
        /// <returns>
        /// <return name="points">Left-top corner points of detected objects boundaries.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.HogDetectMultiScale")]
        public static Rect[] HogDetectMultiScale( //void no output? TODO testing
            [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Default)] double hitThreshold = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Size? winStride = null,
            //[InputPin(PropertyMode = PropertyMode.Default)] Size? padding = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double scale = 1.05,
            [InputPin(PropertyMode = PropertyMode.Default)] int groupThreshold = 2
            )
        {

            return hogDescriptor.DetectMultiScale(img, hitThreshold, winStride, null, scale, groupThreshold);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="hogDescriptor"></param>
        ///// <param name="img"></param>
        ///// <param name="hitThreshold"></param>
        ///// <param name="winStride"></param>
        ///// <param name="padding"></param>
        ///// <param name="scale"></param>
        ///// <param name="groupThreshold"></param>
        ///// <returns>
        ///// <return name="foundLocations"></return>
        ///// <return name="locations"></return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Features2d.HogDetectMultiScaleRoi", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Tuple<Rect[],DetectionROI[]> HogDetectMultiScaleRoi( //void no output? TODO testing
        //    [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
        //    [InputPin(PropertyMode = PropertyMode.Default)] double hitThreshold = 0,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Size? winStride = null,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Size? padding = null,
        //    [InputPin(PropertyMode = PropertyMode.Default)] double scale = 1.05,
        //    [InputPin(PropertyMode = PropertyMode.Default)] int groupThreshold = 2
        //    )
        //{
        //    Rect[] foundLocations = new Rect[1];
        //    DetectionROI[] locations = new DetectionROI[1];
        //    hogDescriptor.DetectMultiScaleROI(img, out foundLocations, out locations, hitThreshold, groupThreshold);
        //    return Tuple.Create(foundLocations, locations);
        //}

        // Missing docu
        //// <param name="padding">Mock parameter to keep the CPU interface compatibility. It must be (0,0).</param>


        /// <summary>
        /// Computes the HOG descriptors.
        /// </summary>
        /// <param name="hogDescriptor">HOG descriptor.</param>
        /// <param name="img">Source image. CV_8UC1 and CV_8UC4 types are supported for now.</param>
        /// <param name="winStride">Window stride. It must be a multiple of block stride</param>
        /// <param name="locations"></param>
        /// <returns>
        /// <return name="descriptors"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Features2d.HogCompute")]
        public static float[] HogCompute(
           [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
           [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
           [InputPin(PropertyMode = PropertyMode.Default)] Size? winStride = null,
           //[InputPin(PropertyMode = PropertyMode.Default)] Size? padding = null,
           [InputPin(PropertyMode = PropertyMode.Allow)] Point[] locations = null
           )
        {
            return hogDescriptor.Compute(img, winStride, null, locations);
        }

        /// <summary>
        /// Groups the object candidate rectangles.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/objdetect/doc/cascade_classification.html#grouprectangles"/>
        /// <param name="hogDescriptor">HOG descriptor.</param>
        /// <param name="rectList">Input vector of rectangles. Output vector includes retained and grouped rectangles.</param>
        /// <param name="groupThreshold">Minimum possible number of rectangles minus 1. The threshold is used in a group of rectangles to retain it.</param>
        /// <param name="eps">Relative difference between sides of the rectangles to merge them into a group.</param>
        /// <returns>
        /// <return name="rects">Output vector of rectangles. Output vector includes retained and grouped rectangles.</return>
        /// <return name="weights">Weights.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Objdetect.GroupRectangles")]
        public static Tuple<Rect[], Double[]> GroupRectangles(
            [InputPin(PropertyMode = PropertyMode.Allow)] HOGDescriptor hogDescriptor,
            [InputPin(PropertyMode = PropertyMode.Allow)] Rect[] rectList,
            [InputPin(PropertyMode = PropertyMode.Default)] int groupThreshold,
                [InputPin(PropertyMode = PropertyMode.Default)] double eps
            )
        {
            Rect[] rectListOutput = (Rect[])rectList.Clone();
            Double[] weights = new Double[1];
            hogDescriptor.GroupRectangles(out rectListOutput, out weights, groupThreshold, eps);
            return Tuple.Create(rectListOutput, weights);
        }

        /// <summary>
        /// Loads a classifier from a file or construct a new one.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/objdetect/doc/cascade_classification.html#cascadeclassifier-load"/>
        /// <param name="path">If path empty, load new CascadeClassifier.</param>
        /// <returns>
        /// <return name="cascadeClassifier">New cascade classifier.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Objdetect.CascadeClassifierLoad")]
        public static CascadeClassifier CascadeClassifierLoad(
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path = null
            )
        {

            CascadeClassifier cascade = new CascadeClassifier();
            if (path != null)
            {
                try
                {
                    cascade = new CascadeClassifier(path);
                }
                catch
                {
                    cascade.Dispose();
                    cascade = new CascadeClassifier();
                }
            }
            return cascade;
        }

        /// <summary>
        /// Detects objects of different sizes in the input image. The detected objects are returned as a list of rectangles.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/objdetect/doc/cascade_classification.html#cascadeclassifier-detectmultiscale"/>
        /// <param name="cascadeClassifier">Classifier.</param>
        /// <param name="image">Matrix of the type CV_8U containing an image where objects are detected.</param>
        /// <param name="scaleFactor">Parameter specifying how much the image size is reduced at each image scale.</param>
        /// <param name="minNeighbors">Parameter specifying how many neighbors each candidate rectangle should have to retain it.</param>
        /// <param name="flags">Parameter with the same meaning for an old cascade as in the function cvHaarDetectObjects. It is not used for a new cascade.</param>
        /// <param name="minSize">Minimum possible object size. Objects smaller than that are ignored.</param>
        /// <param name="maxSize">Maximum possible object size. Objects larger than that are ignored.</param>
        /// <param name="outputRejectLevels"></param>
        /// <returns>
        /// <return name="objects">Vector of rectangles where each rectangle contains the detected object.</return>
        /// <return name="rejectLevels"></return>
        /// <return name="levelWeights"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Objdetect.CascadeClassifierDetectMultiScale")]
        public static Tuple<Rect[], int[], double[]> CascadeClassifierDetectMultiScale(
            [InputPin(PropertyMode = PropertyMode.Allow)] CascadeClassifier cascadeClassifier,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat image,
            [InputPin(PropertyMode = PropertyMode.Default)] double scaleFactor = 1.1,
            [InputPin(PropertyMode = PropertyMode.Default)] int minNeighbors = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] HaarDetectionType flags = HaarDetectionType.DoCannyPruning,
            [InputPin(PropertyMode = PropertyMode.Default)] Size? minSize = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Size? maxSize = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool outputRejectLevels = false
            )
        {
            int[] rejectLevels = new int[1];
            double[] levelWeights = new double[1];
            Rect[] rects;
            try
            {
                rects = cascadeClassifier.DetectMultiScale(image, out rejectLevels, out levelWeights, scaleFactor, minNeighbors, flags, minSize, maxSize);
            }
            finally
            {
                cascadeClassifier.Dispose();
            }
            return Tuple.Create(rects, rejectLevels, levelWeights);

        }

        // ## AKo: TODO Check how model loading works..
        /*/// <summary>
        /// The method load loads the complete model state with the specified name (or default model-dependent name) from the specified XML or YAML file.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/statistical_models.html?highlight=cvstatmodel#cvstatmodel-load"/>
        /// <param name="cvStatModel"></param>
        /// <param name="path">Filepath.</param>
        /// <returns>
        /// <return name="cvStatModel">Statistical model (svm).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.CvStatModelLoad")]
        public static OpenCvSharp.ML.StatModel CvStatModelLoad(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.StatModel cvStatModel,
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path
            )
        {
            cvStatModel.Load(path);
            return cvStatModel;
        }*/

        /// <summary>
        /// The method save saves the complete model state to the specified XML or YAML file with the specified name or default name (which depends on a particular class). Data persistence functionality from CxCore is used.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/statistical_models.html?highlight=cvstatmodel#cvstatmodel-save"/>
        /// <param name="cvStatModel"></param>
        /// <param name="path">Filepath</param>
        /// <returns>
        /// <return name="cvStatModel">Statistical model (svm).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.CvStatModelSave")]
        public static void CvStatModelSave(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.StatModel cvStatModel,
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path
            )
        {
            cvStatModel.Save(path);
        }

        /// <summary>
        /// Default Constructors.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/support_vector_machines.html?highlight=cvsvm#cvsvm-cvsvm"/>
        /// <returns>
        /// <return name="Svm">Instance of default support vector machine.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.Svm")]
        public static OpenCvSharp.ML.SVM Svm(
            )
        {
            return OpenCvSharp.ML.SVM.Create();
        }

        // ## AKo: TODO Check OpenCv ML functions...
        /*
        /// <summary>
        /// Trains an SVM.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/support_vector_machines.html?highlight=cvsvm#cvsvm-train"/>
        /// <param name="cvSvm">Model.</param>
        /// <param name="trainData">The train_data must have the CV_32FC1 (32-bit floating-point, single-channel) format.</param>
        /// <param name="responses">Responses are usually stored in a 1D vector (a row or a column) of CV_32SC1 (only in the classification problem) or CV_32FC1 format, one value per input vector. For classification problems, the responses are discrete class labels. For regression problems, the responses are values of the function to be approximated.</param>
        /// <param name="varIdx">Identifies training data of interest. selects subset of trainData. The vector is either integer (CV_32SC1) vector (list of 0-based indices) or 8-bit (CV_8UC1) masks of active variable/sample. You may pass NULL pointers instead of either of the arguments, meaning that all of the samples are used for training.</param>
        /// <param name="sampleIdx">Identifies response data of interest. selects subset of responses. The vector is either integer (CV_32SC1) vector (list of 0-based indices) or 8-bit (CV_8UC1) masks of active variable/sample. You may pass NULL pointers instead of either of the arguments, meaning that all of the variables are used for training.</param>
        /// <param name="param"></param>
        /// <returns>
        /// <return name="CvSvm">Trained model.</return>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.SvmTrain")]
        public static Tuple<OpenCvSharp.ML.SVM, bool> SvmTrain(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.SVM cvSvm,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat trainData,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat responses,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat varIdx = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat sampleIdx = null,
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.SVM.ParamTypes param = null
            )
        {
            var result = cvSvm.Train(trainData, responses, varIdx, sampleIdx, param);
            return Tuple.Create(cvSvm, result);
        }

        //// <param name="termCrit">Termination criteria of the iterative SVM training procedure which solves a partial case of constrained quadratic optimization problem. You can specify tolerance and/or the maximum number of iterations. </param>

        /// <summary>
        /// Create a parameter set, to train a svm
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/support_vector_machines.html#cvsvmparams-cvsvmparams"/>
        /// <param name="svmType">Type of a SVM formulation. Possible values are:
        ///
        ///CvSVM::C_SVC C-Support Vector Classification. n-class classification ($n \geq 2$), allows imperfect separation of classes with penalty multiplier $C$ for outliers.
        ///CvSVM::NU_SVC $\nu$-Support Vector Classification. n-class classification with possible imperfect separation. Parameter $\nu$ (in the range 0..1, the larger the value, the smoother the decision boundary) is used instead of $C$.
        ///CvSVM::ONE_CLASS Distribution Estimation (One-class SVM). All the training data are from the same class, SVM builds a boundary that separates the class from the rest of the feature space.
        ///CvSVM::EPS_SVR \epsilon-Support Vector Regression. The distance between feature vectors from the training set and the fitting hyper-plane must be less than p. For outliers the penalty multiplier C is used.
        ///CvSVM::NU_SVR $\nu$-Support Vector Regression. $\nu$ is used instead of $p$.
        ///</param>
        /// <param name="kernelType">
        /// kernel_type –
        ///Type of a SVM kernel. Possible values are:
        ///
        ///CvSVM::LINEAR Linear kernel. No mapping is done, linear discrimination (or regression) is done in the original feature space. It is the fastest option. $K(x_i, x_j) = x_i^T x_j$.
        ///CvSVM::POLY Polynomial kernel: $K(x_i, x_j) = (\gamma x_i^T x_j + coef0)^{degree}$, $\gamma > 0$.
        ///CvSVM::RBF Radial basis function (RBF), a good choice in most cases. $K(x_i, x_j) = e^{-\gamma ||x_i - x_j||^2}$, $\gamma > 0$.
        ///CvSVM::SIGMOID Sigmoid kernel: $K(x_i, x_j) = \tanh(\gamma x_i^T x_j + coef0)$.
        ///
        ///</param>
        /// <param name="degree"> Parameter $degree$ of a kernel function (POLY). </param>
        /// <param name="gamma">Parameter $\gamma$ of a kernel function (POLY / RBF / SIGMOID). </param>
        /// <param name="coef0">Parameter $coef0$ of a kernel function (POLY / SIGMOID). </param>
        /// <param name="c">Parameter $C$ of a SVM optimization problem (C_SVC / EPS_SVR / NU_SVR). </param>
        /// <param name="nu"> Parameter $\nu$ of a SVM optimization problem (NU_SVC / ONE_CLASS / NU_SVR). </param>
        /// <param name="p">Parameter $\epsilon$ of a SVM optimization problem (EPS_SVR). </param>
        /// <param name="classWeights">Optional weights in the C_SVC problem , assigned to particular classes. They are multiplied by C so the parameter C of class #i becomes class\_weights_i * C. Thus these weights affect the misclassification penalty for different classes. The larger weight, the larger penalty on misclassification of data from the corresponding class. </param>
        /// <param name="epsilon">Termination criteria of the iterative SVM training procedure which solves a partial case of constrained quadratic optimization problem. You can specify tolerance.</param>
        /// <param name="maxIter">Termination criteria of the iterative SVM training procedure which solves a partial case of constrained quadratic optimization problem. You can specify the maximum number of iterations.</param>
        /// <returns>
        /// <return name="svmParams"/>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.CvSvmParams")]
        public static CvSVMParams CvSvmParams(
            [InputPin(PropertyMode = PropertyMode.Default)] SVMType svmType = SVMType.OneClass,
            [InputPin(PropertyMode = PropertyMode.Default)] SVMKernelType kernelType = SVMKernelType.Rbf,
            [InputPin(PropertyMode = PropertyMode.Default)] double degree = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double gamma = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double coef0 = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double c = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double nu = 0.3,
            [InputPin(PropertyMode = PropertyMode.Default)] double p = 0,
            [InputPin(PropertyMode = PropertyMode.Allow)] CvMat classWeights = null,
            [InputPin(PropertyMode = PropertyMode.Default)] int maxIter = 1000,
            [InputPin(PropertyMode = PropertyMode.Default)] double epsilon = 1e-5
            //            [InputPin(PropertyMode = PropertyMode.Allow)] TermCriteria termCrit = TermCriteria.Both()
            )
        {
            CvTermCriteria termCrit = new CvTermCriteria(maxIter, epsilon);
            return new CvSVMParams(svmType, kernelType, degree, gamma, coef0, c, nu, p, classWeights, termCrit);
        }

        /// <summary>
        /// Generates a grid for SVM parameters.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/support_vector_machines.html?highlight=cvsvm#cvsvm-get-default-grid"/>
        /// <param name="svmParamType">For which Variable.</param>
        /// <returns>
        /// <return name="svmGrid">The function generates a grid for the specified parameter of the SVM algorithm. The grid may be passed to the function CvSVM::train_auto().</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.SvmGrid")]
        public static OpenCvSharp.ML.ParamGrid SvmGrid(
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.SVM.ParamTypes svmParamType
            )
        {
            return OpenCvSharp.ML.SVM.GetDefaultGrid(svmParamType);
        }

        /// <summary>
        /// Predicts the response for input sample.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/support_vector_machines.html?highlight=cvsvm#cvsvm-predict"/>
        /// <param name="cvSvm">Model.</param>
        /// <param name="sample">Input sample for prediction.</param>
        /// <param name="returnDfVal">Specifies a type of the return value. If true and the problem is 2-class classification then the method returns the decision function value that is signed distance to the margin, else the function returns a class label (classification) or estimated function value (regression).</param>
        /// <returns>
        /// <return name="result">The method is used to predict the response for a new sample. In case of a classification, the method returns the class label. In case of a regression, the method returns the output function value.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.SvmPredict")]
        public static float SvmPredict(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.SVM cvSvm,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat sample,
            [InputPin(PropertyMode = PropertyMode.Default)] bool returnDfVal = false
            )
        {
            return cvSvm.Predict(sample.Clone().ToCvMat(), returnDfVal);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        /// <return name="bayes"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.NormalBayesClassifier")]
        public static OpenCvSharp.ML.NormalBayesClassifier NormalBayesClassifier(
            )
        {
            return OpenCvSharp.ML.NormalBayesClassifier.Create();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="normalBayesClassifier"></param>
        /// <param name="trainData"></param>
        /// <param name="responses"></param>
        /// <param name="varIdx"></param>
        /// <param name="sampleIdx"></param>
        /// <param name="update">Adds known samples to model(true) or makes a new one(false)</param>
        /// <returns>
        /// <return name="bayes"></return>
        /// <return name="response"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.NormalBayesClassifierTrain")]
        public static Tuple<OpenCvSharp.ML.NormalBayesClassifier, bool> NormalBayesClassifierTrain(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.NormalBayesClassifier normalBayesClassifier,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat trainData,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat responses,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat varIdx,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat sampleIdx,
            [InputPin(PropertyMode = PropertyMode.Default)] bool update = false
            )
        {
            var nbcOutput = normalBayesClassifier;
            var response = nbcOutput.Train(trainData, responses, varIdx, sampleIdx, update);
            return Tuple.Create(nbcOutput, response);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="normalBayesClassifier"></param>
        /// <param name="sample"></param>
        /// <returns>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.NormalBayesClassifierPredict")]
        public static Mat NormalBayesClassifierPredict(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.NormalBayesClassifier normalBayesClassifier,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat sample
            )
        {
            var result = new Mat();
            var response = normalBayesClassifier.Predict(sample, result);
            return result;
        }

        // KNearest

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        /// <return name="knearest"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.KNearest")]
        public static OpenCvSharp.ML.KNearest KNearest(
            )
        {
            return OpenCvSharp.ML.KNearest.Create();
        }

        /// <summary>
        /// Trains the model
        /// </summary>
        /// <param name="kNearest">Model.</param>
        /// <param name="trainData">Known samples ($m\times n$)</param>
        /// <param name="responses">Classes for known samples ($m\times 1$)</param>
        /// <param name="sampleIdx"></param>
        /// <param name="isRegression"></param>
        /// <param name="maxK">Maximum number of neighbors to return.</param>
        /// <returns>
        /// <return name="kNearest"></return>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.KNearestTrain")]
        public static Tuple<OpenCvSharp.ML.KNearest, bool> KNearestTrain(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.KNearest kNearest,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat trainData,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat responses,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat sampleIdx = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool isRegression = false,
            [InputPin(PropertyMode = PropertyMode.Default)] int maxK = 32
            )
        {
            var result = kNearest.Train(trainData.Clone(), responses.Clone(), sampleIdx, isRegression, maxK, false);
            return Tuple.Create(kNearest, result);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="kNearest"></param>
        /// <param name="samples"></param>
        /// <param name="k"></param>
        /// <returns>
        /// <return name="results"></return>
        /// <return name="neighborResponses"></return>
        /// <return name="dists"></return>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.KNearestFindNearest")]
        public static Tuple<Mat, Mat, Mat, float> KNearestFindNearest(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.KNearest kNearest,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat samples,
            [InputPin(PropertyMode = PropertyMode.Default)] int k
            )
        {
            var results = new Mat();
            var neighborResponses = new Mat();
            var dists = new Mat();
            var result = kNearest.FindNearest(samples, k, results, neighborResponses, dists);
            return Tuple.Create(results, neighborResponses, dists, result);
        }

        /// <summary>
        /// Trains an SVM with optimal parameters.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/ml/doc/support_vector_machines.html?highlight=cvsvm#cvsvm-train-auto"/>
        /// <param name="cvSvm">Model</param>
        /// <param name="trainData">The train_data must have the CV_32FC1 (32-bit floating-point, single-channel) format.</param>
        /// <param name="responses">Responses are usually stored in a 1D vector (a row or a column) of CV_32SC1 (only in the classification problem) or CV_32FC1 format, one value per input vector. For classification problems, the responses are discrete class labels. For regression problems, the responses are values of the function to be approximated.</param>
        /// <param name="param"></param>
        /// <param name="varIdx">Identifies training data of interest. selects subset of trainData. The vector is either integer (CV_32SC1) vector (list of 0-based indices) or 8-bit (CV_8UC1) masks of active variable/sample. You may pass NULL pointers instead of either of the arguments, meaning that all of the samples are used for training.</param>
        /// <param name="sampleIdx">Identifies response data of interest. selects subset of responses. The vector is either integer (CV_32SC1) vector (list of 0-based indices) or 8-bit (CV_8UC1) masks of active variable/sample. You may pass NULL pointers instead of either of the arguments, meaning that all of the variables are used for training.</param>
        /// <param name="kFold">Cross-validation parameter. The training set is divided into k_fold subsets. One subset is used to test the model, the others form the train set. So, the SVM algorithm is executed k_fold times.</param>
        /// <param name="cGrid">Iteration grid for the corresponding SVM parameter.</param>
        /// <param name="gammaGrid">Iteration grid for the corresponding SVM parameter.</param>
        /// <param name="pGrid">Iteration grid for the corresponding SVM parameter.</param>
        /// <param name="nuGrid">Iteration grid for the corresponding SVM parameter.</param>
        /// <param name="coefGrid">Iteration grid for the corresponding SVM parameter.</param>
        /// <param name="degreeGrid">Iteration grid for the corresponding SVM parameter.</param>
        /// <param name="balanced">If true and the problem is 2-class classification then the method creates more balanced cross-validation subsets that is proportions between classes in subsets are close to such proportion in the whole train dataset.</param>
        /// <returns>
        /// <return name="Svm">Trained model.</return>
        /// <return name="result"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ML.SvmTrainAuto")]
        public static Tuple<OpenCvSharp.ML.SVM, bool> SvmTrainAuto(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.SVM cvSvm,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat trainData,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat responses,
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.ML.SVM.ParamTypes param,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat varIdx = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat sampleIdx = null,
            [InputPin(PropertyMode = PropertyMode.Default)] int kFold = 10,
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.ParamGrid? cGrid = null,
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.ParamGrid? gammaGrid = null,
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.ParamGrid? pGrid = null,
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.ParamGrid? nuGrid = null,
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.ParamGrid? coefGrid = null,
            [InputPin(PropertyMode = PropertyMode.Default)] OpenCvSharp.ML.ParamGrid? degreeGrid = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool balanced = false

            )
        {
            var cvSvmOutput = cvSvm;
            var result = cvSvmOutput.TrainAuto(trainData, responses, varIdx == null ? new Mat() : varIdx, sampleIdx == null ? new Mat() : sampleIdx, param, kFold, cGrid, gammaGrid, pGrid, nuGrid, coefGrid, degreeGrid, balanced);
            return Tuple.Create(cvSvmOutput, result);
        }
        */

        /// <summary>
        /// Applies a GNU Octave/MATLAB equivalent colormap on a given image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/colormaps.html#applycolormap"/>
        /// <param name="src">The source image, grayscale or colored does not matter.</param>
        /// <param name="colormap">The colormap to apply.</param>
        /// <returns>
        /// <return name="dst">The result is the colormapped source image. Note: Mat::create() is called on dst.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.ApplyColorMap", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat ApplyColorMap(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] ColormapTypes colormap
        )
        {
            var dst = new Mat();
            Cv2.ApplyColorMap(src, dst, colormap);
            return dst;
        }

        /// <summary>
        /// Training and prediction must be done on grayscale images, use cvtColor() to convert between the color spaces.
        /// The eigenfaces method makes the assumption, that the training and test images are of equal size.
        /// This model does not support updating.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/facerec_api.html#createeigenfacerecognizer"/>
        /// <param name="numComponents">The number of components (read: Eigenfaces) kept for this Prinicpal Component Analysis.
        /// As a hint: There’s no rule how many components (read: Eigenfaces) should be kept for good reconstruction capabilities.
        /// It is based on your input data, so experiment with the number. Keeping 80 components should almost always be sufficient.</param>
        /// <param name="threshold">The threshold applied in the prediciton.</param>
        /// <returns>
        /// <return name="eigenFace">Face recognizer model.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.EigenFaceRecognizer")]
        public static OpenCvSharp.Face.FaceRecognizer EigenFaceRecognizer(
            [InputPin(PropertyMode = PropertyMode.Default)] int numComponents = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double threshold = 1.79769e+308
            )
        {
            return OpenCvSharp.Face.EigenFaceRecognizer.Create(numComponents, threshold);
        }

        /// <summary>
        /// Training and prediction must be done on grayscale images, use cvtColor() to convert between the color spaces.
        /// The fisherfaces method makes the assumption, that the training and test images are of equal size.
        /// This model does not support updating.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/facerec_api.html#createfisherfacerecognizer"/>
        /// <param name="numComponents">The number of components (read: Fisherfaces) kept for this Linear Discriminant Analysis with the Fisherfaces criterion.
        /// It’s useful to keep all components, that means the number of your classes c (read: subjects, persons you want to recognize). If you leave this at the default (0) or set it to a value less-equal 0 or greater (c-1), it will be set to the correct number (c-1) automatically.</param>
        /// <param name="threshold">The threshold applied in the prediction. If the distance to the nearest neighbor is larger than the threshold, this method returns -1.</param>
        /// <returns>
        /// <return name="fisherFace">Face recognizer model.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Face.FisherFaceRecognizer.Create")]
        public static OpenCvSharp.Face.FaceRecognizer FisherFaceRecognizer(
            [InputPin(PropertyMode = PropertyMode.Default)] int numComponents = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double threshold = 1.79769e+308
            )
        {
            return OpenCvSharp.Face.FisherFaceRecognizer.Create(numComponents, threshold);
        }

        /// <summary>
        /// The Circular Local Binary Patterns (used in training and prediction) expect the data given as grayscale images, use cvtColor() to convert between the color spaces.
        /// This model supports updating.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/facerec_api.html#createlbphfacerecognizer"/>
        /// <param name="radius">The radius used for building the Circular Local Binary Pattern.</param>
        /// <param name="neighbors">The number of sample points to build a Circular Local Binary Pattern from. An appropriate value is to use `` 8`` sample points. Keep in mind: the more sample points you include, the higher the computational cost.</param>
        /// <param name="gridX">The number of cells in the horizontal direction, 8 is a common value used in publications. The more cells, the finer the grid, the higher the dimensionality of the resulting feature vector.</param>
        /// <param name="gridY">The number of cells in the vertical direction, 8 is a common value used in publications. The more cells, the finer the grid, the higher the dimensionality of the resulting feature vector.</param>
        /// <param name="threshold">The threshold applied in the prediction. If the distance to the nearest neighbor is larger than the threshold, this method returns -1.</param>
        /// <returns>
        /// <return name="lbphFace">Face recognizer model.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.LbphFaceRecognizer")]
        public static OpenCvSharp.Face.FaceRecognizer LbphFaceRecognizer(
            [InputPin(PropertyMode = PropertyMode.Default)] int radius = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] int neighbors = 8,
            [InputPin(PropertyMode = PropertyMode.Default)] int gridX = 8,
            [InputPin(PropertyMode = PropertyMode.Default)] int gridY = 8,
            [InputPin(PropertyMode = PropertyMode.Default)] double threshold = 1.79769e+308
            )
        {
            return OpenCvSharp.Face.LBPHFaceRecognizer.Create(radius, neighbors, gridX, gridY, threshold);
        }
        /// <summary>
        /// Trains a FaceRecognizer with given data and associated labels.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/facerec_api.html#facerecognizer-train"/>
        /// <param name="faceRecognizer">Model.</param>
        /// <param name="src">The training images, that means the faces you want to learn.</param>
        /// <param name="labels">The labels corresponding to the images.</param>
        /// <returns>
        /// <return name="faceRec">Trained Model.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.FaceRecognizerTrain")]
        public static OpenCvSharp.Face.FaceRecognizer FaceRecognizerTrain(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.Face.FaceRecognizer faceRecognizer,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> src,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<int> labels
            )
        {
            var faceRecognizerOutput = faceRecognizer;
            faceRecognizerOutput.Train(src, labels);
            return faceRecognizerOutput;
        }

        /// <summary>
        /// Predicts a label and associated confidence (e.g. distance) for a given input image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/facerec_api.html#facerecognizer-predict"/>
        /// <param name="faceRecognizer">Model.</param>
        /// <param name="src">Sample image to get a prediction from.</param>
        /// <returns>
        /// <return name="label">The predicted label for the given image.</return>
        /// <return name="confidence">Associated confidence (e.g. distance) for the predicted label.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.FaceRecognizerPredict")]
        public static Tuple<int, double> FaceRecognizerPredict(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.Face.FaceRecognizer faceRecognizer,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
            )
        {
            int label = new int();
            double confidence = new double();
            faceRecognizer.Predict(src, out label, out confidence);
            return Tuple.Create(label, confidence);
        }

        /// <summary>
        /// Updates a FaceRecognizer with given data and associated labels
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/facerec_api.html#facerecognizer-update"/>
        /// <param name="faceRecognizer">Model.</param>
        /// <param name="src">The training images, that means the faces you want to learn.</param>
        /// <param name="labels">The labels corresponding to the images.</param>
        /// <returns>
        /// <return name="faceRecognizer">Updated model.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.FaceRecognizerUpdate")]
        public static OpenCvSharp.Face.FaceRecognizer FaceRecognizerUpdate(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.Face.FaceRecognizer faceRecognizer,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Mat> src,
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<int> labels
            )
        {
            var faceRecognizerOutput = faceRecognizer;
            faceRecognizerOutput.Update(src, labels);
            return faceRecognizerOutput;
        }

        // AKo: TODO check

        /*/// <summary>
        /// Loads a FaceRecognizer and its model state.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/contrib/doc/facerec/facerec_api.html#facerecognizer-load"/>
        /// <param name="faceRecognizer">Model.</param>
        /// <param name="fileName">Path to YAML or XML file.</param>
        /// <returns>
        /// <return name="faceRecognizer">Model.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.FaceRecognizerLoad")]
        public static OpenCvSharp.Face.FaceRecognizer FaceRecognizerLoad(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.Face.FaceRecognizer faceRecognizer,
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string fileName
            )
        {
            faceRecognizer.Load(fileName);
            return faceRecognizer;
        }*/

        /// <summary>
        ///
        /// </summary>
        /// <param name="faceRecognizer"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [StaticModule(ModuleType = "OpenCv.Contrib.FaceRecognizerSave")]
        public static void FaceRecognizerSave(
            [InputPin(PropertyMode = PropertyMode.Allow)] OpenCvSharp.Face.FaceRecognizer faceRecognizer,
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path
            )
        {
            faceRecognizer.Save(path);
        }


        //// TODO cluster Background/Foreground segmentation

        ///// <summary>
        ///// Computes a foreground mask.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#backgroundsubtractor-operator"/>
        ///// <param name="backgroundSubtractotor">Background subtractor.</param>
        ///// <param name="iamge">Next video frame.</param>
        //// <param name="learningRate"></param>

        ///// <returns>
        ///// <return name="fgmask">The output foreground mask as an 8-bit binary image.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Video.BackgroundSubstractorRun", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Mat BackgroundSubstractorRun(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] BackgroundSubtractor backgroundSubtractotor,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat image
        //  //  [InputPin(PropertyMode = PropertyMode.Default)] double learningRate
        //    )
        //{
        //    var fgmask = new Mat();
        //    backgroundSubtractotor.Run(image, fgmask);
        //    return fgmask;
        //}

        ///// <summary>
        ///// Computes a background image.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#backgroundsubtractor-getbackgroundimage"/>
        ///// <param name="backgroundSubtractotor">Background subtractor.</param>
        ///// <returns>
        ///// <return name="backgroundImage">The output background image.</return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Video.BackgroundSubtractorGetBackground", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Mat BackgroundSubtractorGetBackground(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] BackgroundSubtractor backgroundSubtractotor
        //    )
        //{
        //    var backgroundImage = new Mat();
        //    backgroundSubtractotor.GetBackgroundImage(backgroundImage);
        //    return backgroundImage;
        //}

        //[StaticModule(ModuleType = "OpenCv.Video.Background", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static BackgroundSubtractor Background(
        //    )
        //{
        //    return new BackgroundSubtractorGMG();
        //} same problem with clone and instances

        /// <summary>
        /// the full constructor taking the dimensionality of the state, of the measurement and of the control vector
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#kalmanfilter-kalmanfilter"/>
        /// <param name="dynamParams">Dimensionality of the state.</param>
        /// <param name="measureParams">Dimensionality of the measurement.</param>
        /// <param name="controlParams">Dimensionality of the control vector.</param>
        /// <param name="type">Type of the created matrices that should be CV_32F or CV_64F.</param>
        /// <returns>
        /// <return name="kalmanFilter">Kalmanfilter.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.CreateKalmanFilter")]
        public static KalmanFilter CreateKalmanFilter(
            [InputPin(PropertyMode = PropertyMode.Default)] int dynamParams,
            [InputPin(PropertyMode = PropertyMode.Default)] int measureParams,
            [InputPin(PropertyMode = PropertyMode.Default)] int controlParams = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] int type = MatType.CV_32F
            )
        {
            return new KalmanFilter(dynamParams, measureParams, controlParams, type);
        }

        /// <summary>
        /// Updates the predicted state from the measurement.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#kalmanfilter-correct"/>
        /// <param name="kalmanFilter">Kalmanfilter.</param>
        /// <param name="measurement">The measured system parameters</param>
        /// <returns>
        /// <return name="mat"></return>
        /// <return name="kalmanFilter"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.KalmanFilterCorrect")]
        public static Tuple<Mat, KalmanFilter> KalmanFilterCorrect(
            [InputPin(PropertyMode = PropertyMode.Allow)] KalmanFilter kalmanFilter,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat measurement
            )
        {
            var retval = kalmanFilter.Correct(measurement);
            return Tuple.Create(retval, kalmanFilter);
        }

        /// <summary>
        /// Computes a predicted state.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#kalmanfilter-predict"/>
        /// <param name="kalmanFilter">Kalmanfilter.</param>
        /// <param name="control">The optional input control</param>
        /// <returns>
        /// <return name="mat"></return>
        /// <return name="kalmanFilter">new State of the Kalman Filter.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.KalmanFilterPredict")]
        public static Tuple<Mat, KalmanFilter> KalmanFilterPredict(
            [InputPin(PropertyMode = PropertyMode.Allow)] KalmanFilter kalmanFilter,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat control = null
            )
        {
            var retval = kalmanFilter.Predict(control == null ? new Mat() : control);
            return Tuple.Create(retval, kalmanFilter);
        }

        /// <summary>
        /// Finds an object center, size, and orientation.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#camshift"/>
        /// <param name="probImage">Back projection of the object histogram. </param>
        /// <param name="criteria">Stop criteria for the underlying MeanShift() .</param>
        /// <returns>
        /// <return name="rotatedRect">rotated rectangle structure the includes the object position, size and orientation.</return>
        /// <return name="window">Initial search window.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.CamShift", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<RotatedRect, Rect> CamShift(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat probImage,
            [InputPin(PropertyMode = PropertyMode.Default)] TermCriteria criteria
        )
        {
            var window = new Rect();
            var rotatedRect = Cv2.CamShift(probImage, ref window, criteria);
            return Tuple.Create(rotatedRect, window);
        }

        /// <summary>
        /// Finds an object on a back projection image.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#meanshift"/>
        /// <param name="probImage">Back projection of the object histogram.</param>
        /// <param name="criteria">Stop criteria for the iterative search algorithm.</param>
        /// <returns>
        /// <return name="ret">Number of iterations CAMSHIFT took to converge.</return>
        /// <return name="window">Initial search window.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.MeanShift", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Int32, Rect> MeanShift(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat probImage,
            [InputPin(PropertyMode = PropertyMode.Default)] TermCriteria criteria
        )
        {
            var window = new Rect();
            var ret = Cv2.MeanShift(probImage, ref window, criteria);
            return Tuple.Create(ret, window);
        }

        /// <summary>
        /// Constructs a pyramid which can be used as input for calcOpticalFlowPyrLK
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#buildopticalflowpyramid"/>
        /// <param name="img">8-bit input image.</param>
        /// <param name="winSize">window size of optical flow algorithm.
        /// Must be not less than winSize argument of calcOpticalFlowPyrLK().
        /// It is needed to calculate required padding for pyramid levels.</param>
        /// <param name="maxLevel">0-based maximal pyramid level number.</param>
        /// <param name="withDerivatives">set to precompute gradients for the every pyramid level.
        /// If pyramid is constructed without the gradients then calcOpticalFlowPyrLK() will
        /// calculate them internally.</param>
        /// <param name="pyrBorder">the border mode for pyramid layers.</param>
        /// <param name="derivBorder">the border mode for gradients.</param>
        /// <param name="tryReuseInputImage">put ROI of input image into the pyramid if possible.
        /// You can pass false to force data copying.</param>
        /// <returns>
        /// <return name="pyramid">output pyramid.</return>
        /// <return name="ret">number of levels in constructed pyramid. Can be less than maxLevel.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.BuildOpticalFlowPyramid")]
        public static Tuple<Mat, Int32> BuildOpticalFlowPyramid(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Default)] Size winSize,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxLevel,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean withDerivatives = true,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes pyrBorder = BorderTypes.Default,
            [InputPin(PropertyMode = PropertyMode.Default)] BorderTypes derivBorder = BorderTypes.Constant,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean tryReuseInputImage = true
        )
        {
            var pyramid = new Mat();
            var ret = Cv2.BuildOpticalFlowPyramid(img, pyramid, winSize, maxLevel, withDerivatives, pyrBorder, derivBorder, tryReuseInputImage);
            return Tuple.Create(pyramid, ret);
        }


        /// <summary>
        /// Calculates an optical flow for a sparse feature set using the iterative Lucas-Kanade method with pyramids.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#calcopticalflowpyrlk"/>
        /// <param name="prevImg">first 8-bit input image or pyramid constructed by buildOpticalFlowPyramid().</param>
        /// <param name="nextImg">second input image or pyramid of the same size and the same type as prevImg.</param>
        /// <param name="prevPts">vector of 2D points for which the flow needs to be found; point coordinates must be single-precision floating-point numbers.</param>
        /// <param name="nextPts">vector of 2D points (with single-precision floating-point coordinates).</param>
        /// <param name="winSize">size of the search window at each pyramid level.</param>
        /// <param name="maxLevel">0-based maximal pyramid level number; if set to 0, pyramids are not used (single level), if set to 1, two levels are used, and so on; if pyramids are passed to input then algorithm will use as many levels as pyramids have but no more than maxLevel.</param>
        /// <param name="criteria">parameter, specifying the termination criteria of the iterative search algorithm (after the specified maximum number of iterations criteria.maxCount or when the search window moves by less than criteria.epsilon.</param>
        /// <param name="flags">operation flags:</param>
        /// <param name="minEigThreshold">the algorithm calculates the minimum eigen value of a $2 \times 2$ normal matrix of optical flow equations (this matrix is called a spatial gradient matrix in [Bouguet00]), divided by number of pixels in a window; if this value is less than minEigThreshold, then a corresponding feature is filtered out and its flow is not processed, so it allows to remove bad points and get a performance boost.</param>
        /// <returns>
        /// <return name="nextPts">output vector of 2D points (with single-precision floating-point coordinates) containing the calculated new positions of input features in the second image; when OPTFLOW_USE_INITIAL_FLOW flag is passed, the vector must have the same size as in the input.</return>
        /// <return name="status">output status vector (of unsigned chars); each element of the vector is set to 1 if the flow for the corresponding features has been found, otherwise, it is set to 0.</return>
        /// <return name="err">output vector of errors; each element of the vector is set to an error for the corresponding feature, type of the error measure can be set in flags parameter; if the flow wasn’t found then the error is not defined (use the status parameter to find such cases).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.CalcOpticalFlowPyrLK")]
        public static Tuple<Mat, Mat, Mat> CalcOpticalFlowPyrLK(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat prevImg,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat nextImg,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat prevPts,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat nextPts,
            [InputPin(PropertyMode = PropertyMode.Default)] Size? winSize = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxLevel = 3,
            [InputPin(PropertyMode = PropertyMode.Default)] TermCriteria? criteria = null,
            [InputPin(PropertyMode = PropertyMode.Default)] OpticalFlowFlags flags = OpticalFlowFlags.None,
            [InputPin(PropertyMode = PropertyMode.Default)] Double minEigThreshold = 0.0001
        )
        {
            var nextPtsOutput = nextPts.Clone();
            var status = new Mat();
            var err = new Mat();
            Cv2.CalcOpticalFlowPyrLK(prevImg, nextImg, prevPts, nextPtsOutput, status, err, winSize, maxLevel, criteria, flags, minEigThreshold);
            return Tuple.Create(nextPtsOutput, status, err);
        }

        /// <summary>
        /// Computes a dense optical flow using the Gunnar Farneback's algorithm.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#calcopticalflowfarneback"/>
        /// <param name="prev">first 8-bit single-channel input image.</param>
        /// <param name="next">second input image of the same size and the same type as prev.</param>
        /// <param name="flow">flow image that has the same size as prev and type CV_32FC2(for flag OPTFLOW_USE_INITIAL_FLOW).</param>
        /// <param name="pyrScale">parameter, specifying the image scale ($\lt 1$) to build pyramids for each image;
        /// $pyrScale=0.5$ means a classical pyramid, where each next layer is twice smaller than the previous one.</param>
        /// <param name="levels">number of pyramid layers including the initial image;
        /// levels=1 means that no extra layers are created and only the original images are used.</param>
        /// <param name="winsize">averaging window size; larger values increase the algorithm robustness to
        /// image noise and give more chances for fast motion detection, but yield more blurred motion field.</param>
        /// <param name="iterations">number of iterations the algorithm does at each pyramid level.</param>
        /// <param name="polyN">size of the pixel neighborhood used to find polynomial expansion in each pixel;
        /// larger values mean that the image will be approximated with smoother surfaces,
        /// yielding more robust algorithm and more blurred motion field, typically $poly_n =5$ or $7$.</param>
        /// <param name="polySigma">standard deviation of the Gaussian that is used to smooth derivatives used as
        /// a basis for the polynomial expansion; for polyN=5, you can set $polySigma=1.1$,
        /// for $polyN=7$, a good value would be $polySigma=1.5$.</param>
        /// <param name="flags">operation flags that can be a combination of OPTFLOW_USE_INITIAL_FLOW and/or OPTFLOW_FARNEBACK_GAUSSIAN</param>
        /// <returns>
        /// <return name="flow">computed flow image that has the same size as prev and type CV_32FC2.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.CalcOpticalFlowFarneback", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat CalcOpticalFlowFarneback(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat prev,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat next,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat flow,
            [InputPin(PropertyMode = PropertyMode.Default)] Double pyrScale,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 levels,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 winsize,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 iterations,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 polyN,
            [InputPin(PropertyMode = PropertyMode.Default)] Double polySigma,
            [InputPin(PropertyMode = PropertyMode.Default)] OpticalFlowFlags flags
        )
        {
            var flowOutput = flow.Clone();
            Cv2.CalcOpticalFlowFarneback(prev, next, flowOutput, pyrScale, levels, winsize, iterations, polyN, polySigma, flags);
            return flowOutput;
        }

        /// <summary>
        /// Estimates the best-fit Euqlidean, similarity, affine or perspective transformation
        /// that maps one 2D point set to another or one image to another.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/video/doc/motion_analysis_and_object_tracking.html#estimaterigidtransform"/>
        /// <param name="src">First input 2D point set stored in std::vector or Mat, or an image stored in Mat.</param>
        /// <param name="dst">Second input 2D point set of the same size and the same type as A, or another image.</param>
        /// <param name="fullAffine">If true, the function finds an optimal affine transformation with no additional restrictions (6 degrees of freedom).
        /// Otherwise, the class of transformations to choose from is limited to combinations of translation, rotation, and uniform scaling (5 degrees of freedom).</param>
        /// <returns>
        /// <return name="mat">optimal affine transformation.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.EstimateRigidTransform", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat EstimateRigidTransform(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean fullAffine
        )
        {
            return Cv2.EstimateRigidTransform(src, dst, fullAffine);
        }

        // TODO found no docu
        /// <summary>
        ///
        /// </summary>
        /// <returns>
        /// <return name="frameSource"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.CreateFrameSource_Empty")]
        public static FrameSource CreateFrameSource_Empty()
        {
            return Cv2.CreateFrameSource_Empty();
        }

        // TODO found no docu
        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>
        /// <return name="frameSource"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.CreateFrameSource_Video")]
        public static FrameSource CreateFrameSource_Video(
            [InputPin(PropertyMode = PropertyMode.Default)] String fileName
        )
        {
            return Cv2.CreateFrameSource_Video(fileName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="frameSource"></param>
        /// <returns>
        /// <return name=""></return>
        /// <return name="frameSource"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Video.FrameSourceNextFrame")]
        public static Tuple<Mat, FrameSource> FrameSourceNextFrame(
            [InputPin(PropertyMode = PropertyMode.Always)] FrameSource frameSource
            )
        {
            var frame = new Mat();
            frameSource.NextFrame(frame);
            return Tuple.Create(frame, frameSource);
            //TODO frame -> ISequence maybe?
        }

        /// <summary>
        /// Calculates an absolute value of each matrix element.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#abs"/>
        /// <param name="src">Matrix.</param>
        /// <returns>
        /// <return name="dst">Matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Abs", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Abs(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            return Cv2.Abs(src).ToMat();
        }

        /// <summary>
        /// Computes the per-element sum of two arrays or an array and a scalar.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#add"/>
        /// <param name="src1">The first source array</param>
        /// <param name="src2">The second source array. It must have the same size and same type as src1</param>
        /// <param name="mask">The optional operation mask, 8-bit single channel array; specifies elements of the destination array to be changed. [By default this is null]</param>
        /// <param name="dtype">Optional type of the output matrix. When it is negative, the output matrix will have the same type as src . Otherwise, it will be type=CV_MAT_DEPTH(dtype) that should be either CV_32F or CV_64F .</param>
        /// <returns>
        /// <return name="dst">The destination array; it will have the same size and same type as src1</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Add", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Add(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype = -1
        )
        {
            var dst = new Mat();
            Cv2.Add(src1, src2, dst, mask == null ? new Mat() : mask, dtype);
            return dst;
        }

        /// <summary>
        /// Calculates per-element difference between two arrays or array and a scalar
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#subtract"/>
        /// <param name="src1">The first source array</param>
        /// <param name="src2">The second source array. It must have the same size and same type as src1</param>
        /// <param name="mask">The optional operation mask, 8-bit single channel array; specifies elements of the destination array to be changed. [By default this is null]</param>
        /// <param name="dtype">Optional type of the output matrix. When it is negative, the output matrix will have the same type as src . Otherwise, it will be type=CV_MAT_DEPTH(dtype) that should be either CV_32F or CV_64F .</param>
        /// <returns>
        /// <return name="dst">The destination array; it will have the same size and same type as src1</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Subtract", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Subtract(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype = -1
        )
        {
            var dst = new Mat();
            Cv2.Subtract(src1, src2, dst, mask == null ? new Mat() : mask, dtype);
            return dst;
        }

        /// <summary>
        /// Calculates the per-element scaled product of two arrays
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#multiply"/>
        /// <param name="src1">The first source array</param>
        /// <param name="src2">The second source array of the same size and the same type as src1</param>
        /// <param name="scale">The optional scale factor. [By default this is 1]</param>
        /// <param name="dtype">Optional type of the output matrix. When it is negative, the output matrix will have the same type as src . Otherwise, it will be type=CV_MAT_DEPTH(dtype) that should be either CV_32F or CV_64F .</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and the same type as src1</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Multiply", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Multiply(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Double scale = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype = -1
        )
        {
            var dst = new Mat();
            Cv2.Multiply(src1, src2, dst, scale, dtype);
            return dst;
        }

        /// <summary>
        /// Performs per-element division of two arrays or a scalar by an array.
        /// </summary>
        /// <param name="src1">The first source array</param>
        /// <param name="src2">The second source array; should have the same size and same type as src1</param>
        /// <param name="scale">Scale factor [By default this is 1]</param>
        /// <param name="dtype">Optional type of the output matrix. When it is negative, the output matrix will have the same type as src . Otherwise, it will be type=CV_MAT_DEPTH(dtype) that should be either CV_32F or CV_64F .</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and same type as src2</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Divide", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Divide(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Double scale = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype = -1
        )
        {
            var dst = new Mat();
            Cv2.Divide(src1, src2, dst, scale, dtype);
            return dst;
        }


        /// <summary>
        /// adds scaled array to another one ($dst = alpha \cdot src1 + src2$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#scaleadd"/>
        /// <param name="src1">First input array.</param>
        /// <param name="src2">Second input array of the same size and type as src1.</param>
        /// <param name="alpha">Scale factor for the first array.</param>
        /// <returns>
        /// <return name="dst"> output array of the same size and type as src1.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.ScaleAdd", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat ScaleAdd(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Double alpha = 1
        )
        {
            var dst = new Mat();
            Cv2.ScaleAdd(src1, alpha, src2, dst);
            return dst;
        }

        /// <summary>
        /// computes weighted sum of two arrays (dst = alpha*src1 + beta*src2 + gamma)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#addweighted"/>
        /// <param name="src1">First input array.</param>
        /// <param name="alpha">Weight of the first array elements.</param>
        /// <param name="src2">Second input array of the same size and channel number as src1.</param>
        /// <param name="beta">Weight of the second array elements.</param>
        /// <param name="gamma">scalar added to each sum.</param>
        /// <param name="dtype">Optional depth of the output array; when both input arrays have the same depth, dtype can be set to -1, which will be equivalent to src1.depth().</param>
        /// <returns>
        /// <return name="dst">Output array that has the same size and number of channels as the input arrays.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.AddWeighted", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat AddWeighted(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Double alpha = 0.5,
            [InputPin(PropertyMode = PropertyMode.Default)] Double beta = 0.5,
            [InputPin(PropertyMode = PropertyMode.Default)] Double gamma = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype = -1
        )
        {
            var dst = new Mat();
            Cv2.AddWeighted(src1, alpha, src2, beta, gamma, dst, dtype);
            return dst;
        }

        /// <summary>
        /// Scales, computes absolute values and converts the result to 8-bit.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#convertscaleabs"/>
        /// <param name="src">The source array</param>
        /// <param name="alpha">The optional scale factor. [By default this is 1]</param>
        /// <param name="beta">The optional delta added to the scaled values. [By default this is 0]</param>
        /// <returns>
        /// <return name="dst">The destination array</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.ConvertScaleAbs", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat ConvertScaleAbs(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Double alpha = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Double beta = 0
        )
        {
            var dst = new Mat();
            Cv2.ConvertScaleAbs(src, dst, alpha, beta);
            return dst;
        }

        //[StaticModule(ModuleType = "OpenCv.ImgProc.DistanceTransformWithLabels", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        // public static void LUT(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat lut,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] OutputArray dst,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Int32 interpolation = 0
        //)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// computes sum of array elements
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#sum"/>
        /// <param name="src">The source array; must have 1 to 4 channels</param>
        /// <returns>
        /// <return name="sum">The functions sum calculate and return the sum of array elements, independently for each channel.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Sum")]
        public static Scalar Sum(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            return Cv2.Sum(src);
        }

        /// <summary>
        /// Computes the number of nonzero array elements.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#countnonzero"/>
        /// <param name="mtx">Single-channel array.</param>
        /// <returns>
        /// <return name="count">number of non-zero elements in mtx.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.CountNonZero")]
        public static Int32 CountNonZero(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mtx
        )
        {
            return Cv2.CountNonZero(mtx);
        }

        /// <summary>
        /// returns the list of locations of non-zero pixels
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#countnonzero"/>
        /// <param name="src">Single-channel array.</param>
        /// <returns>
        /// <return name="idx">Non-zero elements in mtx.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.FindNonZero", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FindNonZero(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var idx = new Mat();
            Cv2.FindNonZero(src, idx);
            return idx;
        }

        /// <summary>
        /// computes mean value of selected array elements
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#mean"/>
        /// <param name="src">The source array; it should have 1 to 4 channels
        /// (so that the result can be stored in Scalar)</param>
        /// <param name="mask">The optional operation mask</param>
        /// <returns>
        /// <return name="mean">The function $mean$ calculates the mean value M of array elements, independently for each channel, and return it.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Mean")]
        public static Scalar Mean(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            return Cv2.Mean(src, mask == null ? new Mat() : mask);
        }

        /// <summary>
        /// computes mean value and standard deviation of all or selected array elements
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#meanstddev"/>
        /// <param name="src">The source array; it should have 1 to 4 channels
        /// (so that the results can be stored in Scalar's)</param>
        /// <param name="mask">The optional operation mask</param>
        /// <returns>
        /// <return name="mean">Computed mean value.</return>
        /// <return name="stddev">Computed standard deviation.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.MeanStdDev")]
        public static Tuple<Mat, Mat> MeanStdDev(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            var mean = new Mat();
            var stddev = new Mat();
            Cv2.MeanStdDev(src, mean, stddev, mask == null ? new Mat() : mask);
            return Tuple.Create(mean, stddev);
        }

        /// <summary>
        /// computes norm of selected part of the difference between two arrays.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#norm"/>
        /// <param name="src1">The first source array.</param>
        /// <param name="src2">The second source array of the same size and the same type as src1.</param>
        /// <param name="normType">Type of the norm.</param>
        /// <param name="mask">The optional operation mask.</param>
        /// <returns>
        /// <return name="norm">Computed norm.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Norm")]
        public static Double Norm(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] NormTypes normType = NormTypes.L2,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            return Cv2.Norm(src1, src2, normType, mask == null ? new Mat() : mask);
        }

        /// <summary>
        /// scales and shifts array elements so that either the specified norm (alpha)
        /// or the minimum (alpha) and maximum (beta) array values get the specified values
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#normalize"/>
        /// <param name="src">the source array</param>
        /// <param name="alpha">the norm value to normalize to or the lower range boundary
        /// in the case of range normalization</param>
        /// <param name="beta">the upper range boundary in the case of range normalization;
        /// not used for norm normalization</param>
        /// <param name="normType">the normalization type</param>
        /// <param name="dtype">when the parameter is negative,
        /// the destination array will have the same type as src,
        /// otherwise it will have the same number of channels as src and the depth =cv_mat_depth(rtype)</param>
        /// <param name="mask">the optional operation mask</param>
        /// <returns>
        /// <return name="dst">the destination array; will have the same size as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Normalize", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Normalize(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] double alpha = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double beta = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] NormTypes normType = NormTypes.L2,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype = -1,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            var dst = new Mat();
            Cv2.Normalize(src, dst, alpha, beta, normType, dtype, mask == null ? new Mat() : mask);
            return dst;
        }

        /// <summary>
        /// finds global minimum and maximum array elements and returns their values and their locations
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#minmaxloc"/>
        /// <param name="src">The source single-channel array</param>
        /// <param name="mask">The optional mask used to select a sub-array</param>
        /// <returns>
        /// <return name="minVal">Minimum value</return>
        /// <return name="maxVal">Maximum value</return>
        /// <return name="minLoc">Minimum location</return>
        /// <return name="maxLoc">Maximum location</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.MinMaxLoc")]
        public static Tuple<Double, Double, Point, Point> MinMaxLoc(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            Double minVal, maxVal;
            Point minLoc, maxLoc;
            Cv2.MinMaxLoc(src, out minVal, out maxVal, out minLoc, out maxLoc, mask == null ? new Mat() : mask);
            return Tuple.Create(minVal, maxVal, minLoc, maxLoc);
        }

        /// <summary>
        /// transforms 2D matrix to 1D row or column vector by taking sum, minimum, maximum or mean value over all the rows
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#reduce"/>
        /// <param name="src">The source 2D matrix</param>
        /// <param name="dim">The dimension index along which the matrix is reduced.
        /// 0 means that the matrix is reduced to a single row and 1 means that the matrix is reduced to a single column</param>
        /// <param name="rtype">reduction operation</param>
        /// <param name="dtype">When it is negative, the destination vector will have
        /// the same type as the source matrix, otherwise, its type will be CV_MAKE_TYPE(CV_MAT_DEPTH(dtype), mtx.channels())</param>
        /// <returns>
        /// <return name="dst">The destination vector.
        /// Its size and type is defined by dim and dtype parameters</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Reduce")]
        public static Mat Reduce(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] ReduceDimension dim,
            [InputPin(PropertyMode = PropertyMode.Default)] ReduceTypes rtype,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype
        )
        {
            var dst = new Mat();
            Cv2.Reduce(src, dst, dim, rtype, dtype);
            return dst;
        }

        /// <summary>
        /// makes multi-channel array out of several single-channel arrays
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#merge"/>
        /// <param name="mv"> input array or vector of matrices to be merged; all the matrices in mv must have the same size and the same depth.</param>
        /// <returns>
        /// <return name="dst">output array of the same size and the same depth as mv[0]; The number of channels will be the total number of channels in the matrix array.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Merge", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Merge(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat[] mv
        )
        {
            var dst = new Mat();
            Cv2.Merge(mv, dst);
            return dst;
        }

        /// <summary>
        /// Extracts each channel of a multi-channel array to a dedicated array
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#split"/>
        /// <param name="src">The source multi-channel array</param>
        /// <returns>
        /// <return name="Channel0">Channel 0</return>
        /// <return name="Channel1">Channel 1</return>
        /// <return name="Channel2">Channel 2</return>
        /// <return name="Channel3">Channel 3</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Split")]
        public static Tuple<Mat, Mat, Mat, Mat> Split(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var result = Cv2.Split(src);

            if (result.Length == 1)
                return Tuple.Create(result[0], (Mat)null, (Mat)null, (Mat)null);
            else if (result.Length == 2)
                return Tuple.Create(result[0], result[1], (Mat)null, (Mat)null);
            else if (result.Length == 3)
                return Tuple.Create(result[0], result[1], result[2], (Mat)null);
            else if (result.Length == 4)
                return Tuple.Create(result[0], result[1], result[2], result[3]);

            else
                throw new Exception("Count of splitted channels must be between 1-4");
        }

        /// <summary>
        /// extracts a single channel from src (coi is 0-based index)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html"/> //TODO no docu for this
        /// <param name="src">The source array</param>
        /// <param name="coi"></param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.ExtractChannel", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat ExtractChannel(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 coi
        )
        {
            var dst = new Mat();
            Cv2.ExtractChannel(src, dst, coi);
            return dst;
        }

        /// <summary>
        /// reverses the order of the rows, columns or both in a matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#flip"/>
        /// <param name="src">The source array</param>
        /// <param name="flipCode">Specifies how to flip the array:
        /// 0 means flipping around the x-axis, positive (e.g., 1) means flipping around y-axis,
        /// and negative (e.g., -1) means flipping around both axes. See also the discussion below for the formulas.</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Flip", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Flip(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] FlipMode flipCode
        )
        {
            var dst = new Mat();
            Cv2.Flip(src, dst, flipCode);
            return dst;
        }

        /// <summary>
        /// replicates the input matrix the specified number of times in the horizontal and/or vertical direction
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#repeat"/>
        /// <param name="src">The source array to replicate</param>
        /// <param name="ny">How many times the src is repeated along the vertical axis</param>
        /// <param name="nx">How many times the src is repeated along the horizontal axis</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Repeat", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Repeat(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 ny,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 nx
        )
        {
            var dst = new Mat();
            Cv2.Repeat(src, ny, nx, dst);
            return dst;
        }

        /// <summary>
        /// Concat two matrices horizontally.
        /// </summary>
        /// /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html"/>
        /// <param name="src1">First matrix.</param>
        /// <param name="src2">Second matrix.</param>
        /// <returns>
        /// <return name="dst">Output matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.HConcat", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat HConcat(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2
        )
        {
            var dst = new Mat();
            Cv2.HConcat(src1, src2, dst);
            return dst;
        }

        /// <summary>
        /// Concat two matrices vertically.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html"/>
        /// <param name="src1">First matrix.</param>
        /// <param name="src2">Second matrix.</param>
        /// <returns>
        /// <return name="dst">Output matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.VConcat", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat VConcat(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2
        )
        {
            var dst = new Mat();
            Cv2.VConcat(src1, src2, dst);
            return dst;
        }

        /// <summary>
        /// computes bitwise conjunction of the two arrays ($dst = src1 \And src2$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#bitwise-and"/>
        /// <param name="src1">first input array or a scalar.</param>
        /// <param name="src2">second input array or a scalar.</param>
        /// <param name="mask">optional operation mask, 8-bit single channel array, that specifies elements of the output array to be changed.</param>
        /// <returns>
        /// <return name="dst">output array that has the same size and type as the input arrays.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.BitwiseAnd", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat BitwiseAnd(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            var dst = new Mat();
            Cv2.BitwiseAnd(src1, src2, dst, mask == null ? new Mat() : mask);
            return dst;
        }

        /// <summary>
        /// computes bitwise disjunction of the two arrays ($dst = src1 | src2$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#bitwise-or"/>
        /// <param name="src1">first input array or a scalar.</param>
        /// <param name="src2">second input array or a scalar.</param>
        /// <param name="mask">optional operation mask, 8-bit single channel array, that specifies elements of the output array to be changed.</param>
        /// <returns>
        /// <return name="dst">output array that has the same size and type as the input arrays.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.BitwiseOr", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat BitwiseOr(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            var dst = new Mat();
            Cv2.BitwiseOr(src1, src2, dst, mask == null ? new Mat() : mask);
            return dst;
        }

        /// <summary>
        /// computes bitwise exclusive-or of the two arrays ($dst = src1 ^ src2$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#bitwise-xor"/>
        /// <param name="src1">first input array or a scalar.</param>
        /// <param name="src2">second input array or a scalar.</param>
        /// <param name="mask">optional operation mask, 8-bit single channel array, that specifies elements of the output array to be changed.</param>
        /// <returns>
        /// <return name="dst">output array that has the same size and type as the input arrays.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.BitwiseXor", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat BitwiseXor(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            var dst = new Mat();
            Cv2.BitwiseXor(src1, src2, dst, mask == null ? new Mat() : mask);
            return dst;
        }

        /// <summary>
        /// inverts each bit of array ($dst = \not src$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#bitwise-not"/>
        /// <param name="src">input array.</param>
        /// <param name="mask">optional operation mask, 8-bit single channel array, that specifies elements of the output array to be changed.</param>
        /// <returns>
        /// <return name="dst">output array that has the same size and type as the input array.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.BitwiseNot", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat BitwiseNot(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat mask = null
        )
        {
            var dst = new Mat();
            Cv2.BitwiseNot(src, dst, mask == null ? new Mat() : mask);
            return dst;
        }

        /// <summary>
        /// computes element-wise absolute difference of two arrays ($dst = abs(src1 - src2)$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#absdiff"/>
        /// <param name="src1">first input array or a scalar.</param>
        /// <param name="src2">second input array or a scalar.</param>
        /// <returns>
        /// <return name="dst">output array that has the same size and type as input arrays.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Absdiff", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Absdiff(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2
        )
        {
            var dst = new Mat();
            Cv2.Absdiff(src1, src2, dst);
            return dst;
        }

        /// <summary>
        /// Checks if array elements lie between the elements of two other arrays.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#inrange"/>
        /// <param name="src">first input array.</param>
        /// <param name="lowerb">inclusive lower boundary array.</param>
        /// <param name="upperb">inclusive upper boundary array.</param>
        /// <returns>
        /// <return name="dst">output array of the same size as src and CV_8U type.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.InRange", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat InRange(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "0")] Scalar lowerb,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "255")] Scalar upperb
        )
        {
            var dst = new Mat();
            Cv2.InRange(src, lowerb, upperb, dst);
            return dst;
        }

        /// <summary>
        /// compares elements of two arrays ($dst = src1 [cmpop] src2$).
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#compare"/>
        /// <param name="src1">First input array or a scalar (in the case of cvCmp, cv.Cmp, cvCmpS, cv.CmpS it is always an array); when it is an array, it must have a single channel.</param>
        /// <param name="src2">Second input array or a scalar (in the case of cvCmp and cv.Cmp it is always an array; in the case of cvCmpS, cv.CmpS it is always a scalar); when it is an array, it must have a single channel.</param>
        /// <param name="cmpop">a flag, that specifies correspondence between the arrays.
        ///     CMP_EQ src1 is equal to src2.
        ///     CMP_GT src1 is greater than src2.
        ///     CMP_GE src1 is greater than or equal to src2.
        ///     CMP_LT src1 is less than src2.
        ///     CMP_LE src1 is less than or equal to src2.
        ///     CMP_NE src1 is unequal to src2.</param>
        /// <returns>
        /// <return name="dst">output array that has the same size and type as the input arrays.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Compare", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Compare(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] CmpTypes cmpop
        )
        {
            var dst = new Mat();
            Cv2.Compare(src1, src2, dst, cmpop);
            return dst;
        }

        /// <summary>
        /// Calculates per-element minimum of two arrays.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#min"/>
        /// <param name="src1">First input array.</param>
        /// <param name="src2">Second input array of the same size and type as src1.</param>
        /// <returns>
        /// <return name="dst">Output array of the same size and type as src1.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Min", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Min(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2
        )
        {
            var dst = new Mat();
            Cv2.Min(src1, src2, dst);
            return dst;
        }

        /// <summary>
        /// Calculates per-element maximum of two arrays.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#max"/>
        /// <param name="src1">first input array.</param>
        /// <param name="src2">second input array of the same size and type as src1 .</param>
        /// <returns>
        /// <return name="dst">output array of the same size and type as src1.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Max", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Max(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2
        )
        {
            var dst = new Mat();
            Cv2.Max(src1, src2, dst);
            return dst;
        }

        /// <summary>
        /// computes square root of each matrix element ($dst = src**0.5$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#sqrt"/>
        /// <param name="src">The source floating-point array</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and the same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Sqrt", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Sqrt(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var dst = new Mat();
            Cv2.Sqrt(src, dst);
            return dst;
        }

        /// <summary>
        /// raises the input matrix elements to the specified power ($b = a**power$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#pow"/>
        /// <param name="src">The source array</param>
        /// <param name="power">The exponent of power</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and the same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Pow", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Pow(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Double power
        )
        {
            var dst = new Mat();
            Cv2.Pow(src, power, dst);
            return dst;
        }

        /// <summary>
        /// computes exponent of each matrix element ($dst = e**src$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#exp"/>
        /// <param name="src">The source array</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Exp", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Exp(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var dst = new Mat();
            Cv2.Exp(src, dst);
            return dst;
        }

        /// <summary>
        /// computes natural logarithm of absolute value of each matrix element: ($dst = \log(\abs(src)$)
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#log"/>
        /// <param name="src">The source array</param>
        /// <returns>
        /// <return name="dst">The destination array; will have the same size and same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Log", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Log(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var dst = new Mat();
            Cv2.Log(src, dst);
            return dst;
        }

        /// <summary>
        /// computes cube root of the argument
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/utility_and_system_functions_and_macros.html#cuberoot"/>
        /// <param name="val">A function argument.</param>
        /// <returns>
        /// <return name="ret">Computed value.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.CubeRoot")]
        public static Single CubeRoot(
            [InputPin(PropertyMode = PropertyMode.Default)] Single val
        )
        {
            return Cv2.CubeRoot(val);
        }

        /// <summary>
        /// computes the angle in degrees ($0..360$) of the vector $(x,y)$.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/utility_and_system_functions_and_macros.html#fastatan2"/>
        /// <param name="y">x-coordinate of the vector.</param>
        /// <param name="x">y-coordinate of the vector.</param>
        /// <returns>
        /// <return name="ret">Computed Value.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.FastAtan2")]
        public static Single FastAtan2(
            [InputPin(PropertyMode = PropertyMode.Default)] Single y,
            [InputPin(PropertyMode = PropertyMode.Default)] Single x
        )
        {
            return Cv2.FastAtan2(y, x);
        }

        /// <summary>
        /// converts polar coordinates to Cartesian
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#polartocart"/>
        /// <param name="magnitude">input floating-point array of magnitudes of 2D vectors; it can be an empty matrix ($=Mat()$), in this case, the function assumes that all the magnitudes are $=1$; if it is not empty, it must have the same size and type as angle.</param>
        /// <param name="angle">input floating-point array of angles of 2D vectors.</param>
        /// <param name="angleInDegrees">when true, the input angles are measured in degrees, otherwise, they are measured in radians.</param>
        /// <returns>
        /// <return name="x">output array of x-coordinates of 2D vectors; it has the same size and type as angle.</return>
        /// <return name="y">output array of y-coordinates of 2D vectors; it has the same size and type as angle.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PolarToCart", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Mat> PolarToCart(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat magnitude,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat angle,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean angleInDegrees = false
        )
        {
            var x = new Mat();
            var y = new Mat();
            Cv2.PolarToCart(magnitude == null ? new Mat() : magnitude, angle, x, y, angleInDegrees);
            return Tuple.Create(x, y);
        }

        /// <summary>
        /// converts Cartesian coordinates to polar
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#carttopolar"/>
        /// <param name="x">array of x-coordinates; this must be a single-precision or double-precision floating-point array.</param>
        /// <param name="y">array of y-coordinates, that must have the same size and same type as x.</param>
        /// <param name="angleInDegrees">a flag, indicating whether the angles are measured in radians (which is by default), or in degrees.</param>
        /// <returns>
        /// <return name="magnitude">output array of magnitudes of the same size and type as x.</return>
        /// <return name="angle">output array of angles that has the same size and type as x; the angles are measured in radians (from 0 to 2*Pi) or in degrees (0 to 360 degrees).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.CartToPolar", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Mat> CartToPolar(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat x,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat y,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean angleInDegrees = false
        )
        {
            var magnitude = new Mat();
            var angle = new Mat();
            Cv2.CartToPolar(x, y, magnitude, angle, angleInDegrees);
            return Tuple.Create(magnitude, angle);
        }

        // TODO mismatching docu and function
        ///// <summary>
        ///// computes angle (angle(i)) of each (x(i), y(i)) vector
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#phase"/>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="angle"></param>
        ///// <param name="angleInDegrees"></param>
        //[StaticModule(ModuleType = "OpenCv.Core.Phase", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static void Phase(
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat x,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] Mat y,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] OutputArray angle,
        //    [InputPin(PropertyMode = PropertyMode.Default)] Boolean angleInDegrees = false
        //)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// computes magnitude ($magnitude(i)$) of each $(x(i), y(i))$ vector
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#magnitude"/>
        /// <param name="x">floating-point array of x-coordinates of the vectors.</param>
        /// <param name="y">floating-point array of y-coordinates of the vectors; it must have the same size as x.</param>
        /// <returns>
        /// <return name="magnitude">output array of the same size and type as x.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Magnitude", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Magnitude(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat x,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat y
        )
        {
            var magnitude = new Mat();
            Cv2.Magnitude(x, y, magnitude);
            return magnitude;
        }

        /// <summary>
        /// checks that each matrix element is within the specified range.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#checkrange"/>
        /// <param name="src">The array to check</param>
        /// <param name="quiet">The flag indicating whether the functions quietly
        /// return false when the array elements are out of range,
        /// or they throw an exception.</param>//TODO
        /// <param name="minVal">The inclusive lower boundary of valid values range</param>
        /// <param name="maxVal">The exclusive upper boundary of valid values range</param>
        /// <returns>
        /// <return name="check">false, if some values are out of range</return>
        /// <return name="pos">The position of the first outlier.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.CheckRange")]
        public static Tuple<Boolean, Point> CheckRange(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean quiet,
            [InputPin(PropertyMode = PropertyMode.Default)] Double minVal = Double.MinValue,
            [InputPin(PropertyMode = PropertyMode.Default)] Double maxVal = Double.MaxValue
        )
        {
            var pos = new Point();
            var check = Cv2.CheckRange(src, quiet, out pos, minVal, maxVal);
            return Tuple.Create(check, pos);

        }

        /// <summary>
        /// converts NaN's to the given number
        /// </summary>
        /// <param name="a">source matrix.</param>
        /// <param name="val">patch each NaN with val.</param>
        /// <returns>
        /// <return name="a">output matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PatchNaNs", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat PatchNaNs(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat a,
            [InputPin(PropertyMode = PropertyMode.Default)] Double val = 0
        )
        {
            var aOutput = a.Clone();
            Cv2.PatchNaNs(a, val);
            return aOutput;
        }

        /// <summary>
        /// implements generalized matrix product algorithm GEMM from BLAS
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#gemm"/>
        /// <param name="src1">first multiplied input matrix that should have CV_32FC1, CV_64FC1, CV_32FC2, or CV_64FC2 type.</param>
        /// <param name="src2">second multiplied input matrix of the same type as src1.</param>
        /// <param name="alpha">weight of the matrix product.</param>
        /// <param name="src3">third optional delta matrix added to the matrix product; it should have the same type as src1 and src2.</param>
        /// <param name="gamma">weight of src3.</param>
        /// <param name="flags">operation flags:
        /// GEMM_1_T transposes src1.
        /// GEMM_2_T transposes src2.
        /// GEMM_3_T transposes src3.</param>
        /// <returns>
        /// <return name="dst">output matrix; it has the proper size and the same type as input matrices.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Gemm")]
        public static Mat Gemm(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src2,
            [InputPin(PropertyMode = PropertyMode.Default)] Double alpha,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src3,
            [InputPin(PropertyMode = PropertyMode.Default)] Double gamma,
            [InputPin(PropertyMode = PropertyMode.Default)] GemmFlags flags = GemmFlags.None
        )
        {
            var dst = new Mat();
            Cv2.Gemm(src1, src2, alpha, src3, gamma, dst, flags);
            return dst;
        }

        /// <summary>
        /// multiplies matrix by its transposition from the left or from the right
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#multransposed"/>
        /// <param name="src">The source matrix</param>
        /// <param name="aTa">Specifies the multiplication ordering; see the link</param>
        /// <param name="delta">The optional delta matrix, subtracted from src before the
        /// multiplication. When the matrix is empty ( delta=Mat() ), it’s assumed to be
        /// zero, i.e. nothing is subtracted, otherwise if it has the same size as src,
        /// then it’s simply subtracted, otherwise it is "repeated" to cover the full src
        /// and then subtracted. Type of the delta matrix, when it's not empty, must be the
        /// same as the type of created destination matrix, see the rtype description</param>
        /// <param name="scale">The optional scale factor for the matrix product</param>
        /// <param name="dtype">When it’s negative, the destination matrix will have the
        /// same type as src . Otherwise, it will have type=CV_MAT_DEPTH(rtype),
        /// which should be either CV_32F or CV_64F</param>
        /// <returns>
        /// <return name="dst">The destination square matrix</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.MulTransposed")]
        public static Mat MulTransposed(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean aTa,
            [InputPin(PropertyMode = PropertyMode.Default)] Mat delta = null,
            [InputPin(PropertyMode = PropertyMode.Default)] Double scale = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 dtype = -1
        )
        {
            var dst = new Mat();
            Cv2.MulTransposed(src, dst, aTa, delta == null ? new Mat() : delta, scale, dtype);
            return dst;
        }

        /// <summary>
        /// transposes the matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#transpose"/>
        /// <param name="src">The source array</param>
        /// <returns>
        /// <return name="dst">The destination array of the same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Transpose", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Transpose(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var dst = new Mat();
            Cv2.Transpose(src, dst);
            return dst;
        }


        /// <summary>
        /// Performs the matrix transformation of every array element.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#transform"/>
        /// <param name="src">Input array that must have as many channels (1 to 4) as m.cols or m.cols-1.</param>
        /// <param name="m">Transformation $2\times 2$ or $2 \times 3$ floating-point matrix.</param>
        /// <returns>
        /// <return name="dst">Output array of the same size and depth as src; it has as many channels as m.rows.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Transform", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Transform(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat m
        )
        {
            var dst = new Mat();
            Cv2.Transform(src, dst, m);
            return dst;
        }

        /// <summary>
        /// performs perspective transformation of each element of multi-channel input matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#perspectivetransform"/>
        /// <param name="src">The source two-channel or three-channel floating-point array;
        /// each element is 2D/3D vector to be transformed</param>
        /// <param name="m">$3\times 3$ or $4 \times 4$ transformation matrix</param>
        /// <returns>
        /// <return name="dst">The destination array; it will have the same size and same type as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PerspectiveTransform", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat PerspectiveTransform(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat m
        )
        {
            var dst = new Mat();
            Cv2.PerspectiveTransform(src, dst, m);
            return dst;
        }

        /// <summary>
        /// extends the symmetrical matrix from the lower half or from the upper half
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#completesymm"/>
        /// <param name="mtx"> Input floating-point square matrix</param>
        /// <param name="lowerToUpper">If true, the lower half is copied to the upper half,
        /// otherwise the upper half is copied to the lower half</param>
        /// <returns>
        /// <return name="mtx"> output floating-point square matrix</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.CompleteSymm")]
        public static Mat CompleteSymm(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mtx,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean lowerToUpper = false
        )
        {
            var mtxOutput = new Mat();
            Cv2.CompleteSymm(mtxOutput, lowerToUpper);
            return mtxOutput;
        }

        /// <summary>
        /// initializes scaled identity matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#setidentity"/>
        /// <param name="mtx">The matrix to initialize (not necessarily square)</param>
        /// <param name="s">The value to assign to the diagonal elements</param>
        /// <returns>
        /// <return name="mtx">Output matrix</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.SetIdentity")]
        public static Mat SetIdentity(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mtx,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar? s = null
        )
        {
            var mtxOutput = new Mat();
            Cv2.SetIdentity(mtxOutput, s);
            return mtxOutput;
        }


        /// <summary>
        /// computes determinant of a square matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#determinant"/>
        /// <param name="mtx">The input matrix; must have CV_32FC1 or CV_64FC1 type and square size</param>
        /// <returns>
        /// <return name="det">determinant of the specified matrix.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Determinant")]
        public static Double Determinant(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mtx
        )
        {
            return Cv2.Determinant(mtx);
        }

        /// <summary>
        /// computes trace of a matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#trace"/>
        /// <param name="mtx">The source matrix</param>
        /// <returns>
        /// <return name="trace">trace</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Trace")]
        public static Scalar Trace(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mtx
        )
        {
            return Cv2.Trace(mtx);
        }

        /// <summary>
        /// computes inverse or pseudo-inverse matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#invert"/>
        /// <param name="src">The source floating-point MxN matrix</param>
        /// <param name="flags">The inversion method</param>
        /// <returns>
        /// <return name="dst">The destination matrix; will have NxM size and the same type as src</return>
        /// <return name="ret">dependent of src and flags, see link</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Invert")]
        public static Tuple<Mat, Double> Invert(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] DecompTypes flags = DecompTypes.LU
        )
        {
            var dst = new Mat();
            var ret = Cv2.Invert(src, dst, flags);
            return Tuple.Create(dst, ret);
        }

        /// <summary>
        /// Sorts each row or each column of a matrix.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#sort"/>
        /// <param name="src">input single-channel array.</param>
        /// <param name="flags">a combination of operation flags</param>
        /// <returns>
        /// <return name="dst">output array of the same size and type as src.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Sort", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Sort(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] SortFlags flags
        )
        {
            var dst = new Mat();
            Cv2.Sort(src, dst, flags);
            return dst;
        }

        /// <summary>
        /// sorts independently each matrix row or each matrix column
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#sortidx"/>
        /// <param name="src">The source single-channel array</param>
        /// <param name="flags">The operation flags, a combination of SortFlag values</param>
        /// <returns>
        /// <return name="dst">The destination integer array of the same size as src</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.SortIdx", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat SortIdx(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] SortFlags flags
        )
        {
            var dst = new Mat();
            Cv2.SortIdx(src, dst, flags);
            return dst;
        }

        /// <summary>
        /// finds real roots of a cubic polynomial
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#solvecubic"/>
        /// <param name="coeffs">The equation coefficients, an array of 3 or 4 elements</param>
        /// <returns>
        /// <return name="roots">The destination array of real roots which will have 1 or 3 elements</return>
        /// <return name="ret"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.SolveCubic")]
        public static Tuple<Mat, Int32> SolveCubic(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat coeffs
        )
        {
            var roots = new Mat();
            var ret = Cv2.SolveCubic(coeffs, roots);
            return Tuple.Create(roots, ret);
        }

        /// <summary>
        /// finds real and complex roots of a polynomial
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#solvepoly"/>
        /// <param name="coeffs">The array of polynomial coefficients</param>
        /// <param name="maxIters">The maximum number of iterations the algorithm does</param>
        /// <returns>
        /// <return name="roots">The destination (complex) array of roots</return>
        /// <return name="ret"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.SolvePoly")]
        public static Tuple<Mat, Double> SolvePoly(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat coeffs,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxIters = 300
        )
        {
            var roots = new Mat();
            var ret = Cv2.SolvePoly(coeffs, roots, maxIters);
            return Tuple.Create(roots, ret);
        }

        /// <summary>
        /// Computes eigenvalues and eigenvectors of a symmetric matrix.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#eigen"/>
        /// <param name="src">The input matrix; must have CV_32FC1 or CV_64FC1 type,
        /// square size and be symmetric: $src^T == src$</param>
        /// <returns>
        /// <return name="eigenvalues">The output vector of eigenvalues of the same type as src;
        /// The eigenvalues are stored in the descending order.</return>
        /// <return name="eigenvectors">The output matrix of eigenvectors;
        /// It will have the same size and the same type as src; The eigenvectors are stored
        /// as subsequent matrix rows, in the same order as the corresponding eigenvalues</return>
        /// <return name="ret"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Eigen")]
        public static Tuple<Mat, Mat, Boolean> Eigen(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src
        )
        {
            var eigenvalues = new Mat();
            var eigenvectors = new Mat();
            var ret = Cv2.Eigen(src, eigenvalues, eigenvectors);
            return Tuple.Create(eigenvalues, eigenvectors, ret);
        }

        /// <summary>
        /// Calculates the covariance matrix of a set of vectors.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#calccovarmatrix"/>
        /// <param name="samples">samples stored as separate matrices.</param>
        /// <param name="flags">combination of operation flags</param>
        /// <param name="mean">optional input (depending on the flags) array as the average value of the input vectors.</param>
        /// <returns>
        /// <return name="covar">output covariance matrix of the type ctype and square size.</return>
        /// <return name="mean">optional output (depending on the flags) array as the average value of the input vectors.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.CalcCovarMatrix")]
        public static Tuple<Mat, Mat> CalcCovarMatrix(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat[] samples,
            [InputPin(PropertyMode = PropertyMode.Default)] CovarFlags flags,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mean = null
        //[InputPin(PropertyMode = PropertyMode.Default)] MatType ctype
        )
        {
            var meanOutput = (mean == null ? new Mat() : mean.Clone());
            var covar = new Mat();
            Cv2.CalcCovarMatrix(samples, covar, meanOutput, flags);
            return Tuple.Create(covar, meanOutput);
        }

        /// <summary>
        /// Performs Principal Component Analysis of the supplied dataset.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#pca-operator"/>
        /// <param name="data">input samples stored as the matrix rows or as the matrix columns.</param>
        /// <param name="mean">optional mean value; if the matrix is empty (noArray()), the mean is computed from the data.</param>
        /// <param name="maxComponents">maximum number of components that PCA should retain; by default, all the components are retained.</param>
        /// <returns>
        /// <return name="mean">optional mean value; if the matrix is empty (noArray()), the mean is computed from the data.</return>
        /// <return name="eigenvectors">Eigenvectors.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PCACompute")]
        public static Tuple<Mat, Mat> PCACompute(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat data,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mean,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 maxComponents = 0
        )
        {
            var eigenvectors = new Mat();
            Mat meanOutput = (mean == null ? new Mat() : mean.Clone());
            Cv2.PCACompute(data, meanOutput, eigenvectors, maxComponents);
            return Tuple.Create(meanOutput, eigenvectors);
        }

        /// <summary>
        /// Performs Principal Component Analysis of the supplied dataset.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#pca-operator"/>
        /// <param name="data">input samples stored as the matrix rows or as the matrix columns.</param>
        /// <param name="mean">optional mean value; if the matrix is empty (noArray()), the mean is computed from the data.</param>
        /// <param name="retainedVariance">Percentage of variance that PCA should retain. Using this parameter will let the PCA decided how many components to retain but it will always keep at least 2.</param>
        /// <returns>
        /// <return name="eigenvectors">Eigenvectors.</return>
        /// <return name="mean">optional output mean value; if the matrix is empty (noArray()), the mean is computed from the data.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PCAComputeVar")]
        public static Tuple<Mat, Mat> PCAComputeVar(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat data,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mean,
            [InputPin(PropertyMode = PropertyMode.Default)] Double retainedVariance
        )
        {
            var eigenvectors = new Mat();
            Mat meanOutput = (mean == null ? new Mat() : mean.Clone());
            Cv2.PCAComputeVar(data, meanOutput, eigenvectors, retainedVariance);
            return Tuple.Create(eigenvectors, meanOutput);
        }

        /// <summary>Projects vector(s) to the principal component subspace.</summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#pca-project"/>
        /// <param name="data">Input vector(s); must have the same dimensionality and the same layout as the input data used at PCA phase, that is, if $CV_PCA_DATA_AS_ROW$ are specified, then $vec.cols==data.cols$ (vector dimensionality) and $vec.rows$ is the number of vectors to project, and the same is true for the $CV_PCA_DATA_AS_COL$ case.</param>
        /// <param name="mean">optional mean value; if the matrix is empty (noArray()), the mean is computed from the data.</param>
        /// <param name="eigenvectors">optional mean value; if the matrix is empty (noArray()), the mean is computed from the data.</param>
        /// <returns>
        /// <return name="result">In case of $CV_PCA_DATA_AS_COL$, the output matrix has as many columns as the number of input vectors, this means that $result.cols==vec.cols$ and the number of rows match the number of principal components (for example, maxComponents parameter passed to the constructor).</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PCAProject")]
        public static Mat PCAProject(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat data,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mean = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat eigenvectors = null)
        {
            var result = new Mat();
            Cv2.PCAProject(data, mean == null ? new Mat() : mean, eigenvectors == null ? new Mat() : eigenvectors, result);
            return result;
        }

        /// <summary>Reconstructs vectors from their PC projections.</summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#pca-backproject"/>
        /// <param name="data">Coordinates of the vectors in the principal component subspace, the layout and size are the same as of $PCA::project$ output vectors.</param>
        /// <param name="mean"></param>
        /// <param name="eigenvectors"></param>
        /// <returns>
        /// <return name="result">Reconstructed vectors; the layout and size are the same as of PCA::project input vectors.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PCABackProject")]
        public static Mat PCABackProject(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat data,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mean = null,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat eigenvectors = null)
        {
            var result = new Mat();
            Cv2.PCABackProject(data, mean == null ? new Mat() : mean, eigenvectors == null ? new Mat() : eigenvectors, result);
            return result;
        }

        /// <summary>
        /// Returns a zero array of the specified size.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/basic_structures.html?highlight=mat#mat-zeros"/>
        /// <param name="size">Matrix size specification Size(cols, rows).</param>
        /// <returns>
        /// <return name="mat">Output array of type CV_64FC1</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Zeros", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Zeros(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "512, 512")] Size size
            )
        {
            return Mat.Zeros(size, MatType.MakeType(MatType.CV_64F, 1)).ToMat();
        }

        /// <summary>
        /// Returns an array of all 1’s of the specified size.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/basic_structures.html?highlight=mat#mat-ones"/>
        /// <param name="size">Matrix size specification Size(cols, rows).</param>
        /// <returns>
        /// <return name="mat">Output array of type CV_64FC1.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Ones", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Ones(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "512, 512")] Size size
            )
        {
            return Mat.Ones(size, MatType.MakeType(MatType.CV_64F, 1)).ToMat();
        }

        /// <summary>
        /// Returns an identity matrix of the specified size.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/basic_structures.html?highlight=mat#mat-eye"/>
        /// <param name="size">Matrix size specification as Size(cols, rows).</param>
        /// <returns>
        /// <return name="mat">Identity Matrix of type CV_64FC1.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Eye", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Eye(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "512, 512")] Size size
            )
        {
            return Mat.Eye(size, MatType.MakeType(MatType.CV_64F, 1)).ToMat();
        }

        /// <summary>
        /// extracts a rectangle from a given matrix by referencing the original matrix data
        /// </summary>
        /// <param name="mat">Input array</param>
        /// <param name="roi">Rectangle</param>
        /// <returns>
        /// <return name="mat">Output array.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.SubMat", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat SubMat(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mat,
            [InputPin(PropertyMode = PropertyMode.Default)] Rect roi
            )
        {
            var mrect = new Rect(0, 0, mat.Cols, mat.Rows);
            var region = roi & mrect;
            return new Mat(mat, region);
        }

        /// <summary>
        /// extracts a rectangle from a given matrix and copies it to a new matrix
        /// </summary>
        /// <param name="mat">Input array</param>
        /// <param name="roi">Rectangle</param>
        /// <returns>
        /// <return name="mat">Output array.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Crop", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Crop(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mat,
            [InputPin(PropertyMode = PropertyMode.Default)] Rect roi
            )
        {
            var mrect = new Rect(0, 0, mat.Cols, mat.Rows);
            var region = roi & mrect;
            var submat = new Mat(mat, region);
            Mat submatCopy = new Mat(new Size(region.Width, region.Height), mat.Type());
            submat.CopyTo(submatCopy);
            return submatCopy;
        }

        /// <summary>
        /// Changes the shape and/or the number of channels of a 2D matrix without copying the data.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/basic_structures.html#mat-reshape"/>
        /// <param name="mat"></param>
        /// <param name="cn">New number of channels. If the parameter is 0, the number of channels remains the same.</param>
        /// <param name="rows">New number of rows. If the parameter is 0, the number of rows remains the same.</param>
        /// <returns>
        /// <return name="mat"></return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Reshape", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Reshape(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat mat,
            [InputPin(PropertyMode = PropertyMode.Default)] int cn = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] int rows = 0
            )
        {
            var reshape = mat.Clone();
            reshape.Reshape(cn, rows);
            return reshape;
        }

        /// <summary>
        /// Computes SVD of src.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#svd-compute"/>
        /// <param name="src">Decomposed matrix</param>
        /// <param name="flags">Operation flags </param>
        /// <returns>
        /// <return name="w">Calculated singular values</return>
        /// <return name="u">Calculated left singular vectors</return>
        /// <return name="vt">Transposed matrix of right singular values</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.SVDecomp")]
        public static Tuple<Mat, Mat, Mat> SVDecomp(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] SVD.Flags flags = SVD.Flags.None
            )
        {
            var w = new Mat();
            var u = new Mat();
            var vt = new Mat();
            Cv2.SVDecomp(src, w, u, vt, flags);
            return Tuple.Create(w, u, vt);
        }


        /// <summary>
        /// Performs back substitution for the previously computed SVD.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#svd-backsubst"/>
        /// <param name="w">Singular values.</param>
        /// <param name="u">Left singular vectors.</param>
        /// <param name="vt">Transposed matrix of right singular vectors.</param>
        /// <param name="rhs">Right-hand side of a linear system $(u*w*v')*dst = rhs$ to be solved, where $A$ has been previously decomposed.</param>
        /// <returns>
        /// <return name="dst">Found solution of the system.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.SVBackSubst", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat SVBackSubst(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat w,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat u,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat vt,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat rhs
            )
        {
            var dst = new Mat();
            Cv2.SVBackSubst(w, u, vt, rhs, dst);
            return dst;
        }


        /// <summary>
        /// Computes Mahalanobis distance between two vectors: $\sqrt{(v1-v2)\cdot icovar \cdot (v1-v2)}$, where icovar is the inverse covariation matrix
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#mahalanobis"/>
        /// <param name="v1">First 1D input vector.</param>
        /// <param name="v2">Second 1D input vector.</param>
        /// <param name="icovar">Inverse covariance matrix.</param>
        /// <returns>
        /// <return name="mahalonobis">Weighted distance</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Mahalonobis")]
        public static Double Mahalonobis(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat v1,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat v2,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat icovar
            )
        {
            return Cv2.Mahalanobis(v1, v2, icovar);
        }


        /// <summary>
        /// Performs a forward or inverse Discrete Fourier transform of a 1D or 2D floating-point array.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#dft"/>
        /// <param name="src">Input array that could be real or complex.</param>
        /// <param name="flags">Transformation flags, combination ot the following values: DFT_INVERSE, DFT_SCALE, DFT_ROWS, DFT_COMPLEX_OUTPUT, DFT_REAL_OUTPUT</param>
        /// <param name="nonzeroRows">When the parameter is not zero, the function assumes that only the first nonzeroRows rows of the input array (DFT_INVERSE is not set) or only the first nonzeroRows of the output array (DFT_INVERSE is set) contain non-zeros, thus, the function can handle the rest of the rows more efficiently and save some time; this technique is very useful for calculating array cross-correlation or convolution using DFT.</param>
        /// <returns>
        /// <return name="dst">Output array whose size and type depends on the flags.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Dft", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Dft(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] DftFlags flags = DftFlags.None,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 nonzeroRows = 0
            )
        {
            var dst = new Mat();
            Cv2.Dft(src, dst, flags, nonzeroRows);
            return dst;
        }


        /// <summary>
        /// Performs an inverse Discrete Fourier transform of 1D or 2D floating-point array.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#idft"/>
        /// <param name="src">The source array, real or complex</param>
        /// <param name="flags">Transformation flags, a combination of the DftFlag2 values</param>
        /// <param name="nonzeroRows">When the parameter $!= 0$, the function assumes that
        /// only the first nonzeroRows rows of the input array ( DFT_INVERSE is not set)
        /// or only the first nonzeroRows of the output array ( DFT_INVERSE is set) contain non-zeros,
        /// thus the function can handle the rest of the rows more efficiently and
        /// thus save some time. This technique is very useful for computing array cross-correlation
        /// or convolution using DFT</param>
        /// <returns>
        /// <return name="dst">Output array, which size and type depends on the flags.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Idft", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Idft(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] DftFlags flags = DftFlags.None,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 nonzeroRows = 0
            )
        {
            var dst = new Mat();
            Cv2.Idft(src, dst, flags, nonzeroRows);
            return dst;
        }


        /// <summary>
        /// Performs a forward or inverse discrete Cosine transform of 1D or 2D array.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#dct"/>
        /// <param name="src">Input floating-point array.</param>
        /// <param name="flags">Transformation flags as a combination of the following values: DCT_INVERSE, DCT_ROWS</param>
        /// <returns>
        /// <return name="dst">Output array of the same size and type as src .</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Dct", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Dct(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] DctFlags flags = DctFlags.None
            )
        {
            var dst = new Mat();
            Cv2.Dct(src, dst, flags);
            return dst;
        }


        /// <summary>
        /// Performs inverse 1D or 2D Discrete Cosine Transformation
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#idct"/>
        /// <param name="src">Input floating-point array</param>
        /// <param name="flags">Transformation flags, a combination of DctFlag2 values</param>
        /// <returns>
        /// <return name="dst">Output array</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Idct", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Idct(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat src,
            [InputPin(PropertyMode = PropertyMode.Default)] DctFlags flags = DctFlags.None
            )
        {
            var dst = new Mat();
            Cv2.Idct(src, dst, flags);
            return dst;
        }


        /// <summary>
        /// computes element-wise product of the two Fourier spectrums. The second spectrum can optionally be conjugated before the multiplication
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#mulspectrums"/>
        /// <param name="a">first input array.</param>
        /// <param name="b">second input array of the same size and type as src1 .</param>
        /// <param name="flags">operation flags; currently, the only supported flag is DFT_ROWS, which indicates that each row of src1 and src2 is an independent 1D Fourier spectrum.</param>
        /// <param name="conjB">optional flag that conjugates the second input array before the multiplication (true) or not (false).</param>
        /// <returns>
        /// <return name="c">output array.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.MulSpectrums", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat MulSpectrums(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat a,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat b,
            [InputPin(PropertyMode = PropertyMode.Default)] DftFlags flags,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean conjB = false
            )
        {
            var c = new Mat();
            Cv2.MulSpectrums(a, b, c, flags, conjB);
            return c;
        }


        /// <summary>
        /// computes the minimal vector size $vecsize1 \geq vecsize$ so that the dft() of the vector of length vecsize1 can be computed efficiently
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#getoptimaldftsize"/>
        /// <param name="vecsize">Maximal dft size</param>
        /// <returns>
        /// <return name="opt">optimal dft size.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.GetOptimalDFTSize")]
        public static Int32 GetOptimalDFTSize(
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 vecsize
            )
        {
            return Cv2.GetOptimalDFTSize(vecsize);
        }


        /// <summary>
        /// Finds centers of clusters and groups input samples around the clusters.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/clustering.html#kmeans"/>
        /// <param name="data">Data for clustering.</param>
        /// <param name="k">Number of clusters to split the set by.</param>
        /// <param name="bestLabels">Input integer array that stores the cluster indices for every sample.</param>
        /// <param name="criteria">The algorithm termination criteria, that is, the maximum number of iterations and/or the desired accuracy. The accuracy is specified as criteria.epsilon. As soon as each of the cluster centers moves by less than criteria.epsilon on some iteration, the algorithm stops.</param>
        /// <param name="attempts">Flag to specify the number of times the algorithm is executed using different initial labellings. The algorithm returns the labels that yield the best compactness (see the last function parameter).</param>
        /// <param name="flags">Flag that can take the following values: KMEANS_RANDOM_CENTERS, KMEANS_PP_CENTERS, KMEANS_USE_INITIAL_LABELS</param>
        /// <returns>
        /// <return name="centers">Output matrix of the cluster centers, one row per each cluster center.</return>
        /// <return name="bestLabels">Output integer array that stores the cluster indices for every sample.</return>
        /// <return name="compactness">Compactness measure.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Kmeans", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Mat, Mat, Double> Kmeans(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat data,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 k,
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat bestLabels,
            [InputPin(PropertyMode = PropertyMode.Default)] TermCriteria criteria,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 attempts,
            [InputPin(PropertyMode = PropertyMode.Default)] KMeansFlags flags)
        {
            var centers = new Mat();
            var bestLabelsOutput = bestLabels.Clone();
            var compactness = Cv2.Kmeans(data, k, bestLabelsOutput, criteria, attempts, flags);
            return Tuple.Create(centers, bestLabelsOutput, compactness);
        }


        /// <summary>
        /// shuffles the input array elements
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#randshuffle"/>
        /// <param name="dst">The input numerical 1D array</param>
        /// <param name="iterFactor">The scale factor that determines the number of random swap operations.</param>
        /// <param name="rng">The optional random number generator used for shuffling.
        /// If it is null, theRng() is used instead.</param>
        /// <returns>
        /// <return name="dst">output array</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.RandShuffle", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat RandShuffle(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat dst,
            [InputPin(PropertyMode = PropertyMode.Default)] Double iterFactor,
            [InputPin(PropertyMode = PropertyMode.Default)] RNG rng = null
            )
        {
            var dstOutput = dst.Clone();
            Cv2.RandShuffle(dstOutput, iterFactor, rng);
            return dstOutput;
        }

        /// <summary>
        /// Draws for each triple (startPoint, endPoint, color) a line segment.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#line"/>
        /// <param name="img">The image. </param>
        /// <param name="startPoints">First point of the line segment. </param>
        /// <param name="endPoints">Second point of the line segment. </param>
        /// <param name="color">Line color. </param>
        /// <param name="thickness">Line thickness. [By default this is 1]</param>
        /// <param name="lineType">Type of the line. [By default this is LineType.Link8]</param>
        /// <param name="shift">Number of fractional bits in the point coordinates. [By default this is 0]</param>
        /// <returns>
        /// <return name="img">The image with the drawn line.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Lines", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Lines(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Allow)] Point[] startPoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] Point[] endPoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 thickness = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] LineTypes lineType = LineTypes.Link8,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 shift = 0
            )
        {
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;

            Scalar c = color;
            var imgOutput = img.Clone();
            for (int i = 0; i < startPoints.Count(); i++)
            {
                if (randomColor)
                    c = Scalar.RandomColor();
                Cv2.Line(imgOutput, startPoints[i], endPoints[i], c, thickness, lineType, shift);
            }
            return imgOutput;
        }

        /// <summary>
        /// Draws simple, thick or filled rectangle
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#rectangle"/>
        /// <param name="img">Image. </param>
        /// <param name="rects">Array of Rectangles.</param>
        /// <param name="color">Line color (RGB) or brightness (grayscale image). If only one color is given, all rectangles are drawn in this color.</param>
        /// <param name="thickness">Thickness of lines that make up the rectangle. For negative values the function draws a filled rectangle. [By default this is 1]</param>
        /// <param name="lineType">Type of the line, see cvLine description. [By default this is LineType.Link8]</param>
        /// <param name="shift">Number of fractional bits in the point coordinates. [By default this is 0]</param>
        /// <returns>
        /// <return name="img">Image with drawn rectangle.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Rectangles", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Rectangles(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Allow)] Rect[] rects,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 thickness = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] LineTypes lineType = LineTypes.Link8,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 shift = 0
            )
        {
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;

            Scalar c = color;
            var imgOutput = img.Clone();
            for (int i = 0; i < rects.Count(); i++)
            {
                if (randomColor)
                    c = Scalar.RandomColor();
                Cv2.Rectangle(imgOutput, rects[i], c, thickness, lineType, shift);
            }
            return imgOutput;
        }

        /// <summary>
        /// Draws simple rotated rectangle
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#rectangle"/>
        /// <param name="img">Image. </param>
        /// <param name="rects">Array of rectangles.</param>
        /// <param name="color">Line color (RGB) or brightness (grayscale image).</param>
        /// <param name="thickness">Thickness of lines that make up the rectangle. [By default this is 1]</param>
        /// <param name="lineType">Type of the line, see cvLine description. [By default this is LineType.Link8]</param>
        /// <param name="shift">Number of fractional bits in the point coordinates. [By default this is 0]</param>
        /// <param name="ellipse"> If true, draw all rectagnles as ellipse.</param>
        /// <returns>
        /// <return name="img">Image with drawn rectangle.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.RotatedRectangles", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat RotatedRectangles(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Allow)] RotatedRect[] rects,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 thickness = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] LineTypes lineType = LineTypes.Link8,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 shift = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] Boolean ellipse = false
            )
        {
            var imgOutput = img.Clone();
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;

            Scalar c = color;
            if (ellipse == false)
            {
                for (int i = 0; i < rects.Count(); i++)
                {
                    var rectPoints = rects[i].Points();
                    for (int j = 0; j < 4; j++)
                    {
                        if (randomColor)
                            c = Scalar.RandomColor();
                        Point pt1;
                        Point pt2;
                        pt1.X = (int)rectPoints[j].X;
                        pt1.Y = (int)rectPoints[j].Y;
                        pt2.X = (int)rectPoints[(j + 1) % 4].X;
                        pt2.Y = (int)rectPoints[(j + 1) % 4].Y;
                        Cv2.Line(imgOutput, pt1, pt2, c, thickness, lineType, shift);
                    }
                }

            }
            else
            {
                for (int i = 0; i < rects.Count(); i++)
                {
                    if (randomColor)
                        c = Scalar.RandomColor();
                    Cv2.Ellipse(imgOutput, rects[i], c, thickness, lineType);
                }
            }
            return imgOutput;
        }

        /// <summary>
        /// Draws a circle for each center point and radius.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#circle"/>
        /// <param name="img">Image. </param>
        /// <param name="centers">Sequence of center of the circle.</param>
        /// <param name="radius">Sequence of radius of the circle.</param>
        /// <param name="color">Circle color. If only one color is given, all circles are drawn in this color.</param>
        /// <param name="thickness">Thickness of the circle outline if positive, otherwise indicates that a filled circle has to be drawn. [By default this is 1]</param>
        /// <param name="lineType">Type of the circle boundary. [By default this is LineType.Link8]</param>
        /// <param name="shift">Number of fractional bits in the center coordinates and radius value. [By default this is 0]</param>
        /// <returns>
        /// <return name="img">Image with drawn circles.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Circles", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Circles(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Allow)] Point[] centers,
            [InputPin(PropertyMode = PropertyMode.Allow)] Int32[] radius,
            [InputPin(PropertyMode = PropertyMode.Default)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 thickness = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] LineTypes lineType = LineTypes.Link8,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 shift = 0
            )
        {
            var imgOutput = img.Clone();
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;
            Scalar c = color;
            for (int i = 0; i < centers.Count(); i++)
            {
                if (randomColor)
                    c = Scalar.RandomColor();
                Cv2.Circle(imgOutput, centers[i], radius[i], c, thickness, lineType, shift);
            }
            return imgOutput;
        }

        /// <summary>
        /// Draws simple or thick elliptic arc or fills ellipse sector for each center
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#ellipse"/>
        /// <param name="img">Image. </param>
        /// <param name="centers">Sequence of the centers of the ellipse. </param>
        /// <param name="axes">Sequence of the lengths of the ellipse axes.</param>
        /// <param name="angles">Sequence of rotation angles. </param>
        /// <param name="startAngles">Sequence of starting angles of the elliptic arc. </param>
        /// <param name="endAngles">Sequence of ending angles of the elliptic arc. </param>
        /// <param name="color">Ellipse color. If only one is given, draw every ellipse in this color.</param>
        /// <param name="thickness">Thickness of the ellipse arc. [By default this is 1]</param>
        /// <param name="lineType">Type of the ellipse boundary. [By default this is LineType.Link8]</param>
        /// <param name="shift">Number of fractional bits in the center coordinates and axes' values. [By default this is 0]</param>
        /// <returns>
        /// <return name="img">Image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.ImgProc.Ellipses", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Ellipses(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Default)] Point[] centers,
            [InputPin(PropertyMode = PropertyMode.Allow)] Size[] axes,
            [InputPin(PropertyMode = PropertyMode.Allow)] Double[] angles,
            [InputPin(PropertyMode = PropertyMode.Allow)] Double[] startAngles,
            [InputPin(PropertyMode = PropertyMode.Allow)] Double[] endAngles,
            [InputPin(PropertyMode = PropertyMode.Allow)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 thickness = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] LineTypes lineType = LineTypes.Link8,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 shift = 0)
        {
            var imgOutput = img.Clone();
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;

            Scalar c = color;
            for (int i = 0; i < centers.Count(); i++)
            {
                if (randomColor)
                    c = Scalar.RandomColor();
                Cv2.Ellipse(img, centers[i], axes[i], angles[i], startAngles[i], endAngles[i], c, thickness, lineType, shift);
            }

            return imgOutput;
        }

        /// <summary>
        /// Draws a Array of text strings.
        /// </summary>
        /// <reference href="http://docs.opencv.org/modules/core/doc/drawing_functions.html#puttext" />
        /// <param name="img">Image.</param>
        /// <param name="texts">Text string to be drawn.</param>
        /// <param name="origins">Bottom-left corner of the text string in the image.</param>
        /// <param name="fontFace">Font type. One of FONT_HERSHEY_SIMPLEX, FONT_HERSHEY_PLAIN, FONT_HERSHEY_DUPLEX, FONT_HERSHEY_COMPLEX, FONT_HERSHEY_TRIPLEX, FONT_HERSHEY_COMPLEX_SMALL, FONT_HERSHEY_SCRIPT_SIMPLEX, or FONT_HERSHEY_SCRIPT_COMPLEX, where each of the font ID’s can be combined with FONT_HERSHEY_ITALIC to get the slanted letters.</param>
        /// <param name="fontScale">Font scale factor that is multiplied by the font-specific base size.</param>
        /// <param name="color">Text color. If only one is supplied, only one will be used.</param>
        /// <param name="thickness">Thickness of the lines used to draw a text.</param>
        /// <param name="lineType">Line type. See the line for details.</param>
        /// <param name="bottomLeftOrigin">When true, the image data origin is at the bottom-left corner. Otherwise, it is at the top-left corner.</param>
        /// <returns>
        /// <return name="img">Image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.PutTexts", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat PutTexts(
            [InputPin(PropertyMode = PropertyMode.Allow)] Mat img,
            [InputPin(PropertyMode = PropertyMode.Allow)] string[] texts,
            [InputPin(PropertyMode = PropertyMode.Allow)] Point[] origins,
            [InputPin(PropertyMode = PropertyMode.Default)] HersheyFonts fontFace,
            [InputPin(PropertyMode = PropertyMode.Allow)] Scalar color,
            [InputPin(PropertyMode = PropertyMode.Default)] Double fontScale = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] Int32 thickness = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] LineTypes lineType = LineTypes.Link8,
            [InputPin(PropertyMode = PropertyMode.Default)] bool bottomLeftOrigin = false
            )
        {
            var dst = img.Clone();
            bool randomColor = false;
            if (color.Val0 < 0 || color.Val1 < 0 || color.Val2 < 0 || color.Val3 < 0)
                randomColor = true;

            Scalar c = color;
            for (int i = 0; i < texts.Count(); i++)
            {
                if (randomColor)
                    c = Scalar.RandomColor();
                Cv2.PutText(dst, texts[i], origins[i], fontFace, fontScale, c, thickness, lineType, bottomLeftOrigin);
            }
            return dst;
        }

        /// <summary>
        /// Fills a convex polygon.
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#fillconvexpoly"/>
        /// <param name="img">Image</param>
        /// <param name="pts">The polygon vertices</param>
        /// <param name="color">Polygon color</param>
        /// <param name="lineType">Type of the polygon boundaries</param>
        /// <param name="shift">The number of fractional bits in the vertex coordinates</param>
        /// <returns>
        /// <return name="img">Image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.FillConvexPoly", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FillConvexPoly(Mat img, IEnumerable<Point> pts, Scalar color, LineTypes lineType = LineTypes.Link8, Int32 shift = 0)
        {
            var imgOutput = img.Clone();
            Cv2.FillConvexPoly(imgOutput, pts, color, lineType, shift);
            return imgOutput;
        }

        /// <summary>
        /// Fills the area bounded by one or more polygons
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#fillpoly"/>
        /// <param name="img">Image</param>
        /// <param name="pts">Array of polygons, each represented as an array of points</param>
        /// <param name="color">Polygon color</param>
        /// <param name="lineType">Type of the polygon boundaries</param>
        /// <param name="shift">The number of fractional bits in the vertex coordinates</param>
        /// <param name="offset">Optional offset of all points of the contours.</param>
        /// <returns>
        /// <return name="img">Image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.FillPoly", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat FillPoly(Mat img, IEnumerable<IEnumerable<Point>> pts, Scalar color, LineTypes lineType = LineTypes.Link8, Int32 shift = 0, Point? offset = null)
        {
            var imgOutput = img.Clone();
            Cv2.FillPoly(imgOutput, pts, color, lineType, shift, offset);
            return imgOutput;
        }

        /// <summary>
        /// draws one or more polygonal curves
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#polylines"/>
        /// <param name="img">Image.</param>
        /// <param name="pts">Array of polygonal curves.</param>
        /// <param name="isClosed">Flag indicating whether the drawn polylines are closed or not. If they are closed, the function draws a line from the last vertex of each curve to its first vertex.</param>
        /// <param name="color">Polyline color.</param>
        /// <param name="thickness">Thickness of the polyline edges.</param>
        /// <param name="lineType">Type of the line segments.</param>
        /// <param name="shift">Number of fractional bits in the vertex coordinates.</param>
        /// <returns>
        /// <return name="img">Image.</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.Polylines", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Mat Polylines(Mat img, IEnumerable<IEnumerable<Point>> pts, Boolean isClosed, Scalar color, Int32 thickness = 1, LineTypes lineType = LineTypes.Link8, Int32 shift = 0)
        {
            var imgOutput = img.Clone();
            Cv2.Polylines(imgOutput, pts, isClosed, color, thickness, lineType, shift);
            return imgOutput;
        }

        /// <summary>
        /// Clips the line against the image rectangle
        /// </summary>
        /// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/drawing_functions.html#clipline"/>
        /// <param name="imgRect">sThe image rectangle</param>
        /// <returns>
        /// <return name="result"></return>
        /// <return name="pt1">The first line point</return>
        /// <return name="pt2">The second line point</return>
        /// </returns>
        [StaticModule(ModuleType = "OpenCv.Core.ClipLine", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        public static Tuple<Boolean, Point, Point> ClipLine(Rect imgRect)
        {
            var pt1 = new Point();
            var pt2 = new Point();
            var result = Cv2.ClipLine(imgRect, ref pt1, ref pt2);
            return Tuple.Create(result, pt1, pt2);
        }

        //// TODO cluster core
        ///// <summary>
        ///// Fills arrays with random numbers.
        ///// </summary>
        ///// <reference href="http://docs.opencv.org/2.4.8/modules/core/doc/operations_on_arrays.html#rng-fill"/>
        ///// <param name="size">Size of a 2D-Matrix with one channel.</param>
        ///// <param name="distType">distribution type, RNG::UNIFORM or RNG::NORMAL.</param>
        ///// <param name="a">First distribution parameter; in case of the uniform distribution, this is an inclusive lower boundary, in case of the normal distribution, this is a mean value.</param>
        ///// <param name="b">Second distribution parameter; in case of the uniform distribution, this is a non-inclusive upper boundary, in case of the normal distribution, this is a standard deviation (diagonal of the standard deviation matrix or the full standard deviation matrix).</param>
        ///// <param name="saturateRange">pre-saturation flag; for uniform distribution only; if true, the method will first convert a and b to the acceptable value range (according to the mat datatype) and then will generate uniformly distributed random numbers within the range [saturate(a), saturate(b)), if saturateRange=false, the method will generate uniformly distributed random numbers in the original range [a, b) and then will saturate them, it means, for example, that theRNG().fill(mat_8u, RNG::UNIFORM, -DBL_MAX, DBL_MAX) will likely produce array mostly filled with 0’s and 255’s, since the range (0, 255) is significantly smaller than [-DBL_MAX, DBL_MAX).</param>
        ///// <returns>
        ///// <return name="mat"></return>
        ///// </returns>
        //[StaticModule(ModuleType = "OpenCv.Core.RngFill", PreviewGenerator = typeof(OpenCvPreviewGenerator))]
        //public static Mat RngFill(
        //    [InputPin(PropertyMode = PropertyMode.Default)] Size size,
        //    [InputPin(PropertyMode = PropertyMode.Default)] DistributionType distType,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] int a,
        //    [InputPin(PropertyMode = PropertyMode.Allow)] int b,
        //    [InputPin(PropertyMode = PropertyMode.Default)] bool saturateRange = false)
        //{
        //    var mat = new Mat(size, MatType.CV_32FC1);

        //    RNG rng = new RNG();
        //    rng.Fill(mat, distType, a, b, saturateRange);
        //    return mat;
        //}
    }
}