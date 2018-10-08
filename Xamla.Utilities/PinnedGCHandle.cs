using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
//using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    /// <summary>
    /// A wrapper for <c>GCHandle.Alloc(obj, GCHandleType.Pinned)</c> and <see cref="GCHandle.Free"/> that supports <see cref="IDisposable"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct PinnedGCHandle : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PinnedGCHandle"/> class. <see cref="Dispose"/> must be called to unpin the object when it is no longer required.
        /// </summary>
        /// <param name="obj">The object to pin.</param>
        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public PinnedGCHandle(object obj)
        {
            handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        /// <summary>
        /// Unpins the object that was pinned.
        /// </summary>
        public void Dispose()
        {
            if (handle.IsAllocated)
                handle.Free();
        }

        /// <summary>
        /// Gets an <see cref="IntPtr"/> to the pinned object.
        /// </summary>
        /// <value>The pointer to the memory of the object that was pinned when this object was constructed.</value>
        public IntPtr Pointer
        {
            //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return handle.AddrOfPinnedObject();
            }
        }

        /// <summary>
        /// Implicitly converts this <see cref="PinnedGCHandle"/> object to an <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="handle">The <see cref="PinnedGCHandle"/> to convert.</param>
        /// <returns>An <see cref="IntPtr"/> to the memory of the object pinned by the <see cref="PinnedGCHandle"/>.</returns>
        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static implicit operator IntPtr(PinnedGCHandle handle)
        {
            return handle.Pointer;
        }

        readonly GCHandle handle;
    }
}
