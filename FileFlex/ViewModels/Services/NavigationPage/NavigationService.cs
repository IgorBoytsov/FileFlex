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

        private static readonly Page ImageConvert = new ImageConvertPage();
        private static readonly Page DocumentConvert = new DocumentConverterPage();
        private static readonly Page CreateGIF = new CreateGIFPage();

        public NavigationServices(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(string pageName, object parameter)
        {
            switch (pageName)
            {
                case "ImageConvertPage":
                    _frame.Navigate(ImageConvert);
                    break;

                case "DocumentConverterPage":
                    _frame.Navigate(DocumentConvert);
                    break;
                
                case "CreatePDFPage":
                    _frame.Navigate(new CreatePDFPage());
                    break;

                case "CreateGIFPage":
                    _frame.Navigate(CreateGIF);
                    break;
                default:
                    throw new ArgumentException($"Страница '{pageName}' не найдена.");
            }
        }
    }
}
