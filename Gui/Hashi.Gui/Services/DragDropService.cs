using Hashi.Enums;
using Hashi.Gui.Interfaces.Helpers;

namespace Hashi.Gui.Services;

/// <inheritdoc />
public class DragDropService : IDragDropService
{
    private const double Threshold = 1.0;

    /// <inheritdoc />
    public DirectionEnum GetDragDirection(double deltaX, double deltaY)
    {
        if (Math.Abs(deltaX) < Threshold && Math.Abs(deltaY) < Threshold)
        {
            return DirectionEnum.None;
        }

        var angle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI);

        if (angle < 0)
        {
            angle += 360;
        }

        return angle switch
        {
            >= 45 and < 135 => DirectionEnum.Down,
            >= 135 and < 225 => DirectionEnum.Left,
            >= 225 and < 315 => DirectionEnum.Up,
            _ => DirectionEnum.Right
        };
    }
}
