using System.Drawing;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Models;

namespace Hashi.Gui.Extensions;

public static class HashiPointExtensions
{
    /// <summary>
    ///     Converts a normal drawing point to a HashiPoint
    /// </summary>
    /// <param name="point"></param>
    /// <param name="pointType">The point type. Default is normal.</param>
    /// <returns>a Hashi Point.</returns>
    public static IHashiPoint ToHashiPoint(this Point point, HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal)
    {
        return new HashiPoint(point.X, point.Y, pointType);
    }
}