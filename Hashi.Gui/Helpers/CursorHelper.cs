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

    public static Rect GetAbsolutePlacement(this FrameworkElement element, bool relativeToScreen = false)
    {
        var absolutePos = element.PointToScreen(new Point(0, 0));
        if (relativeToScreen) return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
        var posMW = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
        absolutePos = new Point(absolutePos.X - posMW.X, absolutePos.Y - posMW.Y);
        return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Win32Point
    {
        public Int32 X;
        public Int32 Y;
    }
}