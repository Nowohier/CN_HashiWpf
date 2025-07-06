using Hashi.Enums;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Models;
using System.Windows.Media;

namespace Hashi.Gui.Helpers
{
    public class HashiBrushResolver(IApplicationWrapper applicationWrapper) : IHashiBrushResolver
    {
        public IHashiBrush ResolveBrush(HashiColor color)
        {
            if (applicationWrapper.GetApplicationResource(color.ToString()) is SolidColorBrush brush)
            {
                return new HashiBrush(brush);
            }

            throw new ArgumentException($@"No brush found for color: {color}", nameof(color));
        }
    }
}
