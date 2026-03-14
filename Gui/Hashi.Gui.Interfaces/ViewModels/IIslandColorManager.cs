using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Provides color management for an island view model.
/// </summary>
public interface IIslandColorManager
{
    /// <summary>
    ///     Gets or sets the color of the island.
    /// </summary>
    IHashiBrush IslandColor { get; set; }

    /// <summary>
    ///     Gets the helper instance for performing color-related operations.
    /// </summary>
    IHashiBrushResolver BrushResolver { get; }

    /// <summary>
    ///     Refreshes the island color and sets it to the default color if it is not set.
    /// </summary>
    void RefreshIslandColor();
}
