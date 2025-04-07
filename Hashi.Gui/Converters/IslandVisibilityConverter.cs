using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Converters;

/// <summary>
///     Converts the visibility of an island depending on the number of connections.
/// </summary>
public class IslandVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not int islandConnections || islandConnections == 0
            ? Visibility.Hidden
            : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}