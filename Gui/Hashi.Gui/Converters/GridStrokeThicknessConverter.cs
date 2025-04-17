using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hashi.Gui.Converters;

/// <summary>
///     A converter that returns a <see cref="Thickness" /> based on the size of the <see cref="ItemsControl" />.
///     This fixes a WPF issue that shapes are not rendered correctly when the size is too small.
/// </summary>
public class GridStrokeThicknessConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not ItemsControl itemsControl ? 1 :
            itemsControl.ActualWidth > 1000 || itemsControl.ActualHeight > 1000 ? 2 : 1;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}