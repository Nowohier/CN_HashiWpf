namespace Hashi.LinearSolver.Interfaces.Models;

/// <summary>
///     Interface for a bridge pair.
/// </summary>
public interface IBridgePair
{
    /// <summary>
    ///     The first bridge.
    /// </summary>
    int[] Bridge1 { get; }

    /// <summary>
    ///     The second bridge.
    /// </summary>
    int[] Bridge2 { get; }
}