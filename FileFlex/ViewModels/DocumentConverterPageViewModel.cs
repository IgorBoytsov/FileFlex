using FileFlex.Model;
using FontAwesome5;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FileFlex.ViewModels
{
    internal class DocumentConverterPageViewModel : BaseViewModel
    {

        public DocumentConverterPageViewModel() 
        {
            DefoultComboBoxValue();
        }

        #region Вспоывающие подсказки

        public string ToolTipQualityLabelText { get; set; } = "Выберите подходящее качество изображения. Чем выше качество, тем больше весит файл. И наоборот, чем ниже качество, тем меньше размер файла.";

        public string ToolTipAdditionalSettingsLabelText { get; set; } = "Разнообразные доп. настройки";

        #endregion

        #region Свойства

        public ObservableCollection<FileInformation> FileInformation { get; set; } = new ObservableCollection<FileInformation>()
        {
            new FileInformation(){ FileName = "Чистая архитектура. Искусство разработки программного обеспечения.", FileUri = @"C:\Users\light\Downloads\Чистая архитектура. Искусство разработки программного обеспечения. 2018 (1).docx", FileSize = "34", FileType = "PDF", FontAwesomeIcon = EFontAwesomeIcon.Regular_FileExcel, ColorIcon = Color.Green},
            new FileInformation(){ FileName = "Залупа", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "567", FileType = "xlsx", FontAwesomeIcon = EFontAwesomeIcon.Regular_FileExcel, ColorIcon = Color.Blue},
            new FileInformation(){ FileName = "Херня", FileUri = @"C:\Users\light\Downloads\Чистая архитектура. Искусство разработки программного обеспечения. 2018.docx", FileSize = "34", FileType = "Docx", FontAwesomeIcon = EFontAwesomeIcon.Regular_FileWord, ColorIcon = Color.Blue},
            new FileInformation(){ FileName = "Гном", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "6", FileType = "PDF"},
            new FileInformation(){ FileName = "Книга", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "34", FileType = "xls"},
            new FileInformation(){ FileName = "Залупа 3д", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "567567", FileType = "PDF"},
            new FileInformation(){ FileName = "Властелин колец", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "567", FileType = "Fb2"},
            new FileInformation(){ FileName = "Гном", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "576", FileType = "PDF"},
            new FileInformation(){ FileName = "Гарри Поттер", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "375676574", FileType = "Word"},
            new FileInformation(){ FileName = "Челюсти 3д", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "567", FileType = "PDF"},
            new FileInformation(){ FileName = "ЧВК Вагнер", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "576", FileType = "PDF"},
            new FileInformation(){ FileName = "Путин", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "34", FileType = "xls"},
            new FileInformation(){ FileName = "Маша и медведь", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "34", FileType = "Word"},
            new FileInformation(){ FileName = "Война и Мир", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "56756", FileType = "PDF"},
            new FileInformation(){ FileName = "Под куполом", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "34", FileType = "xls"},
            new FileInformation(){ FileName = "Гном", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "567567", FileType = "PDF"},
            new FileInformation(){ FileName = "Воинственный бох асура", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "567567", FileType = "PDF"},
            new FileInformation(){ FileName = "Извевающийся дракон", FileUri = @"C:\Users\light\Downloads\Тестовые изображение\20240406102718_1.jpg", FileSize = "34", FileType = "xls"},
        };

        public ObservableCollection<string> ConvertFormat { get; set; } = ["Не выбран", "docx", "PDF", "xlsx", "fb2"];

        public EFontAwesomeIcon FontAwesomeIcon { get; set; }

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

        #endregion

        #region Команды

        #endregion

        #region Методы

        private void DefoultComboBoxValue()
        {
            SelectedFormat = ConvertFormat.First();
        }

        #endregion
    }
}
