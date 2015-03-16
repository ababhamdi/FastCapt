using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using FastCapt.Core;
using FastCapt.Recorders;
using FastCapt.Recorders.Interfaces;
using FastCapt.Services;
using FastCapt.Services.Interfaces;
using Microsoft.Win32;

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
        private ICommand _selectRecordingArea;
        private IScreenSelectorService _screenSelectorService;
        private IRecorder _recorder;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            Initialize();

            // we only have the gif recorder for now.
            _recorder = new GifRecorder();
            _screenSelectorService = new ScreenSelectorService();
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
                        try
                        {
                            StopDurationTimer();

                            _recorder.Stop();
                            var saveFileDialog = new SaveFileDialog();
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                var fileName = saveFileDialog.FileName;
                                if (File.Exists(fileName))
                                {
                                    File.Delete(fileName);
                                }

                                _recorder.Save(new FileStream(fileName, FileMode.CreateNew));
                            }
                        }
                        finally
                        {
                            IsRecordingPaused = IsRecording = IsRecordingAreaSelected = false;
                        }

                    },
                    o => IsRecordingAreaSelected && (IsRecording || IsRecordingPaused));
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
                        try
                        {
                            StartDurationTimer();
                            var rect = _screenSelectorService.RecordingArea;
                            _recorder.Start(new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
                            IsRecording = true;
                        }
                        catch (Exception)
                        {
                            IsRecording = false;
                        }
                    },
                    o => IsRecordingAreaSelected );
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
                        _recorder.Pause();
                    });
                }
                return _pauseRecordingCommand;
            }
        }

        public ICommand SelectRecordingAreaCommand
        {
            get
            {
                if (_selectRecordingArea == null)
                {
                    _selectRecordingArea = new RelayCommand(
                        o =>
                        {
                            var result = _screenSelectorService.SelectArea();
                            if (result)
                            {
                                IsRecordingAreaSelected = true;
                            }
                        },
                        o =>
                        {
                            // you can select again, if you're recording or paused.
                            return !(IsRecording || IsRecordingPaused);
                        });
                }
                return _selectRecordingArea;
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
