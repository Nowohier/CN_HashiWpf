using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Interfaces.Extensions;

/// <summary>
///     Extension methods for filtering <see cref="IHashiPoint" /> collections by point type.
/// </summary>
public static class HashiPointExtensions
{
    /// <summary>
    ///     Returns the connections matching the specified point type.
    /// </summary>
    /// <param name="connections">The connections collection.</param>
    /// <param name="pointType">The point type to filter by.</param>
    /// <returns>The matching connections.</returns>
    public static IEnumerable<IHashiPoint> GetConnectionsByPointType(
        this IList<IHashiPoint> connections, HashiPointTypeEnum pointType)
    {
        return pointType switch
        {
            HashiPointTypeEnum.All => connections,
            HashiPointTypeEnum.Hint => connections.Where(x => x.PointType == HashiPointTypeEnum.Hint),
            HashiPointTypeEnum.Test => connections.Where(x => x.PointType == HashiPointTypeEnum.Test),
            HashiPointTypeEnum.Normal => connections.Where(x => x.PointType == HashiPointTypeEnum.Normal),
            _ => throw new ArgumentOutOfRangeException(nameof(pointType), pointType, @"Invalid point type.")
        };
    }
}
