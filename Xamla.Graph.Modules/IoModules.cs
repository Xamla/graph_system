using System;
using System.Collections.Generic;
using System.Text;
using Xamla.Graph.MethodModule;
using Xamla.Utilities;

namespace Xamla.Graph.Modules
{
    public static class IoModules
    {
        [StaticModule(ModuleType = "Xamla.IO.CopyDirectoryRecursively", Flow = true)]
        public static void CopyDirectoryRecursively(
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string source,
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string target
        )
        {
            FileSystemUtilities.CopyDirectoryRecursively(source, target);
        }
    }
}
