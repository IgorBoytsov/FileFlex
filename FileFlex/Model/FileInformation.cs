using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string FileTimeOfChange{ get; set; }

        public string FileOwner { get; set; }

        public int NumberFiles { get;set; }
    }
}
