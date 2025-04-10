using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public abstract class BaseRule : Rule
{
    protected abstract string RuleMessage { get; }

    /// <summary>
    ///     Adds a connection to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    /// <param name="connectionManager">The connection manager.</param>
    protected virtual void AddConnection(IIslandViewModel source, IIslandViewModel target,
        IConnectionManagerViewModel connectionManager)
    {
        if (!AreRulesBeingApplied(connectionManager)) return;
        ExecuteAddConnection(source, target, connectionManager);
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
        if (!AreRulesBeingApplied(connectionManager)) return;

        foreach (var target in targets) ExecuteAddConnection(source, target, connectionManager);
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
        if (!AreRulesBeingApplied(connectionManager)) return;

        foreach (var target in targets)
        {
            ExecuteAddConnection(source, target, connectionManager);
            ExecuteAddConnection(source, target, connectionManager);
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
        if (!AreRulesBeingApplied(connectionManager)) return;

        for (var i = 0; i < missingConnectionsCount; i++)
            if (!ExecuteAddConnection(source, target, connectionManager))
                break;
    }

    private bool ExecuteAddConnection(IIslandViewModel source, IIslandViewModel target,
        IConnectionManagerViewModel connectionManager)
    {
        if (source.MaxConnectionsReached ||
            target.MaxConnectionsReached ||
            target.AllConnections.Count(x => source.Coordinates.Equals(x)) == 2 ||
            source.AllConnections.Count(x => target.Coordinates.Equals(x)) == 2) return false;

        connectionManager.AddConnection(source, target, true);
        target.RefreshIslandColor();
        source.RefreshIslandColor();
        return true;
    }

    private bool AreRulesBeingApplied(IConnectionManagerViewModel connectionManager)
    {
        if (connectionManager.AreRulesBeingApplied == false) return false;
        connectionManager.AreRulesBeingApplied = false;
        connectionManager.RuleMessage = RuleMessage;
        return true;
    }
}