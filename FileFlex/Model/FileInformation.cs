using FontAwesome5;
using System.Drawing;

namespace FileFlex.Model
{
    class FileInformation
    {
        public string FileUri { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileResolution { get; set; }

        public string FileSize { get; set; }

        public string FileCreatedTieme { get; set; }

        public string FileTimeOfChange { get; set; }

        public string FileOwner { get; set; }

        public EFontAwesomeIcon FontAwesomeIcon { get; set; }

        public Color ColorIcon { get; set; }
    }
}
