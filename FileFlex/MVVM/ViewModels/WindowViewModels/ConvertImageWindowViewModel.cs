using FileFlex.MVVM.Model.AppModel;
using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.Utils.Services.NavigationServices;
using System.Collections.ObjectModel;

namespace FileFlex.MVVM.ViewModels.WindowViewModels
{
    public class ConvertImageWindowViewModel : BaseViewModel, IUpdatable
    {
        public ObservableCollection<FileData> FilesConversion { get; set; } = [];

        private readonly IServiceProvider _serviceProvider;

        public ConvertImageWindowViewModel(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public void Update(object parameter)
        {
            if (parameter is IEnumerable<FileData> filesConversion)
            {
                foreach (FileData file in filesConversion)
                {
                    FilesConversion.Add(file);
                }
                
            }
        }
    }
}
