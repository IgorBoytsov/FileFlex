using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.Utils.Services.FileConvertServices;
using Microsoft.Extensions.DependencyInjection;

namespace FileFlex.MVVM.ViewModels.WindowViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private IServiceProvider _serviceProvider;

        private IFileConvertService _firstMessageService;
        private IFileConvertService _secondMessageService;

        public MainWindowViewModel(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;

            var fileConvertServices = _serviceProvider.GetServices<IFileConvertService>().ToList();

            _firstMessageService = fileConvertServices[0];
            _secondMessageService = fileConvertServices[1];
        }
    }
}
