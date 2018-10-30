using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xamla.Utilities
{
    public class PosixPath
        : IEquatable<PosixPath>
    {
        public static readonly PosixPath Root = new PosixPath("/");

        readonly IEqualityComparer<string> comparer;
        readonly List<string> parts;
        readonly bool rooted;
        readonly bool empty;

        public List<string> Parts => parts;

        public static PosixPath Combine(params string[] parts)
        {
            if (parts == null || parts.Length == 0)
                return new PosixPath(string.Empty);

            var path = new PosixPath(parts[0]);
            for (int i = 1; i < parts.Length; ++i)
                path = path.Append(parts[i]);
            return path;
        }

        public static PosixPath Combine(params PosixPath[] parts)
        {
            if (parts == null || parts.Length == 0)
                return new PosixPath(string.Empty);

            var path = parts[0];
            for (int i = 1; i < parts.Length; ++i)
                path = path.Append(parts[i]);
            return path;
        }

        public static PosixPath Combine(PosixPath path1, string path2)
        {
            return PosixPath.Combine(path1, (PosixPath)path2);
        }

        public static string GetFileName(string path)
        {
            return new PosixPath(path).FileName;
        }

        public static string GetExtension(string path)
        {
            return new PosixPath(path).Extension;
        }

        public static PosixPath GetCurrentDirectory()
        {
            return (PosixPath)System.IO.Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Create a file path from a path string.
        /// </summary>
        public PosixPath(string input)
            : this(input, StringComparer.OrdinalIgnoreCase)
        {
        }

        public PosixPath(string input, IEqualityComparer<string> comparer)
        {
            this.comparer = comparer;
            this.parts = input.Split(new[] { '/', '\\', ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            this.rooted = HasRoot(input);
            this.empty = string.IsNullOrWhiteSpace(input);
        }

        protected PosixPath(IEnumerable<string> orderedElements, bool rooted, IEqualityComparer<string> comparer)
        {
            this.comparer = comparer;
            this.parts = orderedElements.ToList();
            this.rooted = rooted;
            this.empty = parts.Count == 0;
        }

        public bool IsRooted
        {
            get { return rooted; }
        }

        public PosixPath Append(string right)
        {
            return this.Append(new PosixPath(right));
        }

        /// <summary>
        /// Append 'right' to this path, ignoring navigation semantics.
        /// </summary>
        public PosixPath Append(PosixPath right)
        {
            return empty ? right : new PosixPath(parts.Concat(right.parts), rooted, comparer);
        }

        public PosixPath RemoveLastElement()
        {
            if (parts.Count >= 1)
                return new PosixPath(parts.Take(parts.Count - 1), rooted, comparer);
            else
                throw new InvalidOperationException("Path is empty.");
        }

        /// <summary>
        /// Append 'right' to this path, obeying standard navigation semantics .
        /// </summary>
        public PosixPath Navigate(PosixPath navigation)
        {
            if (navigation.rooted)
                return navigation.Normalize();

            return new PosixPath(parts.Concat(navigation.parts), rooted, comparer).Normalize();
        }

        /// <summary>
        /// Remove a common root from this path and return a relative path.
        /// </summary>
        public PosixPath RemoveCommonParts(PosixPath root)
        {
            for (int i = 0; i < root.parts.Count; i++)
            {
                if (parts.Count <= i)
                    throw new InvalidOperationException("Root supplied is longer than full path");

                if (!comparer.Equals(parts[i], root.parts[i]))
                    throw new InvalidOperationException("Full path is not a subpath of root");
            }

            return new PosixPath(parts.Skip(root.parts.Count), false, comparer);
        }

        /// <summary>
        /// Checks if this path is a subpath of another path
        /// </summary>
        public bool IsSubpathOf(PosixPath other)
        {
            if (parts.Count <= other.parts.Count)
                return false;

            for (int i = 0; i < parts.Count && i < other.parts.Count; i++)
            {
                if (!comparer.Equals(parts[i], other.parts[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a minimal relative path from source to current path.
        /// </summary>
        public PosixPath RelativeTo(PosixPath source)
        {
            if (source.IsRooted != this.IsRooted)
                throw new Exception("Cannot compute relative path between absolute and relative path.");

            int shorter = Math.Min(parts.Count, source.parts.Count);
            int common;

            for (common = 0; common < shorter; common++)
            {
                if (!comparer.Equals(parts[common], source.parts[common]))
                    break;
            }

            if (common == 0)
            {
                if (rooted)
                    return this;
                return source.Navigate(this);
            }

            var result = new List<string>();
            if (source.parts.Count > common)
            {
                int differences = source.parts.Count - common;
                for (int i = 0; i < differences; i++)
                    result.Add("..");
            }

            result.AddRange(parts.Skip(common));

            return new PosixPath(result, false, comparer);
        }

        /// <summary>
        /// Remove single dots, remove path elements for double dots.
        /// </summary>
        public PosixPath Normalize()
        {
            var result = new List<string>();
            uint leading = 0;

            for (int index = parts.Count - 1; index >= 0; index--)
            {
                var part = parts[index];
                if (part == ".")
                    continue;

                if (part == "..")
                {
                    leading++;
                    continue;
                }

                if (leading > 0)
                {
                    leading--;
                    continue;
                }

                result.Insert(0, part);
            }

            if (rooted && leading > 0)
                throw new InvalidOperationException("Tried to navigate before path root");

            for (int i = 0; i < leading; i++)
            {
                result.Insert(0, "..");
            }

            return new PosixPath(result, rooted, comparer);
        }

        /// <summary>
        /// Returns true if the path specified was empty, false otherwise
        /// </summary>
        public bool IsEmpty
        {
            get { return empty; }
        }

        /// <summary>
        /// Returns a string representation of the path using Posix path separators
        /// </summary>
        public string ToPosixPath()
        {
            return rooted
                ? "/" + string.Join("/", parts)
                : string.Join("/", parts);
        }

        /// <summary>
        /// Returns a string representation of the path using Windows path separators
        /// </summary>
        public string ToWindowsPath()
        {
            return rooted ? RootedWindowsPath() : string.Join(@"\", Normalize().parts);
        }

        string RootedWindowsPath()
        {
            return string.Join(@"\", WindowsDriveSpecOrFolder(), string.Join(@"\", parts.Skip(1)));
        }

        public string ToEnvironmentalPathWithoutFileName()
        {
            var path = ToEnvironmentalPath();
            return path.Substring(0, path.Length - this.FileName.Length);
        }

        /// <summary>
        /// Returns a string representation of the path using path separators for the current execution environment
        /// </summary>
        public string ToEnvironmentalPath()
        {
            return PosixOS() ? ToPosixPath() : ToWindowsPath();
        }

        static bool PosixOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        string WindowsDriveSpecOrFolder()
        {
            if (parts.Count < 1)
                return string.Empty;

            if (parts[0].Length == 1)
                return parts[0] + ":";
            return @"\" + parts[0];
        }

        public static bool HasRoot(string input)
        {
            return (input.Length >= 1 && (input[0] == '/' || input[0] == '\\'))
                || (input.Length >= 2 && input[1] == ':');
        }

        public string Extension
        {
            get
            {
                var fileName = this.LastElement;
                if (fileName == null)
                    return null;

                int dot = fileName.LastIndexOf('.');
                if (dot < 0)
                    return string.Empty;

                return fileName.Substring(dot);
            }
        }

        public string LastElement
        {
            get { return parts.LastOrDefault(); }
        }

        public string FileName
        {
            get { return parts.Last(); }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                var fileName = this.FileName;
                var extension = this.Extension;
                if (string.IsNullOrEmpty(extension))
                    return fileName;
                return fileName.Remove(fileName.Length - extension.Length);
            }
        }

        #region Operators and equality

        public static explicit operator PosixPath(string src)
        {
            return new PosixPath(src);
        }

        public bool Equals(PosixPath other)
        {
            return !ReferenceEquals(null, other)
                && (ReferenceEquals(this, other) || comparer.Equals(Normalize().ToPosixPath(), other.Normalize().ToPosixPath()));
        }

        public override string ToString()
        {
            return ToPosixPath();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PosixPath);
        }

        public override int GetHashCode()
        {
            return HashHelper.CombineHashCode(HashHelper.GetHashCode(parts), rooted);
        }

        public static bool operator ==(PosixPath a, PosixPath b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(PosixPath a, PosixPath b)
        {
            return !(a == b);
        }

        public static bool operator ==(PosixPath a, string b)
        {
            return a == (PosixPath)b;
        }

        public static bool operator !=(PosixPath a, string b)
        {
            return a == (PosixPath)b;
        }

        #endregion
    }
}
