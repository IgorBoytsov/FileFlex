using System.Globalization;
using System.Windows.Data;

namespace FileFlex.Utils.ValueConverters
{
    class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long fileSize)
            {
                // Единицы измерения
                string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
                double len = fileSize;
                int order = 0;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len /= 1024;
                }

                return $"{len:0.##} {sizes[order]}";
            }

            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
