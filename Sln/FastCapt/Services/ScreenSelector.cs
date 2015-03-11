using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using FastCapt.Controls;
using FastCapt.Core;
using FastCapt.Interop;
using FastCapt.Services.Interfaces;
using FastCapt.ViewModels;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace FastCapt.Services
{
    public interface IScreenSelectorService : IStartupService
    {
        bool SelectArea();
    }

    public class ScreenSelectorService : ObservableObject, IScreenSelectorService
    {
        #region "Fields"

        private HwndSource _previewShadowHwndSource;
        private SelectionShadow _previewShadow;
        private Rect _recordingArea;

        private const int AREA_ADORNER_PADDING = 5;

        private IntPtr _selectorHwnd;
        private IntPtr _adornerHwnd;

        #endregion

        #region "Properties"

        public Rect RecordingArea
        {
            get { return _recordingArea; }
            set
            {
                if (_recordingArea == value)
                    return;

                _recordingArea = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        private DispatcherFrame _dispatcherFrame;
        private bool _isAreaSelected;

        public bool SelectArea()
        {
            _isAreaSelected = false;
            InitAreaSelection();
            InitiateDialogFunctionality();
            return _isAreaSelected;
        }

        private void InitiateDialogFunctionality()
        {
            try
            {
                ComponentDispatcher.PushModal();
                _dispatcherFrame = new DispatcherFrame();
                Dispatcher.PushFrame(_dispatcherFrame);
            }
            finally
            {
                ComponentDispatcher.PopModal();
            }
        }

        private void InitAreaSelection()
        {
            InitializePreviewShadowWnd();
        }

        private void InitializePreviewShadowWnd()
        {
            // get the screen where the cursor is located.
            var mousePos = Mouse.GetPosition(null);
            var activeScreen = Screen.FromPoint(new System.Drawing.Point((int)mousePos.X, (int)mousePos.Y));

            var parameters = new HwndSourceParameters("PreviewShadow", activeScreen.Bounds.Width, activeScreen.Bounds.Height)
            {
                UsesPerPixelOpacity = true,
                PositionX = 0,
                PositionY = 0,
                WindowStyle = (int)(WS.POPUP),
                ExtendedWindowStyle = (int)(WS_EX.TOPMOST | WS_EX.NOPARENTNOTIFY | WS_EX.TOOLWINDOW),
            };

            _previewShadowHwndSource = new HwndSource(parameters)
            {
                RootVisual = PreparePreviewShadow()
            };


            _previewShadow.AreaSelected += (sender, args) =>
            {
                OnAreaSelected();
            };

            Win32.ShowWindow(_previewShadowHwndSource.Handle, (int)SW.SHOW);
        }

        private void OnAreaSelected()
        {
            var mousePos = Mouse.GetPosition(null);
            var activeScreen = Screen.FromPoint(new System.Drawing.Point((int)mousePos.X, (int)mousePos.Y));
            var width = activeScreen.Bounds.Width;
            var height = activeScreen.Bounds.Height;

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

            var areaAdornerHwndSource = new HwndSource(parameters)
            {
                RootVisual = PrepareAreaAdornerRoot()
            };

            Win32.ShowWindow(areaAdornerHwndSource.Handle, (int)SW.SHOW);
            DismissModalFrame();
            _isAreaSelected = true;
        }

        private void DismissModalFrame()
        {
            // get rid of the dispatcher frame, and return the result to the user.
            Debug.Assert(_dispatcherFrame != null);
            if (_dispatcherFrame != null)
            {
                _dispatcherFrame.Continue = false;
                _dispatcherFrame = null;
            }
        }

        private UIElement PrepareAreaAdornerRoot()
        {
            var areaAdorner = new AreaAdorner
            {
                DataContext = new MainViewModel()
            };

            areaAdorner.SetBinding(AreaAdorner.RecordingRectProperty, new System.Windows.Data.Binding
            {
                Source = this,
                Path = new PropertyPath("RecordingArea"),
                Mode = BindingMode.TwoWay
            });

            Canvas.SetLeft(areaAdorner, RecordingArea.Left);
            Canvas.SetTop(areaAdorner, RecordingArea.Top);
            areaAdorner.Width = RecordingArea.Width;
            areaAdorner.Height = RecordingArea.Height;

            var canvas = new Canvas();
            canvas.Children.Add(areaAdorner);
            return canvas;
        }

        private UIElement PreparePreviewShadow()
        {
            if (_previewShadow != null)
            {
                _previewShadow.Visibility = Visibility.Hidden;
                _previewShadow.RecordingRect = Rect.Empty;
                return _previewShadow;
            }

            _previewShadow = new SelectionShadow
            {
                Visibility = Visibility.Visible
            };

            _previewShadow.SetBinding(SelectionShadow.RecordingRectProperty, new System.Windows.Data.Binding
            {
                Source = this,
                Path = new PropertyPath("RecordingArea"),
                Mode = BindingMode.TwoWay
            });

            return _previewShadow;
        }

        #region "IStartupService interface implementation"

        public void Run()
        {
            InputManager.Current.PostNotifyInput += CurrentOnPostNotifyInput;
        }

        public void Stop()
        {
            InputManager.Current.PostNotifyInput -= CurrentOnPostNotifyInput;
        }

        #endregion

        private void CurrentOnPostNotifyInput(object sender, NotifyInputEventArgs e)
        {
            if (e.StagingItem.Input.RoutedEvent == Keyboard.KeyDownEvent)
            {
                var keyEventArgs = e.StagingItem.Input as KeyEventArgs;
                if (keyEventArgs != null)
                {
                    if (keyEventArgs.Key == Key.Escape)
                    {
                        _isAreaSelected = false;
                        DismissModalFrame();
                    }
                }
            }
        }
    }
}
