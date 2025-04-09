using Hashi.Gui.Helpers;
using System.Windows.Data;

namespace Hashi.Gui.Converters
{
    /// <summary>
    /// Converts a string value to a SolidColorBrush value.
    /// </summary>
    public class MessageBackgroundBrushConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not string stringValue) return HashiColorHelper.MenuBackgroundBrush;
            return string.IsNullOrEmpty(stringValue) ? HashiColorHelper.MenuBackgroundBrush : HashiColorHelper.GreenIslandBrush;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
