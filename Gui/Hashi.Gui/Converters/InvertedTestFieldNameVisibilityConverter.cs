using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Providers;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Converters
{
    /// <summary>
    ///     A converter that determines the visibility of a field name based on its value.
    /// </summary>
    public class InvertedTestFieldNameVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not ISolutionProvider provider)
            {
                return Visibility.Collapsed;
            }

            return provider.Name is TestSolutionProvider.NewSolutionName ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
