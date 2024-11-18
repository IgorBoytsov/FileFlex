using FileFlex.Utils.Enums;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FileFlex.Utils.ValueConverters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TypeFile mode && parameter is string targetMode)
            {
                return mode.ToString() == targetMode ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}