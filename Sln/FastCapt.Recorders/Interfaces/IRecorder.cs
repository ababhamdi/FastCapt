using System.Drawing;
using System.IO;

namespace FastCapt.Recorders.Interfaces
{
    public interface IRecorder
    {
        void Stop();
        void Save(Stream stream);
        void Start(Rectangle rect);
        void Pause();
    }
}
