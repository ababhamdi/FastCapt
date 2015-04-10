using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using FastCapt.Interop;
using FastCapt.Services.Interfaces;
using FastCapt.ViewModels;
using Point = System.Drawing.Point;

namespace FastCapt.Controls
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectionShadowElement
    {
        #region "Fields"

        private readonly IScreenSelectorService _screenSelectorService;
        private HwndSource _hwndSource;
        private DispatcherFrame _selectionFrame;
        private bool _result;

        #endregion
        
        #region "Constructors"

        [ImportingConstructor]
        public SelectionShadowElement([Import] IScreenSelectorService screenSelectorService)
        {
            _screenSelectorService = screenSelectorService;
        }

        #endregion
        
        public bool Show()
        {
            InitHwndSource();
            Win32.ShowWindow(_hwndSource.Handle, SW.SHOW);
            EnterDialogMode();
            return _result;
        }

        public void Close()
        {
            ExitDialogMode();
            Win32.ShowWindow(_hwndSource.Handle, SW.HIDE);
            _hwndSource.Dispose();
            _result = false;
        }

        private void InitHwndSource()
        {
            var mousePos = Mouse.GetPosition(null);
            var activeScreen = Screen.FromPoint(new Point((int)mousePos.X, (int)mousePos.Y));
            var parameters = new HwndSourceParameters("SelectionShadow", activeScreen.Bounds.Width, activeScreen.Bounds.Height)
            {
                UsesPerPixelOpacity = true,
                PositionX = 0,
                PositionY = 0,
                WindowStyle = (int)(WS.POPUP | WS.VISIBLE | WS.CLIPCHILDREN | WS.CLIPSIBLINGS),
                ExtendedWindowStyle = (int)(WS_EX.TOPMOST | WS_EX.NOPARENTNOTIFY | WS_EX.TOOLWINDOW),
            };

            _hwndSource = new HwndSource(parameters)
            {
                RootVisual = InitRootVisual()
            };
        }

        private UIElement InitRootVisual()
        {
            var rootVisual = new SelectionShadow
            {
                DataContext = Container.Current.GetExportedValue<MainViewModel>()
            };
            rootVisual.SetBinding(SelectionShadow.RecordingRectProperty, _screenSelectorService, "RecordingArea");
            rootVisual.SetBinding(SelectionShadow.IsRecordingProperty, _screenSelectorService, "IsRecording");
            rootVisual.AreaSelected += OnAreaSelected;
            return rootVisual;
        }

        private void OnAreaSelected(object sender, RoutedEventArgs e)
        {
            var selectionShadow = (SelectionShadow) sender;
            selectionShadow.AreaSelected -= OnAreaSelected;
            _result = true;
            ExitDialogMode();
        }

        private void EnterDialogMode()
        {
            _selectionFrame = new DispatcherFrame();
            Dispatcher.PushFrame(_selectionFrame);
        }

        private void ExitDialogMode()
        {
            if (_selectionFrame == null)
            {
                return;
            }

            _selectionFrame.Continue = false;
            _selectionFrame = null;
        }
    }
}
