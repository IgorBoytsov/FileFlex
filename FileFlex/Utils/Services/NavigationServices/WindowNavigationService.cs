using FileFlex.MVVM.View.Windows;
using FileFlex.MVVM.ViewModels.WindowViewModels;
using System.Windows;

namespace FileFlex.Utils.Services.NavigationServices
{
    public class WindowNavigationService : IWindowNavigationService
    {
        private readonly Dictionary<string, Window> _openWindows = new();

        public void NavigateTo(string pageName, object parameter = null)
        {
            if (_openWindows.TryGetValue(pageName, out var existingWindow))
            {
                if (existingWindow.DataContext is IUpdatable viewModel)
                {
                    viewModel.Update(parameter);
                    existingWindow.Activate();
                }
                return;
            }

            Action action = pageName switch
            {
                "ConvertImageWindow" => () =>
                {
                    var viewModel = new ConvertImageWindowViewModel();
                    var convertImageWindow = new ConvertImageWindow
                    {
                        DataContext = viewModel
                    };
                    viewModel.Update(parameter);

                    _openWindows[pageName] = convertImageWindow;
                    convertImageWindow.Closed += (s, e) => _openWindows.Remove(pageName); 
                    convertImageWindow.Show();
                }
                ,
                _ => () =>
                {
                    throw new Exception($"Окна {pageName} не существует.");
                }
                ,
            };
            action.Invoke();
        }
    }
}
