using FileFlex.Model;
using FileFlex.ViewModels.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFlex.ViewModels
{
    internal class ImageConvertPageViewModel : BaseViewModel
    {
        
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

        #endregion

        #region Переменные

        OpenFileDialog openFileDialog = new();

        FileInfo fileInfo = null;

        #endregion

        public ImageConvertPageViewModel() 
        {
           
        }
        #region Команды

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
                    });
                }
            }
        }

        #endregion
    }
}
