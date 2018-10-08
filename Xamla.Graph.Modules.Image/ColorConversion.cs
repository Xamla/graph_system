using System;
using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules.ImageBuffer
{
    public enum ColorSpaceConversionType
    {
        BrgToRgb,
        RgbToHsv,
        HsvToRgb,
        RgbToYuv,
        YuvToRgb,
        RgbToXyz,
        XyzToRgb,
        XyzToLab,
        LabToXyz,
        XyzToLuv,
        LuvToXyz
    }

    [Module(ModuleType = "Xamla.Image.ColorSpaceConversion")]
    public class ColorSpaceConversion
        : SingleInstanceMethodModule
    {
        public ColorSpaceConversion(IGraphRuntime runtime)
            : base(runtime, new ImageBufferPreviewGenerator(runtime))
        {
        }

        static Func<I<float>, I<float>> GetConverter(ColorSpaceConversionType conversion)
        {
            switch (conversion)
            {
                case ColorSpaceConversionType.BrgToRgb:
                    return I.BgrToRgb;
                case ColorSpaceConversionType.RgbToHsv:
                    return I.RgbToHsv;
                case ColorSpaceConversionType.HsvToRgb:
                    return I.HsvToRgb;
                case ColorSpaceConversionType.RgbToYuv:
                    return I.RgbToYuv;
                case ColorSpaceConversionType.YuvToRgb:
                    return I.YuvToRgb;
                case ColorSpaceConversionType.RgbToXyz:
                    return I.RgbToXyz;
                case ColorSpaceConversionType.XyzToRgb:
                    return I.XyzToRgb;
                case ColorSpaceConversionType.XyzToLab:
                    return I.XyzToLab;
                case ColorSpaceConversionType.LabToXyz:
                    return I.LabToXyz;
                case ColorSpaceConversionType.XyzToLuv:
                    return I.XyzToLuv;
                case ColorSpaceConversionType.LuvToXyz:
                    return I.LuvToXyz;
            }

            throw new ArgumentException("Unsupported conversion specified", "conversion");
        }

        [ModuleMethod]
        public I<float> ConvertColor(
            IImageBuffer image,
            [InputPin(PropertyMode = PropertyMode.Default)] ColorSpaceConversionType conversion
        )
        {
            var converter = GetConverter(conversion);
            return converter(image.ToF32());
        }
    }
}
