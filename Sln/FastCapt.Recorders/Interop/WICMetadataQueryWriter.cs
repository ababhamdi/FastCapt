using System;
using System.Runtime.InteropServices;

namespace FastCapt.Recorders.Interop
{
    internal static class WICMetadataQueryWriter
    {
        ///<SecurityNote>
        /// Critical as this code performs an elevation.
        ///</SecurityNote>
        [DllImport("WindowsCodecs.dll", EntryPoint = "IWICMetadataQueryWriter_SetMetadataByName_Proxy")]
        internal static extern int SetMetadataByName(IntPtr handle, [Out, MarshalAs(UnmanagedType.LPWStr)] string wzName, ref PROPVARIANT propValue);

        ///<SecurityNote>
        /// Critical as this code performs an elevation.
        ///</SecurityNote>
        [DllImport("WindowsCodecs.dll", EntryPoint = "IWICMetadataQueryWriter_RemoveMetadataByName_Proxy")]
        internal static extern int RemoveMetadataByName(IntPtr handle, [Out, MarshalAs(UnmanagedType.LPWStr)] string wzName);
    }
}