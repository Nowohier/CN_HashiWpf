using Hashi.Gui.Interfaces.Models;
using System.Windows;

namespace Hashi.Gui.Models;

/// <inheritdoc />
public class HashiPoint : IHashiPoint
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HashiPoint" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public HashiPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc />
    public int X { get; }

    /// <inheritdoc />
    public int Y { get; }

    /// <summary>
    ///     Converts a <see cref="HashiPoint" /> to a <see cref="Point" />.
    /// </summary>
    /// <param name="hashiPoint"></param>
    public static implicit operator Point(HashiPoint hashiPoint)
    {
        return new Point(hashiPoint.X, hashiPoint.Y);
    }

    /// <summary>
    ///     Converts a <see cref="Point" /> to a <see cref="HashiPoint" />.
    /// </summary>
    /// <param name="point"></param>
    public static implicit operator HashiPoint(Point point)
    {
        return new HashiPoint((int)point.X, (int)point.Y);
    }
}