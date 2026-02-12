using Hashi.Enums;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Core.Helpers;

/// <inheritdoc cref="IIslandViewModelHelper" />
public class IslandViewModelHelper : IIslandViewModelHelper
{
    /// <inheritdoc />
    public ConnectionTypeEnum GetConnectionType(IIslandViewModel source, IIslandViewModel target)
    {
        if (source.Coordinates.X == target.Coordinates.X)
        {
            return ConnectionTypeEnum.Vertical;
        }

        return source.Coordinates.Y == target.Coordinates.Y
            ? ConnectionTypeEnum.Horizontal
            : ConnectionTypeEnum.Diagonal;
    }

    /// <inheritdoc />
    public bool IsValidDropTarget(IIslandViewModel source, IIslandViewModel? target)
    {
        return target != null && target != source && source.MaxConnections > 0 && target.MaxConnections > 0 &&
               !source.MaxConnectionsReached && !target.MaxConnectionsReached &&
               GetConnectionType(source, target) != ConnectionTypeEnum.Diagonal;
    }

    /// <inheritdoc />
    public bool? MaxBridgesReachedToTarget(IIslandViewModel source, IIslandViewModel? target)
    {
        if (target == null)
        {
            return null;
        }

        return source.AllConnections.Count(c => c.X == target.Coordinates.X && c.Y == target.Coordinates.Y) >= 2 ||
               target.AllConnections.Count(c => c.X == source.Coordinates.X && c.Y == source.Coordinates.Y) >= 2;
    }

    /// <inheritdoc />
    public List<IHashiPoint> GetBridgesLeft(IIslandViewModel island)
    {
        return island.AllConnections.Where(x => x.X < island.Coordinates.X && x.Y == island.Coordinates.Y).ToList();
    }

    /// <inheritdoc />
    public List<IHashiPoint> GetBridgesRight(IIslandViewModel island)
    {
        return island.AllConnections.Where(x => x.X > island.Coordinates.X && x.Y == island.Coordinates.Y).ToList();
    }

    /// <inheritdoc />
    public List<IHashiPoint> GetBridgesUp(IIslandViewModel island)
    {
        return island.AllConnections.Where(x => x.X == island.Coordinates.X && x.Y < island.Coordinates.Y).ToList();
    }

    /// <inheritdoc />
    public List<IHashiPoint> GetBridgesDown(IIslandViewModel island)
    {
        return island.AllConnections.Where(x => x.X == island.Coordinates.X && x.Y > island.Coordinates.Y).ToList();
    }
}
