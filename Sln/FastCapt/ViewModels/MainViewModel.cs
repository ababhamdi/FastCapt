using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FastCapt.Core;
using FastCapt.Recorders;
using FastCapt.Recorders.Interfaces;
using FastCapt.Services;
using FastCapt.Services.Interfaces;

namespace FastCapt.ViewModels
{
    [Export(typeof(MainViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class MainViewModel : ObservableObject
    {
        #region "Fields"

        private ICommand _startRecordingCommand;
        private ICommand _stopRecordingCommand;
        private ICommand _pauseRecordingCommand;
        private string _title;
        private bool _isRecording;
        private bool _isRecordingAreaSelected;
        private bool _isRecordingPaused;
        private TimeSpan _recordingDuration;
        private DispatcherTimer _durationTimer;
        private ICommand _selectRecordingArea;
        private IScreenSelectorService _screenSelectorService;
        private IRecorder _recorder;
        private DialogManager _dialogManager;
        private ICommand _closeCommand;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="MainViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public MainViewModel([Import]IScreenSelectorService screenSelectorService)
        {
            Initialize();

            // we only have the gif recorder for now.
            _recorder = new GifRecorder();
            _screenSelectorService = screenSelectorService;
            _screenSelectorService.Run();

            _dialogManager = new DialogManager();
        }

        #endregion

        #region "Commands"

        public ICommand StopRecordingCommand
        {
            get
            {
                return _stopRecordingCommand ?? (_stopRecordingCommand = new RelayCommand(o => OnStopRecording(),
                    o => IsRecordingAreaSelected && (IsRecording || IsRecordingPaused)));
            }
        }

        public ICommand StartRecordingCommand
        {
            get
            {
                return _startRecordingCommand ??
                       (_startRecordingCommand = new RelayCommand(o => OnStartRecording(), o => IsRecordingAreaSelected));
            }
        }

        public ICommand PauseRecordingCommand
        {
            get
            {
                return _pauseRecordingCommand ?? (_pauseRecordingCommand = new RelayCommand(o => OnPauseRecording()));
            }
        }

        public ICommand SelectRecordingAreaCommand
        {
            get
            {
                return _selectRecordingArea ?? (_selectRecordingArea = new RelayCommand(a => OnSelectRecordingArea(),
                    o => !(IsRecording || IsRecordingPaused)));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(o =>
                    {
                        _screenSelectorService.Close();
                        if (IsRecordingAreaSelected)
                        {
                            IsRecordingAreaSelected = false;
                        }
                    });
                }
                return _closeCommand;
            }
        }

        #endregion

        #region "Properties"

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                    return;

                _title = value;
                RaisePropertyChanged();
            }
        }

        public bool IsRecording
        {
            get { return _isRecording; }
            private set
            {
                if (_isRecording == value)
                    return;

                _isRecording = value;
                RaisePropertyChanged();
            }
        }

        public TimeSpan RecordingDuration
        {
            get { return _recordingDuration; }
            private set
            {
                if (_recordingDuration == value)
                {
                    return;
                }

                _recordingDuration = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value that is set when the recording is paused.
        /// </summary>
        public bool IsRecordingPaused
        {
            get { return _isRecordingPaused; }
            private set
            {
                if (_isRecordingPaused == value)
                {
                    return;
                }

                _isRecordingPaused = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value that is set when the user is done selecting the recording area.
        /// </summary>
        public bool IsRecordingAreaSelected
        {
            get { return _isRecordingAreaSelected; }
            private set
            {
                if (_isRecordingAreaSelected == value)
                {
                    return;
                }

                _isRecordingAreaSelected = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region "Methods"

        private void OnSelectRecordingArea()
        {
            if (_screenSelectorService.SelectArea())
                IsRecordingAreaSelected = true;
        }

        private void OnStartRecording()
        {
            try
            {
                _screenSelectorService.IsRecording = IsRecording = true;
                StartDurationTimer();
                var rect = _screenSelectorService.RecordingArea;
                _recorder.Start(new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
            }
            catch (Exception)
            {
                IsRecording = false;
                _screenSelectorService.IsRecording = false;
            }
        }

        private void OnPauseRecording()
        {
            IsRecordingPaused = !IsRecordingPaused;
            PauseOrResumeDurationTimer();
            _recorder.Pause();
        }

        private void OnStopRecording()
        {
            try
            {
                StopDurationTimer();
                _recorder.Stop();
                _screenSelectorService.Close();
                string fileName;
                if (_dialogManager.ShowRecordingSaveDialog(out fileName))
                {
                    _recorder.Save(new FileStream(fileName, FileMode.CreateNew));
                }
            }
            finally
            {
                IsRecordingPaused = IsRecording = IsRecordingAreaSelected = false;
                _screenSelectorService.IsRecording = false;
            }
        }

        private void Initialize()
        {
            Title = "FastCapt [ Beta ]";

            // initialize the duration timer.
            _durationTimer = new DispatcherTimer(DispatcherPriority.Normal) {Interval = TimeSpan.FromSeconds(1)};
            _durationTimer.Tick += (sender, args) =>
            {
                this.RecordingDuration += TimeSpan.FromSeconds(1);
            };
        }

        private void StartDurationTimer()
        {
            RecordingDuration = new TimeSpan();
            _durationTimer.Start();
        }

        private void StopDurationTimer()
        {
            _durationTimer.Stop();
        }

        private void PauseOrResumeDurationTimer()
        {
            _durationTimer.IsEnabled = !IsRecordingPaused;
        }

        public void OnMainWindowMinimized()
        {

        }

        public void OnMainWindowRestored()
        {

        }

        #endregion
    }
}
