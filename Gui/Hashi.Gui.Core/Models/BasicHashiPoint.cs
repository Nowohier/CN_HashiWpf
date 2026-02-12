using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Core.Models;

/// <summary>
///     A lightweight implementation of <see cref="IHashiPoint" /> without WPF dependencies.
///     Used as the shared basic coordinate representation in the core library and simulation layer.
/// </summary>
public interface IBasicHashiPoint : IHashiPoint;

/// <inheritdoc cref="IBasicHashiPoint" />
public class BasicHashiPoint : IBasicHashiPoint
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BasicHashiPoint" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="pointType">The point type.</param>
    public BasicHashiPoint(int x, int y, HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal)
    {
        X = x;
        Y = y;
        PointType = pointType;
    }

    /// <inheritdoc />
    public int X { get; }

    /// <inheritdoc />
    public int Y { get; }

    /// <inheritdoc />
    public HashiPointTypeEnum PointType { get; set; }

    /// <inheritdoc />
    public object Clone()
    {
        return new BasicHashiPoint(X, Y, PointType);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is IHashiPoint other)
        {
            return X == other.X && Y == other.Y && PointType == other.PointType;
        }

        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, PointType);
    }
}
