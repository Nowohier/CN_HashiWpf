using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Helpers;

/// <summary>
///     Provides helper methods for shared stateless computations
///     used by both the WPF <see cref="IIslandViewModel" /> and the simulation view model.
/// </summary>
public interface IIslandViewModelHelper
{
    /// <summary>
    ///     Determines the connection type between two islands based on their coordinates.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>The <see cref="ConnectionTypeEnum" /> between the two islands.</returns>
    ConnectionTypeEnum GetConnectionType(IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Determines whether the source island is a valid drop target for the given target island.
    /// </summary>
    /// <param name="source">The source island (this).</param>
    /// <param name="target">The potential target island.</param>
    /// <returns><c>true</c> if the target is a valid drop target; otherwise, <c>false</c>.</returns>
    bool IsValidDropTarget(IIslandViewModel source, IIslandViewModel? target);

    /// <summary>
    ///     Determines whether the maximum number of bridges (2) has been reached between the source and target islands.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>
    ///     <c>true</c> if the maximum bridges have been reached; <c>false</c> if not; <c>null</c> if target is
    ///     <c>null</c>.
    /// </returns>
    bool? MaxBridgesReachedToTarget(IIslandViewModel source, IIslandViewModel? target);

    /// <summary>
    ///     Gets bridges connecting to the left of the given island.
    /// </summary>
    /// <param name="island">The island.</param>
    /// <returns>A list of connection points to the left.</returns>
    List<IHashiPoint> GetBridgesLeft(IIslandViewModel island);

    /// <summary>
    ///     Gets bridges connecting to the right of the given island.
    /// </summary>
    /// <param name="island">The island.</param>
    /// <returns>A list of connection points to the right.</returns>
    List<IHashiPoint> GetBridgesRight(IIslandViewModel island);

    /// <summary>
    ///     Gets bridges connecting upward from the given island.
    /// </summary>
    /// <param name="island">The island.</param>
    /// <returns>A list of connection points upward.</returns>
    List<IHashiPoint> GetBridgesUp(IIslandViewModel island);

    /// <summary>
    ///     Gets bridges connecting downward from the given island.
    /// </summary>
    /// <param name="island">The island.</param>
    /// <returns>A list of connection points downward.</returns>
    List<IHashiPoint> GetBridgesDown(IIslandViewModel island);
}
