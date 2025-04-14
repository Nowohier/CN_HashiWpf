using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Models;

/// <inheritdoc cref="IHashiPoint" />
public class HashiPoint : ObservableRecipient, IHashiPoint
{
    private string hintMessage = string.Empty;
    private HashiPointTypeEnum pointType;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HashiPoint" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="pointType">The point type.</param>
    public HashiPoint(int x, int y, HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal)
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
    public HashiPointTypeEnum PointType
    {
        get => pointType;
        set => SetProperty(ref pointType, value);
    }

    /// <inheritdoc />
    public object Clone()
    {
        var clone = new HashiPoint(X, Y, PointType)
        {
            hintMessage = hintMessage
        };
        return clone;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Coordinate (X = {X}, Y = {Y}), PointType = {PointType}";
    }
}