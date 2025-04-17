using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Hashi.Gui.Providers;

/// <inheritdoc cref="IIslandProvider" />
public class IslandProvider :
    ObservableObject,
    IIslandProvider,
    IRecipient<IUpdateAllIslandColorsMessage>
{
    private readonly Func<bool?, IAllConnectionsSetMessage> allConnectionsSetMessageFactory;
    private readonly Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge> bridgeFactory;
    private readonly IDialogWrapper dialogWrapper;
    private readonly Func<int, int, int, IIslandViewModel> islandFactory;

    /// <inheritdoc cref="IIslandProvider" />
    public IslandProvider
    (
        Func<int, int, int, IIslandViewModel> islandFactory,
        Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge> bridgeFactory,
        Func<bool?, IAllConnectionsSetMessage> allConnectionsSetMessageFactory,
        IDialogWrapper dialogWrapper
    )
    {
        this.islandFactory = islandFactory;
        this.bridgeFactory = bridgeFactory;
        this.allConnectionsSetMessageFactory = allConnectionsSetMessageFactory;
        this.dialogWrapper = dialogWrapper;

        WeakReferenceMessenger.Default.Register(this);
    }

    private bool AreAllConnectionsSet =>
        IslandsFlat.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached);

    /// <inheritdoc />
    public ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; } = [];

    /// <summary>
    ///      The history of all connections made in the game. This is used for undo functionality.
    /// </summary>
    public IList<IHashiBridge> History { get; } = new List<IHashiBridge>();

    /// <summary>
    ///     The solution provider that contains the current solution.
    /// </summary>
    public ISolutionProvider? Solution { get; private set; }

    /// <inheritdoc />
    public IEnumerable<IIslandViewModel> IslandsFlat => Islands.SelectMany(row => row);

    /// <inheritdoc />
    public void InitializeNewSolution(ISolutionProvider solutionProvider)
    {
        ArgumentNullException.ThrowIfNull(solutionProvider, nameof(solutionProvider));
        ArgumentNullException.ThrowIfNull(solutionProvider.HashiField, nameof(solutionProvider.HashiField));

        var hashiField = solutionProvider.HashiField;
        Islands.Clear();
        History.Clear();

        Solution = solutionProvider;
        for (var row = 0; row < hashiField.Count; row++)
        {
            var rowCollection = new ObservableCollection<IIslandViewModel>();
            for (var column = 0; column < hashiField[0].Length; column++)
                rowCollection.Add(islandFactory.Invoke(column, row, hashiField[row][column]));
            Islands.Add(rowCollection);
        }
    }

    /// <inheritdoc />
    public void InitializeNewSolutionAndSetBridges(ISolutionProvider solutionProvider)
    {
        ArgumentNullException.ThrowIfNull(solutionProvider, nameof(solutionProvider));
        ArgumentNullException.ThrowIfNull(solutionProvider.HashiField, nameof(solutionProvider.HashiField));
        ArgumentNullException.ThrowIfNull(solutionProvider.BridgeCoordinates,
            nameof(solutionProvider.BridgeCoordinates));

        InitializeNewSolution(solutionProvider);

        foreach (var bridge in solutionProvider.BridgeCoordinates)
            for (var i = 0; i < bridge.AmountBridges; i++)
            {
                var sourceIsland = GetIslandByCoordinates(bridge.Location1.ToHashiPoint());
                var targetIsland = GetIslandByCoordinates(bridge.Location2.ToHashiPoint());
                if (sourceIsland.MaxConnections == 0 && targetIsland.MaxConnections == 0)
                    continue;
                AddConnection(sourceIsland, targetIsland);
                sourceIsland.RefreshIslandColor();
                targetIsland.RefreshIslandColor();
            }
    }

    /// <inheritdoc />
    public void AddConnection(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland,
        HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal)
    {
        ArgumentNullException.ThrowIfNull(sourceIsland, nameof(sourceIsland));
        ArgumentNullException.ThrowIfNull(targetIsland, nameof(targetIsland));

        if (!IsValidConnectionBetweenSourceAndTarget(sourceIsland, targetIsland)) return;
        if (sourceIsland.MaxBridgesReachedToTarget(targetIsland) is true)
        {
            RemoveAllConnections(sourceIsland, targetIsland);
            return;
        }

        History.Add(bridgeFactory.Invoke(BridgeOperationTypeEnum.Add, sourceIsland.Coordinates,
            targetIsland.Coordinates));

        ManageConnections(sourceIsland, targetIsland, (island, coordinates) => island.AddConnection(coordinates),
            pointType);
        if (AreAllConnectionsSet) WeakReferenceMessenger.Default.Send(allConnectionsSetMessageFactory.Invoke(null));

        if (CountIsolatedIslandGroups() > 0 && !pointType.Equals(HashiPointTypeEnum.Test))
            dialogWrapper.Show(TranslationSource.Instance["MessageIsolatedGroupCaption"]!,
                TranslationSource.Instance["MessageIsolatedGroupText"]!, DialogButton.Ok, DialogImage.Warning);
    }

    /// <inheritdoc />
    public void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland)
    {
        ArgumentNullException.ThrowIfNull(sourceIsland, nameof(sourceIsland));

        if (targetIsland == null)
        {
            // Clears all source island connections
            foreach (var target in sourceIsland.AllConnections.Distinct().Select(GetIslandByCoordinates).ToList())
                ManageConnections(sourceIsland, target,
                    (island, coordinates) => island.RemoveAllConnectionsMatchingCoordinates(coordinates));

            return;
        }

        ManageConnections(sourceIsland, targetIsland,
            (island, coordinates) => island.RemoveAllConnectionsMatchingCoordinates(coordinates));
    }

    /// <inheritdoc />
    public void HighlightPathToTargetIsland(IIslandViewModel source, IIslandViewModel target)
    {
        var islands = GetAllIslandsInvolvedInConnection(source, target);
        var connectionType = source.GetConnectionType(target);
        foreach (var island in islands)
        {
            if (island.MaxConnections == 0)
            {
                island.IsHighlightHorizontalLeft = connectionType == ConnectionTypeEnum.Horizontal;
                island.IsHighlightHorizontalRight = connectionType == ConnectionTypeEnum.Horizontal;
                island.IsHighlightVerticalTop = connectionType == ConnectionTypeEnum.Vertical;
                island.IsHighlightVerticalBottom = connectionType == ConnectionTypeEnum.Vertical;
            }

            if (island == source)
            {
                island.IsHighlightHorizontalLeft = connectionType == ConnectionTypeEnum.Horizontal &&
                                                   target.Coordinates.X < source.Coordinates.X;
                island.IsHighlightHorizontalRight = connectionType == ConnectionTypeEnum.Horizontal &&
                                                    target.Coordinates.X > source.Coordinates.X;
                island.IsHighlightVerticalTop = connectionType == ConnectionTypeEnum.Vertical &&
                                                target.Coordinates.Y < source.Coordinates.Y;
                island.IsHighlightVerticalBottom = connectionType == ConnectionTypeEnum.Vertical &&
                                                   target.Coordinates.Y > source.Coordinates.Y;
            }

            if (island == target)
            {
                island.IsHighlightHorizontalLeft = connectionType == ConnectionTypeEnum.Horizontal &&
                                                   source.Coordinates.X < target.Coordinates.X;
                island.IsHighlightHorizontalRight = connectionType == ConnectionTypeEnum.Horizontal &&
                                                    source.Coordinates.X > target.Coordinates.X;
                island.IsHighlightVerticalTop = connectionType == ConnectionTypeEnum.Vertical &&
                                                source.Coordinates.Y < target.Coordinates.Y;
                island.IsHighlightVerticalBottom = connectionType == ConnectionTypeEnum.Vertical &&
                                                   source.Coordinates.Y > target.Coordinates.Y;
            }
        }
    }

    /// <inheritdoc />
    public void RemoveAllHighlights()
    {
        foreach (var island in IslandsFlat)
        {
            island.IsHighlightHorizontalLeft = false;
            island.IsHighlightHorizontalRight = false;
            island.IsHighlightVerticalTop = false;
            island.IsHighlightVerticalBottom = false;
        }
    }

    /// <inheritdoc />
    public void RefreshIslandColors()
    {
        foreach (var island in IslandsFlat) island.RefreshIslandColor();
    }

    /// <inheritdoc />
    public void ClearTemporaryDropTargets()
    {
        foreach (var island in IslandsFlat) island.ResetDropTarget();
    }

    /// <inheritdoc />
    public void RemoveAllBridges(HashiPointTypeEnum pointType)
    {
        foreach (var island in IslandsFlat)
        {
            var connectionsToRemove = pointType switch
            {
                HashiPointTypeEnum.All => island.AllConnections,
                HashiPointTypeEnum.Hint => island.AllConnections.Where(x => x.PointType == HashiPointTypeEnum.Hint),
                HashiPointTypeEnum.Test => island.AllConnections.Where(x => x.PointType == HashiPointTypeEnum.Test),
                HashiPointTypeEnum.Normal => island.AllConnections.Where(x => x.PointType == HashiPointTypeEnum.Normal),
                _ => throw new ArgumentOutOfRangeException(nameof(pointType), pointType, @"Invalid point type.")
            };

            foreach (var hashiPoint in connectionsToRemove.ToList()) island.AllConnections.Remove(hashiPoint);

            island.NotifyBridgeConnections();
        }
    }

    /// <inheritdoc />
    public void UndoConnection()
    {
        if (!History.Any()) return;

        var lastEntry = History.Last();
        var island1 = GetIslandByCoordinates(lastEntry.Point1);
        var island2 = GetIslandByCoordinates(lastEntry.Point2);

        var islands = GetAllIslandsInvolvedInConnection(island1, island2);

        foreach (var island in islands.Where(x => x.MaxConnections == 0))
            RemoveConnectionFromIsland(island, lastEntry.Point1, lastEntry.Point2);

        RemoveConnectionFromIsland(island1, lastEntry.Point1, lastEntry.Point2);
        RemoveConnectionFromIsland(island2, lastEntry.Point2, lastEntry.Point1);

        island1.RefreshIslandColor();
        island2.RefreshIslandColor();
        History.Remove(lastEntry);
    }

    /// <inheritdoc />
    public IIslandViewModel? GetVisibleNeighbor(IIslandViewModel source, DirectionEnum direction)
    {
        return direction switch
        {
            DirectionEnum.Up => CheckDirection(source, 0, -1, ConnectionTypeEnum.Vertical),
            DirectionEnum.Down => CheckDirection(source, 0, 1, ConnectionTypeEnum.Vertical),
            DirectionEnum.Left => CheckDirection(source, -1, 0, ConnectionTypeEnum.Horizontal),
            DirectionEnum.Right => CheckDirection(source, 1, 0, ConnectionTypeEnum.Horizontal),
            _ => null
        };
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetAllVisibleNeighbors(IIslandViewModel source)
    {
        var neighbors = new List<IIslandViewModel>();

        if (CheckDirection(source, 0, -1, ConnectionTypeEnum.Vertical) is { } above)
            neighbors.Add(above);

        if (CheckDirection(source, 0, 1, ConnectionTypeEnum.Vertical) is { } below)
            neighbors.Add(below);

        if (CheckDirection(source, -1, 0, ConnectionTypeEnum.Horizontal) is { } left)
            neighbors.Add(left);

        if (CheckDirection(source, 1, 0, ConnectionTypeEnum.Horizontal) is { } right)
            neighbors.Add(right);

        return neighbors;
    }

    /// <inheritdoc />
    public int CountIsolatedIslandGroups()
    {
        // Set to track visited islands
        var visited = new HashSet<IIslandViewModel>();
        var groupCount = 0;

        // Filter islands with connections and MaxConnections > 0
        var validIslands = IslandsFlat.Where(island => island.AllConnections.Any() && island.MaxConnections > 0)
            .ToList();

        // Iterate through all valid islands
        foreach (var island in validIslands)
        {
            // Skip already visited islands
            if (visited.Contains(island))
                continue;

            // Recursively find all connected islands in the group
            var group = new List<IIslandViewModel>();
            FindConnectedIslands(island, group, visited);

            Debug.WriteLine(
                $"GroupCount: {group.Count} | Islands: {string.Join("|", group.Select(x => x.Coordinates))}");

            // Count the group as isolated if all islands in the group have MaxConnectionsReached
            if (group.All(i => i.MaxConnectionsReached) && group.Count > 1)
                groupCount++;
        }

        return groupCount;
    }

    /// <inheritdoc cref="IIslandProvider.Receive(IUpdateAllIslandColorsMessage)" />
    public void Receive(IUpdateAllIslandColorsMessage message)
    {
        RefreshIslandColors();
    }

    private void FindConnectedIslands(IIslandViewModel island, List<IIslandViewModel> group,
        HashSet<IIslandViewModel> visited)
    {
        // Add the current island to the group and mark it as visited
        group.Add(island);
        visited.Add(island);

        // Recursively check all connected neighbors
        foreach (var neighbor in GetConnectedNeighbors(island))
            if (!visited.Contains(neighbor))
                FindConnectedIslands(neighbor, group, visited);
    }

    private IEnumerable<IIslandViewModel> GetConnectedNeighbors(IIslandViewModel island)
    {
        var result = GetAllVisibleNeighbors(island)
            .Where(neighbor => neighbor.AllConnections.Any(connection =>
                island.Coordinates.X == connection.X && island.Coordinates.Y == connection.Y));
        return result;
    }

    private IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(IIslandViewModel source,
        IIslandViewModel target)
    {
        var islandsBetween = new List<IIslandViewModel>();
        var connectionType = source.GetConnectionType(target);

        switch (connectionType)
        {
            case ConnectionTypeEnum.Vertical:
                {
                    var minY = Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                    var maxY = Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                    for (var y = minY; y <= maxY; y++)
                    {
                        var island = Islands[y][source.Coordinates.X];
                        islandsBetween.Add(island);
                    }

                    break;
                }
            case ConnectionTypeEnum.Horizontal:
                {
                    var minX = Math.Min(source.Coordinates.X, target.Coordinates.X);
                    var maxX = Math.Max(source.Coordinates.X, target.Coordinates.X);
                    for (var x = minX; x <= maxX; x++)
                    {
                        var island = Islands[source.Coordinates.Y][x];
                        islandsBetween.Add(island);
                    }

                    break;
                }
            case ConnectionTypeEnum.Diagonal:
            default:
                throw new ArgumentOutOfRangeException();
        }

        return islandsBetween;
    }

    private bool IsValidConnectionBetweenSourceAndTarget(IIslandViewModel? source, IIslandViewModel? target)
    {
        // Check if the source and/or target islands are null -> invalid
        if (source == null || target == null) return false;

        // Check if the source and target coordinates are the same -> invalid
        if (source == target) return false;

        // Check if the source and target coordinates are not on the same axis -> invalid
        if (source.GetConnectionType(target) == ConnectionTypeEnum.Diagonal) return false;

        // Check if an island is in between the source and target coordinates -> invalid
        if (IsIslandInBetweenSourceAndTarget(source, target)) return false;

        // Check if the source or target island has reached its maximum connections -> invalid
        if (source.MaxConnectionsReached || target.MaxConnectionsReached) return false;

        // Check if potential connection would collide with existing connections
        if (WouldConnectionCollide(source, target)) return false;

        return true;
    }

    private IIslandViewModel GetIslandByCoordinates(IHashiPoint coordinates)
    {
        return Islands[coordinates.Y][coordinates.X];
    }

    private bool IsIslandInBetweenSourceAndTarget(IIslandViewModel source, IIslandViewModel target)
    {
        var connectionType = source.GetConnectionType(target);

        switch (connectionType)
        {
            case ConnectionTypeEnum.Vertical:
                {
                    var minY = Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                    var maxY = Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                    for (var y = minY + 1; y < maxY; y++)
                        if (Islands[y][source.Coordinates.X].MaxConnections > 0)
                            return true;

                    break;
                }
            case ConnectionTypeEnum.Horizontal:
                {
                    var minX = Math.Min(source.Coordinates.X, target.Coordinates.X);
                    var maxX = Math.Max(source.Coordinates.X, target.Coordinates.X);
                    for (var x = minX + 1; x < maxX; x++)
                        if (Islands[source.Coordinates.Y][x].MaxConnections > 0)
                            return true;

                    break;
                }
            case ConnectionTypeEnum.Diagonal:
            default:
                throw new InvalidOperationException(
                    "Invalid connection type. Diagonal connections are not allowed here.");
        }

        return false;
    }

    private bool WouldConnectionCollide(IIslandViewModel source, IIslandViewModel target)
    {
        var connectionType = source.GetConnectionType(target);
        var islands = GetAllIslandsInvolvedInConnection(source, target).Where(x => x.MaxConnections == 0);
        return islands.Any(island => IsCollidingConnection(island, connectionType));
    }

    private IIslandViewModel? CheckDirection(IIslandViewModel source, int dx, int dy, ConnectionTypeEnum connectionType)
    {
        if (connectionType == ConnectionTypeEnum.Diagonal) return null;

        var currentX = source.Coordinates.X + dx;
        var currentY = source.Coordinates.Y + dy;

        while (currentY >= 0 && currentY < Islands.Count && currentX >= 0 && currentX < Islands[currentY].Count)
        {
            var neighbor = Islands[currentY][currentX];
            if (IsCollidingConnection(neighbor, connectionType)) break;

            if (neighbor.MaxConnections > 0) return neighbor;

            currentX += dx;
            currentY += dy;
        }

        return null;
    }

    private bool IsCollidingConnection(IIslandViewModel target, ConnectionTypeEnum connectionType)
    {
        if (target.MaxConnections > 0) return false;

        return connectionType switch
        {
            ConnectionTypeEnum.Vertical => target.BridgesLeft.Count > 0 || target.BridgesRight.Count > 0,
            ConnectionTypeEnum.Horizontal => target.BridgesUp.Count > 0 || target.BridgesDown.Count > 0,
            _ => throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType,
                @"Invalid connection type.")
        };
    }

    private void ManageConnections(IIslandViewModel? source, IIslandViewModel? target,
        Action<IIslandViewModel, IHashiPoint> connectionAction,
        HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal)
    {
        if (source == null) throw new ArgumentNullException(nameof(source), @"Source island cannot be null.");

        if (target == null) throw new ArgumentNullException(nameof(target), @"Target island cannot be null.");

        if (source.GetConnectionType(target) == ConnectionTypeEnum.Diagonal)
            throw new InvalidOperationException("Diagonal connections are not allowed.");

        var sourceCoordinates = (IHashiPoint)source.Coordinates.Clone();
        sourceCoordinates.PointType = pointType;
        var targetCoordinates = (IHashiPoint)target.Coordinates.Clone();
        targetCoordinates.PointType = pointType;

        var islandsToConnect = GetAllIslandsInvolvedInConnection(source, target).ToList();

        foreach (var island in islandsToConnect)
        {
            if (island.MaxConnections == 0)
            {
                connectionAction(island, sourceCoordinates);
                connectionAction(island, targetCoordinates);
            }

            if (island == source) connectionAction(island, targetCoordinates);

            if (island == target) connectionAction(island, sourceCoordinates);
        }
    }

    private void RemoveConnectionFromIsland(IIslandViewModel island, IHashiPoint point1, IHashiPoint point2)
    {
        var firstConnection = island.AllConnections.FirstOrDefault(x => x.Equals(point1));
        var secondConnection = island.AllConnections.FirstOrDefault(x => x.Equals(point2));

        if (firstConnection != null) island.AllConnections.Remove(firstConnection);
        if (secondConnection != null) island.AllConnections.Remove(secondConnection);
    }
}