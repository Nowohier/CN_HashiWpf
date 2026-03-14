namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Provides highlight state properties for an island's directional paths.
/// </summary>
public interface IIslandHighlightState
{
    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted left horizontally.
    /// </summary>
    bool IsHighlightHorizontalLeft { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted right horizontally.
    /// </summary>
    bool IsHighlightHorizontalRight { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted vertically top.
    /// </summary>
    bool IsHighlightVerticalTop { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted vertically bottom.
    /// </summary>
    bool IsHighlightVerticalBottom { get; set; }
}
