using FileFlex.Model;
using FileFlex.ViewModels.Commands;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using System.Windows;
using FileFlex.Utils;
using System.Windows.Media.Imaging;

namespace FileFlex.ViewModels
{
    internal class ImageConvertPageViewModel : BaseViewModel
    {
        public ImageConvertPageViewModel()
        {
            SetInfoInputlImage();
            SelectedFormat = ConvertFormat.First();
            SelectedColorFilter = ColorFilter.First();
            SelectedFileFilter = SortingSelection.First();
        }

        #region Свойства с текстом для всплывающих подсказок

        public string ToolTipQualityLabelText { get; set; } = "Выберите подходящее качество изображения. Чем выше качество, тем больше весит файл. И наоборот, чем ниже качество, тем меньше размер файла.";

        public string ToolTipTargetFormatLabelText { get; set; } = "Формат, в который будет конвертировано изображение";

        public string ToolTipResizeLabelText { get; set; } = "Изменение размера изображение. Считается в пикселях";

        public string ToolTipCallorFilterLabelText { get; set; } = "Применить цветной фильтр к изображению";

        public string ToolTipAdditionalSettingsLabelText { get; set; } = "Разнообразные доп. настройки";

        #endregion

        #region Cвойства ComboBox

        public List<string> ConvertFormat { get; set; } = ["Не выбран", "PNG", "JPG"];

        public List<string> ColorFilter { get; set; } = ["Без фильтра", "Цветное", "Гридиент серого", "Монохромное", "Инвертировать цвета", "Ретро", "Сепия",];

        public ObservableCollection<string> SortingSelection { get; set; } = ["Все файлы", "Выбрать все PNG", "Выбрать все JPG",];

        private string _selectedFormat;
        public string SelectedFormat
        {
            get => _selectedFormat;
            set
            {
                _selectedFormat = value;
                OnPropertyChanged();
            }
        }

        private string _selectedColorFilter;
        public string SelectedColorFilter
        {
            get => _selectedColorFilter;
            set
            {
                _selectedColorFilter = value;
                OnPropertyChanged();
            }
        }

