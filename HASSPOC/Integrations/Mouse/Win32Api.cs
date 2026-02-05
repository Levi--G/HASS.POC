using System.Runtime.InteropServices;

namespace HASSPOC.Integrations.Mouse;

internal partial class Win32Api
{
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out LPPOINT lppoint);

    /// <summary>
    /// Struct representing a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LPPOINT
    {
        public int X;
        public int Y;
    }
}
