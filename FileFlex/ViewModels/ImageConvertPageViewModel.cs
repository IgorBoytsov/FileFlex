    using FileFlex.Model;
    using FileFlex.Utils;
    using FileFlex.ViewModels.Commands;
    using Microsoft.Win32;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;

namespace FileFlex.ViewModels
{
    internal class ImageConvertPageViewModel : BaseViewModel
    {
        public ImageConvertPageViewModel()
        {
            SetInfoInputImage();
            DefaultComboBoxValue();

            SliderValue = 100;
        }

        #region Всплывающие подсказки

        public string ToolTipQualityLabelText { get; set; } = "Выберите подходящее качество изображения. Чем выше качество, тем больше весит файл. И наоборот, чем ниже качество, тем меньше размер файла.";

        public string ToolTipTargetFormatLabelText { get; set; } = "Формат, в который будет конвертировано изображение";

        public string ToolTipResizeLabelText { get; set; } = "Изменение размера изображение. Считается в пикселях";

        public string ToolTipColorFilterLabelText { get; set; } = "Применить цветной фильтр к изображению";

        public string ToolTipAdditionalSettingsLabelText { get; set; } = "Разнообразные доп. настройки";

        #endregion

        #region Cвойства значений для выходного изображение

        private int _sliderValue;
        public int SliderValue
        {
            get => _sliderValue;
            set
            {
                if (value > 100)
                {
                    MessageBox.Show("Максимальное значение 100");
                    SliderValue = 0;
                }
                else _sliderValue = value;

                OnPropertyChanged();
            }
        }

        private int _widthImageValue;
        public int WidthImageValue
        {
            get => _widthImageValue;
            set
            {
                _widthImageValue = value;
                OnPropertyChanged();
            }
        }

        private int _heightImageValue;
        public int HeightImageValue
        {
            get => _heightImageValue;
            set
            {
                _heightImageValue = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Cвойства ComboBox

        public List<string> ConvertFormat { get; set; } = ["Не выбран", "PNG", "JPG"];

        public List<string> ColorFilter { get; set; } = ["Без фильтра", "Ч/Б", "Инвертировать цвета", "Ретро", "Сепия",];

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
                SetInfoInputImage();
                DefaultTextBoxResolutionTextBoxValue();
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

        private string _inputTimeOfChangeSelected;
        public string InputTimeOfChange
        {
            get => _inputTimeOfChangeSelected;
            set
            {
                _inputTimeOfChangeSelected = value;
                OnPropertyChanged();
            }
        }

        private string _inputFileCreatedTimeSelected;
        public string InputFileCreatedTime
        {
            get => _inputFileCreatedTimeSelected;
            set
            {
                _inputFileCreatedTimeSelected = value;
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
                    if (SelectedFileInformation != null)
                    {
                        await PreviewRenderImageAsync(SelectedFileInformation.FileUri);
                    }
                    else return;  
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
                    SaveCurrentImage();
                });
            }
        }

        private RelayCommand _convertAndSaveImage;
        public RelayCommand ConvertAndSaveAllImageCommand
        {
            get
            {
                return _convertAndSaveImage ??= new RelayCommand(async obj =>
                {
                    await ConvertAndSaveAllImage();
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
                        FileCreatedTime = fileInfo.CreationTime.ToString("f"),
                        FileTimeOfChange = fileInfo.LastWriteTime.ToString("f"),
                        FileResolution = await ImageProcessing.GetImageResolutionAsync(item),
                    });
                }
            }
        }

        private async Task PreviewRenderImageAsync(string path)
        {
            if (path == null)
            {
                MessageBox.Show("Выберите изображение");
            }
            else
            {
                Bitmap bitmap = new Bitmap(path);

                //bitmap = await ImageProcessing.RemoveHalfPixelsAsync(bitmap);
                if (bitmap.Width != WidthImageValue && bitmap.Height != HeightImageValue)
                {
                    bitmap = await ImageProcessing.ResizeImageAsync(bitmap, WidthImageValue, HeightImageValue);
                    WidthImageValue = bitmap.Width;
                    HeightImageValue = bitmap.Height;
                }

                bitmap = await ImageProcessing.RetroFilter(bitmap);
                SaveRender = bitmap;
                OutputImageSource = await ImageProcessing.ConvertBitmapToBitmapImageAsync(bitmap);
            }
        }

        private void SetInfoInputImage()
        {
            if (_selectedFileInformation == null)
            {
                ImageSource = "Нету данных";
                InputFileNameSelected = "Нету данных";
                InputFileTypeSelected = "Нету данных";
                InputFileSizeSelected = "Нету данных";
                InputFileResolutionSelected = "Нету данных";
                InputFileCreatedTime = "Нету данных";
                InputTimeOfChange = "Нету данных";
            }
            else
            {
                ImageSource = _selectedFileInformation.FileUri;
                InputFileNameSelected = _selectedFileInformation.FileName;
                InputFileTypeSelected = _selectedFileInformation.FileType;
                InputFileSizeSelected = _selectedFileInformation.FileSize;
                InputFileResolutionSelected = _selectedFileInformation.FileResolution;
                InputFileCreatedTime = _selectedFileInformation.FileCreatedTime;
                InputTimeOfChange = _selectedFileInformation.FileCreatedTime;

            }
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

        private void SaveCurrentImage()
        {
            if (SelectedFileInformation == null)
            {
                MessageBox.Show("Вы не загрузили, либо не выбрали изображение");
            }
            else if (OutputImageSource == null)
            {
                MessageBox.Show("Нет измененного изображение");
            }
            else if (SelectedFormat == "Не выбран")
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
        }

        private async Task ConvertAndSaveAllImage()
        {
            if (SelectedFormat != "Не выбран")
            {
                foreach (var item in ListFiles)
                {
                    var result = ImageProcessing.GetImageWidthAndHeight(item.FileUri);
                    WidthImageValue = result.width;
                    HeightImageValue = result.height;

                    await PreviewRenderImageAsync(item.FileUri);

                    switch (SelectedFormat)
                    {
                        case "PNG":
                            string SaveUrlPng = @$"D:\Тест сохранение файлов\{Path.GetFileNameWithoutExtension(item.FileName)}.png";
                            ImageProcessing.SaveFileToPng(SaveRender, SaveUrlPng);
                            break;
                        case "JPG":
                            string SaveUrlJpeg = @$"D:\Тест сохранение файлов\{Path.GetFileNameWithoutExtension(item.FileName)}.jpeg";
                            ImageProcessing.SaveFileToPng(SaveRender, SaveUrlJpeg);
                            break;
                    }
                    OutputImageSource = null;
                }
            }
            else MessageBox.Show("Не выбран формат файла");
            
            
        }

        private void DefaultComboBoxValue()
        {
            SelectedFormat = ConvertFormat.First();
            SelectedColorFilter = ColorFilter.First();
            SelectedFileFilter = SortingSelection.First();
        }

        private void DefaultTextBoxResolutionTextBoxValue()
        {
            if (SelectedFileInformation != null)
            {
                var res = ImageProcessing.GetImageWidthAndHeight(SelectedFileInformation.FileUri);
                WidthImageValue = res.width;
                HeightImageValue = res.height;
            }
            else
            {
                WidthImageValue = 0;
                HeightImageValue = 0;
            }
            
        }

        #endregion
    }
}