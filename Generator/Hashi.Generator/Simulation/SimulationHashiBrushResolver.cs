using Hashi.Enums;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Generator.Simulation;

/// <summary>
///     A lightweight simulation implementation of <see cref="IHashiBrushResolver" /> used for rule-based solvability
///     validation.
/// </summary>
internal class SimulationHashiBrushResolver : IHashiBrushResolver
{
    /// <inheritdoc />
    public IHashiBrush ResolveBrush(HashiColor color)
    {
        return new SimulationHashiBrush();
    }
}
