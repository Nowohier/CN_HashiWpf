using System.Globalization;
using System.Windows.Data;

namespace Hashi.Gui.Converters;

/// <summary>
///     A converter that is used to set the maximum width of the message text in the UI.
/// </summary>
public class MaxMessageTextWidthConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double val) return 0;

        return val - 30;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}