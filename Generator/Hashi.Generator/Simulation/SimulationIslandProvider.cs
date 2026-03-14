using Hashi.Enums;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Extensions;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Generator.Simulation;

/// <summary>
///     A lightweight simulation implementation of <see cref="IIslandProvider" /> used for rule-based solvability
///     validation.
///     This class reproduces the neighbor discovery and connection management logic without WPF dependencies.
/// </summary>
internal class SimulationIslandProvider : IIslandProvider
{
    private readonly IIslandProviderCore core;
    private readonly IIslandViewModelHelper helper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SimulationIslandProvider" /> class.
    /// </summary>
    /// <param name="core">The shared island provider core logic.</param>
    /// <param name="helper">The island view model helper.</param>
    public SimulationIslandProvider(IIslandProviderCore core, IIslandViewModelHelper helper)
    {
        this.core = core ?? throw new ArgumentNullException(nameof(core));
        this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
    }

    /// <inheritdoc />
    public ObservableCollection<ObservableCollection<IIslandViewModel>> Islands { get; } = [];

    /// <inheritdoc />
    public IEnumerable<IIslandViewModel> IslandsFlat => Islands.SelectMany(row => row);

    /// <summary>
    ///     Initializes the simulation grid from a puzzle field.
    /// </summary>
    /// <param name="hashiField">The puzzle field where each cell value represents the bridge count (0 = empty).</param>
    public void InitializeFromField(int[][] hashiField)
    {
        Islands.Clear();

        for (var row = 0; row < hashiField.Length; row++)
        {
            var rowCollection = new ObservableCollection<IIslandViewModel>();
            for (var column = 0; column < hashiField[0].Length; column++)
            {
                rowCollection.Add(new SimulationIslandViewModel(column, row, hashiField[row][column], helper));
            }

            Islands.Add(rowCollection);
        }
    }

    /// <summary>
    ///     Checks whether all island connections have been satisfied.
    /// </summary>
    /// <returns><c>true</c> if all islands with bridges required have their max connections reached.</returns>
    public bool AreAllConnectionsSet =>
        IslandsFlat.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached);

    /// <inheritdoc />
    public void InitializeNewSolution(ISolutionProvider solutionProvider)
    {
        // Not used in simulation
    }

    /// <inheritdoc />
    public void InitializeNewSolutionAndSetBridges(ISolutionProvider solutionProvider)
    {
        // Not used in simulation
    }

    /// <inheritdoc />
    public void RemoveAllHighlights()
    {
        // No-op in simulation
    }

    /// <inheritdoc />
    public void RefreshIslandColors()
    {
        // No-op in simulation
    }

    /// <inheritdoc />
    public void ClearTemporaryDropTargets()
    {
        // No-op in simulation
    }

    /// <inheritdoc />
    public void HighlightPathToTargetIsland(IIslandViewModel source, IIslandViewModel target)
    {
        // No-op in simulation
    }

    /// <inheritdoc />
    public int CountIsolatedIslandGroups()
    {
        return core.CountIsolatedIslandGroups(Islands, IslandsFlat,
            island => core.GetAllVisibleNeighbors(Islands, island));
    }

    /// <inheritdoc />
    public IList<IHashiBridge> History { get; } = new List<IHashiBridge>();

    /// <inheritdoc />
    public IList<IHashiBridge> RedoHistory { get; } = new List<IHashiBridge>();

    /// <inheritdoc />
    public void UndoConnection()
    {
        // Not used in simulation
    }

    /// <inheritdoc />
    public void RedoConnection()
    {
        // Not used in simulation
    }

    /// <inheritdoc />
    public void RemoveAllBridges(HashiPointTypeEnum pointType)
    {
        foreach (var island in IslandsFlat)
        {
            foreach (var hashiPoint in island.AllConnections.GetConnectionsByPointType(pointType).ToList())
                island.AllConnections.Remove(hashiPoint);
        }
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
    public void AddConnection(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland,
        HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal)
    {
        if (sourceIsland == null || targetIsland == null)
        {
            return;
        }

        if (!core.IsValidConnectionBetweenSourceAndTarget(Islands, sourceIsland, targetIsland))
        {
            return;
        }

        if (helper.MaxBridgesReachedToTarget(sourceIsland, targetIsland) is true)
        {
            return;
        }

        core.ManageConnections(Islands, sourceIsland, targetIsland,
            (island, coordinates) => island.AddConnection(coordinates),
            pointType);
    }

    /// <inheritdoc />
    public void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland)
    {
        if (sourceIsland == null)
        {
            return;
        }

        if (targetIsland == null)
        {
            return;
        }

        core.ManageConnections(Islands, sourceIsland, targetIsland,
            (island, coordinates) => island.RemoveAllConnectionsMatchingCoordinates(coordinates));
    }

    /// <inheritdoc />
    public void Receive(IUpdateAllIslandColorsMessage message)
    {
        // No-op in simulation
    }
}
