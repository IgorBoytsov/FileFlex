using FileFlex.Model;
using FileFlex.ViewModels.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileFlex.ViewModels
{
    internal class ImageConvertPageViewModel : BaseViewModel
    {
        public ImageConvertPageViewModel()
        {

        }

        #region Свойства с текстом для всплывающих подсказок

        public string ToolTipQualityLabelText { get; set; } = "Выберите подходящее качество изображения. Чем выше качество, тем больше весит файл. И наоборот, чем ниже качество, тем меньше размер файла.";

        public string ToolTipTargetFormatLabelText { get; set; } = "Формат, в который будет конвертированно изображение";

        public string ToolTipResizeLabelText { get; set; } = "Изменение размера изображение. Считается в пикселях";

        public string ToolTipCallorFilterLabelText { get; set; } = "Применить цветной фильтр к изображению";

        public string ToolTipAdditionalSettingsLabelText { get; set; } = "Разнообразные доп. настройки";

        #endregion

        #region Свойства для ComboBox

        public List<string> ConvertFormat { get; set; } = ["PNG", "JPG", "ICO", "BMP", "EPS", "HDR/EXR", "TGA", "TIFF", "WBMP", "WebP",];

        public List<string> ColorFilter { get; set; } = ["Цветное", "Гридиент серого", "Монохромное", "Инвертировать цвета", "Ретро", "Сепия",];

        public ObservableCollection<string> SortingSelection { get; set; } = ["Выбрать все PNG", "Выбрать все JPG",];

        #endregion

        #region Свойства

        public ObservableCollection<FileInformation> ListFiles { get; set; } = new();

        private FileInformation _selectedFileInformation;
        public FileInformation SelectedFileInformation
        {
            get => _selectedFileInformation;
            set
            {
                _selectedFileInformation = value;
                OnPropertyChanged();
                SetInfoImage();
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

        #endregion

        #region Свойства для блока с информацией о изображение

        private string _fileNameSelected;
        public string FileNameSelected
        {
            get => _fileNameSelected;
            set
            {
                _fileNameSelected = value;
                OnPropertyChanged();
            }
        }

        private string _fileTypeSelected;
        public string FileTypeSelected
        {
            get => _fileTypeSelected;
            set
            {
                _fileTypeSelected = value;
                OnPropertyChanged();
            }
        }

        private string _fileSizeSelected;
        public string FileSizeSelected
        {
            get => _fileSizeSelected;
            set
            {
                _fileSizeSelected = value;
                OnPropertyChanged();
            }
        }

        private string _fileResolutionSelected;
        public string FileResolutionSelected
        {
            get => _fileResolutionSelected;
            set
            {
                _fileResolutionSelected = value;
                OnPropertyChanged();
            }
        }
        #endregion



        #region Переменные

        OpenFileDialog openFileDialog = new();

        FileInfo fileInfo = null;

        #endregion
 
        #region Команды

        private RelayCommand _deliteFileCommand;
        public RelayCommand DeliteFileCommand
        {
            get
            {
                return _deliteFileCommand ?? (_deliteFileCommand = new RelayCommand(obj =>
                {
                    DelitFiles();
                }));
            }
        }

        private RelayCommand _selectFileCommand;
        public RelayCommand SelectFileCommand
        {
            get
            {
                return _selectFileCommand ?? (_selectFileCommand = new RelayCommand(obj =>
                {
                    GetInfoFListFile();
                }));
            }
        }

        #endregion

        #region Методы 

        private void GetInfoFListFile()
        {
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Изображения (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.InitialDirectory = @"D:\Steam\userdata\189964443\760\remote\381210\screenshots";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var item in openFileDialog.FileNames)
                {
                    fileInfo = new FileInfo(item);
                    ListFiles.Add(new FileInformation
                    {
                        FileUri = item,
                        FileName = fileInfo.Name,
                        FileType = fileInfo.Extension,
                        FileSize = (fileInfo.Length / 1024).ToString("N2") + " Кб",
                        FileCreatedTieme = fileInfo.CreationTime.ToString(),
                        FileTimeOfChange = fileInfo.LastWriteTime.ToString(),
                        FileResolution = GetImageResolution(item),
                        NumberFiles = ListFiles.Count,
                    });
                }
            }
        }

        private string GetImageResolution(string url)
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
        }

        private void DelitFiles()
        {
            if (MessageBox.Show("Вы уверены что хотите все удалить?", "Точно?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ListFiles.Clear();
            }
            else return; 
        }

        private void SetInfoImage()
        {
            if (_selectedFileInformation == null)
            {
                ImageSource = null;
                FileNameSelected = "Нету данных";
                FileTypeSelected = "Нету данных";
                FileSizeSelected = "Нету данных";
                FileResolutionSelected = "Нету данных";
            }
            else
            {
                ImageSource = _selectedFileInformation.FileUri;
                FileNameSelected = Path.GetFileNameWithoutExtension(_selectedFileInformation.FileName);
                FileTypeSelected = _selectedFileInformation.FileType;
                FileSizeSelected = _selectedFileInformation.FileSize;
                FileResolutionSelected = _selectedFileInformation.FileResolution;
            }
        }
        #endregion
    }
}
