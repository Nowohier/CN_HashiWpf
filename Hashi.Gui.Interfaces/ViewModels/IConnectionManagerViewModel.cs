using Hashi.Generator.Interfaces.Models;
using Hashi.Gui.Interfaces.Models;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Manages the connections between islands.
/// </summary>
public interface IConnectionManagerViewModel
{
    /// <summary>
    ///     Gets the islands.
    /// </summary>
    ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; }

    /// <summary>
    /// Determines if the rules are being applied.
    /// </summary>
    bool AreRulesBeingApplied { get; set; }

    /// <summary>
    ///   Gets the rule message.
    /// </summary>
    string RuleMessage { get; set; }

    /// <summary>
    ///    Gets or sets the solution container.
    /// </summary>
    ISolutionContainer? Solution { get; }

    /// <summary>
    ///    Rebuilds the island collection with a new solution container.
    /// </summary>
    /// <param name="solutionContainer"></param>
    void InitializeNewSolution(ISolutionContainer solutionContainer);

    /// <summary>
    ///     Adds a connection between two islands.
    /// </summary>
    /// <param name="sourceIsland">The source island.</param>
    /// <param name="targetIsland">The target island.</param>
    /// <param name="isHint">Determines if the connection is a hint.</param>
    void AddConnection(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland, bool isHint = false);

    /// <summary>
    ///     Removes a connection between two islands.
    /// </summary>
    /// <param name="sourceIsland">The source island.</param>
    /// <param name="targetIsland">The target island.</param>
    void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland);

    /// <summary>
    ///     Highlights the path to the target island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    void HighlightPathToTargetIsland(IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Gets all islands involved in a connection between two islands.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>an IEnumerable of islands.</returns>
    IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///    Resets the hint connections.
    /// </summary>
    void ResetAllHintConnections();

    /// <summary>
    ///     Removes all highlights from the islands.
    /// </summary>
    void RemoveAllHighlights();

    /// <summary>
    ///     Removes all potential island coordinates.
    /// </summary>
    void ClearTemporaryDropTargets();

    /// <summary>
    ///    Refreshes the island colors.
    /// </summary>
    void RefreshIslandColors();

    /// <summary>
    /// Generates a hint for the current solution container and islands.
    /// </summary>
    void GenerateHint();

    /// <summary>
    ///     Gets an island by coordinates.
    /// </summary>
    /// <param name="coordinates">The island coordinates.</param>
    /// <returns>an island.</returns>
    IIslandViewModel GetIslandByCoordinates(IHashiPoint coordinates);

    /// <summary>
    ///     Checks if the drop target is valid.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>a boolean value if drop target is valid.</returns>
    bool IsValidDropTarget(IIslandViewModel? source, IIslandViewModel? target);
}