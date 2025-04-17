using System.Windows.Media;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Models;

/// <inheritdoc cref="IHashiBrush" />
/// >
public class HashiBrush(SolidColorBrush brush) : IHashiBrush
{
    public object Brush { get; } = brush;
}