using Hashi.Gui.Interfaces.Models;

namespace Hashi.Generator.Simulation;

/// <summary>
///     A lightweight simulation implementation of <see cref="IHashiBrush" />.
/// </summary>
internal class SimulationHashiBrush : IHashiBrush
{
    /// <inheritdoc />
    public object Brush => new object();
}
