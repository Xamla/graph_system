using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamla.Types;
using Xamla.Types.Converters;

namespace Xamla.Graph.Modules.Image
{
    public class ImageBufferConverter
    {
        public IEnumerable<ITypeConverter> GetConverters()
        {
            return new[]
            {
                TypeConverter.Create<I<byte>, I<float>>(x => x.ToF32()),
                TypeConverter.Create<I<float>, I<byte>>(x => x.ToU8())
            };
        }
    }
}
