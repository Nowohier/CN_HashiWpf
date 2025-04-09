using System.Globalization;
using System.Windows.Data;

namespace Hashi.Gui.Converters
{
    /// <summary>
    ///      Converts an empty string to a boolean value: True if the string is not empty, false otherwise.
    /// </summary>
    public class EmptyStringToBoolConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string stringValue) return false;
            return !string.IsNullOrEmpty(stringValue);
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
