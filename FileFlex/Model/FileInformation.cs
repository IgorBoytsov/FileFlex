using FontAwesome5;
using System.Windows.Media;

namespace FileFlex.Model
{
    class FileInformation
    {
        /// <summary>
        /// Путь к файлу.
        /// </summary>
        public string FileUri { get; set; }

        /// <summary>
        /// Название файла.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Тип файла. (.png, .docx, .ico и т.д)
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Разрешение файла. Устанавливать только у изображений.
        /// </summary>
        public string FileResolution { get; set; }

        /// <summary>
        /// Размер файла. Изначально измеряется в КБ.
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// Время создания файла. 
        /// </summary>
        public string FileCreatedTime { get; set; }

        /// <summary>
        /// Время изменение файла.
        /// </summary>
        public string FileTimeOfChange { get; set; }

        /// <summary>
        /// Владелец файла.
        /// </summary>
        public string FileOwner { get; set; }

        /// <summary>
        /// Иконка для файла. Использует библиотеку FontAwesome5.
        /// </summary>
        public EFontAwesomeIcon FontAwesomeIcon { get; set; }

        /// <summary>
        /// Цвет иконки из библиотеки FontAwesome5.
        /// </summary>
        public SolidColorBrush ColorIcon { get; set; }
    }
}
