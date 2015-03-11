using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace FastCapt.Interop
{
    internal class Win32
    {
        private const string Gdi32 = "gdi32.dll";
        private const string User32 = "user32.dll";


        [DllImport(Gdi32)]
        public static extern int GetObject(IntPtr hgdiobj, int cbBuffer, HandleRef lpvObject);

        [DllImport(User32)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport(User32)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(User32, SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(User32, SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport(Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport(User32)]
        public static extern IntPtr CreateBitmap(
            int nWidth,
            int nHeight,
            uint cPlanes,
            uint cBitsPerPel,
            IntPtr lpvBits);

        /// <summary>
        /// Creates a bitmap compatible with the device that is
        /// associated with the specified device context.
        /// </summary>
        /// <param name="hdc">A handle to a device context.</param>
        /// <param name="nWidth">The bitmap width, in pixels.</param>
        /// <param name="nHeight">The bitmap height, in pixels.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the
        /// compatible bitmap (DDB).
        /// If the function fails, the return value is <see cref="System.IntPtr.Zero"/>.
        /// </returns>
        [DllImport(Gdi32, EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(
            [In] IntPtr hdc,
            int nWidth,
            int nHeight);

        /// <summary>
        ///    Performs a bit-block transfer of the color data corresponding to a
        ///    rectangle of pixels from the specified source device context into
        ///    a destination device context.
        /// </summary>
        /// <param name="hdc">
        /// Handle to the destination device context.</param>
        /// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
        /// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
        /// <param name="hdcSrc">Handle to the source device context.</param>
        /// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
        /// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
        /// <param name="dwRop">A raster-operation code.</param>
        /// <returns>
        ///    <c>true</c> if the operation succeedes, <c>false</c> otherwise. To get extended error information, call <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/>.
        /// </returns>
        [DllImport(Gdi32, EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(
            [In] IntPtr hdc,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            [In] IntPtr hdcSrc,
            int nXSrc,
            int nYSrc,
            TernaryRasterOperations dwRop);

        [DllImport(Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport(Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pci"></param>
        /// <returns></returns>
        [DllImport(User32)]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        /// <summary>
        /// Retrieves information about the specified icon or cursor.
        /// </summary>
        /// <param name="hIcon">A handle to the icon or cursor.</param>
        /// <param name="pIconInfo"></param>
        /// <returns></returns>
        [DllImport(User32)]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO pIconInfo);

        [DllImport(User32, SetLastError = true)]
        public static extern bool DrawIconEx(
            IntPtr hdc,
            int xLeft,
            int yTop,
            IntPtr hIcon,
            int cxWidth,
            int cyHeight,
            int istepIfAniCur,
            IntPtr hbrFlickerFreeDraw,
            DrawingFlags diFlags);

        [DllImport(User32)]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(User32, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Enables or disables mouse and keyboard input to the specified window or control.
        /// When input is disabled, the window does not receive input such as mouse clicks
        /// and key presses. When input is enabled, the window receives all input.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be enabled or disabled. </param>
        /// <param name="bEnable">Indicates whether to enable or disable the window. 
        /// If this parameter is <c>true</c>, the window is enabled.
        /// If the parameter is <c>false</c>, the window is disabled. </param>
        /// <returns></returns>
        [DllImport(User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

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

        [DllImport(User32, SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpwndpl"></param>
        /// <returns></returns>
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpwndpl"></param>
        /// <returns></returns>
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);


        internal static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
        {
            var windowPlacement = WINDOWPLACEMENT.Default;
            GetWindowPlacement(hwnd, ref windowPlacement);
            return windowPlacement;
        }

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        public static void RegisterHotKey(ModifierKeys modifierKeys, Key keys, int identifier)
        {
            var vKey = (uint)KeyInterop.VirtualKeyFromKey(keys);
            var success = RegisterHotKey(IntPtr.Zero, identifier, (uint)modifierKeys, vKey);
            if (success)
            {
                return;
            }


            var lastError = Marshal.GetLastWin32Error();
        }

        [DllImport(User32)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void UnregisterHotKey(int id)
        {
            var success = UnregisterHotKey(IntPtr.Zero, id);
            if (success)
            {
                return;
            }

            var lastError = Marshal.GetLastWin32Error();
        }
    }
}
