using Microsoft.Win32;
using System.IO;

namespace FileFlex.Utils.Services.FileDialogServices
{
    public class SaveFileDialogService : IFileDialogService
    {
        private readonly SaveFileDialog saveFileDialog = new SaveFileDialog();

        public string[] OpenDialog()
        {
            saveFileDialog.Title = "Выберите путь";
            saveFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                                           "Downloads");
            if (saveFileDialog.ShowDialog() == true) return saveFileDialog.FileNames;
            else return null;
        }
    }
}
