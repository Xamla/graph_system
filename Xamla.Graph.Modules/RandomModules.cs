using System;
using System.Collections.Generic;
using Xamla.Graph.MethodModule;
using Xamla.Utilities;

namespace Xamla.Graph.Modules
{
    public static class RandomModules
    {
        [StaticModule(ModuleType = "Xamla.Random.RandomFloat64")]
        public static double RandomFloat64() =>
            ThreadSafeRandom.Generator.NextDouble();

        [StaticModule(ModuleType = "Xamla.Random.Rng")]
        public static Random Rng(int? seed = null) =>
            seed.HasValue ? new Random(seed.Value) : ThreadSafeRandom.Generator;

        [StaticModule(ModuleType = "Xamla.Random.RandomBytes")]
        public static byte[] RandomBytes(
            [InputPin(PropertyMode = PropertyMode.Default)] int count = 64,
            [InputPin(PropertyMode = PropertyMode.Default)] int? seed = null
        )
        {
            var rng = seed.HasValue ? new Random(seed.Value) : ThreadSafeRandom.Generator;
            var buffer = new byte[count];
            rng.NextBytes(buffer);
            return buffer;
        }

        [StaticModule(ModuleType = "Xamla.Random.RandomString")]
        public static string RandomString(
            [InputPin(PropertyMode = PropertyMode.Default)] int size = 32,
            [InputPin(PropertyMode = PropertyMode.Default)] string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            [InputPin(PropertyMode = PropertyMode.Default)] int? seed = null
        )
        {
            var rng = seed.HasValue ? new Random(seed.Value) : ThreadSafeRandom.Generator;
            char[] buffer = new char[size];
            for (int i = 0; i < size; i++)
                buffer[i] = charset[rng.Next(charset.Length)];

            return new string(buffer);
        }
    }
}
