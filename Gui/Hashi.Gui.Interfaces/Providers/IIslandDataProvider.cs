using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides access to island data on the Hashi game board.
/// </summary>
public interface IIslandDataProvider
{
    /// <summary>
    ///     Gets the islands.
    /// </summary>
    ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; }

    /// <summary>
    ///     Gets a flat enumerable of all islands.
    /// </summary>
    IEnumerable<IIslandViewModel> IslandsFlat { get; }
}
