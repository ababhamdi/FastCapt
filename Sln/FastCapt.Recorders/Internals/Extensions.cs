using System;
using System.Drawing;

namespace FastCapt.Recorders.Internals
{
    internal static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        internal static Rectangle GetBounds(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }

            return new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        }

        internal static Bitmap Clip(this Bitmap source, int xPos, int yPos, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, source.PixelFormat);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                var destRect = new Rectangle(0, 0, width, height);
                g.DrawImage(source, destRect, xPos, yPos, width, height, GraphicsUnit.Pixel);
            }
            return bitmap;
        }
    }
}
