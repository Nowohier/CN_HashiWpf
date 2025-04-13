using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public abstract class BaseRule : Rule
{
    protected abstract string RuleMessage { get; }

    internal virtual bool EnsureRulesAreBeingApplied(IConnectionManagerViewModel connectionManager)
    {
        if (connectionManager.AreRulesBeingApplied == false) return false;
        connectionManager.RuleMessage = RuleMessage;
        return true;
    }

    /// <summary>
    ///     Adds a connection to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <param name="connectionManager">The connection manager.</param>
    internal virtual void AddConnection(IIslandViewModel source, IIslandViewModel target,
        IConnectionManagerViewModel connectionManager)
    {
        if (EnsureRulesAreBeingApplied(connectionManager) && ExecuteAddConnection(source, target, connectionManager))
        {
            FinalizeConnection(source, target);
        }
    }

    /// <summary>
    ///     Adds multiple connections to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="targets">The target islands.</param>
    /// <param name="connectionManager">The connection manager.</param>
    internal virtual void AddConnections(IIslandViewModel source, List<IIslandViewModel> targets,
        IConnectionManagerViewModel connectionManager)
    {
        if (!EnsureRulesAreBeingApplied(connectionManager)) return;

        foreach (var target in targets)
        {
            if (ExecuteAddConnection(source, target, connectionManager))
            {
                FinalizeConnection(source, target);
            }
        }
    }

    /// <summary>
    ///     Adds multiple connections to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="targets">The target islands.</param>
    /// <param name="connectionManager">The connection manager.</param>
    internal virtual void AddMultipleConnections(IIslandViewModel source, List<IIslandViewModel> targets,
        IConnectionManagerViewModel connectionManager)
    {
        if (!EnsureRulesAreBeingApplied(connectionManager)) return;

        foreach (var target in targets)
        {
            for (var i = 0; i < 2; i++)
            {
                if (!ExecuteAddConnection(source, target, connectionManager)) break;
            }
            FinalizeConnection(source, target);
        }
    }

    /// <summary>
    ///     Adds multiple connections to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <param name="missingConnectionsCount">Amount of connections missing.</param>
    /// <param name="connectionManager">The connection manager.</param>
    internal virtual void AddMissingConnectionsToOneTarget(IIslandViewModel? source, IIslandViewModel? target,
        int missingConnectionsCount,
        IConnectionManagerViewModel connectionManager)
    {
        if (!EnsureRulesAreBeingApplied(connectionManager)) return;

        for (var i = 0; i < missingConnectionsCount; i++)
        {
            if (!ExecuteAddConnection(source, target, connectionManager)) break;
        }
        FinalizeConnection(source, target);
    }

    /// <summary>
    /// Finalizes the connection by refreshing the colors of the source and target islands.
    /// </summary>
    internal void FinalizeConnection(IIslandViewModel? source, IIslandViewModel? target)
    {
        source?.RefreshIslandColor();
        target?.RefreshIslandColor();
    }

    internal bool ExecuteAddConnection(IIslandViewModel? source, IIslandViewModel? target,
        IConnectionManagerViewModel? connectionManager)
    {
        if (source == null || target == null || source == target || connectionManager == null || source.MaxConnectionsReached ||
            target.MaxConnectionsReached ||
            target.AllConnections.Count(x => source.Coordinates.Equals(x)) == 2 ||
            source.AllConnections.Count(x => target.Coordinates.Equals(x)) == 2) return false;

        connectionManager.AddConnection(source, target, true);
        return true;
    }

    /// <summary>
    ///   Gets the connectable neighbors of the source island that do not have a connection set to the source island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="allNeighbors">The visible neighbor islands.</param>
    /// <returns>connectable neighbors of the source island that do not have a connection set to the source island.</returns>
    internal List<IIslandViewModel> GetConnectableNeighborsWithoutConnection(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
    {
        return allNeighbors.Where(x => !x.MaxConnectionsReached && !x.AllConnections.Any(y => y.X == source.Coordinates.X && y.Y == source.Coordinates.Y)).ToList();
    }

    /// <summary>
    ///   Checks if all islands are connected to the source island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="allNeighbors">The visible neighbor islands.</param>
    /// <returns>a boolean value indicating if all islands are connected to the source island.</returns>
    internal bool AreAllNeighborsConnected(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
    {
        return allNeighbors.All(x => x.AllConnections.Contains(source.Coordinates));
    }

    /// <summary>
    ///   Gets the islands connected to the source island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="allNeighbors">The visible neighbor islands.</param>
    /// <param name="amountConnections">(optional) The amount of connections per neighbor to the source island.</param>
    /// <returns>the islands connected by one connection to the source island.</returns>
    internal List<IIslandViewModel> GetConnectedNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
    {
        if (amountConnections == null)
        {
            var result = allNeighbors.Where(x => x.AllConnections.Any(y => DoCoordinatesMatch(source.Coordinates, y))).ToList();
            return result;
        }
        else
        {
            var result = allNeighbors
                .Where(x => x.AllConnections.Count(y => DoCoordinatesMatch(source.Coordinates, y)) == (int)amountConnections)
                .ToList();
            return result;
        }
    }

    /// <summary>
    ///  Gets the amount of connections to the source island from the neighbors.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="neighbors">The visible neighbor islands.</param>
    /// <returns></returns>
    internal int CountConnectionsToNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> neighbors)
    {
        var result = neighbors.Sum(x => x.AllConnections.Count(y => DoCoordinatesMatch(source.Coordinates, y)));
        return result;
    }

    /// <summary>
    /// Checks if the remaining connections of the island are within the range of the two values.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="minValue">The first value.</param>
    /// <param name="maxValue">The second value.</param>
    /// <returns></returns>
    internal bool AreRemainingConnectionsWithinRange(IIslandViewModel source, int minValue, int maxValue)
    {
        var result = source.RemainingConnections >= minValue && source.RemainingConnections <= maxValue;
        return result;
    }

    /// <summary>
    ///   Gets the islands connected to the source island which have reached the maximum connections.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="allNeighbors">The visible neighbor islands.</param>
    /// <param name="amountConnections">The amount of connections per neighbor to the source island.</param>
    /// <returns>the islands connected to the source island which have reached the maximum connections.</returns>
    internal List<IIslandViewModel> GetMaxedOutConnectedNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
    {
        var result = GetConnectedNeighbors(source, allNeighbors, amountConnections).Where(x => x.MaxConnectionsReached)
            .ToList();
        return result;
    }

    private bool DoCoordinatesMatch(IHashiPoint source, IHashiPoint target)
    {
        return source.X == target.X && source.Y == target.Y;
    }
}