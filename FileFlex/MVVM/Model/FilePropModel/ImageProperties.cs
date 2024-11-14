using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileFlex.MVVM.Model.FilePropModel
{
    public class ImageProperties : FileBaseProperties
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public double DpiX { get; set; }

        public double DpiY { get; set; }

        public string PixelFormat { get; set; }

        public string Author { get; set; }

        public int FramesCount { get; set; }

        public string MimeType { get; set; }
    }
}
