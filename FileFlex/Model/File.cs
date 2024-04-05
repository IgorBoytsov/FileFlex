using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFlex.Model
{
    class File
    {
        public string FileUri { get; set; }

        public string FileName {  get; set; }

        public string FileType { get; set; }

        public File(string uri, string name, string type) 
        {
            FileUri = uri;
            FileName = name;
            FileType = type;
        }
    }
}
