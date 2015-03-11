using System;
using System.Runtime.InteropServices;

namespace FastCapt.Interop
{
    /// <summary>
    /// Contains global cursor information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct CURSORINFO
    {
        /// <summary>
        /// Specifies the size, in bytes, of the structure. 
        /// The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
        /// </summary>
        public Int32 cbSize;

        /// <summary>
        /// Specifies the cursor state. This parameter can be one of the following values
        /// </summary>
        public CursorState flags;

        /// <summary>
        /// A handle to the cursor.
        /// </summary>
        public IntPtr hCursor;

        /// <summary>
        /// A POINT structure that receives the screen coordinates of the cursor.
        /// </summary>
        public POINT ptScreenPos;
    }
}