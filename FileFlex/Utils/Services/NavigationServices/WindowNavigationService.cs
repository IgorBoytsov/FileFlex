using FileFlex.MVVM.View.Windows;
using FileFlex.MVVM.ViewModels.WindowViewModels;
using System.Windows;

namespace FileFlex.Utils.Services.NavigationServices
{
    public class WindowNavigationService(IServiceProvider serviceProvider) : IWindowNavigationService
    {
        private readonly Dictionary<string, Window> _openWindows = new();

        private readonly IServiceProvider _serviceProvider = serviceProvider;

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

            OpenWindow(pageName, parameter);
        }

        private void OpenWindow(string pageName, object parameter = null)
        {
            Action action = pageName switch
            {
                "ConvertImageWindow" => () =>
                {
                    var viewModel = new ConvertImageWindowViewModel(_serviceProvider);
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
                "ImageViewerWindow" => () =>
                {
                    var viewModel = new ImageViewerWindowViewModel();
                    var convertImageWindow = new ImageViewerWindow
                    {
                        DataContext = viewModel
                    };
                    viewModel.Update(parameter);

                    _openWindows[pageName] = convertImageWindow;
                    convertImageWindow.Closed += (s, e) => _openWindows.Remove(pageName);
                    convertImageWindow.Show();
                }
                ,
                "SettingsWindow" => () =>
                {
                    var viewModel = new SettingsWindowViewModel(_serviceProvider);
                    var settingsWindow = new SettingsWindow
                    {
                        DataContext = viewModel
                    };
                    
                    _openWindows[pageName] = settingsWindow;
                    settingsWindow.Closed += (s, e) => _openWindows.Remove(pageName);
                    settingsWindow.Show();
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