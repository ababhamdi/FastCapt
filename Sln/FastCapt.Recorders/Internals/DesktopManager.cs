using System;
using FastCapt.Recorders.Interop;

namespace FastCapt.Recorders.Internals
{
    /// <summary>
    /// a helper class that encapsulates the desktop.
    /// </summary>
    internal class DesktopManager
    {
        private IntPtr _desktopWindowHandle;
        private IntPtr _desktopDeviceContext;

        public IntPtr DeviceContext
        {
            get { return _desktopDeviceContext; }
        }

        public void AcquireDeviceContext()
        {
            _desktopWindowHandle = NativeMethods.GetDesktopWindow();
            if (_desktopWindowHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to get the desktop window handle");
            }

            _desktopDeviceContext = NativeMethods.GetDC(_desktopWindowHandle);
        }

        public void RelaseDeviceContext()
        {
            if (_desktopDeviceContext == IntPtr.Zero || _desktopWindowHandle == IntPtr.Zero)
            {
                return;
            }

            bool success = NativeMethods.ReleaseDC(_desktopWindowHandle, _desktopDeviceContext);
            if (!success)
            {

            }
        }

        #region Native Methods



        #endregion
    }
}
