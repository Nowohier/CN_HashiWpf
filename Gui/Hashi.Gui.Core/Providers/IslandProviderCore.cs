using Hashi.Enums;
using Hashi.Gui.Core.Models;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Core.Providers;

/// <inheritdoc cref="IIslandProviderCore" />
public class IslandProviderCore : IIslandProviderCore
{
    private readonly IIslandViewModelHelper helper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IslandProviderCore" /> class.
    /// </summary>
    /// <param name="helper">The island view model helper.</param>
    public IslandProviderCore(IIslandViewModelHelper helper)
    {
        this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
    }

    /// <inheritdoc />
    public IIslandViewModel? CheckDirection(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, int dx, int dy, ConnectionTypeEnum connectionType)
    {
        if (connectionType == ConnectionTypeEnum.Diagonal)
        {
            return null;
        }

        var currentX = source.Coordinates.X + dx;
        var currentY = source.Coordinates.Y + dy;

        while (currentY >= 0 && currentY < islands.Count && currentX >= 0 && currentX < islands[currentY].Count)
        {
            var neighbor = islands[currentY][currentX];
            if (IsCollidingConnection(neighbor, connectionType))
            {
                break;
            }

            if (neighbor.MaxConnections > 0)
            {
                return neighbor;
            }

            currentX += dx;
            currentY += dy;
        }

        return null;
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetAllVisibleNeighbors(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source)
    {
        var neighbors = new List<IIslandViewModel>();

        if (CheckDirection(islands, source, 0, -1, ConnectionTypeEnum.Vertical) is { } above)
        {
            neighbors.Add(above);
        }

        if (CheckDirection(islands, source, 0, 1, ConnectionTypeEnum.Vertical) is { } below)
        {
            neighbors.Add(below);
        }

        if (CheckDirection(islands, source, -1, 0, ConnectionTypeEnum.Horizontal) is { } left)
        {
            neighbors.Add(left);
        }

        if (CheckDirection(islands, source, 1, 0, ConnectionTypeEnum.Horizontal) is { } right)
        {
            neighbors.Add(right);
        }

        return neighbors;
    }

    /// <inheritdoc />
    public IIslandViewModel? GetVisibleNeighbor(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, DirectionEnum direction)
    {
        return direction switch
        {
            DirectionEnum.Up => CheckDirection(islands, source, 0, -1, ConnectionTypeEnum.Vertical),
            DirectionEnum.Down => CheckDirection(islands, source, 0, 1, ConnectionTypeEnum.Vertical),
            DirectionEnum.Left => CheckDirection(islands, source, -1, 0, ConnectionTypeEnum.Horizontal),
            DirectionEnum.Right => CheckDirection(islands, source, 1, 0, ConnectionTypeEnum.Horizontal),
            _ => null
        };
    }

    /// <inheritdoc />
    public IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target)
    {
        var islandsBetween = new List<IIslandViewModel>();
        var connectionType = helper.GetConnectionType(source, target);

        switch (connectionType)
        {
            case ConnectionTypeEnum.Vertical:
            {
                var minY = Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                var maxY = Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                for (var y = minY; y <= maxY; y++)
                {
                    islandsBetween.Add(islands[y][source.Coordinates.X]);
                }

                break;
            }
            case ConnectionTypeEnum.Horizontal:
            {
                var minX = Math.Min(source.Coordinates.X, target.Coordinates.X);
                var maxX = Math.Max(source.Coordinates.X, target.Coordinates.X);
                for (var x = minX; x <= maxX; x++)
                {
                    islandsBetween.Add(islands[source.Coordinates.Y][x]);
                }

                break;
            }
            case ConnectionTypeEnum.Diagonal:
            default:
                throw new ArgumentOutOfRangeException();
        }

        return islandsBetween;
    }

