using FileFlex.Command;
using FileFlex.MVVM.Model.AppModel;
using FileFlex.MVVM.ViewModels.BaseVM;
using FileFlex.Utils.Services.FileDialogServices;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using FileFlex.Utils.Enums;
using Microsoft.VisualBasic.FileIO;
using FileFlex.Utils.Helpers;
using FileFlex.Utils.Services.NavigationServices;

namespace FileFlex.MVVM.ViewModels.WindowViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Коллекции

        private List<FileData> _files = [];

        public ObservableCollection<FileData> Files { get; set; } = [];

        public List<FileData> SelectedFiles { get; set; } = [];

        #endregion

        private readonly IServiceProvider _serviceProvider;

        private readonly INavigationService _pageNavigationService;
        private readonly INavigationService _windowNavigationService;

        private readonly IFileDialogService _openFileDialogService;
        private readonly IFileDialogService _saveFileDialogService;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var navigationServices = _serviceProvider.GetServices<INavigationService>().ToList();
            var fileDialogServices = _serviceProvider.GetServices<IFileDialogService>().ToList();

            _pageNavigationService = navigationServices[0];
            _windowNavigationService = navigationServices[1];

            _openFileDialogService = fileDialogServices[0];
            _saveFileDialogService = fileDialogServices[1];
        }
 
        #region Получение и отображение файлов

        #region Свойства

        private FileData _selectedFile;
        public FileData SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (_selectedFile != value)
                {
                    _selectedFile = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Команды

        private RelayCommand _addFilesCommand;
        public RelayCommand AddFilesCommand { get => _addFilesCommand ??= new (obj => { InteractionFiles(FileAction.Add); }); } 
        
        private RelayCommand _clearFilesCommand;
        public RelayCommand ClearFilesCommand { get => _clearFilesCommand ??= new (obj => { InteractionFiles(FileAction.Clear); }); }

        private RelayCommand _removeFilesCommand;
        public RelayCommand RemoveFilesCommand { get => _removeFilesCommand ??= new(obj => { InteractionFiles(FileAction.Remove); }); }

        private RelayCommand _deleteFilesCommand;
        public RelayCommand DeleteFilesCommand { get => _deleteFilesCommand ??= new(obj => { InteractionFiles(FileAction.Delete); }); }

        private RelayCommand _moveFilesToTrashCommand;
        public RelayCommand MoveFilesToTrashCommand { get => _moveFilesToTrashCommand ??= new(obj => { InteractionFiles(FileAction.MoveToTrash); }); }

        #endregion

        #region Методы: Возоимодействия с списком файлов.

        // TODO: После создание кастюмного окна с сообщениями  Сделать проверку на то, что пользователь согласен удалить файлы с устройства.

        private void InteractionFiles(FileAction fileAction)
        {
            Action action = fileAction switch
            {
                FileAction.Add => async () =>
                {
                    string[] files = _openFileDialogService.OpenDialog();

                    if (files.Length != 0)
                    {
                        foreach (var file in files)
                        {
                            FileInfo fileInfo = new(file);
                            var fileData = new FileData()
                            {
                                FileName = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                                FileExtension = fileInfo.Extension,
                                FileIcon = await IconExtractionHelper.ExtractionFileIconAsync(fileInfo.FullName),
                                FilePath = fileInfo.FullName,
                                DateCreate = fileInfo.CreationTime,
                                //FileWeight = Math.Round((double)fileInfo.Length / (1024 * 1024), 3), //Получаем размер в МБ,
                                FileWeight = Math.Round((double)fileInfo.Length / 1024, 1), //Получаем размер в КБ,
                            };
                            Files.Add(fileData);
                            LoadFilesInPrivateList();
                        }
                    }
                }
                ,
                FileAction.Remove => () =>
                {
                    if (SelectedFiles.Count > 1)
                    {
                        if (MessageBox.Show($"Вы точно хотите убрать {SelectedFiles.Count} файлов?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            var filesToRemove = SelectedFiles.ToList();
                            foreach (var file in filesToRemove)
                            {
                                RemoveItemInFiles(file);
                            }
                        }
                    }
                    else
                    {
                        if (SelectedFile != null)
                        {
                            if (MessageBox.Show($"Вы точно хотите убрать файл?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                RemoveItemInFiles(SelectedFile);
                            }
                        }
                    }
                }
                ,
                FileAction.Clear => () =>
                {
                    if (Files != null && _files != null)
                    {
                        if (MessageBox.Show("Вы точно хотите очистить список?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Files.Clear();
                            _files.Clear();
                        }
                    }
                }
                ,
                FileAction.Delete => () =>
                {
                    if (SelectedFiles.Count > 1)
                    {
                        if (MessageBox.Show($"Вы точно хотите удалить {SelectedFiles.Count} файлов с устройства? Восстановить их будет нельзя!", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            var filesToRemove = SelectedFiles.ToList();
                            foreach (var file in filesToRemove)
                            {
                                if (File.Exists(file.FilePath))
                                {
                                    File.Delete(file.FilePath);
                                    RemoveItemInFiles(SelectedFile);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (SelectedFile != null)
                        {
                            if (MessageBox.Show($"Вы точно хотите удалить файл? Восстановить его будет нельзя!", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                if (File.Exists(SelectedFile.FilePath))
                                {
                                    File.Delete(SelectedFile.FilePath);
                                    RemoveItemInFiles(SelectedFile);
                                }
                            }

                        }
                    }
                }
                ,
                FileAction.MoveToTrash => () =>
                {
                    if (SelectedFiles.Count > 1)
                    {
                        if (MessageBox.Show($"Вы точно хотите переместить {SelectedFiles.Count} файлов в корзину? Восстановить их будет можно.", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            var filesToRemove = SelectedFiles.ToList();
                            foreach (var file in filesToRemove)
                            {
                                if (File.Exists(file.FilePath))
                                {
                                    FileSystem.DeleteFile(file.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                                    RemoveItemInFiles(file);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (SelectedFile != null)
                        {
                            if (MessageBox.Show($"Вы точно хотите переместить файл в корзину? Восстановить его будет можно.", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                if (File.Exists(SelectedFile.FilePath))
                                {
                                    FileSystem.DeleteFile(SelectedFile.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                                    RemoveItemInFiles(SelectedFile);
                                }
                            }
                        }
                    }
                }
                ,
                _ => () => { return; }
            };
            action.Invoke();
        }

        private void LoadFilesInPrivateList()
        {
            if (Files.Count != 0)
            {
                _files.Clear();

                foreach (var item in _files)
                {
                    _files.Add(item);
                }
            }
        }

        #endregion 

        #region Метод: Удаление файлов из списка

        private void RemoveItemInFiles(FileData fileToRemove)
        {
            _files.Remove(fileToRemove);
            Files.Remove(fileToRemove);
        }

        #endregion 

        #endregion
    }
}