using Hashi.Enums;
using Hashi.Gui.Core.Models;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Generator.Simulation;

/// <summary>
///     A lightweight simulation implementation of <see cref="IIslandViewModel" /> used for rule-based solvability
///     validation.
///     This class does not depend on WPF and provides the minimal behavior needed for NRules to evaluate hint rules.
/// </summary>
internal class SimulationIslandViewModel : IIslandViewModel
{
    private readonly IIslandViewModelHelper helper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SimulationIslandViewModel" /> class.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="maxConnections">The maximum number of connections (bridge count) for this island.</param>
    /// <param name="helper">The island view model helper.</param>
    public SimulationIslandViewModel(int x, int y, int maxConnections, IIslandViewModelHelper helper)
    {
        this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
        MaxConnections = maxConnections;
        Coordinates = new BasicHashiPoint(x, y);
        BrushResolver = new SimulationHashiBrushResolver();
        IslandColor = BrushResolver.ResolveBrush(HashiColor.LightBlueBrush);
    }

    /// <inheritdoc />
    public IHashiBrush IslandColor { get; set; }

    /// <inheritdoc />
    public bool IsHighlightHorizontalLeft { get; set; }

    /// <inheritdoc />
    public bool IsHighlightHorizontalRight { get; set; }

    /// <inheritdoc />
    public bool IsHighlightVerticalTop { get; set; }

    /// <inheritdoc />
    public bool IsHighlightVerticalBottom { get; set; }

    /// <inheritdoc />
    public ObservableCollection<IHashiPoint> AllConnections { get; } = [];

    /// <inheritdoc />
    public bool MaxConnectionsReached => AllConnections.Count >= MaxConnections;

    /// <inheritdoc />
    public List<IHashiPoint> BridgesLeft => helper.GetBridgesLeft(this);

    /// <inheritdoc />
    public List<IHashiPoint> BridgesRight => helper.GetBridgesRight(this);

    /// <inheritdoc />
    public List<IHashiPoint> BridgesUp => helper.GetBridgesUp(this);

    /// <inheritdoc />
    public List<IHashiPoint> BridgesDown => helper.GetBridgesDown(this);

    /// <inheritdoc />
    public int MaxConnections { get; }

    /// <inheritdoc />
    public int RemainingConnections => MaxConnections - AllConnections.Count;

    /// <inheritdoc />
    public IHashiPoint Coordinates { get; }

    /// <inheritdoc />
    public IHashiBrushResolver BrushResolver { get; }

    /// <inheritdoc />
    public void ResetDropTarget()
    {
        // No-op in simulation
    }

    /// <inheritdoc />
    public void RefreshIslandColor()
    {
        // No-op in simulation
    }

    /// <inheritdoc />
    public ConnectionTypeEnum GetConnectionType(IIslandViewModel targetIsland)
    {
        return helper.GetConnectionType(this, targetIsland);
    }

    /// <inheritdoc />
    public void NotifyBridgeConnections()
    {
        // No-op in simulation
    }

    /// <inheritdoc />
    public void AddConnection(IHashiPoint connection)
    {
        AllConnections.Add(connection);
    }

    /// <inheritdoc />
    public void RemoveAllConnectionsMatchingCoordinates(IHashiPoint connection)
    {
        var toRemove = AllConnections.Where(c => c.X == connection.X && c.Y == connection.Y).ToList();
        foreach (var item in toRemove)
        {
            AllConnections.Remove(item);
        }
    }

    /// <inheritdoc />
    public bool IsValidDropTarget(IIslandViewModel? target)
    {
        return helper.IsValidDropTarget(this, target);
    }

    /// <inheritdoc />
    public bool? MaxBridgesReachedToTarget(IIslandViewModel? target)
    {
        return helper.MaxBridgesReachedToTarget(this, target);
    }
}
