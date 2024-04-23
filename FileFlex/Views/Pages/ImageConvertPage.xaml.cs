using FileFlex.ViewModels;
using System.Windows.Controls;

namespace FileFlex.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ImageConvertPage.xaml
    /// </summary>
    public partial class ImageConvertPage : Page
    {
        public ImageConvertPage()
        {
            InitializeComponent();

            DataContext = new ImageConvertPageViewModel();
        }
    }
}
