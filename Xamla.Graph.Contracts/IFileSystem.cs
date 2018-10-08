using System.IO;

namespace Xamla.Graph
{
    public interface IFileSystem
    {
        Stream Open(string path, FileMode mode, FileAccess access, FileShare share = FileShare.None);
        bool Exists(string path);
    }
}
