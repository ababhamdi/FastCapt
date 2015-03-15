using System;
using System.Runtime.InteropServices;

namespace FastCapt.Recorders.Interop
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct PROPVARIANT
    {
        [FieldOffset(0)]
        internal ushort varType;
        [FieldOffset(2)]
        internal ushort wReserved1;
        [FieldOffset(4)]
        internal ushort wReserved2;
        [FieldOffset(6)]
        internal ushort wReserved3;

        [FieldOffset(8)]
        internal byte bVal;
        [FieldOffset(8)]
        internal sbyte cVal;
        [FieldOffset(8)]
        internal ushort uiVal;
        [FieldOffset(8)]
        internal short iVal;
        [FieldOffset(8)]
        internal UInt32 uintVal;
        [FieldOffset(8)]
        internal Int32 intVal;
        [FieldOffset(8)]
        internal UInt64 ulVal;
        [FieldOffset(8)]
        internal Int64 lVal;
        [FieldOffset(8)]
        internal float fltVal;
        [FieldOffset(8)]
        internal double dblVal;
        [FieldOffset(8)]
        internal short boolVal;
        [FieldOffset(8)]
        internal IntPtr pclsidVal; //this is for GUID ID pointer
        [FieldOffset(8)]
        internal IntPtr pszVal; //this is for ansi string pointer
        [FieldOffset(8)]
        internal IntPtr pwszVal; //this is for Unicode string pointer
        [FieldOffset(8)]
        internal IntPtr punkVal; //this is for punkVal (interface pointer)
        [FieldOffset(8)]
        internal PROPARRAY ca;
        [FieldOffset(8)]
        internal System.Runtime.InteropServices.ComTypes.FILETIME filetime;
    }
}