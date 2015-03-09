using System;
using System.Windows.Input;
using System.Windows.Threading;
using FastCapt.Core;

namespace FastCapt.ViewModels
{
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

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            Initialize();
        }

        #endregion

        #region "Commands"

        public ICommand StopRecordingCommand
        {
            get
            {
                if (_stopRecordingCommand == null)
                {
                    _stopRecordingCommand = new RelayCommand(o =>
                    {
                        StopDurationTimer();
                        IsRecordingPaused = IsRecording = false;
                    });
                }

                return _stopRecordingCommand;
            }
        }

        public ICommand StartRecordingCommand
        {
            get
            {
                if (_startRecordingCommand == null)
                {
                    _startRecordingCommand = new RelayCommand(o =>
                    {
                        StartDurationTimer();
                        IsRecording = true;
                    });
                }

                return _startRecordingCommand;
            }
        }

        public ICommand PauseRecordingCommand
        {
            get
            {
                if (_pauseRecordingCommand == null)
                {
                    _pauseRecordingCommand = new RelayCommand(o =>
                    {
                        IsRecordingPaused = !IsRecordingPaused;
                        PauseOrResumeDurationTimer();
                    });
                }
                return _pauseRecordingCommand;
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

        #endregion
    }
}
