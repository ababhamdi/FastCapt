using System;
using System.Runtime.InteropServices;

namespace FastCapt.Recorders.Interop
{
    internal static class NativeMethods
    {

        [DllImport(ExternalDlls.USER32, SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(ExternalDlls.USER32)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(ExternalDlls.USER32)]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);


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
    }
}
