using Hashi.Enums;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides methods for finding neighboring islands in the grid.
/// </summary>
public interface INeighborFinder
{
    /// <summary>
    ///     Checks a single direction from a source island for the next visible neighbor,
    ///     stopping at colliding connections or grid boundaries.
    /// </summary>
    IIslandViewModel? CheckDirection(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, int dx, int dy, ConnectionTypeEnum connectionType);

    /// <summary>
    ///     Gets all visible neighbors of a source island (up, down, left, right).
    /// </summary>
    List<IIslandViewModel> GetAllVisibleNeighbors(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source);

    /// <summary>
    ///     Gets the visible neighbor of the source island in the given direction.
    /// </summary>
    IIslandViewModel? GetVisibleNeighbor(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, DirectionEnum direction);

    /// <summary>
    ///     Gets all islands involved in a connection between source and target,
    ///     including intermediate empty cells.
    /// </summary>
    IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target);
}
