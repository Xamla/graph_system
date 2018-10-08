using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using Xamla.Types;
using Xamla.Types.Converters;

namespace Xamla.Graph.Modules.OpenCv
{
    public class OpenCvConverter
        : ITypeConversionProvider
    {
        static Dictionary<PixelType, MatType> PixelTypeToDepth = new Dictionary<PixelType, MatType>
        {
            { PixelType.U8C1, MatType.CV_8UC1 },
            { PixelType.U8C3, MatType.CV_8UC3 },
            { PixelType.U8C4, MatType.CV_8UC4 },
            { PixelType.U16C1, MatType.CV_16UC1 },
            { PixelType.U16C3, MatType.CV_16UC3 },
            { PixelType.U16C4, MatType.CV_16UC4 },
            { PixelType.F32C1, MatType.CV_32FC1 },
            { PixelType.F32C3, MatType.CV_32FC3 },
            { PixelType.F32C4, MatType.CV_32FC4 },
            { PixelType.F64C1, MatType.CV_64FC1 },
            { PixelType.F64C3, MatType.CV_64FC3 },
            { PixelType.F64C4, MatType.CV_64FC4 },
        };

        public OpenCvConverter()
        {
        }

        public PixelFormat GetPixelFormat(Mat image)
        {
            var ranges = new Range<double>[image.Channels()];

            var matType = image.Type();
            if (matType.Equals(MatType.MakeType(MatType.CV_8U, 1))
                || matType.Equals(MatType.MakeType(MatType.CV_8U, 3))
                || matType.Equals(MatType.MakeType(MatType.CV_8U, 4))
                )
            {
                for (int i = 0; i < image.Channels(); ++i)
                    ranges[i] = new Range<double>(byte.MinValue, byte.MaxValue);

                return new PixelFormat((PixelType)((int)ChannelType.U8 | image.Channels()), image.Channels() == 1 ? PixelChannels.Gray : PixelChannels.Unknown, typeof(byte), ranges, ColorSpace.Unknown);
            }
            else if (matType.Equals(MatType.MakeType(MatType.CV_32F, 1))
                || matType.Equals(MatType.MakeType(MatType.CV_32F, 3))
                || matType.Equals(MatType.MakeType(MatType.CV_32F, 4))
                )
            {
                for (int i = 0; i < image.Channels(); ++i)
                    ranges[i] = new Range<double>(float.MinValue, float.MaxValue);

                return new PixelFormat((PixelType)((int)ChannelType.F32 | image.Channels()), image.Channels() == 1 ? PixelChannels.Gray : PixelChannels.Unknown, typeof(float), ranges, ColorSpace.Unknown);
            }

            throw new Exception("PixelFormat is not supported.");
        }

        public Mat ImageBufferToMat(IImageBuffer imageBuffer)
        {
            var i = imageBuffer.ToU8();
            using (var pin = i.Pin())
            {
                var mat = new Mat(i.Height, i.Width, PixelTypeToDepth[i.Format.PixelType], pin.Pointer);
                return mat.Clone();
            }
        }

        public IImageBuffer MatToImageBuffer(Mat mat)
        {
            var arraySize = mat.Width * mat.Height * mat.Channels();

            var matType = mat.Type();
            if (matType.Equals(MatType.MakeType(MatType.CV_8U, 1))
                || matType.Equals(MatType.MakeType(MatType.CV_8U, 3))
                || matType.Equals(MatType.MakeType(MatType.CV_8U, 4))
                )
            {
                var dst = new I<byte>(this.GetPixelFormat(mat), mat.Height, mat.Width, mat.Channels());
                if (mat.IsContinuous())
                {
                    Marshal.Copy(mat.Data, dst.Data.Buffer, 0, arraySize);
                }
                else
                {
                    var buffer = dst.Data.Buffer;
                    int stride = dst.Data.Stride[0];
                    for (int i = 0, y = 0; y < mat.Height; ++y, i += stride)
                    {
                        Marshal.Copy(mat.Ptr(y), buffer, i, stride);
                    }
                    return dst;
                }
                return dst;
            }
            else if (matType.Equals(MatType.MakeType(MatType.CV_32F, 1))
                || matType.Equals(MatType.MakeType(MatType.CV_32F, 3))
                || matType.Equals(MatType.MakeType(MatType.CV_32F, 4))
                )
            {
                var dst = new I<float>(this.GetPixelFormat(mat), mat.Height, mat.Width, mat.Channels());
                if (mat.IsContinuous())
                {
                    Marshal.Copy(mat.Data, dst.Data.Buffer, 0, arraySize);
                }
                else
                {
                    var buffer = dst.Data.Buffer;
                    int stride = dst.Data.Stride[0];
                    for (int i = 0, y = 0; y < mat.Height; ++y, i += stride)
                    {
                        Marshal.Copy(mat.Ptr(y), buffer, i, stride);
                    }
                    return dst;
                }
                return dst;
            }

            throw new Exception("Unknown Image. This Provider can just convert Mat to ImageBuffer.");
        }

        public unsafe A MatToA(Mat mat, Type destinationType = null)
        {
            if (mat.Channels() != 1)
                throw new NotSupportedException();

            uint arraySize = (uint)(mat.Width * mat.Height * mat.Channels());
            var d = mat.Type().Depth;

            Type underlyingMatType = null;
            if (d == MatType.CV_16S)
            {
                underlyingMatType = typeof(short);
            }
            else if (d == MatType.CV_16U)
            {
                underlyingMatType = typeof(ushort);
            }
            else if (d == MatType.CV_32F)
            {
                underlyingMatType = typeof(float);
            }
            else if (d == MatType.CV_32S)
            {
                underlyingMatType = typeof(int);
            }
            else if (d == MatType.CV_64F)
            {
                underlyingMatType = typeof(double);
            }
            else if (d == MatType.CV_8S)
            {
                underlyingMatType = typeof(sbyte);
            }
            else if (d == MatType.CV_8U)
            {
                underlyingMatType = typeof(byte);
            }

            if (destinationType == null)
                destinationType = underlyingMatType;

            if (underlyingMatType == null)
                throw new Exception("Underlying MatType is null");

            var mType = typeof(A<>).MakeGenericType(destinationType);
            var a = (A)Activator.CreateInstance(mType, mat.Rows, mat.Cols);

            if (underlyingMatType != destinationType)
                throw new NotImplementedException();

            using (var destinationBase = a.Pin())
            {
                if (mat.IsContinuous())
                {
                    Buffer.MemoryCopy(mat.Data.ToPointer(), destinationBase.Pointer.ToPointer(), a.SizeInBytes, arraySize * (uint)Marshal.SizeOf(destinationType));
                }
                else
                {
                    int stride = a.Stride[0] * Marshal.SizeOf(destinationType);     // stride in bytes
                    for (int offset = 0, r = 0; r < a.Dimension[0]; ++r, offset += stride)
                    {
                        Buffer.MemoryCopy(mat.Ptr(r).ToPointer(), IntPtr.Add(destinationBase.Pointer, offset).ToPointer(), (uint)stride, (uint)stride);
                    }
                }
            }

            return a;
        }

        public Mat AToMat(A a)
        {
            if (a.Rank > 3)
                throw new Exception("Mat does not support Rank > 3");

            int depth;
            if (a.ElementType == typeof(short))
            {
                depth = MatType.CV_16S;
            }
            else if (a.ElementType == typeof(ushort))
            {
                depth = MatType.CV_16U;
            }
            else if (a.ElementType == typeof(float))
            {
                depth = MatType.CV_32F;
            }
            else if (a.ElementType == typeof(int))
            {
                depth = MatType.CV_32S;
            }
            else if (a.ElementType == typeof(double))
            {
                depth = MatType.CV_64F;
            }
            else if (a.ElementType == typeof(sbyte))
            {
                depth = MatType.CV_8S;
            }
            else if (a.ElementType == typeof(byte))
            {
                depth = MatType.CV_8U;
            }
            else
            {
                throw new Exception("ElementType is not supported for Mat.");
            }

            int cols = 1;
            if (a.Rank > 1)
                cols = a.Dimension[1];

            int channels = 1;
            if (a.Rank == 3)
                channels = a.Dimension[2];

            var matType = MatType.MakeType(depth, channels);
            var mat = new Mat(a.Dimension[0], cols, matType, a.Pin().Pointer);
            return mat.Clone();
        }

        public IEnumerable<ITypeConverter> GetConverters()
        {
            return new[]
            {
                // Point
                TypeConverter.Create<Point, Xamla.Types.Int2>(p => new Int2(p.X, p.Y)),
                TypeConverter.Create<Xamla.Types.Int2, Point>(p => new Point(p.X, p.Y)),
                TypeConverter.Create<Xamla.Types.Float2, Point>(p => new Point(p.X, p.Y)),
                TypeConverter.Create<Point, Xamla.Types.Float2>(p => new Float2(p.X, p.Y)),

                // Point2d
                TypeConverter.Create<Xamla.Types.Float2, Point2d>(p => new Point2d(p.X, p.Y)),
                TypeConverter.Create<Point2d, Xamla.Types.Float2>(p => new Float2(p.X, p.Y)),

                // Point2f
                TypeConverter.Create<Xamla.Types.Float2, Point2f>(p => new Point2f((float)p.X, (float)p.Y)),
                TypeConverter.Create<Point2f, Xamla.Types.Float2>(p => new Float2(p.X, p.Y)),

                // Point3i
                TypeConverter.Create<Xamla.Types.Int3, Point3i>(p => new Point3i(p.X, p.Y, p.Z)),
                TypeConverter.Create<Point3i, Xamla.Types.Int3>(p => new Int3(p.X, p.Y, p.Z)),

                // Point3f
                TypeConverter.Create<Xamla.Types.Float3, Point3f>(p => new Point3f((float)p.X, (float)p.Y, (float)p.Z)),
                TypeConverter.Create<Point3f, Xamla.Types.Float3>(p => new Float3(p.X, p.Y, p.Z)),

                // Point3d
                TypeConverter.Create<Xamla.Types.Float3, Point3d>(p => new Point3d(p.X, p.Y, p.Z)),
                TypeConverter.Create<Point3d, Xamla.Types.Float3>(p => new Float3(p.X, p.Y, p.Z)),

                // Size
                TypeConverter.Create<Size, Xamla.Types.Int2>(s => new Int2(s.Width, s.Height)),
                TypeConverter.Create<Xamla.Types.Int2, Size>(i2 => new Size(i2.X, i2.Y)),

                // Size2f
                TypeConverter.Create<Size2f, Xamla.Types.Float2>(s => new Float2(s.Width, s.Height)),
                TypeConverter.Create<Xamla.Types.Float2, Size2f>(p => new Size2f(p.X, p.Y)),

                TypeConverter.Create<IImageBuffer, Mat>(this.ImageBufferToMat),
                TypeConverter.Create<Mat, IImageBuffer>(this.MatToImageBuffer),
                TypeConverter.Create<Mat, InputArray>(InputArray.Create),
                TypeConverter.Create<Mat, InputOutputArray>(x => x),
                TypeConverter.Create<OutputArray, Mat>(x => x.GetMat()),
                TypeConverter.Create<OutputArray, InputArray>(x => InputArray.Create(x.GetMat())),
                TypeConverter.Create<IntRect, Rect>(x => new Rect(x.Left, x.Top, x.Size.X, x.Size.Y)),
                TypeConverter.Create<Rect, IntRect>(x => new IntRect(x.Left, x.Top, x.Right, x.Bottom)),
                TypeConverter.Create<Point2f[], Mat>(points => {
                    var type = MatType.MakeType(MatType.CV_32F, 2);
                    var m = new Mat(1, points.Length, type, points);
                    return m;
                }),
                TypeConverter.Create<Mat, Point2f[]>(mat => {
                    var array = new Point2f[mat.Cols * mat.Rows];
                    mat.GetArray(0, 0, array);
                    return array;
                }),
                TypeConverter.Create<RotatedFloatRect, RotatedRect>(x => new RotatedRect(new Point2f((float)x.Center.X, (float)x.Center.Y), new Size2f((float)x.Size.X, (float)x.Size.Y), (float)x.AngleDegrees)),
                TypeConverter.Create<RotatedRect, RotatedFloatRect>(x => new RotatedFloatRect(x.Center.X, x.Center.Y, x.Size.Width, x.Size.Height, RotatedFloatRect.DegreeToRadian(x.Angle))),
                TypeConverter.Create<KeyPoint, Point2f>(keyPoint => keyPoint.Pt),

                TypeConverter.Create<Point, Point2f>(p => new Point2f(p.X, p.Y)),
                TypeConverter.Create<Point2f, Point>(p => new Point(p.X, p.Y)),
                TypeConverter.Create<Point, Point2d>(p => new Point2d(p.X, p.Y)),
                TypeConverter.Create<Point2d, Point>(p => new Point(p.X, p.Y)),
                TypeConverter.Create<Point2d, Point2f>(p => new Point2f((float)p.X, (float)p.Y)),
                TypeConverter.Create<Point2f, Point2d>(p => new Point2d(p.X, p.Y)),

                TypeConverter.Create<Point3i, Point3f>(p => new Point3f(p.X, p.Y, p.Z)),
                TypeConverter.Create<Point3f, Point3i>(p => new Point3i((int)p.X, (int)p.Y, (int)p.Z)),
                TypeConverter.Create<Point3d, Point3i>(p => new Point3i((int)p.X, (int)p.Y, (int)p.Z)),
                TypeConverter.Create<Point3i, Point3d>(p => new Point3d(p.X, p.Y, p.Z)),
                TypeConverter.Create<Point3f, Point3d>(p => new Point3d(p.X, p.Y, p.Z)),
                TypeConverter.Create<Point3d, Point3f>(p => new Point3f((float)p.X, (float)p.Y, (float)p.Z)),

                TypeConverter.Create<Mat, M>(mat => M.FromArray(this.MatToA(mat))),
                TypeConverter.Create<M, Mat>(m => this.AToMat(m.UnderlyingArray)),
                //TypeConverter.Create<float[], Mat>(f => new Mat(f.Length, 1, MatType.CV_32FC1, f))
            };
        }

        public IEnumerable<IDynamicTypeConverter> GetDynamicConverters()
        {
            return new[]
            {
                new DynamicTypeConverter(typeof(Mat), typeof(M<>), (t1, t2) => new TypeConverter(t1, t2, source => M.FromArray(this.MatToA((Mat)source, t2.GetGenericArguments()[0])))),
                new DynamicTypeConverter(typeof(M<>), typeof(Mat), (t1, t2) => new TypeConverter(t1, t2, source => this.AToMat(((M)source).UnderlyingArray))),
                new DynamicTypeConverter(typeof(Mat), typeof(A<>), (t1, t2) => new TypeConverter(t1, t2, source => this.MatToA((Mat)source, t2.GetGenericArguments()[0]))),
                new DynamicTypeConverter(typeof(A<>), typeof(Mat), (t1, t2) => new TypeConverter(t1, t2, source => this.AToMat((A)source))),
                new DynamicTypeConverter(typeof(Mat), typeof(I<>), (t1, t2) => new TypeConverter(t1, t2, source => this.MatToImageBuffer((Mat)source))),
                new DynamicTypeConverter(typeof(I<>), typeof(Mat), (t1, t2) => new TypeConverter(t1, t2, source => this.ImageBufferToMat((IImageBuffer)source))),
            };
        }

        public Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> GetSerializers()
        {
            return customSerializedTypes;
        }

        static readonly Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>> customSerializedTypes = new Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>>>
        {
            {
                typeof(Size), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    size =>
                    {
                        return string.Concat(((Size)size).Width, ", ", ((Size)size).Height);
                    },
                    str =>
                    {
                        var token = str.ToString().Split(',');
                        if (token.Count() != 2)
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Size).Name));

                        return new Size(Int32.Parse(token[0]), Int32.Parse(token[1]));
                    }
                )
            },
            {
                typeof(Size?), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    size =>
                    {
                        var s = (Size?)size;
                        if (!s.HasValue)
                            return string.Empty;

                        return string.Concat(s.Value.Width, ", ", s.Value.Height);
                    },
                    str =>
                    {
                        var s = str.ToString();
                        if (string.IsNullOrEmpty(s))
                            return null;

                        var token = s.Split(',');
                        if (token.Count() != 2)
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Size).Name));

                        return new Size?(new Size(Int32.Parse(token[0]), Int32.Parse(token[1])));
                    }
                )
            },
            {
                typeof(Point), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    point =>
                    {
                        return string.Concat(((Point)point).X, ", ", ((Point)point).Y);
                    },
                    str =>
                    {
                        var token = str.ToString().Split(',');
                        if (token.Count() != 2)
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Point).Name));

                        return new Point(Int32.Parse(token[0]), Int32.Parse(token[1]));
                    }
                )
            },
            {
                typeof(Point?), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    point =>
                    {
                        var p = (Point?)point;
                        if (p.HasValue)
                            return string.Concat(p.Value.X, ", ", p.Value.Y);
                        else
                            return string.Empty;
                    },
                    str =>
                    {
                        var s = str.ToString();
                        if (string.IsNullOrEmpty(s))
                            return null;

                        var token = s.Split(',');
                        if (token.Count() != 2)
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Point).Name));

                        return new Point?(new Point(Int32.Parse(token[0]), Int32.Parse(token[1])));
                    }
                )
            },
            {
                typeof(Rect), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    rect =>
                    {
                        Rect r = (Rect)rect;
                        return string.Concat(r.X, ", ", r.Y, ", ", r.Width, ", ", r.Height);
                    },
                    str =>
                    {
                        var token = str.ToString().Split(',');
                        if (token.Count() != 4)
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Rect).Name));

                        return new Rect(Int32.Parse(token[0]), Int32.Parse(token[1]), Int32.Parse(token[2]), Int32.Parse(token[3]));
                    }
                )
            },
            {
                typeof(Rect?), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    rect =>
                    {
                        var r = (Rect?)rect;
                        if (!r.HasValue)
                            return string.Empty;

                        var v = r.Value;
                        return string.Concat(v.X, ", ", v.Y, ", ", v.Width, ", ", v.Height);
                    },
                    str =>
                    {
                        var s = str.ToString();
                        if (string.IsNullOrEmpty(s))
                            return null;

                        var token = s.Split(',');
                        if (token.Count() != 4)
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Rect).Name));

                        return new Rect?(new Rect(Int32.Parse(token[0]), Int32.Parse(token[1]), Int32.Parse(token[2]), Int32.Parse(token[3])));
                    }
                )
            },
            {
                typeof(Scalar), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    scalar =>
                    {
                        Scalar s = (Scalar)scalar;

                        return string.Concat(s.Val0, ", ", s.Val1, ", ", s.Val2, ", ", s.Val3);
                    },
                    str =>
                    {
                        var token = str.ToString().Split(',');

                        if (token.Count() == 1)
                            return new Scalar(Double.Parse(token[0]));
                        else if (token.Count() == 2)
                            return new Scalar(Double.Parse(token[0]), Double.Parse(token[1]));
                        else if (token.Count() == 3)
                            return new Scalar(Double.Parse(token[0]), Double.Parse(token[1]), Double.Parse(token[2]));
                        else if(token.Count() == 4)
                            return new Scalar(Double.Parse(token[0]), Double.Parse(token[1]), Double.Parse(token[2]), Double.Parse(token[3]));
                        else
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Scalar).Name));
                    }
                )
            },
            {
                typeof(Scalar?), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(
                    scalar =>
                    {
                        Scalar? s = (Scalar?)scalar;
                        if (!s.HasValue)
                            return string.Empty;

                        var v = s.Value;
                        return string.Concat(v.Val0, ", ", v.Val1, ", ", v.Val2, ", ", v.Val3);
                    },
                    str =>
                    {
                        var s = str.ToString();
                        if (string.IsNullOrEmpty(s))
                            return null;

                        var token = s.Split(',');

                        if (token.Count() == 0)
                            return null;
                        if (token.Count() == 1)
                            return new Scalar?(new Scalar(Double.Parse(token[0])));
                        else if (token.Count() == 2)
                            return new Scalar?(new Scalar(Double.Parse(token[0]), Double.Parse(token[1])));
                        else if (token.Count() == 3)
                            return new Scalar?(new Scalar(Double.Parse(token[0]), Double.Parse(token[1]), Double.Parse(token[2])));
                        else if(token.Count() == 4)
                            return new Scalar?(new Scalar(Double.Parse(token[0]), Double.Parse(token[1]), Double.Parse(token[2]), Double.Parse(token[3])));
                        else
                            throw new Exception(string.Format("Could not deserialize: '{0}' to type: '{1}'", str, typeof(Scalar).Name));
                    }
                )
            },
        };
    }
}
