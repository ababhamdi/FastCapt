using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using FastCapt.Recorders.Interfaces;
using FastCapt.Recorders.Interop;

namespace FastCapt.Recorders
{
    /// <summary>
    /// A Helper class that will snap a picture of a screen region.
    /// </summary>
    [Export(typeof(ISnapshotManager))]
    internal class SnapshotManager : ISnapshotManager
    {
        #region "Fields"

        private readonly int _height;
        private readonly int _left;
        private readonly int _top;
        private readonly int _width;
        private IntPtr _snapshotDc;
        private IntPtr _desktopDc; 

        #endregion

        #region "Constructors"

        public SnapshotManager(int left, int top, int width, int height)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        } 

        #endregion

        #region "Properties"

        public int Height { get { return _height; } }
        public int Width { get { return _width; } } 

        #endregion

        #region "Methods"

        public void Initialize(IntPtr desktopDc)
        {
            _desktopDc = desktopDc;
        }

        public Bitmap TakeSnapshot()
        {
            // create the in-memory device context.
            _snapshotDc = NativeMethods.CreateCompatibleDC(_desktopDc);

            // initialize the buffer
            var snapshotPtr = NativeMethods.CreateCompatibleBitmap(
                _desktopDc,
                Width,
                Height);

            // associate the bitmap buffer with the in-memory device context.
            NativeMethods.SelectObject(_snapshotDc, snapshotPtr);

            var success = NativeMethods.BitBlt(
                _snapshotDc,
                0,
                0,
                Width,
                Height,
                _desktopDc,
                _left,
                _top,
                TernaryRasterOperations.SRCCOPY);

            if (success)
            {
                RenderMouseCursor();
                NativeMethods.DeleteDC(_snapshotDc);
            }

            var image = Image.FromHbitmap(snapshotPtr);
            return ConvertToRGBA(image);
        }

        private Bitmap ConvertToRGBA(Bitmap image)
        {
            var height = image.Height;
            var width = image.Width;
            var clone = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(clone))
            {
                g.DrawImage(image, new Rectangle(0, 0, width, height));
            }

            return clone;
        }

        /// <summary>
        /// This method will render the cursor in the snapeshot if it should be displayed.
        /// </summary>
        private void RenderMouseCursor()
        {
            CURSORINFO cursorInfo;
            cursorInfo.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            bool success = NativeMethods.GetCursorInfo(out cursorInfo);
            if (!success
                || cursorInfo.flags != CursorState.Showing
                || IsCursorOutsideRect(cursorInfo.ptScreenPos))
            {
                return;
            }
            // if the cursor is outside the region we're capturing.



            if (cursorInfo.hCursor == IntPtr.Zero)
            {
                // should log an exception here.
                return;
            }

            using (var iconInfo = new IconInfo(cursorInfo.hCursor))
            {
                int xLeft = Math.Abs(_left - cursorInfo.ptScreenPos.X) - iconInfo.HotPoint.X;
                var yTop = Math.Abs(_top - cursorInfo.ptScreenPos.Y) - iconInfo.HotPoint.Y;
                NativeMethods.DrawIconEx(_snapshotDc,
                    xLeft,
                    yTop,
                    cursorInfo.hCursor,
                    0,
                    0,
                    0,
                    IntPtr.Zero,
                    DrawingFlags.NORMAL);
            }
        }

        private bool IsCursorOutsideRect(POINT location)
        {
            return false;
        }

        #endregion
    }
}
