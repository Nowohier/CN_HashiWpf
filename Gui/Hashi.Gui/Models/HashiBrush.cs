using Hashi.Gui.Interfaces.Models;
using System.Windows.Media;

namespace Hashi.Gui.Models;

/// <inheritdoc cref="IHashiBrush" />
/// >
public class HashiBrush(SolidColorBrush brush) : IHashiBrush
{
    public object Brush { get; } = brush;
}