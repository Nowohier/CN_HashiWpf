using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Generator.Interfaces.Models;
using Hashi.Gui.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Messages;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="IConnectionManagerViewModel" />
[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public class ConnectionManagerViewModel : ObservableObject, IConnectionManagerViewModel
{
    private readonly Func<int, int, int, IIslandViewModel> islandFactory;
    private readonly Func<SolidColorBrush, IHashiBrush> brushFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConnectionManagerViewModel" /> class.
    /// </summary>
    /// <param name="islandFactory">The island factory.</param>
    /// <param name="solutionHelper">The solution helper.</param>
    /// <param name="brushFactory">The brush factory.</param>
    public ConnectionManagerViewModel(Func<int, int, int, IIslandViewModel> islandFactory, ISolutionHelper solutionHelper, Func<SolidColorBrush, IHashiBrush> brushFactory)
    {
        this.islandFactory = islandFactory;
        this.brushFactory = brushFactory;
        SolutionHelper = solutionHelper;

        WeakReferenceMessenger.Default.Register<ConnectionManagerViewModel, AllIslandsRequestMessage>(this, (r, m) => { m.Reply(Islands); });
    }

    /// <inheritdoc />
    public ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; } = new();

    /// <inheritdoc />
    public ISolutionContainer? Solution { get; private set; }

    /// <inheritdoc />
    public ISolutionHelper SolutionHelper { get; }

    /// <inheritdoc />
    public void InitializeNewSolution(ISolutionContainer solutionContainer)
    {
        var hashiField = solutionContainer.HashiField;
        Islands.Clear();
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
    public void AddConnection(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland)
    {
        if (!IsValidConnection(sourceIsland, targetIsland)) return;
        if (MaxBridgesReachedBetweenSourceAndTarget(sourceIsland, targetIsland) is true)
        {
            RemoveAllConnections(sourceIsland, targetIsland);
            return;
        }

        ManageConnections(sourceIsland, targetIsland, (island, coordinates) => island.AddConnection(coordinates));
        if (AreAllConnectionsSet()) WeakReferenceMessenger.Default.SendAsync(new AllConnectionsSetMessage());
    }

    /// <inheritdoc />
    public void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland)
    {
        if (sourceIsland == null)
            throw new ArgumentNullException(nameof(sourceIsland), "Source island cannot be null.");

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
        var islands = SolutionHelper.GetAllIslandsInvolvedInConnection(source, target, Islands);
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
    public void RefreshIslandColors()
    {
        foreach (var row in Islands)
            foreach (var island in row)
                island.IslandColor = island.MaxConnectionsReached
                    ? brushFactory.Invoke(HashiColorHelper.MaxBridgesReachedBrush)
                    : brushFactory.Invoke(HashiColorHelper.BasicIslandBrush);
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
    public void GenerateHint()
    {
        SolutionHelper.GenerateHint(Solution!, Islands);
    }

    private bool AreAllConnectionsSet()
    {
        return Islands.All(row => row.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached));
    }

    private void ManageConnections(IIslandViewModel? source, IIslandViewModel? target,
        Action<IIslandViewModel, IHashiPoint> connectionAction)
    {
        if (source == null) throw new ArgumentNullException(nameof(source), "Source island cannot be null.");

        if (target == null) throw new ArgumentNullException(nameof(target), "Target island cannot be null.");

        if (source.GetConnectionType(target) == ConnectionTypeEnum.Diagonal)
            throw new InvalidOperationException("Diagonal connections are not allowed.");

        var sourceCoordinates = source.Coordinates;
        var targetCoordinates = target.Coordinates;

        var islandsToConnect = SolutionHelper.GetAllIslandsInvolvedInConnection(source, target, Islands).ToList();

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

    /// <summary>
    ///     Checks if the maximum number of bridges has been reached between the source or target islands.
    /// </summary>
    /// <param name="source">The source Island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>a boolean value indicating if max bridges have been reached.</returns>
    private bool? MaxBridgesReachedBetweenSourceAndTarget(IIslandViewModel? source, IIslandViewModel? target)
    {
        return source == null || target == null
            ? null
            : source.AllConnections.Count(c => c == target.Coordinates) >= 2 ||
              target.AllConnections.Count(c => c == source.Coordinates) >= 2;
    }

    /// <summary>
    ///     Checks if the potential connection between the source and target islands is valid.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns></returns>
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

    /// <summary>
    ///     Checks if an island with MaxConnections > 0 is in between the source and target coordinates.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>
    ///     a boolean value indicating if if an island with MaxConnections > 0 is in between the source and target
    ///     coordinates.
    /// </returns>
    private bool IsIslandInBetweenSourceAndTarget(IIslandViewModel source, IIslandViewModel target)
    {
        var connectionType = source.GetConnectionType(target);

        switch (connectionType)
        {
            case ConnectionTypeEnum.Vertical:
                {
                    var minY = (int)Math.Min(source.Coordinates.Y, target.Coordinates.Y);
                    var maxY = (int)Math.Max(source.Coordinates.Y, target.Coordinates.Y);
                    for (var y = minY + 1; y < maxY; y++)
                        if (Islands[y][(int)source.Coordinates.X].MaxConnections > 0)
                            return true;

                    break;
                }
            case ConnectionTypeEnum.Horizontal:
                {
                    var minX = (int)Math.Min(source.Coordinates.X, target.Coordinates.X);
                    var maxX = (int)Math.Max(source.Coordinates.X, target.Coordinates.X);
                    for (var x = minX + 1; x < maxX; x++)
                        if (Islands[(int)source.Coordinates.Y][x].MaxConnections > 0)
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

    /// <summary>
    ///     Checks if the potential connection between the source and target islands would collide with existing connections.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <returns>a boolean value indicating if the connection would collide.</returns>
    private bool WouldConnectionCollide(IIslandViewModel source, IIslandViewModel target)
    {
        var connectionType = source.GetConnectionType(target);
        var islands = SolutionHelper.GetAllIslandsInvolvedInConnection(source, target, Islands).Where(x => x.MaxConnections == 0);

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