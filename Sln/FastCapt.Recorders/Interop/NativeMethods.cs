using System;
using System.Runtime.InteropServices;

namespace FastCapt.Recorders.Interop
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Initializes a <see cref="PROPVARIANT"/> structure using the contents of a buffer.
        /// </summary>
        /// <param name="buffer">Pointer to the buffer.</param>
        /// <param name="celems">The length of the buffer, in bytes.</param>
        /// <param name="ppropvar">When this function returns, contains the initialized <see cref="PROPVARIANT"/> structure.</param>
        [DllImport(ExternalDlls.PROPSYS, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int InitPropVariantFromBuffer(IntPtr buffer, uint celems, out PROPVARIANT ppropvar);
    }
}
