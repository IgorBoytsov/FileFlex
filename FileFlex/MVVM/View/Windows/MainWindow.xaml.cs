using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.MVVM.ViewModels.WindowViewModels;
using FileFlex.Utils.Services;
using System.Windows;

namespace FileFlex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}