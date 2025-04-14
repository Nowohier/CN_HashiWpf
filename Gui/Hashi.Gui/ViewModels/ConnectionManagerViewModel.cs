using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces.Models;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Messages;
using System.Collections.ObjectModel;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="IConnectionManagerViewModel" />
public class ConnectionManagerViewModel : ObservableObject, IConnectionManagerViewModel
{
    internal const string SourceIslandNullErrorMessage = @"Source island cannot be null.";
    internal const string TargetIslandNullErrorMessage = @"Target island cannot be null.";
    private readonly Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge> bridgeFactory;
    private readonly Func<int, int, int, IIslandViewModel> islandFactory;
    private string ruleMessage = string.Empty;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConnectionManagerViewModel" /> class.
    /// </summary>
    /// <param name="islandFactory">The island factory.</param>
    /// <param name="bridgeFactory"></param>
    public ConnectionManagerViewModel(Func<int, int, int, IIslandViewModel> islandFactory,
        Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge> bridgeFactory)
    {
        this.islandFactory = islandFactory;
        this.bridgeFactory = bridgeFactory;

        WeakReferenceMessenger.Default.Register<ConnectionManagerViewModel, AllIslandsRequestMessage>(this,
            (_, m) => { m.Reply(Islands); });
    }

    /// <inheritdoc />
    public ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; } = [];

    /// <inheritdoc />
    public IList<IHashiBridge> History { get; } = new List<IHashiBridge>();

    /// <inheritdoc />
    public bool AreRulesBeingApplied { get; set; }

    /// <inheritdoc />
    public string RuleMessage
    {
        get => ruleMessage;
        set
        {
            if (!SetProperty(ref ruleMessage, value) || ruleMessage != string.Empty) return;
            WeakReferenceMessenger.Default.Send(new HintPopupClosedMessage());
            RefreshIslandColors();
        }
    }

    /// <inheritdoc />
    public ISolutionContainer? Solution { get; private set; }

    /// <inheritdoc />
    public void InitializeNewSolution(ISolutionContainer solutionContainer)
    {
        var hashiField = solutionContainer.HashiField;
        Islands.Clear();
        History.Clear();

        Solution = solutionContainer;
        for (var row = 0; row < hashiField.Count; row++)
        {
            var rowCollection = new ObservableCollection<IIslandViewModel>();
            for (var column = 0; column < hashiField[0].Length; column++)
                rowCollection.Add(islandFactory.Invoke(column, row, hashiField[row][column]));
            Islands.Add(rowCollection);
        }
    }

    /// <inheritdoc />
    public void AddConnection(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland, bool isHint = false)
    {
        if (sourceIsland == null)
            throw new ArgumentNullException(nameof(sourceIsland), SourceIslandNullErrorMessage);

        if (targetIsland == null)
            throw new ArgumentNullException(nameof(targetIsland), TargetIslandNullErrorMessage);

        if (!IsValidConnection(sourceIsland, targetIsland)) return;
        if (MaxBridgesReachedBetweenSourceAndTarget(sourceIsland, targetIsland) is true)
        {
            RemoveAllConnections(sourceIsland, targetIsland);
            return;
        }

        History.Add(bridgeFactory.Invoke(BridgeOperationTypeEnum.Add, sourceIsland.Coordinates,
            targetIsland.Coordinates));

        ManageConnections(sourceIsland, targetIsland, (island, coordinates) => island.AddConnection(coordinates),
            isHint);
        if (AreAllConnectionsSet()) WeakReferenceMessenger.Default.SendAsync(new AllConnectionsSetMessage());
    }

    /// <inheritdoc />
    public void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland)
    {
        if (sourceIsland == null)
            throw new ArgumentNullException(nameof(sourceIsland), SourceIslandNullErrorMessage);

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
    public IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(IIslandViewModel source,
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

    /// <inheritdoc />
    public void RemoveAllHighlights()
    {
        foreach (var row in Islands)
            foreach (var island in row)
            {
                island.IsHighlightHorizontalLeft = false;
                island.IsHighlightHorizontalRight = false;
                island.IsHighlightVerticalTop = false;
                island.IsHighlightVerticalBottom = false;
            }
    }

    /// <inheritdoc />
    public void ResetAllHintConnections()
    {
        foreach (var row in Islands)
            foreach (var island in row)
                foreach (var connection in island.AllConnections)
                    connection.IsHint = false;
    }

    /// <inheritdoc />
    public void RefreshIslandColors()
    {
        foreach (var row in Islands)
            foreach (var island in row)
                island.RefreshIslandColor();
    }

    /// <inheritdoc />
    public void ClearTemporaryDropTargets()
    {
        foreach (var row in Islands)
            foreach (var island in row)
                island.ResetDropTarget();
    }

    /// <inheritdoc />
    public bool IsValidDropTarget(IIslandViewModel? source, IIslandViewModel? target)
    {
        return source != null && target != null && source.MaxConnections > 0 && target.MaxConnections > 0 &&
               !source.MaxConnectionsReached && !target.MaxConnectionsReached &&
               source.GetConnectionType(target) != ConnectionTypeEnum.Diagonal;
    }

    /// <inheritdoc />
    public IIslandViewModel GetIslandByCoordinates(IHashiPoint coordinates)
    {
        return Islands[coordinates.Y][coordinates.X];
    }

    /// <inheritdoc />
    public void UndoConnection()
    {
        if (!History.Any()) return;

        var lastEntry = History.Last();
        var island1 = Islands.SelectMany(x => x).First(x => x.Coordinates.Equals(lastEntry.Point1));
        var island2 = Islands.SelectMany(x => x).First(x => x.Coordinates.Equals(lastEntry.Point2));

        var islands = GetAllIslandsInvolvedInConnection(island1, island2);

        foreach (var island in islands.Where(x => x.MaxConnections == 0))
        {
            var firstConnection = island.AllConnections.First(x => x.Equals(lastEntry.Point2));
            var secondConnection = island.AllConnections.First(x => x.Equals(lastEntry.Point1));

            island.AllConnections.Remove(firstConnection);
            island.AllConnections.Remove(secondConnection);
        }

        var firstConnection1 = island1.AllConnections.First(x => x.Equals(lastEntry.Point2));
        var secondConnection1 = island2.AllConnections.First(x => x.Equals(lastEntry.Point1));
        island1.AllConnections.Remove(firstConnection1);
        island2.AllConnections.Remove(secondConnection1);
        island1.RefreshIslandColor();
        island2.RefreshIslandColor();
        History.Remove(lastEntry);
    }


    private bool AreAllConnectionsSet()
    {
        return Islands.All(row => row.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached));
    }

    private void ManageConnections(IIslandViewModel? source, IIslandViewModel? target,
        Action<IIslandViewModel, IHashiPoint> connectionAction, bool isHint = false)
    {
        if (source == null) throw new ArgumentNullException(nameof(source), @"Source island cannot be null.");

        if (target == null) throw new ArgumentNullException(nameof(target), @"Target island cannot be null.");

        if (source.GetConnectionType(target) == ConnectionTypeEnum.Diagonal)
            throw new InvalidOperationException("Diagonal connections are not allowed.");

        var sourceCoordinates = (IHashiPoint)source.Coordinates.Clone();
        sourceCoordinates.IsHint = isHint;
        var targetCoordinates = (IHashiPoint)target.Coordinates.Clone();
        targetCoordinates.IsHint = isHint;

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

    private bool? MaxBridgesReachedBetweenSourceAndTarget(IIslandViewModel? source, IIslandViewModel? target)
    {
        return source == null || target == null
            ? null
            : source.AllConnections.Count(c => c.Equals(target.Coordinates)) >= 2 ||
              target.AllConnections.Count(c => c.Equals(source.Coordinates)) >= 2;
    }

    private bool IsValidConnection(IIslandViewModel? source, IIslandViewModel? target)
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

        return connectionType switch
        {
            ConnectionTypeEnum.Horizontal => islands.Any(island =>
                island.BridgesUp.Count > 0 || island.BridgesDown.Count > 0),
            ConnectionTypeEnum.Vertical => islands.Any(island =>
                island.BridgesLeft.Count > 0 || island.BridgesRight.Count > 0),
            ConnectionTypeEnum.Diagonal => throw new InvalidOperationException(
                "Invalid connection type. Diagonal connections are not allowed here."),
            _ => true
        };
    }
}