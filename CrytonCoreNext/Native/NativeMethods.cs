using System;
using System.Runtime.InteropServices;

namespace CrytonCoreNext.Native
{
    public static partial class NativeMethods
    {
        [LibraryImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static partial void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

    }
}
