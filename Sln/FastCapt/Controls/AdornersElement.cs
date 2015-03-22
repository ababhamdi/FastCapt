using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using FastCapt.Interop;
using FastCapt.Services.Interfaces;
using FastCapt.ViewModels;
using Point = System.Drawing.Point;

namespace FastCapt.Controls
{
    [Export]
    public class AdornersElement
    {
        #region "Constants"

        private const double MINIMUM_SIZE_REQUIRED = 200;

        #endregion

        #region "Fields"

        private IScreenSelectorService _screenSelectorService;
        private HwndSource _instance;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="AdornersElement"/> class.
        /// </summary>
        [ImportingConstructor]
        public AdornersElement([Import]IScreenSelectorService screenSelectorService)
        {
            _screenSelectorService = screenSelectorService;
        }

        #endregion
        
        #region "Methods"
        private UIElement InitRootVisual()
        {
            var areaAdorner = new AreaAdorner
            {
                DataContext = Container.Current.GetExportedValue<MainViewModel>()
            };
            areaAdorner.SetBinding(AreaAdorner.RecordingRectProperty, _screenSelectorService, "RecordingArea");
            areaAdorner.SetBinding(AreaAdorner.IsRecordingProperty, _screenSelectorService, "IsRecording");

            var canvas = new Canvas();
            canvas.Children.Add(areaAdorner);
            return canvas;
        }

        private void InitHwndSource()
        {
            var mousePos = Mouse.GetPosition(null);
            var activeScreen = Screen.FromPoint(new Point((int)mousePos.X, (int)mousePos.Y));
            var width = activeScreen.Bounds.Width;
            var height = activeScreen.Bounds.Height;

            EnsureMinimumRequiredSize();

            var parameters = new HwndSourceParameters("AreaAdorner",
                width,
                height)
            {
                UsesPerPixelOpacity = true,
                PositionX = 0,
                PositionY = 0,
                WindowStyle = (int)(WS.POPUP),
                ExtendedWindowStyle = (int)(WS_EX.TOPMOST | WS_EX.NOPARENTNOTIFY | WS_EX.TOOLWINDOW)
            };

            _instance = new HwndSource(parameters)
            {
                RootVisual = InitRootVisual()
            };
        }

        public void Show()
        {
            InitHwndSource();
            Win32.ShowWindow(_instance.Handle, SW.SHOW);
        }

        public void Close()
        {
            if (_instance == null)
            {
                return;
            }

            Win32.ShowWindow(_instance.Handle, SW.HIDE);
            _instance.Dispose();
            _instance = null;
        }

        /// <summary>
        /// A method that will check the size selected by the user, and adjust it to meet
        /// the <see cref="MINIMUM_SIZE_REQUIRED"/> for the application.
        /// </summary>
        private void EnsureMinimumRequiredSize()
        {
            var recordingArea = _screenSelectorService.RecordingArea;
            double newWidth = recordingArea.Width;
            double newHeight = recordingArea.Height;

            if (recordingArea.Width < MINIMUM_SIZE_REQUIRED)
                newWidth = MINIMUM_SIZE_REQUIRED;

            if (recordingArea.Height < MINIMUM_SIZE_REQUIRED)
                newHeight = MINIMUM_SIZE_REQUIRED;

            _screenSelectorService.RecordingArea = new Rect(recordingArea.Left, recordingArea.Top, newWidth, newHeight);
        }

        #endregion
    }
}