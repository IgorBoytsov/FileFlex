﻿using FileFlex.Command;
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
using FileFlex.MVVM.Model.FilePropModel;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace FileFlex.MVVM.ViewModels.WindowViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Коллекции

        private List<FileData> _files = [];

        public ObservableCollection<FileData> Files { get; set; } = [];

        public ObservableCollection<FileProperties> FileProps { get; set; } = [];

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

        #region Команды

        private RelayCommand _addFilesCommand;
        public RelayCommand AddFilesCommand { get => _addFilesCommand ??= new(obj => { InteractionFiles(FileAction.Add); }); }

        private RelayCommand _clearFilesCommand;
        public RelayCommand ClearFilesCommand { get => _clearFilesCommand ??= new(obj => { InteractionFiles(FileAction.Clear); }); }

        private RelayCommand _removeFilesCommand;
        public RelayCommand RemoveFilesCommand { get => _removeFilesCommand ??= new(obj => { InteractionFiles(FileAction.Remove); }); }

        private RelayCommand _deleteFilesCommand;
        public RelayCommand DeleteFilesCommand { get => _deleteFilesCommand ??= new(obj => { InteractionFiles(FileAction.Delete); }); }

        private RelayCommand _moveFilesToTrashCommand;
        public RelayCommand MoveFilesToTrashCommand { get => _moveFilesToTrashCommand ??= new(obj => { InteractionFiles(FileAction.MoveToTrash); }); }

        #endregion             

        #region Получение и отображение информации о файле

        /*------------------------------------------------------------------------------------------------*/

        #region Свойства : выбора файла

        private FileData _selectedFile;
        public FileData SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                DetermineFileType(value);
                OnPropertyChanged();
            }
        }

        public List<FileData> SelectedFiles { get; set; } = [];

        #endregion

        #region Свойства : отображение файлов

        private string _selectedImage;
        public string SelectedImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged();
            }
        }

        private BitmapSource _currentFrame;
        public BitmapSource CurrentFrame
        {
            get => _currentFrame;
            private set
            {
                if (_currentFrame != value)
                {
                    _currentFrame = value;
                    OnPropertyChanged(nameof(CurrentFrame));
                }
            }
        }

        private BitmapImage _iconSelectedFile;
        public BitmapImage IconSelectedFile
        {
            get => _iconSelectedFile;
            set
            {
                _iconSelectedFile = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Свойство : Отображение базовой информации файла

        private FileBaseProperties _baseProperties;
        public FileBaseProperties BaseProperties
        {
            get => _baseProperties;
            set
            {
                _baseProperties = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Свойства : Visibility предпросмотра

        private TypeFile _currentDisplayFile;
        public TypeFile CurrentDisplayFile
        {
            get => _currentDisplayFile;
            set
            {
                _currentDisplayFile = value;
                OnPropertyChanged();
                UpdateVisibility();
            }
        }

        private Visibility _imageVisibility;
        public Visibility ImageVisibility
        {
            get => _imageVisibility;
            set
            {
                _imageVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _fileIconVisibility;
        public Visibility FileIconVisibility
        {
            get => _fileIconVisibility;
            set
            {
                _fileIconVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _imageGIFVisibility;
        public Visibility ImageGIFVisibility
        {
            get => _imageGIFVisibility;
            set
            {
                _imageGIFVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /*------------------------------------------------------------------------------------------------*/

        #region Метод : Обновление Visibility 

        private void UpdateVisibility()
        {
            // Отображение предпросмотра файла.
            ImageVisibility = CurrentDisplayFile == TypeFile.Image ? Visibility.Visible : Visibility.Collapsed;
            ImageGIFVisibility = CurrentDisplayFile == TypeFile.GIF ? Visibility.Visible : Visibility.Collapsed;
            FileIconVisibility = CurrentDisplayFile == TypeFile.IconFile ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Методы возоимодействиe с списком файлов : Открытие \ добавление, удаление, очистка, перемещение в карзину

        // TODO: После создание кастюмного окна с сообщениями  Сделать проверку на то, что пользователь согласен удалить файлы с устройства.

        private void InteractionFiles(FileAction fileAction)
        {
            Action action = fileAction switch
            {
                FileAction.Add => async () =>
                {
                    string[] files = _openFileDialogService.OpenDialog();
                    if (files == null) return;

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

        private void RemoveItemInFiles(FileData fileToRemove)
        {
            _files.Remove(fileToRemove);
            Files.Remove(fileToRemove);
        }

        #endregion  

        /*------------------------------------------------------------------------------------------------*/

        #region Метод : Отображение информации файла, контента файла

        private void DetermineFileType(FileData fileData)
        {
            if (fileData != null)
            {
                var renderAction = new Dictionary<string, Action>
                {
                    // Растровые изображения
                    {".jpg",  () => RenderImage(fileData) },
                    {".jpeg", () => RenderImage(fileData) },
                    {".jfif", () => RenderImage(fileData) },
                    {".jpe",  () => RenderImage(fileData) },
                    {".png",  () => RenderImage(fileData) },
                    {".ico",  () => RenderImage(fileData) },
                    {".webp", () => RenderImage(fileData) },
                    {".heic", () => RenderImage(fileData) },
                    {".gif",  () => RenderImage(fileData) },

                    // Специфические файлы
                    {".exe",  () => RenderExe(fileData) },
                };

                var propAction = new Dictionary<string, Action>
                {
                    // Растровые изображения
                    {".jpg",  () => ExtractImageProperty(fileData) },
                    {".jpeg", () => ExtractImageProperty(fileData) },
                    {".jfif", () => ExtractImageProperty(fileData) },
                    {".jpe",  () => ExtractImageProperty(fileData) },
                    {".png",  () => ExtractImageProperty(fileData) },
                    {".ico",  () => ExtractImageProperty(fileData) },
                    {".webp", () => ExtractImageProperty(fileData) },
                    {".heic", () => ExtractImageProperty(fileData) },
                    {".gif",  () => ExtractImageProperty(fileData) },

                    // Специфические файлы
                    {".exe",  () => ExtractExeProperty(fileData) },
                };

                if (renderAction.TryGetValue(fileData.FileExtension.ToLower(), out Action render)) render();
                else
                {
                    IconSelectedFile = fileData.FileIcon;
                    CurrentDisplayFile = TypeFile.IconFile;
                }

                if (propAction.TryGetValue(fileData.FileExtension.ToLower(), out Action prop)) prop();
                else
                {
                    BaseProperties = ExtractBaseFileProperty(fileData.FilePath);
                    FileProps.Clear();
                };
            }
        }

        #endregion

        #region Метод отображение : Изображений

        private void RenderImage(FileData fileData)
        {
            Action action = fileData.FileExtension.ToLower() switch
            {
                ".jpg" or ".jpeg" or ".jfif" or ".jpe" or ".png" or ".ico" or ".webp" or ".heic" => () =>
                {
                    SelectedImage = fileData.FilePath;
                    CurrentDisplayFile = TypeFile.Image;
                }
                ,
                ".gif" => () =>
                {
                    LoadGif(fileData.FilePath);
                    CurrentDisplayFile = TypeFile.GIF;
                }
                ,
                _ => () =>
                {
                    IconSelectedFile = fileData.FileIcon;
                    CurrentDisplayFile = TypeFile.IconFile;
                }
            };
            action.Invoke();
        }

        private Bitmap gifBitmap;
        private BitmapSource[] gifFrames;
        private int currentFrameIndex;
        private DispatcherTimer frameTimer;

        private void LoadGif(string filePath)
        {
            gifBitmap = new Bitmap(filePath);
            gifFrames = ExtractFrames(gifBitmap);

            frameTimer = new DispatcherTimer();
            frameTimer.Interval = TimeSpan.FromMilliseconds(100);
            frameTimer.Tick += UpdateFrame;
            frameTimer.Start();
        }

        private BitmapSource[] ExtractFrames(Bitmap gif)
        {
            int frameCount = gif.GetFrameCount(FrameDimension.Time);
            var frames = new BitmapSource[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                gif.SelectActiveFrame(FrameDimension.Time, i);
                var frame = new Bitmap(gif);
                frames[i] = Imaging.CreateBitmapSourceFromHBitmap(
                    frame.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
                frame.Dispose();
            }

            return frames;
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            CurrentFrame = gifFrames[currentFrameIndex];
            currentFrameIndex = (currentFrameIndex + 1) % gifFrames.Length;
        }

        public void Dispose()
        {
            frameTimer?.Stop();
            gifBitmap?.Dispose();
            foreach (var frame in gifFrames)
            {
                frame?.Freeze();
            }
        }

        #endregion

        #region Метод отображение : Документы

        #endregion

        #region Метод отображение : Медиафайлы

        #endregion

        #region Метод отображение : Аудиофайлы

        #endregion

        #region Метод отображение : Exe

        private void RenderExe(FileData fileData)
        {
            IconSelectedFile = fileData.FileIcon;
            CurrentDisplayFile = TypeFile.IconFile;
        }

        #endregion

        /*------------------------------------------------------------------------------------------------*/

        #region Метод : Получение базовых свойств файла

        private static FileBaseProperties ExtractBaseFileProperty(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            var directory = fileInfo.DirectoryName;
            var creationTime = fileInfo.CreationTime;
            var accessTime = fileInfo.LastAccessTime;
            var writeTime = fileInfo.LastWriteTime;

            return new FileBaseProperties()
            {
                Directory = directory,
                CreationTime = creationTime.ToString("f"),
                AccessTime = accessTime.ToString("f"),
                WriteTime = writeTime.ToString("f"),
            };
        }

        #endregion

        #region Метод получение свойства файлов : Изображений

        private async void ExtractImageProperty(FileData fileData)
        {
            var baseProp = ExtractBaseFileProperty(fileData.FilePath);
            BaseProperties = baseProp;

            var (width, height) = await ImagePropertiesHelper.WidthAndHeightAsync(fileData.FilePath, fileData.FileExtension);
            var (DpiX, DpiY) = ImagePropertiesHelper.Dpi(fileData.FilePath);       
            var pixelFormat = ImagePropertiesHelper.PixelFormat(fileData.FilePath, fileData.FileExtension);
            var author = ImagePropertiesHelper.Authors(fileData.FilePath);
            var framesCount = ImagePropertiesHelper.FramesCount(fileData.FilePath);

            FileProps.Clear();
            FileProps.Add(new FileProperties(){ PropName = "Разрешение", PropValue = $"{width} x {height}" });
            FileProps.Add(new FileProperties(){ PropName = "Точки на дюйм", PropValue = $"X: {DpiX} Y: {DpiY}" });
            FileProps.Add(new FileProperties(){ PropName = "Формат пикселя", PropValue = pixelFormat });
            FileProps.Add(new FileProperties(){ PropName = "Автор", PropValue = author });
            FileProps.Add(new FileProperties(){ PropName = "Число кадров", PropValue = framesCount.ToString() });
        }

        #endregion

        #region  Метод получение свойства файлов : Документы

        #endregion

        #region  Метод получение свойства файлов : Медиафайлы

        #endregion

        #region  Метод получение свойства файлов : Аудиофайлы 

        #endregion

        #region  Метод получение свойства файлов : Exe 

        private void ExtractExeProperty(FileData fileData)
        {
            FileProps.Clear();
            var baseProp = ExtractBaseFileProperty(fileData.FilePath);
            BaseProperties = baseProp;

            FileVersionInfo exeProp = FileVersionInfo.GetVersionInfo(fileData.FilePath);

            FileProps.Add(new FileProperties() { PropName = "Название компании",                        PropValue = exeProp.CompanyName });
            FileProps.Add(new FileProperties() { PropName = "Номер сборки файла",                       PropValue = exeProp.FileBuildPart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Подробное описание",                       PropValue = exeProp.FileDescription });
            FileProps.Add(new FileProperties() { PropName = "Мажорная часть версии файла",              PropValue = exeProp.FileMajorPart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Минорная часть версии файла",              PropValue = exeProp.FileMinorPart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Имя файла",                                PropValue = exeProp.FileName });
            FileProps.Add(new FileProperties() { PropName = "Частная часть версии файла",               PropValue = exeProp.FilePrivatePart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Полная версии файла",                      PropValue = exeProp.FileVersion });
            FileProps.Add(new FileProperties() { PropName = "Внутреннее имя файла",                     PropValue = exeProp.InternalName });
            FileProps.Add(new FileProperties() { PropName = "Отладочный файл?",                         PropValue = exeProp.IsDebug == true ? "Да" : "Нет" });
            FileProps.Add(new FileProperties() { PropName = "Исправлен файл?",                          PropValue = exeProp.IsPatched == true ? "Да" : "Нет" });
            FileProps.Add(new FileProperties() { PropName = "Предварительная версия?",                  PropValue = exeProp.IsPreRelease == true ? "Да" : "Нет" });
            FileProps.Add(new FileProperties() { PropName = "Частная сборка?",                          PropValue = exeProp.IsPrivateBuild == true ? "Да" : "Нет" });
            FileProps.Add(new FileProperties() { PropName = "Специальная сборка?",                      PropValue = exeProp.IsSpecialBuild == true ? "Да" : "Нет" });
            FileProps.Add(new FileProperties() { PropName = "Язык файла",                               PropValue = exeProp.Language });
            FileProps.Add(new FileProperties() { PropName = "Авторские права",                          PropValue = exeProp.LegalCopyright });
            FileProps.Add(new FileProperties() { PropName = "Зарегистрированные торговые марки",        PropValue = exeProp.LegalTrademarks });
            FileProps.Add(new FileProperties() { PropName = "Оригинальное имя файла",                   PropValue = exeProp.OriginalFilename });
            FileProps.Add(new FileProperties() { PropName = "Строка, описывающая частную сборку",       PropValue = exeProp.PrivateBuild });
            FileProps.Add(new FileProperties() { PropName = "Номер сборки продукта",                    PropValue = exeProp.ProductBuildPart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Мажорная часть версии продукта",           PropValue = exeProp.ProductMajorPart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Минорная часть версии продукта",           PropValue = exeProp.ProductMinorPart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Название продукта",                        PropValue = exeProp.ProductName });
            FileProps.Add(new FileProperties() { PropName = "Частная часть версии продукта",            PropValue = exeProp.ProductPrivatePart.ToString() });
            FileProps.Add(new FileProperties() { PropName = "Полная строка версии продукта",            PropValue = exeProp.ProductVersion });
            FileProps.Add(new FileProperties() { PropName = "Строка, описывающая специальную сборку",   PropValue = exeProp.SpecialBuild });
            FileProps.Add(new FileProperties() { PropName = "Комментарии",                              PropValue = exeProp.Comments });
        }

        #endregion

        /*------------------------------------------------------------------------------------------------*/

        #endregion
    }
}