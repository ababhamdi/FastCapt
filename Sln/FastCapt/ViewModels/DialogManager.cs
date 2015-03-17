using System.IO;
using Microsoft.Win32;

namespace FastCapt.ViewModels
{
    public class DialogManager
    {
        public bool ShowRecordingSaveDialog(out string fileName)
        {
            var saveFileDialog = new SaveFileDialog {Filter = "Gif Image (*.gif)|*.gif"};
            if (saveFileDialog.ShowDialog() == true)
            {
                fileName = saveFileDialog.FileName;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                return true;
            }

            fileName = null;
            return false;
        }
    }
}