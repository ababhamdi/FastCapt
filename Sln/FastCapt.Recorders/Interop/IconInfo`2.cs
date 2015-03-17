using System;
using System.Drawing;

namespace FastCapt.Recorders.Interop
{
    internal class IconInfo : IDisposable
    {
        private ICONINFO ii;

        public IconInfo(IntPtr hCursor)
        {
            if (!NativeMethods.GetIconInfo(hCursor, out ii))
            {
                throw new Exception("Bad hCursor");
            }
        }

        public Bitmap ColorBitmap
        {
            get
            {
                if (ii.hbmColor == IntPtr.Zero) return null;
                return Image.FromHbitmap(ii.hbmColor);
            }
        }

        public Bitmap MaskBitmap
        {
            get
            {
                if (ii.hbmMask == IntPtr.Zero) return null;
                return Image.FromHbitmap(ii.hbmMask);
            }
        }

        public Point HotPoint
        {
            get { return new Point(ii.xHotspot, ii.yHotspot); }
        }

        void IDisposable.Dispose()
        {
            if (ii.hbmColor != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(ii.hbmColor);
            }

            if (ii.hbmMask != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(ii.hbmMask);
            }
        }
    }
}
