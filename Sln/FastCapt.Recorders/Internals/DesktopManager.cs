using System;
using FastCapt.Recorders.Interop;
using FastCapt.Recorders.Resources;

namespace FastCapt.Recorders.Internals
{
    /// <summary>
    /// a helper class that encapsulates the desktop.
    /// </summary>
    internal class DesktopManager
    {
        #region "Fields"

        private IntPtr _desktopWindowHandle;
        private IntPtr _desktopDeviceContext;

        #endregion

        #region "Properties"

        public IntPtr DeviceContext
        {
            get { return _desktopDeviceContext; }
        }

        #endregion

        #region "Methods"

        public void AcquireDeviceContext()
        {
            _desktopWindowHandle = NativeMethods.GetDesktopWindow();
            if (_desktopWindowHandle == IntPtr.Zero)
            {
                throw new Exception(Exceptions.Failed_DesktopWindow_Handle);
            }

            _desktopDeviceContext = NativeMethods.GetDC(_desktopWindowHandle);
            if (_desktopDeviceContext == IntPtr.Zero)
            {
                throw new Exception(Exceptions.Failed_DesktopWnd_DC);
            }
        }

        public void RelaseDeviceContext()
        {
            // in case we previously failed to aquire the desktop or dc handle.
            if (_desktopDeviceContext == IntPtr.Zero || _desktopWindowHandle == IntPtr.Zero)
            {
                return;
            }

            if (!NativeMethods.ReleaseDC(_desktopWindowHandle, _desktopDeviceContext))
            {
                throw new Exception(Exceptions.DesktopWnd_FailReleaseDC);
            }
        }

        #endregion
    }
}
