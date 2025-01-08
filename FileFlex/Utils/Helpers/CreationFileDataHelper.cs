using FileFlex.MVVM.Model.AppModel;
using FileFlex.Utils.Enums;
using System.IO;

namespace FileFlex.Utils.Helpers
{
    public static class CreationFileDataHelper
    {
        public static async Task<FileData> FileDataFromFile(string file)
        {
            FileInfo fileInfo = new(file);
            var fileData = new FileData()
            {
                FileName = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                FileExtension = fileInfo.Extension,
                FileIcon = await IconExtractionHelper.ExtractionFileIconAsync(fileInfo.FullName),
                FilePath = fileInfo.FullName,
                DateCreate = fileInfo.CreationTime,
                FileWeight = fileInfo.Length,
                //FileWeight = Math.Round((double)fileInfo.Length / (1024 * 1024), 3), //Получаем размер в МБ,
                //FileWeight = Math.Round((double)fileInfo.Length / 1024, 1), //Получаем размер в КБ,
            };

            return fileData;
        }

        public static async Task<FileData> FileDataFromDirectory(string entry)
        {
            var directory = new DirectoryInfo(entry);
           
            var fileData = new FileData
            {
                FileName = directory.Name,
                FileExtension = EntryType.Folder.ToString(),
                FileIcon = BitmapHelper.ToBitmapImage(BitmapHelper.ToBitmap("D:\\Программирование\\Code\\WPF\\FileFlex\\FileFlex\\MVVM\\View\\Images\\FolderImage.png")),
                FilePath = entry,
                DateCreate = directory.CreationTime,
                //FileWeight = await GetFolderSize(entry),
            };

            return fileData;
        }
    }
}
