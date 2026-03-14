using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides methods for counting and analyzing connected island groups.
/// </summary>
public interface IIslandGroupCounter
{
    /// <summary>
    ///     Counts the number of isolated island groups (fully connected subgraphs where all islands
    ///     have max connections reached, while other islands remain unfinished).
    /// </summary>
    int CountIsolatedIslandGroups(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IEnumerable<IIslandViewModel> islandsFlat,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors);

    /// <summary>
    ///     Recursively finds all connected islands starting from the given island.
    /// </summary>
    void FindConnectedIslands(IIslandViewModel island, ICollection<IIslandViewModel> group,
        HashSet<IIslandViewModel> visited,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors);

    /// <summary>
    ///     Gets all neighbors that have an active connection to the given island.
    /// </summary>
    IEnumerable<IIslandViewModel> GetConnectedNeighbors(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel island,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors);
}
