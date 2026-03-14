using Hashi.Generator.Interfaces.Providers;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides initialization operations for the island collection.
/// </summary>
public interface IIslandInitializationProvider
{
    /// <summary>
    ///     Rebuilds the island collection with a new solution container.
    /// </summary>
    /// <param name="solutionProvider">The solution provider.</param>
    void InitializeNewSolution(ISolutionProvider solutionProvider);

    /// <summary>
    ///     Rebuilds the island collection with a new solution container and sets the bridges.
    /// </summary>
    /// <param name="solutionProvider">The solution provider.</param>
    void InitializeNewSolutionAndSetBridges(ISolutionProvider solutionProvider);
}
