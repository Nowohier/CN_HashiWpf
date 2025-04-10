namespace Hashi.Gui.Interfaces.Models;

/// <summary>
///     A point containing x and y coordinates.
/// </summary>
public interface IHashiPoint : ICloneable
{
    /// <summary>
    ///     Gets the x coordinate of the point.
    /// </summary>
    public int X { get; }

    /// <summary>
    ///     Gets the y coordinate of the point.
    /// </summary>
    public int Y { get; }

    /// <summary>
    ///     Indicates whether the point is a hint.
    /// </summary>
    bool IsHint { get; set; }
}