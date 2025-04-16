using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Converters
{
    /// <summary>
    ///      Converts a boolean value to a <see cref="Visibility" /> value, inverting the logic.
    /// </summary>
    public class InvertedBoolToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Visible;
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
