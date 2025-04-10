using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Models;

/// <inheritdoc cref="IHashiPoint" />
public class HashiPoint : ObservableRecipient, IHashiPoint
{
    private string hintMessage = string.Empty;
    private bool isHint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HashiPoint" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="isHint"></param>
    public HashiPoint(int x, int y, bool isHint = false)
    {
        X = x;
        Y = y;
        IsHint = isHint;
    }

    /// <inheritdoc />
    public int X { get; }

    /// <inheritdoc />
    public int Y { get; }

    /// <inheritdoc />
    public bool IsHint
    {
        get => isHint;
        set => SetProperty(ref isHint, value);
    }

    /// <inheritdoc />
    public object Clone()
    {
        var clone = new HashiPoint(X, Y, IsHint)
        {
            hintMessage = hintMessage
        };
        return clone;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not IHashiPoint item) return false;

        return X.Equals(item.X) && Y.Equals(item.Y);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }
    }


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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Coordinate (X = {X}, Y = {Y}), IsHint = {IsHint}";
    }
}