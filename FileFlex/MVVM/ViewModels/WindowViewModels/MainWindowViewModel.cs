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
using FileFlex.Utils.Services.CustomWindowServices;
using FileFlex.MVVM.Model.FilePropModel;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Drawing.Imaging;
using System.Diagnostics;
using FileFlex.Utils.Settings;

namespace FileFlex.MVVM.ViewModels.WindowViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        /*--Общие константы-------------------------------------------------------------------------------*/

        #region Константы : Текст

        private const string EMPTY_PROP = "Отсутствует";

        #endregion

        /*--Общие коллекции-------------------------------------------------------------------------------*/

        #region Коллекции : Списки файлов, Свойств

        private List<FileData> _files = [];

        public ObservableCollection<FileData> Files { get; set; } = [];

        public ObservableCollection<FileProperties> FileProps { get; set; } = [];

        #endregion

        /*-Свойства---------------------------------------------------------------------------------------*/

        #region Свойство : Текущий тип отображенного файла

        private TypeFile _currentDisplayFile = TypeFile.None;
        public TypeFile CurrentDisplayFile
        {
            get => _currentDisplayFile;
            set
            {
                _currentDisplayFile = value;
                OnPropertyChanged();
            }
        }

        #endregion 

        #region Свойства : Путь сохранение файлов, путь открытие файлов

        private string _openFilePath;
        public string OpenFilePath
        {
            get => _openFilePath;
            set
            {
                _openFilePath = value;
                OnPropertyChanged();
            }
        }

        private string _saveFilePath;
        public string SaveFilePath
        {
            get => _saveFilePath;
            set
            {
                _saveFilePath = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /*--Сервисы---------------------------------------------------------------------------------------*/

        private readonly IServiceProvider _serviceProvider;

        private readonly IWindowNavigationService _windowNavigationService;
        private readonly IPageNavigationService _pageNavigationService;

        private readonly IFileDialogService _openFileDialogService;
        private readonly IFileDialogService _saveFileDialogService;

        private readonly ICustomMessageService _customMessageWindowService;

        private readonly SettingsManager _settingsManager;

        /*--Конструктор-----------------------------------------------------------------------------------*/

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var windowNavigationService = _serviceProvider.GetService<IWindowNavigationService>();
            var pageNavigationService = _serviceProvider.GetService<IPageNavigationService>();
            var fileDialogServices = _serviceProvider.GetServices<IFileDialogService>().ToList();
            var customMessageServices = _serviceProvider.GetServices<ICustomMessageService>().ToList();
            var settingsManager = _serviceProvider.GetRequiredService<SettingsManager>();

            _windowNavigationService = windowNavigationService;
            _pageNavigationService = pageNavigationService;

            _openFileDialogService = fileDialogServices[0];
            _saveFileDialogService = fileDialogServices[1];

            _customMessageWindowService = customMessageServices[0];

            _settingsManager = settingsManager;

            _settingsManager.UpdateSettings += OnSettingsUpdated;
            OnSettingsUpdated();

            EmptyFileProps();
            EmptyBaseFileProps();
        }

        /*--RelayCommands---------------------------------------------------------------------------------*/

        #region Команды

        // Взаимодействие с фалами

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

        // Взаимодействие с свойствами

        private RelayCommand _copyPropValueCommand;
        public RelayCommand CopyPropValueCommand => _copyPropValueCommand ??= new RelayCommand(CopyPropValue);

        // Переключение сетки отображение файлов

        private RelayCommand _toggleItemsPanelTemplateCommand;
        public RelayCommand ToggleItemsPanelTemplateCommand { get => _toggleItemsPanelTemplateCommand ??= new(obj => { ToggleItemsPanelTemplate(); }); }

        // Popup

        private RelayCommand _openFilterCommand;
        public RelayCommand OpenFilterCommand { get => _openFilterCommand ??= new(obj => { IsFilterPopupOpen = true; }); }

        // Открытие окон

        private RelayCommand _openConvertImageCommand;
        public RelayCommand OpenConvertImageCommand { get => _openConvertImageCommand ??= new(obj => { ConvertImageWindowOpen(); }); }

        private RelayCommand _fileViewerCommand;
        public RelayCommand FileViewerWindowCommand { get => _fileViewerCommand ??= new(obj => { FileViewerOpen(); }); }

        private RelayCommand _openSettingsWindowCommand;
        public RelayCommand OpenSettingsWindowCommand { get => _openSettingsWindowCommand ??= new(obj => { SettingsWindowOpen(); }); }

        #endregion

        /*--Установка значений по умолчанию---------------------------------------------------------------*/

        #region Метод : Установка списка с свойствами (PropName = "Отсутствует", PropValue = "Отсутствует")

        private void EmptyFileProps()
        {
            FileProps.Clear();
            FileProps.Add(new FileProperties{ PropName = EMPTY_PROP, PropValue = EMPTY_PROP });
        }

        #endregion

        #region Метод : Установка базовых свойств файла ( BaseProperties == "Отсутствует")

        private void EmptyBaseFileProps()
        {
            BaseProperties = new FileBaseProperties()
            {
                Directory = EMPTY_PROP,
                CreationTime = EMPTY_PROP,
                AccessTime = EMPTY_PROP,
                WriteTime = EMPTY_PROP,
                FileWeight = 0,
            };
        }

        #endregion 

        /*-Открытие окон----------------------------------------------------------------------------------*/

        #region Методы открытие Window : Настрйки

        private void SettingsWindowOpen()
        {
            _windowNavigationService.NavigateTo("SettingsWindow");
        }

        #endregion 

        #region Методы открытие Window : Конвертеры

        private void ConvertImageWindowOpen()
        {
            if (SelectedFiles.Count != 0)
            {              
                var transferredFiles = new List<FileData>();
                var filesNotTransferred = new List<FileData>();

                if (SelectedFiles.Count > 1)
                {
                    foreach (var file in SelectedFiles)
                    {
                        if (CheckFileFormatToImageConvert(file))
                        {
                            transferredFiles.Add(file);
                        }
                        else
                        {
                            filesNotTransferred.Add(file);
                        }
                    }
                    if (filesNotTransferred.Count > 0)
                    {
                        string fileNameList = "";
                        foreach (var file in filesNotTransferred)
                        {
                            fileNameList += ", " + file.FileName + file.FileExtension;
                        }
                        _customMessageWindowService.Show($"Следующие файлы не были переданы {fileNameList}.", "Не переданные файлы.", TypeMessage.Information);
                    }
                }
                else
                {
                    if (CheckFileFormatToImageConvert(SelectedFile))
                    {
                        transferredFiles.Add(SelectedFile);
                    }
                    else
                    {
                        _customMessageWindowService.Show($"Данный файл нельзя использовать в конвертере изображений.", "Не переданные файлы.", TypeMessage.Information);
                    }
                }

                if (transferredFiles.Count > 0)
                {
                    _windowNavigationService.NavigateTo("ConvertImageWindow", transferredFiles);
                }

                transferredFiles.Clear();
                filesNotTransferred.Clear();
            }
            else
            {
                _windowNavigationService.NavigateTo("ConvertImageWindow");
            } 
        }

        #endregion

        #region Методы открытие Window : Просмотр файлов

        private void FileViewerOpen()
        {
            if (SelectedFile != null)
            {
                if (CheckFileFormatToImageConvert(SelectedFile))
                {
                    _windowNavigationService.NavigateTo("ImageViewerWindow", SelectedFile);
                    return;
                }
            }
        }

        #endregion

        #region Методы : Проверки формата файла

        private static bool CheckFileFormatToImageConvert(FileData files)
        {
            if (files.FileExtension == ".jpg" ||
                files.FileExtension == ".jpeg" ||
                files.FileExtension == ".jfif" ||
                files.FileExtension == ".jpe" ||
                files.FileExtension == ".png" ||
                files.FileExtension == ".ico" ||
                files.FileExtension == ".webp" ||
                files.FileExtension == ".heic" ||
                files.FileExtension == ".gif")
                return true;
            else
                return false;
        }

        #endregion

        /*-Обновление \ Получение значение из файла с настройками-----------------------------------------*/

        #region Метод : Обновление значений из файла настроек

        private void OnSettingsUpdated()
        {
            OpenFilePath = _settingsManager.OpenFilePath;
            SaveFilePath = _settingsManager.SaveFilePath;
        }

        #endregion

        /*-Взаимодействие с файлами из списка-------------------------------------------------------------*/

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

        #region Методы возоимодействиe с списком файлов : Открытие \ добавление, удаление, очистка, перемещение в карзину

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
                        if (_customMessageWindowService.Show($"Вы точно хотите убрать {SelectedFiles.Count} файлов?", "Предупреждение", TypeMessage.Warning))
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
                            if (_customMessageWindowService.Show($"Вы точно хотите убрать файл?", "Предупреждение", TypeMessage.Warning))
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
                        if (_customMessageWindowService.Show("Вы точно хотите очистить список?", "Предупреждение", TypeMessage.Warning))
                        {
                            Files.Clear();
                            _files.Clear();
                            EmptyFileProps();
                            EmptyBaseFileProps();
                            CurrentDisplayFile = TypeFile.None;
                        }
                    }
                }
                ,
                FileAction.Delete => () =>
                {
                    if (SelectedFiles.Count > 1)
                    {
                        if (_customMessageWindowService.Show($"Вы точно хотите удалить {SelectedFiles.Count} файлов с устройства? Восстановить их будет нельзя!", "Предупреждение", TypeMessage.Warning))
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
                            if (_customMessageWindowService.Show($"Вы точно хотите удалить файл? Восстановить его будет нельзя!", "Предупреждение", TypeMessage.Warning))
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
                        if (_customMessageWindowService.Show($"Вы точно хотите переместить {SelectedFiles.Count} файлов в корзину? Восстановить их будет можно.", "Предупреждение", TypeMessage.Warning))
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
                            if (_customMessageWindowService.Show($"Вы точно хотите переместить файл в корзину? Восстановить его будет можно.", "Предупреждение", TypeMessage.Warning))
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

        /*-Отображение контента файлов--------------------------------------------------------------------*/

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
                    EmptyFileProps();
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

        /*-Отображение свойств файлов---------------------------------------------------------------------*/

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

        #region Метод : Получение базовых свойств файла

        private static FileBaseProperties ExtractBaseFileProperty(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            var directory = fileInfo.DirectoryName;
            var creationTime = fileInfo.CreationTime;
            var accessTime = fileInfo.LastAccessTime;
            var writeTime = fileInfo.LastWriteTime;
            var FileWeight = fileInfo.Length;

            return new FileBaseProperties()
            {
                Directory = directory,
                CreationTime = creationTime.ToString("f"),
                AccessTime = accessTime.ToString("f"),
                WriteTime = writeTime.ToString("f"),
                //FileWeight = Math.Round((double)fileInfo.Length / (1024 * 1024), 3), //Получаем размер в МБ,
                FileWeight = Math.Round((double)FileWeight / 1024, 1), //Получаем размер в КБ,
            };
        }

        #endregion

        #region Метод получение свойства файлов : Изображений

        private async void ExtractImageProperty(FileData fileData)
        {
            FileProps.Clear();

            var baseProp = ExtractBaseFileProperty(fileData.FilePath);
            BaseProperties = baseProp;

            var (width, height) = await ImagePropertiesHelper.WidthAndHeightAsync(fileData.FilePath, fileData.FileExtension);
            var (DpiX, DpiY) = ImagePropertiesHelper.Dpi(fileData.FilePath);       
            var pixelFormat = ImagePropertiesHelper.PixelFormat(fileData.FilePath, fileData.FileExtension);
            var author = ImagePropertiesHelper.Authors(fileData.FilePath);
            var framesCount = ImagePropertiesHelper.FramesCount(fileData.FilePath);

            var propImage = new Dictionary<string, string>
            {
                { "Разрешение",       $"{width} x {height}" },
                { "Точки на дюйм",    $"X: {DpiX} Y: {DpiY}" },
                { "Формат пикселя",   pixelFormat },
                { "Автор",            author },
                { "Число кадров",     framesCount.ToString() },
            };

            foreach (var prop in propImage)
            {
                FileProps.Add(new FileProperties { PropName = prop.Key, PropValue = prop.Value });
            }
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

            FileVersionInfo fileVersionExeProp = FileVersionInfo.GetVersionInfo(fileData.FilePath);
       
            var propExe = new Dictionary<string, string>
            {
                { "Название компании",                      fileVersionExeProp.CompanyName },
                { "Название продукта",                      fileVersionExeProp.ProductName },
                { "Имя файла",                              fileVersionExeProp.FileName },
                { "Подробное описание",                     fileVersionExeProp.FileDescription },
                { "Полная версии файла",                    fileVersionExeProp.FileVersion },
                { "Язык файла",                             fileVersionExeProp.Language },
                { "Внутреннее имя файла",                   fileVersionExeProp.InternalName },
                { "Номер сборки файла",                     fileVersionExeProp.FileBuildPart.ToString() },
                { "Номер сборки продукта",                  fileVersionExeProp.ProductBuildPart.ToString() },
                { "Мажорная часть версии файла",            fileVersionExeProp.FileMajorPart.ToString() },
                { "Минорная часть версии файла",            fileVersionExeProp.FileMinorPart.ToString() },
                { "Частная часть версии файла",             fileVersionExeProp.FilePrivatePart.ToString() },
                { "Мажорная часть версии продукта",         fileVersionExeProp.ProductMajorPart.ToString() },
                { "Минорная часть версии продукта",         fileVersionExeProp.ProductMinorPart.ToString() },
                { "Частная часть версии продукта",          fileVersionExeProp.ProductPrivatePart.ToString() },
                { "Отладочный файл?",                       fileVersionExeProp.IsDebug ? "Да" : "Нет" },
                { "Исправлен файл?",                        fileVersionExeProp.IsPatched ? "Да" : "Нет" },
                { "Предварительная версия?",                fileVersionExeProp.IsPreRelease ? "Да" : "Нет" },
                { "Частная сборка?",                        fileVersionExeProp.IsPrivateBuild ? "Да" : "Нет" },
                { "Специальная сборка?",                    fileVersionExeProp.IsSpecialBuild ? "Да" : "Нет" },
                { "Авторские права",                        fileVersionExeProp.LegalCopyright },
                { "Зарегистрированные торговые марки",      fileVersionExeProp.LegalTrademarks },
                { "Оригинальное имя файла",                 fileVersionExeProp.OriginalFilename },
                { "Строка, описывающая частную сборку",     fileVersionExeProp.PrivateBuild },
                { "Полная строка версии продукта",          fileVersionExeProp.ProductVersion },
                { "Строка, описывающая специальную сборку", fileVersionExeProp.SpecialBuild },
                { "Комментарии",                            fileVersionExeProp.Comments }
            };

            foreach (var prop in propExe)
            {
                FileProps.Add(new FileProperties { PropName = prop.Key, PropValue = prop.Value });
            }
        }

        #endregion

        /*-Взаимодействие с свойствами файлов-------------------------------------------------------------*/

        #region Метод : Копирования значений у свойства

        private void CopyPropValue(object value)
        {
            if (value is FileProperties fileProp)
                Clipboard.SetText($"Свойство: {fileProp.PropName} \n Значение: {fileProp.PropValue}");
        }

        #endregion

        /*-Выбор ItemsPanelTemplate для отображения-------------------------------------------------------*/

        #region Свойство : Хранение значение для триггера 

        private bool _isWrapView = false;
        public bool IsWrapView
        {
            get => _isWrapView;
            set
            {
                _isWrapView = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Метод : Выбор ItemsPanelTemplate

        private void ToggleItemsPanelTemplate()
        {
            IsWrapView = !IsWrapView;
        }

        #endregion

        /*-Popup------------------------------------------------------------------------------------------*/

        #region Свойство : IsFilterPopupOpen - фильтрация файлов

        private bool _isFilterPopupOpen = false;
        public bool IsFilterPopupOpen
        {
            get => _isFilterPopupOpen;
            set
            {
                _isFilterPopupOpen = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}