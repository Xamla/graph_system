using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Retrieves a <c>FileInfo</c> object for the given filename.
        /// </summary>
        /// <param name="filename">Path of the directory to query information about.</param>
        /// <returns>
        /// <return name="fileInfo">Created <c>FileInfo</c> object.</return>
        /// </returns>
        [StaticModule(ModuleType = "System.IO.FileInfo", Flow = true)]
        public static FileInfo FileInfo(
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string filename
        )
        {
            return new FileInfo(filename);
        }

        /// <summary>
        /// Retrieves a <c>DirectoryInfo</c> object for the given path.
        /// </summary>
        /// <param name="path">Path of the directory to query information about.</param>
        /// <returns>
        /// <return name="directoryInfo">Created <c>DirectoryInfo</c> object.</return>
        /// </returns>
        [StaticModule(ModuleType = "System.IO.DirectoryInfo", Flow = true)]
        public static DirectoryInfo DirectoryInfo(
            [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = true)] string path
        )
        {
            return new DirectoryInfo(path);
        }
    }
}
