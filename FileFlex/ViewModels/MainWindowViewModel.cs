using FileFlex.Model;
using FileFlex.ViewModels.Commands;
using FileFlex.ViewModels.Services;
using FileFlex.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace FileFlex.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        #region Свойства

        private readonly INavigationService _navigationServices;

        public List<string> ConvertImageFormatList { get; set; } = ["Изображение", "Видео", "Аудио", "Документы", "Архивы",];

        private BaseViewModel _currentPage;
        public BaseViewModel СurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;                
                OnPropertyChanged();
            }
        }

        #endregion

        public MainWindowViewModel(NavigationServices navigationServices)
        {
            _navigationServices = navigationServices ?? throw new ArgumentNullException(nameof(navigationServices));
        }

        #region Команды
    
        private RelayCommand _navigateToImageConvertPageCommand;
        public RelayCommand NavigateToImageConvertPageCommand
        {
            get
            {
                return _navigateToImageConvertPageCommand ?? (_navigateToImageConvertPageCommand = new RelayCommand(obj =>
                {
                    NavigatToImageConvertPage();
                }));
            }
        }


        private RelayCommand _navigateToCreateGifPageCommand;

        public RelayCommand NavigateToCreateGifPageCommand
        {
            get
            {
                return _navigateToCreateGifPageCommand ?? (_navigateToCreateGifPageCommand = new RelayCommand(obj =>
                {
                    NavigatToCreateGifPage();
                }));
            }
        }

        private RelayCommand _navigateToCreatePDFPageCommand;

        public RelayCommand NavigateToCreatePDFPageCommand
        {
            get
            {
                return _navigateToCreatePDFPageCommand ?? (_navigateToCreatePDFPageCommand = new RelayCommand(obj =>
                {
                    NavigatToCreatePDFPage();
                }));
            }
        }
        #endregion

        #region Методы

        private void NavigatToImageConvertPage()
        {
            _navigationServices.NavigateTo("ImageConvertPage", null);
        }

        private void NavigatToCreateGifPage()
        {
            _navigationServices.NavigateTo("CreateGIFPage", null);
        }

        private void NavigatToCreatePDFPage()
        {
            _navigationServices.NavigateTo("CreatePDFPage", null);
        }

        #endregion
    }
}
