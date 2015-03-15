using System.Runtime.InteropServices;

namespace FastCapt.Recorders.Internals
{
    internal static class HResult
    {
        internal static void Check(int hResult)
        {
            if (hResult < 0)
            {
                Marshal.ThrowExceptionForHR(hResult);
            }
        }
    }
}
