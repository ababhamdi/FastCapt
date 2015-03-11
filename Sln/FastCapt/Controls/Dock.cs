using System;

namespace FastCapt.Controls
{
    /// <summary>
    /// Indicates the location of the resize thumbs
    /// </summary>
    [Flags]
    internal enum Dock
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        TopLeft = (Top | Left),
        TopRight = (Top | Right),
        BottomLeft = (Bottom | Left),
        BottomRight = (Bottom | Right)
    }
}