using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public abstract class BaseRule : Rule
{
    protected abstract string RuleMessage { get; }

    /// <summary>
    ///   Gets the connectable neighbors of the source island that do not have a connection set to the source island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="allNeighbors">The visible neighbor islands.</param>
    /// <returns>connectable neighbors of the source island that do not have a connection set to the source island.</returns>
    protected static List<IIslandViewModel> GetConnectableNeighborsWithNoConnectionSetToSource(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
    {
        return allNeighbors.Where(x => !x.MaxConnectionsReached && !x.AllConnections.Any(y => y.Equals(source.Coordinates))).ToList();
    }

    /// <summary>
    ///   Checks if all islands are connected to the source island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="allNeighbors">The visible neighbor islands.</param>
    /// <returns>a boolean value indicating if all islands are connected to the source island.</returns>
    protected static bool AreAllIslandsConnectedToSource(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
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
    protected static List<IIslandViewModel> GetIslandsConnected(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
    {
        return amountConnections == null
            ? allNeighbors.Where(x => x.AllConnections.Any(y => y.Equals(source.Coordinates))).ToList()
            : allNeighbors
                .Where(x => x.AllConnections.Count(y => y.Equals(source.Coordinates)) == (int)amountConnections)
                .ToList();
    }

    /// <summary>
    ///  Gets the amount of connections to the source island from the neighbors.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="neighbors">The visible neighbor islands.</param>
    /// <returns></returns>
    protected int GetAmountConnectionsToSource(IIslandViewModel source, IEnumerable<IIslandViewModel> neighbors)
    {
        return neighbors.Sum(x => x.AllConnections.Count(y => y.Equals(source.Coordinates)));
    }

    /// <summary>
    ///   Gets the islands connected to the source island which have reached the maximum connections.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="allNeighbors">The visible neighbor islands.</param>
    /// <param name="amountConnections">The amount of connections per neighbor to the source island.</param>
    /// <returns>the islands connected to the source island which have reached the maximum connections.</returns>
    protected static List<IIslandViewModel> GetIslandsConnectedAndMaxConnectionsReached(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
    {
        return GetIslandsConnected(source, allNeighbors, amountConnections).Where(x => x.MaxConnectionsReached).ToList();
    }

    /// <summary>
    ///     Adds a connection to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <param name="connectionManager">The connection manager.</param>
    protected virtual void AddConnection(IIslandViewModel source, IIslandViewModel target,
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
    protected virtual void AddConnections(IIslandViewModel source, List<IIslandViewModel> targets,
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
    protected virtual void AddMultipleConnections(IIslandViewModel source, List<IIslandViewModel> targets,
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
    protected virtual void AddMissingConnectionsToOneTarget(IIslandViewModel source, IIslandViewModel target,
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

    private bool ExecuteAddConnection(IIslandViewModel source, IIslandViewModel target,
        IConnectionManagerViewModel connectionManager)
    {
        if (source.MaxConnectionsReached ||
            target.MaxConnectionsReached ||
            target.AllConnections.Count(x => source.Coordinates.Equals(x)) == 2 ||
            source.AllConnections.Count(x => target.Coordinates.Equals(x)) == 2) return false;

        connectionManager.AddConnection(source, target, true);
        return true;
    }

    private bool EnsureRulesAreBeingApplied(IConnectionManagerViewModel connectionManager)
    {
        if (connectionManager.AreRulesBeingApplied == false) return false;
        connectionManager.RuleMessage = RuleMessage;
        return true;
    }

    /// <summary>
    /// Finalizes the connection by refreshing the colors of the source and target islands.
    /// </summary>
    private void FinalizeConnection(IIslandViewModel source, IIslandViewModel target)
    {
        source.RefreshIslandColor();
        target.RefreshIslandColor();
    }
}