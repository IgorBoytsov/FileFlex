using Microsoft.Win32;
using System.IO;

namespace FileFlex.Utils.Services.FileDialogServices
{
    public class OpenDirectoryDialog : IFileDialogService
    {
        public string[] OpenDialog()
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                ValidateNames = false,
                FileName = "Выбрать папку" // Это только для отображения
            };

            if (dialog.ShowDialog() == true)
            {
                string[] files = [Path.GetDirectoryName(dialog.FileName)];
                return files;
            }
            return null;
        }
    }
}
