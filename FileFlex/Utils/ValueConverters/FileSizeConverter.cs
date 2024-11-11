using System.Globalization;
using System.Windows.Data;

namespace FileFlex.Utils.ValueConverters
{
    class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double sizeInKb)
            {
                double sizeInBytes = sizeInKb * 1024;

                if (sizeInBytes < 1024)
                    return "байт";
                else if (sizeInKb < 1024)
                    return "Кб";
                else if (sizeInKb < 1024 * 1024)
                    return "Мб";
                else
                    return "Гб";
            }

            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
