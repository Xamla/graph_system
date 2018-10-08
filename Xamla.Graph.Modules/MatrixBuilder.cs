using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Types;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules
{
    public enum DataOrientation
    {
        Row,
        Column
    }

    [Module(ModuleType = "Xamla.ML.MatrixBuilder")]
    public class MatrixBuilder
        : SingleInstanceMethodModule
    {
        public MatrixBuilder(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        public async Task<M> Build(
            [InputPin(PropertyMode = PropertyMode.Allow)] ISequence<V> vectors,
            [InputPin(PropertyMode = PropertyMode.Default)] DataOrientation orientation = DataOrientation.Row)
        {
            var vectorlist = await vectors.ToListAsync();
            int max = 0;
            Type type = vectorlist.First().UnderlyingArray.ElementType;

            foreach (var vector in vectorlist)
            {
                if (vector.UnderlyingArray.Count > max)
                    max = vector.UnderlyingArray.Count;
                if (type != vector.UnderlyingArray.ElementType)
                    throw new Exception("Vector in sequence are of different types");
            }

            A a = null;

            if (orientation == DataOrientation.Column)
            {
                a = A.Create(type, max, vectorlist.Count);

                if (vectorlist.Count == 1)
                {
                    Buffer.BlockCopy(vectorlist[0].UnderlyingArray.Buffer, 0, a.Buffer, 0, vectorlist[0].UnderlyingArray.SizeInBytes);
                }
                else
                {
                    for (var c = 0; c < a.Dimension[0]; ++c)
                    {
                        for (var r = 0; r < a.Dimension[1]; ++r)
                        {
                            if (c < vectorlist[r].UnderlyingArray.Count)
                                a.SetValue(vectorlist[r].UnderlyingArray.GetValue(c), c, r);
                        }
                    }
                }
            }
            else if (orientation == DataOrientation.Row)
            {
                a = A.Create(type, vectorlist.Count, max);
                int i = 0;

                foreach (var v in vectorlist)
                {
                    Buffer.BlockCopy(v.UnderlyingArray.Buffer, 0, a.Buffer, i * Marshal.SizeOf(type) * a.Stride[0], v.UnderlyingArray.SizeInBytes);
                    ++i;
                }
            }

            return M.FromArray(a);
        }
    }
}
