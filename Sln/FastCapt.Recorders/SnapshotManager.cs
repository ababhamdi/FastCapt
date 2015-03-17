using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using FastCapt.Recorders.Interfaces;
using FastCapt.Recorders.Interop;
using FastCapt.Recorders.Resources;

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
        private bool _isInitialized;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="SnapshotManager"/> class.
        /// </summary>
        public SnapshotManager(int left, int top, int width, int height)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        } 

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets a value that indicates the height of the snapshot.
        /// </summary>
        public int Height { get { return _height; } }

        /// <summary>
        /// Gets a value that indicates the width of the snapshot.
        /// </summary>
        public int Width { get { return _width; } } 

        #endregion

        #region "Methods"

        public void Initialize(IntPtr desktopDc)
        {
            if (desktopDc == IntPtr.Zero)
            {
                throw new ArgumentException(Exceptions.DeviceContext_NotInit);
            }

            _desktopDc = desktopDc;
            _isInitialized = true;
        }

        private void EnsureInitialization()
        {
            if (_isInitialized)
            {
                return;
            }

            throw new Exception(Exceptions.SnapshotManager_InitializeFail);
        }

        private void EnsureHandle(IntPtr handle, string errorMessage)
        {
            if (handle == IntPtr.Zero)
            {
                throw new Exception(errorMessage);
            }
        }

        public Bitmap TakeSnapshot()
        {
            EnsureInitialization();

            // create the in-memory device context.
            _snapshotDc = NativeMethods.CreateCompatibleDC(_desktopDc);
            EnsureHandle(_snapshotDc, Exceptions.CompatibleDC_Create_Fail);

            // initialize the buffer
            var snapshotPtr = NativeMethods.CreateCompatibleBitmap(
                _desktopDc,
                Width,
                Height);
            EnsureHandle(snapshotPtr, Exceptions.CompatibleBitmap_Fail);

            // associate the bitmap buffer with the in-memory device context.
            EnsureHandle(NativeMethods.SelectObject(_snapshotDc, snapshotPtr), Exceptions.SelectObject_Fail);

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
            else
            {
                throw new Exception(Exceptions.BitBlt_Fail);
            }

            var image = Image.FromHbitmap(snapshotPtr);
            return ConvertToRGBA(image);
        }

        private Bitmap ConvertToRGBA(Bitmap image)
        {
            var height = image.Height;
            var width = image.Width;
            var clone = new Bitmap(width, height, PixelFormat.Format32bppPArgb);

            using (image)
            {
                using (var g = Graphics.FromImage(clone))
                {
                    g.DrawImage(image, new Rectangle(0, 0, width, height));
                }
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
