using System.Windows.Media.Imaging;

namespace FileFlex.MVVM.Model.AppModel
{
    public class FileData
    {
        public string FileName { get; set; }

        public string FileExtension { get; set; }
        
        public BitmapImage FileIcon { get; set; }

        public string FilePath { get; set; }

        public long FileWeight { get; set; }

        public DateTime DateCreate { get; set; }
    }
}
