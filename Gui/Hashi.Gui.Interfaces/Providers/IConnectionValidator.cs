using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides methods for validating and managing connections between islands.
/// </summary>
public interface IConnectionValidator
{
    /// <summary>
    ///     Validates whether a connection between source and target is allowed.
    /// </summary>
    bool IsValidConnectionBetweenSourceAndTarget(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel? source, IIslandViewModel? target);

    /// <summary>
    ///     Checks whether there is an island with bridges between source and target.
    /// </summary>
    bool IsIslandInBetweenSourceAndTarget(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Checks whether a new connection between source and target would collide with existing perpendicular bridges.
    /// </summary>
    bool WouldConnectionCollide(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Manages adding or removing connections between source and target, applying the given action
    ///     to all involved islands.
    /// </summary>
    void ManageConnections(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target,
        Action<IIslandViewModel, IHashiPoint> action,
        HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal);
}
