using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.ViewModels
{
    /// <summary>
    /// Manages the connections between islands.
    /// </summary>
    public interface IConnectionManagerViewModel : IBaseViewModel
    {
        /// <summary>
        /// Gets the islands.
        /// </summary>
        ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; }

        /// <summary>
        /// Adds a connection between two islands.
        /// </summary>
        /// <param name="sourceIsland">The source island.</param>
        /// <param name="targetIsland">The target island.</param>
        void AddConnection(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland);

        /// <summary>
        /// Removes a connection between two islands.
        /// </summary>
        /// <param name="sourceIsland">The source island.</param>
        /// <param name="targetIsland">The target island.</param>
        void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland);

        /// <summary>
        /// Finds the next island with max connections > 0 in the direction of the target island
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns></returns>
        IIslandViewModel? GetPotentialTargetIsland(IIslandViewModel source, IIslandViewModel target);

        /// <summary>
        /// Highlights the path to the target island.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        void HighlightPathToTargetIsland(IIslandViewModel source, IIslandViewModel target);

        /// <summary>
        /// Removes all highlights from the islands.
        /// </summary>
        void RemoveAllHighlights();

        /// <summary>
        /// Removes all potential island coordinates.
        /// </summary>
        void RemoveAllPotentialIslandCoordinates();

        /// <summary>
        /// Gets all islands involved in a connection between two islands.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns></returns>
        IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(IIslandViewModel source, IIslandViewModel target);

        /// <summary>
        /// Checks if the drop target is valid.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns>a boolean value if drop target is valid.</returns>
        bool IsValidDropTarget(IIslandViewModel? source, IIslandViewModel? target);
    }
}
