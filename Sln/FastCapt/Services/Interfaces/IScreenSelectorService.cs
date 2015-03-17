using System.Security.Policy;
using System.Windows;

namespace FastCapt.Services.Interfaces
{
    public interface IScreenSelectorService : IStartupService
    {
        bool SelectArea();
        Rect RecordingArea { get; set; }

        bool IsRecording { get; set; }

        void Unload();
    }
}