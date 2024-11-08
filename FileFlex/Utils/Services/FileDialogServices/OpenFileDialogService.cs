using Microsoft.Win32;
using System.IO;

namespace FileFlex.Utils.Services.FileDialogServices
{
    public class OpenFileDialogService : IFileDialogService
    {
        private readonly OpenFileDialog openFileDialog = new OpenFileDialog();
       
        public string[] OpenDialog()
        {
            openFileDialog.Title = "Выберите файлы";
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                                                           "Downloads");
            openFileDialog.ShowDialog();

            return openFileDialog.FileNames;
        }
    }
}