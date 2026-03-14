using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides UI-related island operations such as highlights and color refreshes.
/// </summary>
public interface IIslandUiProvider
{
    /// <summary>
    ///     Removes all island highlights.
    /// </summary>
    void RemoveAllHighlights();

    /// <summary>
    ///     Refreshes the island colors.
    /// </summary>
    void RefreshIslandColors();

    /// <summary>
    ///     Clears the temporary drop targets.
    /// </summary>
    void ClearTemporaryDropTargets();

    /// <summary>
    ///     Highlights the path to the target island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    void HighlightPathToTargetIsland(IIslandViewModel source, IIslandViewModel target);

    /// <summary>
    ///     Updates the color of all islands.
    /// </summary>
    /// <param name="message">The <see cref="IUpdateAllIslandColorsMessage" />.</param>
    void Receive(IUpdateAllIslandColorsMessage message);
}