    /// <inheritdoc />
    public bool IsValidConnectionBetweenSourceAndTarget(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel? source, IIslandViewModel? target)
    {
        if (source == null || target == null)
        {
            return false;
        }

        if (source == target)
        {
            return false;
        }

        if (helper.GetConnectionType(source, target) == ConnectionTypeEnum.Diagonal)
        {
            return false;
        }

        if (IsIslandInBetweenSourceAndTarget(islands, source, target))
        {
            return false;
        }

        if (source.MaxConnectionsReached || target.MaxConnectionsReached)
        {
            return false;
        }

        if (WouldConnectionCollide(islands, source, target))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool IsIslandInBetweenSourceAndTarget(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target)
    {
        var connectionType = helper.GetConnectionType(source, target);

        switch (connectionType)
        {
            case ConnectionTypeEnum.Vertical:
            {
                var minY = Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                var maxY = Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                for (var y = minY + 1; y < maxY; y++)
                {
                    if (islands[y][source.Coordinates.X].MaxConnections > 0)
                    {
                        return true;
                    }
                }

                break;
            }
            case ConnectionTypeEnum.Horizontal:
            {
                var minX = Math.Min(source.Coordinates.X, target.Coordinates.X);
                var maxX = Math.Max(source.Coordinates.X, target.Coordinates.X);
                for (var x = minX + 1; x < maxX; x++)
                {
                    if (islands[source.Coordinates.Y][x].MaxConnections > 0)
                    {
                        return true;
                    }
                }

                break;
            }
            case ConnectionTypeEnum.Diagonal:
            default:
                throw new InvalidOperationException(
                    "Invalid connection type. Diagonal connections are not allowed here.");
        }

        return false;
    }

    /// <inheritdoc />
    public bool WouldConnectionCollide(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target)
    {
        var connectionType = helper.GetConnectionType(source, target);
        var involvedIslands = GetAllIslandsInvolvedInConnection(islands, source, target)
            .Where(x => x.MaxConnections == 0);
        return involvedIslands.Any(island => IsCollidingConnection(island, connectionType));
    }

    /// <inheritdoc />
    public void ManageConnections(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel source, IIslandViewModel target,
        Action<IIslandViewModel, IHashiPoint> action,
        HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(target, nameof(target));

        if (helper.GetConnectionType(source, target) == ConnectionTypeEnum.Diagonal)
        {
            throw new InvalidOperationException("Diagonal connections are not allowed.");
        }

        var sourceCoordinates = new BasicHashiPoint(source.Coordinates.X, source.Coordinates.Y, pointType);
        var targetCoordinates = new BasicHashiPoint(target.Coordinates.X, target.Coordinates.Y, pointType);

        var islandsToConnect = GetAllIslandsInvolvedInConnection(islands, source, target).ToList();

        foreach (var island in islandsToConnect)
        {
            if (island.MaxConnections == 0)
            {
                action(island, sourceCoordinates);
                action(island, targetCoordinates);
            }

            if (island == source)
            {
                action(island, targetCoordinates);
            }

            if (island == target)
            {
                action(island, sourceCoordinates);
            }
        }
    }

    /// <inheritdoc />
    public int CountIsolatedIslandGroups(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IEnumerable<IIslandViewModel> islandsFlat,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors)
    {
        var visited = new HashSet<IIslandViewModel>();
        var groupCount = 0;

        var validIslands = islandsFlat.Where(island => island.AllConnections.Any() && island.MaxConnections > 0)
            .ToList();

        foreach (var island in validIslands)
        {
            if (visited.Contains(island))
            {
                continue;
            }

            var group = new List<IIslandViewModel>();
            FindConnectedIslands(island, group, visited, getNeighbors);

            if (group.All(x => x.MaxConnectionsReached))
            {
                var allIslandsWithConnections =
                    islandsFlat.Where(x => x.MaxConnections > 0 && !x.MaxConnectionsReached).ToList();
                if (allIslandsWithConnections.Count > 0)
                {
                    groupCount++;
                }
            }
        }

        return groupCount;
    }

    /// <inheritdoc />
    public void FindConnectedIslands(IIslandViewModel island, ICollection<IIslandViewModel> group,
        HashSet<IIslandViewModel> visited,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors)
    {
        if (!visited.Add(island))
        {
            return;
        }

        group.Add(island);

        foreach (var neighbor in GetConnectedNeighborsInternal(island, getNeighbors))
        {
            if (!visited.Contains(neighbor))
            {
                FindConnectedIslands(neighbor, group, visited, getNeighbors);
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<IIslandViewModel> GetConnectedNeighbors(
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel island,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors)
    {
        return GetConnectedNeighborsInternal(island, getNeighbors);
    }

    private static bool IsCollidingConnection(IIslandViewModel target, ConnectionTypeEnum connectionType)
    {
        if (target.MaxConnections > 0)
        {
            return false;
        }

        return connectionType switch
        {
            ConnectionTypeEnum.Vertical => target.BridgesLeft.Count > 0 || target.BridgesRight.Count > 0,
            ConnectionTypeEnum.Horizontal => target.BridgesUp.Count > 0 || target.BridgesDown.Count > 0,
            _ => throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType,
                @"Invalid connection type.")
        };
    }

    private static IEnumerable<IIslandViewModel> GetConnectedNeighborsInternal(IIslandViewModel island,
        Func<IIslandViewModel, List<IIslandViewModel>> getNeighbors)
    {
        return getNeighbors(island)
            .Where(neighbor => neighbor.AllConnections.Any(connection =>
                island.Coordinates.X == connection.X && island.Coordinates.Y == connection.Y));
    }
}