        private string _slectedFileFilter;
        public string SelectedFileFilter
        {
            get => _slectedFileFilter;
            set
            {
                _slectedFileFilter = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Свойства для отображение файлов
       
        public ObservableCollection<FileInformation> ListFiles { get; set; } = [];

        private FileInformation _selectedFileInformation;
        public FileInformation SelectedFileInformation
        {
            get => _selectedFileInformation;
            set
            {
                _selectedFileInformation = value;
                OnPropertyChanged();
                SetInfoInputlImage();
            }
        }

        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _outputImageSource;
        public BitmapImage OutputImageSource
        {
            get => _outputImageSource;
            set
            {
                _outputImageSource = value;
                OnPropertyChanged();
            }
        }

        private Bitmap _saveRender;
        public Bitmap SaveRender 
        {
            get => _saveRender;
            set
            {
                _saveRender = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Свойства входного изображение для блока с информацией

        private string _inputfileNameSelected;
        public string InputFileNameSelected
        {
            get => _inputfileNameSelected;
            set
            {
                _inputfileNameSelected = value;
                OnPropertyChanged();
            }
        }

        private string _inputfileTypeSelected;
        public string InputFileTypeSelected
        {
            get => _inputfileTypeSelected;
            set
            {
                _inputfileTypeSelected = value;
                OnPropertyChanged();
            }
        }

        private string _inputfileSizeSelected;
        public string InputFileSizeSelected
        {
            get => _inputfileSizeSelected;
            set
            {
                _inputfileSizeSelected = value;
                OnPropertyChanged();
            }
        }

        private string _inputfileResolutionSelected;
        public string InputFileResolutionSelected
        {
            get => _inputfileResolutionSelected;
            set
            {
                _inputfileResolutionSelected = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Переменные

        OpenFileDialog openFileDialog = new();
        SaveFileDialog saveFileDialog = new();

        #endregion
 
        #region Команды

        private RelayCommand _deleteFileCommand;
        public RelayCommand DeleteFileCommand
        {
            get
            {
                return _deleteFileCommand ?? (_deleteFileCommand = new RelayCommand(obj =>
                {
                    DeleteFiles();
                }));
            }
        }

        private RelayCommand _сlearOutputImageCommand;
        public RelayCommand ClearOutputImageCommand
        {
            get
            {
                return _сlearOutputImageCommand ?? (_сlearOutputImageCommand = new RelayCommand(obj =>
                {
                    OutputImageSource = null;
                }));
            }
        }

        private RelayCommand _selectFileCommand;
        public RelayCommand SelectFileCommand
        {
            get
            {
                return _selectFileCommand ??= new RelayCommand(async obj =>
                {
                   await GetInfoListFileAsync();
                });
            }
        }

        private RelayCommand _previewRenderImageCommand;
        public RelayCommand PreviewRenderImageCommand
        {
            get
            {
                return _previewRenderImageCommand ??= new RelayCommand(async obj =>
                {
                    if (SelectedFileInformation == null)
                    {
                        MessageBox.Show("Выберите изображение");
                    }
                    else
                    {
                        Bitmap bitmap = new Bitmap(SelectedFileInformation.FileUri);

                        BitmapImage renderImage = new BitmapImage();

                        bitmap = await ImageProcessing.RemoveHalfPixelsAsync(bitmap);
                        SaveRender = bitmap;
                        OutputImageSource = renderImage = await ImageProcessing.RenderImageAsync(bitmap);
                    }                                           
                });
            }
        }

        private RelayCommand _saveCurrentFileCommand;
        public RelayCommand SaveCurrentFileCommand
        {
            get
            {
                return _saveCurrentFileCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedFileInformation == null)
                    {
                        MessageBox.Show("Вы не загрузили, либо не выбрали изображение");
                    }
                    else if (OutputImageSource == null)
                    {
                        MessageBox.Show("Нет измененного изображение");
                    }
                    else if (SelectedFormat == null || SelectedFormat == "Не выбран")
                    {
                        MessageBox.Show("Не выбран формат файла");
                    }
                    else
                    {
                        switch (SelectedFormat)
                        {
                            case "PNG":
                                string SaveUrlPng = @$"D:\Тест сохранение файлов\{Path.GetFileNameWithoutExtension(SelectedFileInformation.FileName)}.png";
                                ImageProcessing.SaveFileToPng(SaveRender, SaveUrlPng);
                                break;
                            case "JPG":
                                string SaveUrlJpeg = @$"D:\Тест сохранение файлов\{Path.GetFileNameWithoutExtension(SelectedFileInformation.FileName)}.jpeg";
                                ImageProcessing.SaveFileToPng(SaveRender, SaveUrlJpeg);
                                break;
                        }
                    }
                });
            }
        }

        #endregion

        #region Методы 

        private async Task GetInfoListFileAsync()
        {
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Изображения (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.InitialDirectory = @"D:\Steam\userdata\189964443\760\remote\381210\screenshots";

            if (openFileDialog.ShowDialog() == true)
            {
               foreach (var item in openFileDialog.FileNames)
                {
                    FileInfo fileInfo = new FileInfo(item);
                    ListFiles.Add(new FileInformation
                    {
                        FileUri = item,
                        FileName = fileInfo.Name,
                        FileType = fileInfo.Extension,
                        FileSize = (fileInfo.Length / 1024).ToString("N2") + " Кб",
                        FileCreatedTieme = fileInfo.CreationTime.ToString(),
                        FileTimeOfChange = fileInfo.LastWriteTime.ToString(),
                        FileResolution = await GetImageResolutionAsync(item),
                    });
                }
            }
        }

        private Task<string> GetImageResolutionAsync(string url)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var image = Image.FromFile(url))
                    {
                        string width = image.Width.ToString();
                        string height = image.Height.ToString();

                        return width + " x " + height;
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            });        
        }

        private void DeleteFiles()
        {
            if (ListFiles.Count == 0)
            {
                MessageBox.Show("Файлов и так нету, удалять нечего");
            }
            else if (MessageBox.Show("Вы уверены что хотите все удалить?", "Предупреждение :", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {  
                ListFiles.Clear();
            }
            else return; 
        }

        private void SetInfoInputlImage()
        {
            if (_selectedFileInformation == null)
            {
                ImageSource = "Нету данных";
                InputFileNameSelected = "Нету данных";
                InputFileTypeSelected = "Нету данных";
                InputFileSizeSelected = "Нету данных";
                InputFileResolutionSelected = "Нету данных";
            }
            else
            {
                ImageSource = _selectedFileInformation.FileUri;
                InputFileNameSelected = _selectedFileInformation.FileName;
                InputFileTypeSelected = _selectedFileInformation.FileType;
                InputFileSizeSelected = _selectedFileInformation.FileSize;
                InputFileResolutionSelected = _selectedFileInformation.FileResolution;
            }
        }

        #endregion
    }
}