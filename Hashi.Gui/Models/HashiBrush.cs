using Hashi.Gui.Interfaces.Models;
using System.Windows.Media;

namespace Hashi.Gui.Models
{
    /// <inheritdoc cref="IHashiBrush"/>>
    public class HashiBrush : IHashiBrush
    {

        public HashiBrush(SolidColorBrush brush)
        {
            Brush = brush;
        }

        public object Brush { get; }
    }
}
