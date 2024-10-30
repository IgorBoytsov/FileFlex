using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.MVVM.ViewModels.WindowViewModels;
using FileFlex.Utils.Services;
using FileFlex.Utils.Services.FileConvertServices;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FileFlex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<IFileConvertService, ImageConvertService>();
            services.AddTransient<IFileConvertService, DocumentConvertService>();
            services.AddTransient<IFileConvertService, AudioConvertService>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }
}
