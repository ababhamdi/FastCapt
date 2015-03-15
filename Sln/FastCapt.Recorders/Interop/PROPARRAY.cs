using System;
using System.Runtime.InteropServices;

namespace FastCapt.Recorders.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct PROPARRAY
    {
        internal UInt32 cElems;
        internal IntPtr pElems;
    }
}
