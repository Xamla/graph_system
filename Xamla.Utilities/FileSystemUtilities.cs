using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xamla.Utilities
{
    public static class FileSystemUtilities
    {
        public static void CopyDirectoryRecursively(string source, string target)
        {
            CopyDirectoryRecursively(new DirectoryInfo(source), new DirectoryInfo(target));
        }

        public static void CopyDirectoryRecursively(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!Directory.Exists(destination.FullName))
                Directory.CreateDirectory(destination.FullName);

            // Copy each file into the new directory.
            foreach (var fileInfo in source.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(destination.FullName, fileInfo.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var childSource in source.GetDirectories())
            {
                var childTarget = destination.CreateSubdirectory(childSource.Name);
                CopyDirectoryRecursively(childSource, childTarget);
            }
        }
    }
}
