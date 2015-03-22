using System;
using System.Runtime.InteropServices;

namespace FastCapt.Interop
{
    /// <summary>
    /// SetWindowPos Flags
    /// </summary>
    public enum SWP
    {
        NOSIZE = 0x0001,
        NOMOVE = 0x0002,
        NOZORDER = 0x0004,
        NOREDRAW = 0x0008,
        NOACTIVATE = 0x0010,
        DRAWFRAME = 0x0020,
        FRAMECHANGED = 0x0020,
        SHOWWINDOW = 0x0040,
        HIDEWINDOW = 0x0080,
        NOCOPYBITS = 0x0100,
        NOOWNERZORDER = 0x0200,
        NOREPOSITION = 0x0200,
        NOSENDCHANGING = 0x0400,
        DEFERERASE = 0x2000,
        ASYNCWINDOWPOS = 0x4000
    }
    internal class Win32
    {
        private const string User32 = "user32.dll";

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);

        [DllImport(User32, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);


        [DllImport(User32, EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        /// <summary>
        /// Changes an attribute of the specified window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport(User32, EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);
    }
}
