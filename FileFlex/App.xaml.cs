using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.MVVM.ViewModels.WindowViewModels;
using FileFlex.Utils.Services;
using FileFlex.Utils.Services.FileConvertServices;
using FileFlex.Utils.Services.FileDialogServices;
using FileFlex.Utils.Services.NavigationServices;
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

        //  AddTransient - 
        //    Описание: Каждый раз, когда вы запрашиваете сервис, создается новый экземпляр.
        //    Когда использовать: Для легковесных и независимых сервисов, где создание нового объекта не требует значительных ресурсов.
        //  AddScoped
        //      Описание: Экземпляр создается один раз на каждый запрос.Это означает, что в пределах одного HTTP-запроса(например, в ASP.NET Core) вы получите один и тот же экземпляр, но для разных запросов будут создаваться разные экземпляры.
        //      Когда использовать: Когда необходимо, чтобы сервис разделялся между компонентами в рамках одного запроса, но не между разными запросами.
        //  AddSingleton
        //      Описание: Экземпляр создается один раз и используется на протяжении всего времени работы приложения.Все запросы к сервису будут получать один и тот же экземпляр.
        //      Когда использовать: Для тяжелых или долговечных объектов, которые должны сохранять состояние и инициализируются один раз.
        
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();

            services.AddSingleton<IFileConvertService, ImageConvertService>();
            services.AddSingleton<IFileConvertService, DocumentConvertService>();

            services.AddSingleton<INavigationService, PageNavigationService>();
            services.AddSingleton<INavigationService, WindowNavigationService>();

            services.AddSingleton<IFileDialogService, OpenFileDialogService>();
            services.AddSingleton<IFileDialogService, SaveFileDialogService>();
           
        }
    }
}
