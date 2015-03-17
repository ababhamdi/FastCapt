using System;
using System.Drawing;

namespace FastCapt.Recorders.Interfaces
{
    public interface ISnapshotManager
    {
        void Initialize(IntPtr desktopDc);

        Bitmap TakeSnapshot();

        /// <summary>
        /// Gets a value that indicates the height of the snapshot.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets a value that indicates the width of the snapshot.
        /// </summary>
        int Width { get; }
    }
}
