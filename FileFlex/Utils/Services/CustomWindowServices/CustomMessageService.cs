using FileFlex.MVVM.View.Windows;
using FileFlex.MVVM.ViewModels.WindowViewModels;
using FileFlex.Utils.Enums;

namespace FileFlex.Utils.Services.CustomWindowServices
{
    public class CustomMessageWindowService : ICustomMessageService
    {
        public bool Show(string message, string headerMessage, TypeMessage typeMessage)
        {
            var viewModel = new MessageWindowViewModel(message, headerMessage, typeMessage);
            var messageWnd = new MessageWindow
            {
                DataContext = viewModel,
            };
            messageWnd.ShowDialog();

            return viewModel.ResultAction;
        }
    }
}
