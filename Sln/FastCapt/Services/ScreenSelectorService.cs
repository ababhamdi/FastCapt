using System.ComponentModel.Composition;
using System.Windows;
using FastCapt.Controls;
using FastCapt.Core;
using FastCapt.Services.Interfaces;

namespace FastCapt.Services
{
    [Export(typeof(IScreenSelectorService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ScreenSelectorService : ObservableObject, IScreenSelectorService
    {
        #region "Fields"

        private AdornersElement _adornersElement;
        private SelectionShadowElement _shadowElement;
        private Rect _recordingArea;
        private bool _isRecording;
        private bool _closed;

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

        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                if (_isRecording == value)
                    return;

                _isRecording = value;
                RaisePropertyChanged();
            }
        }

        public bool IsLoaded { get; private set; }
        
        #endregion

        #region "Methods"
        
        public bool SelectArea()
        {
            _shadowElement = Container.Current.GetExportedValue<SelectionShadowElement>();
            _closed = false;
            var result = _shadowElement.Show();
            if (result)
            {
                _adornersElement = Container.Current.GetExportedValue<AdornersElement>();
                _adornersElement.Show();
            }

            return result;
        }

        /// <summary>
        /// will unload all elements that make up the screen selector.
        /// </summary>
        public void Close()
        {
            if (_closed)
            {
                return;
            }

            RecordingArea = new Rect();
            _shadowElement.Close();
            if (_adornersElement != null)
            {
                _adornersElement.Close();
            }
            _closed = true;
        }

        #endregion
        
        #region "IStartupService implementation"

        public void Run()
        {
        }

        public void Stop()
        {
            
        }

        #endregion

        #region "Keyboard Events"

        //private void HookKeyboardEvents()
        //{
        //    InputManager.Current.PostNotifyInput += CurrentOnPostNotifyInput;
        //}

        //private void UnhookKeyboardEvents()
        //{
        //    InputManager.Current.PostNotifyInput -= CurrentOnPostNotifyInput;
        //}

        //private void CurrentOnPostNotifyInput(object sender, NotifyInputEventArgs e)
        //{
        //    if (e.StagingItem.Input.RoutedEvent == Keyboard.KeyDownEvent)
        //    {
        //        var keyEventArgs = e.StagingItem.Input as KeyEventArgs;
        //        if (keyEventArgs != null)
        //        {
        //            if (keyEventArgs.Key == Key.Escape)
        //            {
        //                Close();
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
