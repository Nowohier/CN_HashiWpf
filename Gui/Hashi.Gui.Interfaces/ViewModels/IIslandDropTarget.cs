namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Provides drop target validation for an island during drag-drop operations.
/// </summary>
public interface IIslandDropTarget
{
    /// <summary>
    ///     Determines if the island is a valid drop target.
    /// </summary>
    bool IsValidDropTarget(IIslandViewModel? target);

    /// <summary>
    ///     Resets the drop target.
    /// </summary>
    void ResetDropTarget();

    /// <summary>
    ///     Determines if the maximum number of bridges has been reached to the target island. Returns null if target is null.
    /// </summary>
    bool? MaxBridgesReachedToTarget(IIslandViewModel? target);
}
