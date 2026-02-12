using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides the shared grid-navigation and connection management logic
///     used by both the WPF <see cref="IIslandProvider" /> and the simulation provider.
/// </summary>
public interface IIslandProviderCore
{
    /// <summary>
    ///     Checks a single direction from a source island for the next visible neighbor,
    ///     stopping at colliding connections or grid boundaries.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <param name="dx">The horizontal step direction (-1, 0, or 1).</param>
    /// <param name="dy">The vertical step direction (-1, 0, or 1).</param>
    /// <param name="connectionType">The connection type (Horizontal or Vertical).</param>
    /// <returns>The first visible neighbor in the given direction, or <c>null</c> if none exists.</returns>
    IIslandViewModel? CheckDirection(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, int dx, int dy, ConnectionTypeEnum connectionType);

    /// <summary>
    ///     Gets all visible neighbors of a source island (up, down, left, right).
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <returns>A list of all visible neighbor islands.</returns>
    List<IIslandViewModel> GetAllVisibleNeighbors(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source);

    /// <summary>
    ///     Gets the visible neighbor of the source island in the given direction.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <param name="direction">The direction to check.</param>
    /// <returns>The visible neighbor in the given direction, or <c>null</c>.</returns>
    IIslandViewModel? GetVisibleNeighbor(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, DirectionEnum direction);

    /// <summary>
    ///     Gets all islands involved in a connection between source and target,
    ///     including intermediate empty cells.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>All islands on the path between source and target (inclusive).</returns>
    IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Validates whether a connection between source and target is allowed.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns><c>true</c> if the connection is valid; otherwise, <c>false</c>.</returns>
    bool IsValidConnectionBetweenSourceAndTarget(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel? source, IIslandViewModel? target);

    /// <summary>
    ///     Checks whether there is an island with bridges between source and target.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns><c>true</c> if an island blocks the path; otherwise, <c>false</c>.</returns>
    bool IsIslandInBetweenSourceAndTarget(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Checks whether a new connection between source and target would collide with existing perpendicular bridges.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns><c>true</c> if the connection would collide; otherwise, <c>false</c>.</returns>
    bool WouldConnectionCollide(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Manages adding or removing connections between source and target, applying the given action
    ///     to all involved islands.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <param name="action">The action to perform (add or remove).</param>
    /// <param name="pointType">The type of connection point.</param>
    void ManageConnections(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target,
        Action<IIslandViewModel, IHashiPoint> action,
        HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal);

    /// <summary>
    ///     Counts the number of isolated island groups (fully connected subgraphs where all islands
    ///     have max connections reached, while other islands remain unfinished).
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="islandsFlat">A flat enumerable of all islands.</param>
    /// <param name="getNeighbors">A function to get all visible neighbors of an island.</param>
    /// <returns>The number of isolated groups.</returns>
    int CountIsolatedIslandGroups(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IEnumerable<IIslandViewModel> islandsFlat,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors);

    /// <summary>
    ///     Recursively finds all connected islands starting from the given island.
    /// </summary>
    /// <param name="island">The starting island.</param>
    /// <param name="group">The group being built.</param>
    /// <param name="visited">Already visited islands.</param>
    /// <param name="getNeighbors">A function to get all visible neighbors of an island.</param>
    void FindConnectedIslands(IIslandViewModel island, ICollection<IIslandViewModel> group,
        HashSet<IIslandViewModel> visited,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors);

    /// <summary>
    ///     Gets all neighbors that have an active connection to the given island.
    /// </summary>
    /// <param name="islands">The island grid.</param>
    /// <param name="island">The island to check.</param>
    /// <param name="getNeighbors">A function to get all visible neighbors of an island.</param>
    /// <returns>All connected neighbors.</returns>
    IEnumerable<IIslandViewModel> GetConnectedNeighbors(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel island,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors);
}
