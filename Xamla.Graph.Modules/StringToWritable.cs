using System.IO;
using System.Text;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.IO.StringToWritable")]
    public class StringToWritable
        : SingleInstanceMethodModule
    {
        public StringToWritable(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        public IWritable CreateWritable(
            [InputPin(PropertyMode = PropertyMode.Allow)] string data,
            [InputPin(PropertyMode = PropertyMode.Default)] Encoding encoding = null
        )
        {
            return Writable.Create(async (destination, cancel) =>
            {
                using (var sw = new StreamWriter(destination, encoding ?? Encoding.UTF8, 512, true))
                {
                    await sw.WriteAsync(data).ConfigureAwait(false);
                }
            });
        }
    }
}
