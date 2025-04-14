using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.Providers
{

    public interface IIslandProvider
    {
        /// <summary>
        ///     Gets the islands.
        /// </summary>
        ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; }

        /// <summary>
        ///    Gets a flat enumerable of all islands.
        /// </summary>
        IEnumerable<IIslandViewModel> IslandsFlat { get; }

        /// <summary>
        ///     Gets the history of connections made in the game.
        /// </summary>
        IList<IHashiBridge> History { get; }

        /// <summary>
        ///     Gets or sets the solution container.
        /// </summary>
        ISolutionProvider? Solution { get; }

        /// <summary>
        ///     Rebuilds the island collection with a new solution container.
        /// </summary>
        /// <param name="solutionProvider"></param>
        void InitializeNewSolution(ISolutionProvider solutionProvider);

        /// <summary>
        /// Removes all island highlights.
        /// </summary>
        void RemoveAllHighlights();

        /// <summary>
        ///  Refreshes the island colors.
        /// </summary>
        void RefreshIslandColors();

        /// <summary>
        /// Clears the temporary drop targets.
        /// </summary>
        void ClearTemporaryDropTargets();

        /// <summary>
        ///     Highlights the path to the target island.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        void HighlightPathToTargetIsland(IIslandViewModel source, IIslandViewModel target);

        /// <summary>
        /// Undo the last connection made in the game.
        /// </summary>
        void UndoConnection();

        /// <summary>
        /// Removes all bridges from the islands.
        /// </summary>
        void RemoveAllBridges();

        /// <summary>
        ///   Gets the visible neighbor of the source island depending on the direction to the potential target island. The potential target is not necessarily a visible neighbor.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="potentialTarget">The potential target island.</param>
        /// <returns>the visible neighbor of the source island depending on the direction to the potential target island. </returns>
        IIslandViewModel? GetVisibleNeighbor(IIslandViewModel source, IIslandViewModel potentialTarget);

        /// <summary>
        /// Gets all visible neighbors of the source island.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <returns>all visible neighbors of the source island.</returns>
        List<IIslandViewModel> GetAllVisibleNeighbors(IIslandViewModel source);

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
    }
}
