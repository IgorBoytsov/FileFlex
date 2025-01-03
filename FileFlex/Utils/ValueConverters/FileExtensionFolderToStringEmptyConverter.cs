using System.Globalization;
using System.Windows.Data;

namespace FileFlex.Utils.ValueConverters
{
    class FileExtensionFolderToStringEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string FileExtension)
            {
                if (FileExtension == "Folder")
                {
                    return "";
                }
                else
                {
                    return FileExtension;
                }
            }
            return "N/A FileExtension";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
