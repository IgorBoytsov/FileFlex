using FileFlex.MVVM.ViewModels.WindowViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
            StateChanged += MainWindowStateChangeRaised;
            DataContext = viewModel;
            Closing += (s, e) => (DataContext as IDisposable)?.Dispose();
        }

        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            //SystemCommands.CloseWindow(this);
            Application.Current.Shutdown();
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        // Пока что не используется
        #region Сброс выделение элементов в списке ( По клику вне списка, По клику вне элемента внутри списка )

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, был ли клик внутри ListView
            if (e.OriginalSource is DependencyObject source)
            {
                var item = ItemsControl.ContainerFromElement(FilesListView, source);
                // Если клик был не по элементу внутри ListView, то сбрасываем выделение
                if (item == null && !IsClickInsideElement(FilesListView, e))
                {
                    FilesListView.SelectedItems.Clear();
                }
            }
        }

        private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, был ли клик внутри элемента ListView
            if (e.OriginalSource is DependencyObject source)
            {
                var item = ItemsControl.ContainerFromElement((ListView)sender, source);
                // Если клик не по элементу, то сбрасываем выделение
                if (item == null)
                {
                    FilesListView.SelectedItems.Clear();
                }
            }
        }

        // Проверка, был ли клик по указанному элементу или его дочерним элементам
        private static bool IsClickInsideElement(FrameworkElement element, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(element);
            return position.X >= 0 && position.X <= element.ActualWidth &&
                   position.Y >= 0 && position.Y <= element.ActualHeight;
        }

        #endregion
    }
}