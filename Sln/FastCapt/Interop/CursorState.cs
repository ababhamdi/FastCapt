namespace FastCapt.Interop
{
    /// <summary>
    /// 
    /// </summary>
    internal enum CursorState
    {
        /// <summary>
        /// The cursor is hidden.
        /// </summary>
        Hidden = 0x00000000,

        /// <summary>
        /// The cursor is showing.
        /// </summary>
        Showing = 0x00000001,

        /// <summary>
        /// Windows 8: The cursor is suppressed. This flag indicates that
        /// the system is not drawing the cursor because the user is providing
        /// input through touch or pen instead of the mouse.
        /// </summary>
        Suppressed = 0x00000001
    }
}