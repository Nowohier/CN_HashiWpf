using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Enums;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.Extensions;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;
using Hashi.Logging.Interfaces;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Providers;

/// <inheritdoc cref="IIslandProvider" />
public class IslandProvider :
    ObservableObject,
    IIslandProvider,
    IRecipient<IUpdateAllIslandColorsMessage>
{
    private readonly Func<bool?, IAllConnectionsSetMessage> allConnectionsSetMessageFactory;
    private readonly Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge> bridgeFactory;
    private readonly IIslandProviderCore core;
    private readonly IDialogWrapper dialogWrapper;
    private readonly IIslandViewModelHelper helper;
    private readonly Func<int, int, int, IIslandViewModel> islandFactory;
    private readonly ILogger logger;

    /// <inheritdoc cref="IIslandProvider" />
    public IslandProvider
    (
        Func<int, int, int, IIslandViewModel> islandFactory,
        Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge> bridgeFactory,
        Func<bool?, IAllConnectionsSetMessage> allConnectionsSetMessageFactory,
        IDialogWrapper dialogWrapper,
        ILoggerFactory loggerFactory,
        IIslandProviderCore core,
        IIslandViewModelHelper helper
    )
    {
        this.islandFactory = islandFactory ?? throw new ArgumentNullException(nameof(islandFactory));
        this.bridgeFactory = bridgeFactory ?? throw new ArgumentNullException(nameof(bridgeFactory));
        this.allConnectionsSetMessageFactory = allConnectionsSetMessageFactory ?? throw new ArgumentNullException(nameof(allConnectionsSetMessageFactory));
        this.dialogWrapper = dialogWrapper ?? throw new ArgumentNullException(nameof(dialogWrapper));
        this.logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger<IslandProvider>();
        this.core = core ?? throw new ArgumentNullException(nameof(core));
        this.helper = helper ?? throw new ArgumentNullException(nameof(helper));

        WeakReferenceMessenger.Default.Register(this);
        logger.Info("IslandProvider initialized");
    }

    private bool AreAllConnectionsSet =>
        IslandsFlat.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached);

    /// <inheritdoc />
    public ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; } = [];

    /// <inheritdoc />
    public IList<IHashiBridge> History { get; } = new List<IHashiBridge>();

    /// <inheritdoc />
    public IList<IHashiBridge> RedoHistory { get; } = new List<IHashiBridge>();

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
        RedoHistory.Clear();

        Solution = solutionProvider;
        for (var row = 0; row < hashiField.Count; row++)
        {
            var rowCollection = new ObservableCollection<IIslandViewModel>();
            for (var column = 0; column < hashiField[0].Length; column++)
            {
                rowCollection.Add(islandFactory.Invoke(column, row, hashiField[row][column]));
            }

            Islands.Add(rowCollection);
        }
    }

    /// <inheritdoc />
    public void InitializeNewSolutionAndSetBridges(ISolutionProvider solutionProvider)
    {
        ArgumentNullException.ThrowIfNull(solutionProvider, nameof(solutionProvider));
        ArgumentNullException.ThrowIfNull(solutionProvider.HashiField, nameof(solutionProvider.HashiField));

        InitializeNewSolution(solutionProvider);

        if (solutionProvider.BridgeCoordinates == null)
        {
            return;
        }

        foreach (var bridge in solutionProvider.BridgeCoordinates)
            for (var i = 0; i < bridge.AmountBridges; i++)
            {
                var sourceIsland = GetIslandByCoordinates(bridge.Location1.ToHashiPoint());
                var targetIsland = GetIslandByCoordinates(bridge.Location2.ToHashiPoint());
                if (sourceIsland.MaxConnections == 0 && targetIsland.MaxConnections == 0)
                {
                    continue;
                }

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

        if (!core.IsValidConnectionBetweenSourceAndTarget(Islands, sourceIsland, targetIsland))
        {
            return;
        }

        if (helper.MaxBridgesReachedToTarget(sourceIsland, targetIsland) is true)
        {
            if (pointType == HashiPointTypeEnum.Normal)
            {
                RemoveAllConnections(sourceIsland, targetIsland);
            }

            return;
        }

        if (pointType == HashiPointTypeEnum.Normal)
        {
            History.Add(bridgeFactory.Invoke(BridgeOperationTypeEnum.Add, sourceIsland.Coordinates,
                targetIsland.Coordinates));
        }

        core.ManageConnections(Islands, sourceIsland, targetIsland,
            (island, coordinates) => island.AddConnection(coordinates),
            pointType);

        var isolatedGroupCount = CountIsolatedIslandGroups();

        if (AreAllConnectionsSet && isolatedGroupCount == 0)
        {
            WeakReferenceMessenger.Default.Send(allConnectionsSetMessageFactory.Invoke(null));
            return;
        }

        if (isolatedGroupCount > 0 && pointType == HashiPointTypeEnum.Normal)
        {
            dialogWrapper.Show(TranslationSource.Instance.GetRequired("MessageIsolatedGroupCaption"),
                TranslationSource.Instance.GetRequired("MessageIsolatedGroupText"), DialogButton.Ok, DialogImage.Warning);
        }
    }

    /// <inheritdoc />
    public void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland)
    {
        ArgumentNullException.ThrowIfNull(sourceIsland, nameof(sourceIsland));

        if (targetIsland == null)
        {
            // Clears all source island connections
            foreach (var target in sourceIsland.AllConnections.Distinct().Select(GetIslandByCoordinates).ToList())
                core.ManageConnections(Islands, sourceIsland, target,
                    (island, coordinates) => island.RemoveAllConnectionsMatchingCoordinates(coordinates));

            return;
        }

        core.ManageConnections(Islands, sourceIsland, targetIsland,
            (island, coordinates) => island.RemoveAllConnectionsMatchingCoordinates(coordinates));
    }

    /// <inheritdoc />
    public void HighlightPathToTargetIsland(IIslandViewModel source, IIslandViewModel target)
    {
        var islands = core.GetAllIslandsInvolvedInConnection(Islands, source, target);
        var connectionType = helper.GetConnectionType(source, target);
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
        History.Clear();
        RedoHistory.Clear();

        foreach (var island in IslandsFlat)
        {
            foreach (var hashiPoint in island.AllConnections.GetConnectionsByPointType(pointType).ToList())
                island.AllConnections.Remove(hashiPoint);

            island.NotifyBridgeConnections();
        }
    }

    /// <inheritdoc />
    public void UndoConnection()
    {
        if (!History.Any())
        {
            return;
        }

        var lastEntry = History.Last();
        var island1 = GetIslandByCoordinates(lastEntry.Point1);
        var island2 = GetIslandByCoordinates(lastEntry.Point2);

        var islands = core.GetAllIslandsInvolvedInConnection(Islands, island1, island2);

        foreach (var island in islands.Where(x => x.MaxConnections == 0))
            RemoveConnectionFromIsland(island, lastEntry.Point1, lastEntry.Point2);

        RemoveConnectionFromIsland(island1, lastEntry.Point1, lastEntry.Point2);
        RemoveConnectionFromIsland(island2, lastEntry.Point2, lastEntry.Point1);

        island1.RefreshIslandColor();
        island2.RefreshIslandColor();
        History.Remove(lastEntry);
        RedoHistory.Add(lastEntry);
    }

    /// <inheritdoc />
    public void RedoConnection()
    {
        if (!RedoHistory.Any())
        {
            return;
        }

        var lastEntry = RedoHistory.Last();
        var island1 = GetIslandByCoordinates(lastEntry.Point1);
        var island2 = GetIslandByCoordinates(lastEntry.Point2);
        AddConnection(island1, island2);
        RedoHistory.Remove(lastEntry);
    }

    /// <inheritdoc />
    public IIslandViewModel? GetVisibleNeighbor(IIslandViewModel source, DirectionEnum direction)
    {
        return core.GetVisibleNeighbor(Islands, source, direction);
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetAllVisibleNeighbors(IIslandViewModel source)
    {
        return core.GetAllVisibleNeighbors(Islands, source);
    }

    /// <inheritdoc />
    public int CountIsolatedIslandGroups()
    {
        return core.CountIsolatedIslandGroups(Islands, IslandsFlat,
            island => core.GetAllVisibleNeighbors(Islands, island));
    }

    /// <inheritdoc cref="IIslandProvider.Receive(IUpdateAllIslandColorsMessage)" />
    public void Receive(IUpdateAllIslandColorsMessage message)
    {
        RefreshIslandColors();
    }

    private IIslandViewModel GetIslandByCoordinates(IHashiPoint coordinates)
    {
        if (coordinates.Y < 0 || coordinates.Y >= Islands.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(coordinates), "Y coordinate is out of range.");
        }

        if (coordinates.X < 0 || coordinates.X >= Islands[coordinates.Y].Count)
        {
            throw new ArgumentOutOfRangeException(nameof(coordinates), "X coordinate is out of range.");
        }

        return Islands[coordinates.Y][coordinates.X];
    }

    private void RemoveConnectionFromIsland(IIslandViewModel island, IHashiPoint point1, IHashiPoint point2)
    {
        var firstConnection = island.AllConnections.FirstOrDefault(x => x.X == point1.X && x.Y == point1.Y);
        var secondConnection = island.AllConnections.FirstOrDefault(x => x.X == point2.X && x.Y == point2.Y);

        if (firstConnection != null)
        {
            island.AllConnections.Remove(firstConnection);
        }

        if (secondConnection != null)
        {
            island.AllConnections.Remove(secondConnection);
        }
    }
}