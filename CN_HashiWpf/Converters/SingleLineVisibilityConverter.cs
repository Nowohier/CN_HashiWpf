using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CNHashiWpf.Converters
{
    public class SingleLineVisibilityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not List<Point> list)
            {
                return Visibility.Collapsed;
            }

            return list.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
