using System;
using System.Runtime.InteropServices;

namespace FastCapt.Recorders.Interop
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Retrieves a handle to the desktop window. The desktop window covers the entire screen.
        /// The desktop window is the area on top of which other windows are painted.
        /// </summary>
        /// <returns>The return value is a handle to the desktop window.</returns>
        [DllImport(ExternalDlls.USER32, SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// The GetDC function retrieves a handle to a device context (DC) for the client
        /// area of a specified window or for the entire screen.
        /// You can use the returned handle in subsequent GDI functions to draw in the DC.
        /// The device context is an opaque data structure, whose values are used internally by GDI.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window whose DC is to be retrieved.
        /// If this value is NULL, GetDC retrieves the DC for the entire screen.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC for the specified window's client area.
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport(ExternalDlls.USER32)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications.
        /// The effect of the ReleaseDC function depends on the type of DC.
        /// It frees only common and window DCs. It has no effect on class or private DCs.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
        /// <param name="hDc">A handle to the DC to be released.</param>
        /// <returns>true if successful otherwise false.</returns>
        [DllImport(ExternalDlls.USER32)]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);


        /// <summary>
        /// Initializes a <see cref="PROPVARIANT"/> structure using the contents of a buffer.
        /// </summary>
        /// <param name="buffer">Pointer to the buffer.</param>
        /// <param name="celems">The length of the buffer, in bytes.</param>
        /// <param name="ppropvar">When this function returns, contains the initialized <see cref="PROPVARIANT"/> structure.</param>
        [DllImport(ExternalDlls.PROPSYS, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int InitPropVariantFromBuffer(IntPtr buffer, uint celems, out PROPVARIANT ppropvar);

        /// <summary>
        /// Retrieves information about the specified icon or cursor.
        /// </summary>
        /// <param name="hIcon">A handle to the icon or cursor.</param>
        /// <param name="piconinfo"></param>
        /// <returns></returns>
        [DllImport(ExternalDlls.USER32)]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pci"></param>
        /// <returns></returns>
        [DllImport(ExternalDlls.USER32)]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport(ExternalDlls.USER32, SetLastError = true)]
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
        [DllImport(ExternalDlls.GDI32, EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(
            [In] IntPtr hdc,
            int nWidth,
            int nHeight);

        /// <summary>
        /// The CreateCompatibleDC function creates a memory device context (DC)
        /// compatible with the specified device.
        /// </summary>
        /// <param name="hdc">
        /// A handle to an existing DC. If this handle is NULL, the function creates a memory DC
        /// compatible with the application's current screen.</param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to a memory DC.
        /// If the function fails, the return value is <see cref="IntPtr.Zero"/>.
        /// </returns>
        [DllImport(ExternalDlls.GDI32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(ExternalDlls.GDI32, ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport(ExternalDlls.GDI32, ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(ExternalDlls.GDI32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

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
        [DllImport(ExternalDlls.GDI32, EntryPoint = "BitBlt", SetLastError = true)]
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


        /// <summary>
        /// The PropVariantClear function frees all elements that can be freed in a given PROPVARIANT structure. 
        /// For complex elements with known element pointers, the underlying elements are freed prior to freeing
        /// the containing element.
        /// </summary>
        /// <param name="pvar">
        /// A pointer to an initialized <see cref="PROPVARIANT"/> structure for which any deallocatable elements
        /// are to be freed. On return, all zeroes are written to the PROPVARIANT structure.
        /// </param>
        /// <returns></returns>
        [DllImport(ExternalDlls.OLE32)]
        internal static extern int PropVariantClear(ref PROPVARIANT pvar);
    }
}
