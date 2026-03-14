using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Converters;

/// <summary>
///     Converts an empty string to a <see cref="Visibility" /> value: Visible if the string is not empty, Hidden otherwise.
/// </summary>
public class EmptyStringToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string stringValue)
        {
            return Visibility.Hidden;
        }

        return string.IsNullOrEmpty(stringValue) ? Visibility.Hidden : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}