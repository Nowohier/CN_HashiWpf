using System.Windows.Media;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Models;

/// <inheritdoc cref="IHashiBrush" />
/// >
public class HashiBrush : IHashiBrush
{
    public HashiBrush(SolidColorBrush brush)
    {
        Brush = brush;
    }

    public object Brush { get; }
}