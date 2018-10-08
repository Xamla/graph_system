using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;

namespace Xamla.Graph.Modules.Crypt
{
    public enum HashAlgorithmType
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }

    public abstract class ShaGenerator
        : ModuleBase
    {
        HashAlgorithmType type;

        GenericInputPin inputPin;
        GenericOutputPin hashPin;

        public ShaGenerator(IGraphRuntime runtime, HashAlgorithmType type)
            : base(runtime)
        {
            this.type = type;
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.Create<object>(), PropertyMode.Never);
            this.hashPin = AddOutputPin("Hash", PinDataTypeFactory.Create<byte[]>());
        }

        public IInputPin InputPin
        {
            get { return inputPin; }
        }

        public IOutputPin HashPin
        {
            get { return hashPin; }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var input = (object)inputs[0];
            var result = ComputeHash(input, type);

            return Task.FromResult(new object[] { result });
        }

        public static byte[] ComputeHash(object x, HashAlgorithmType type)
        {
            using (var hashAlgoritm = CreateHashAlgorithm(type))
            {
                if (x is IReadable readable)
                {
                    using (var source = readable.Open())
                    {
                        return hashAlgoritm.ComputeHash(source);
                    }
                }
                else if (x is string)
                {
                    return hashAlgoritm.ComputeHash(new MemoryStream(Encoding.UTF8.GetBytes((string)x)));
                }
                else
                {
                    throw new ArgumentException("Invalid Input type.");
                }
            }
        }

        public static HashAlgorithm CreateHashAlgorithm(HashAlgorithmType type)
        {
            return HashAlgorithm.Create(type.ToString());
        }
    }

    [Module(ModuleType = "Xamla.Crypt.MD5Generator")]
    public class MD5Generator : ShaGenerator
    {
        public MD5Generator(IGraphRuntime runtime) : base(runtime, HashAlgorithmType.MD5) { }
    }

    [Module(ModuleType = "Xamla.Crypt.SHA256Generator")]
    public class SHA256Generator : ShaGenerator
    {
        public SHA256Generator(IGraphRuntime runtime) : base(runtime, HashAlgorithmType.SHA256) { }
    }

    [Module(ModuleType = "Xamla.Crypt.SHA384Generator")]
    public class SHA384Generator : ShaGenerator
    {
        public SHA384Generator(IGraphRuntime runtime) : base(runtime, HashAlgorithmType.SHA384) { }
    }

    [Module(ModuleType = "Xamla.Crypt.SHA512Generator")]
    public class SHA512Generator : ShaGenerator
    {
        public SHA512Generator(IGraphRuntime runtime) : base(runtime, HashAlgorithmType.SHA512) { }
    }

    [Module(ModuleType = "Xamla.Crypt.SHA1Generator")]
    public class SHA1Generator : ShaGenerator
    {
        public SHA1Generator(IGraphRuntime runtime) : base(runtime, HashAlgorithmType.SHA1) { }
    }
}
