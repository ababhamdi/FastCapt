using System;

namespace FastCapt.Recorders.Interop
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    internal enum DrawingFlags
    {
        COMPAT = 0x0004,
        DEFAULTSIZE = 0x0008,
        IMAGE = 0x0002,
        MASK = 0x0001,
        NOMIRROR = 0x0010,
        NORMAL = 0x0003
    }
}