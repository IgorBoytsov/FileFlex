using FileFlex.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileFlex.ViewModels.Services
{
    class NavigationServices : INavigationService
    {
        private readonly Frame _frame;

        public NavigationServices(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(string pageName, object parameter)
        {
            switch (pageName)
            {
                case "CreateGIFPage":
                    _frame.Navigate(new CreateGIFPage());
                    break;

                case "ImageConvertPage":
                    _frame.Navigate(new ImageConvertPage());
                    break;
                case "CreatePDFPage":
                    _frame.Navigate(new CreatePDFPage());
                    break;
                case "DocumentConverterPage":
                    _frame.Navigate(new DocumentConverterPage());
                    break;
                default:
                    throw new ArgumentException($"Страница '{pageName}' не найдена.");
            }
        }
    }
}
