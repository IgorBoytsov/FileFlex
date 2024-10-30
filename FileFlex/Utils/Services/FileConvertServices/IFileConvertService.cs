using FileFlex.Utils.Enums;

namespace FileFlex.Utils.Services.FileConvertServices
{
    public interface IFileConvertService
    {
        void Convert(TypeFile typeFile,string filePath);
        void Convert(TypeFile typeFile,IEnumerable<string> filePaths);
    }
}
