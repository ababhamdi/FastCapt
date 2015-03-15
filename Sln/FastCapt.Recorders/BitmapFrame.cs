using System.Drawing;

namespace FastCapt.Recorders
{
    internal class BitmapFrame
    {
        #region "Properties"

        public Bitmap Data { get; private set; }

        public int XPos { get; set; }

        public int YPos { get; set; }

        public int Delay { get; set; }

        #endregion

        #region "Constructors"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="delay"></param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        public BitmapFrame(Bitmap bitmap, int delay, int xPos, int yPos)
        {
            Data = bitmap;
            Delay = delay;
            XPos = xPos;
            YPos = yPos;
        }

        #endregion
    }
}
