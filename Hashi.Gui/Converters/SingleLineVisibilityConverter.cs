using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Converters;

/// <summary>
///     Converts a list of points to a visibility value based on the number of points.
/// </summary>
public class SingleLineVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not List<IHashiPoint> list) return Visibility.Hidden;

        return list.Count == 1 ? Visibility.Visible : Visibility.Hidden;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}