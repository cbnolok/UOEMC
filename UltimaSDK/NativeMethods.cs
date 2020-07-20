using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Ultima
{
    public static class NativeMethods
    {
        [DllImport("Kernel32")]
		public unsafe static extern int _lread( SafeFileHandle hFile, void* lpBuffer, int wBytes );
    }
}
