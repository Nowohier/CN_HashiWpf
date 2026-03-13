using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Converters;

/// <summary>
///     Converts a boolean value to a <see cref="Visibility" /> value.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value == null)
        {
            return Visibility.Collapsed;
        }

        return (bool)value ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        throw new NotImplementedException();
    }
}