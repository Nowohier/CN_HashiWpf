using Hashi.Gui.Enums;
using System.Globalization;
using System.Windows.Data;

namespace Hashi.Gui.Converters
{
    /// <summary>
    /// Converts a Tag Property of a FrameworkElement to a BridgeTypeEnum.
    /// </summary>
    public class TagToBridgeTypeEnumConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is not BridgeTypeEnum bridgeType ? BridgeTypeEnum.None : bridgeType;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
