using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Hashi.Gui.Helpers;

public static class CursorHelper
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(ref Win32Point pt);

    public static Point GetCurrentCursorPosition(Visual relativeTo)
    {
        var w32Mouse = new Win32Point();
        GetCursorPos(ref w32Mouse);
        return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Win32Point
    {
        public Int32 X;
        public Int32 Y;
    }
}