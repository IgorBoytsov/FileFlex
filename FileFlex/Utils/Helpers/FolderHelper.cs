using System.IO;

namespace FileFlex.Utils.Helpers
{
    public static class FolderHelper
    {
        private static async Task<long> GetFolderSize(string folderPath)
        {
            return await Task.Run(() =>
            {
                long totalSize = 0;

                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                    FileInfo[] files = directoryInfo.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        totalSize += file.Length;
                    }

                    DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
                    foreach (DirectoryInfo subDirectory in subDirectories)
                    {
                        totalSize += GetFolderSize(subDirectory.FullName).Result;
                    }
                }
                catch (UnauthorizedAccessException)
                {

                }
                catch (Exception)
                {

                }

                return totalSize;
            });
        }
    }
}
