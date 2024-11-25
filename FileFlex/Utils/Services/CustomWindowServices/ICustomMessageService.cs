using FileFlex.Utils.Enums;

namespace FileFlex.Utils.Services.CustomWindowServices
{
    public interface ICustomMessageService
    {
        public bool Show(string message, string headerMessage, TypeMessage typeMessage);
    }
}
