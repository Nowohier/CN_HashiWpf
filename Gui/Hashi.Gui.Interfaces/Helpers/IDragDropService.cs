using Hashi.Enums;

namespace Hashi.Gui.Interfaces.Helpers;

/// <summary>
///     Provides drag direction calculation for drag-drop operations on islands.
/// </summary>
public interface IDragDropService
{
    /// <summary>
    ///     Calculates the drag direction based on the delta X and Y coordinates.
    /// </summary>
    /// <param name="deltaX">The horizontal distance moved.</param>
    /// <param name="deltaY">The vertical distance moved.</param>
    /// <returns>The calculated direction.</returns>
    DirectionEnum GetDragDirection(double deltaX, double deltaY);
}
