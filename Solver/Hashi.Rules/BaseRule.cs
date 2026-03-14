using Hashi.Enums;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Helpers;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
///     Base class for all rules.
/// </summary>
public abstract class BaseRule : Rule
{
    private readonly IIslandProvider islandProvider;
    private readonly IRuleInfoProvider ruleInfoProvider;

    /// <summary>
    ///     Base class for all rules.
    /// </summary>
    protected BaseRule(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    {
        ArgumentNullException.ThrowIfNull(ruleInfoProvider);
        ArgumentNullException.ThrowIfNull(islandProvider);

        this.ruleInfoProvider = ruleInfoProvider;
        this.islandProvider = islandProvider;
        Analyzer = new RuleNeighborAnalyzer(islandProvider);
    }

    /// <summary>
    ///     Provides query and analysis methods for neighbor inspection.
    /// </summary>
    protected IRuleNeighborAnalyzer Analyzer { get; }

    /// <summary>
    ///     The message to be displayed when the rule is applied.
    /// </summary>
    protected abstract string RuleMessage { get; }

    internal virtual bool EnsureRulesAreBeingApplied()
    {
        if (ruleInfoProvider.AreRulesBeingApplied == false)
        {
            return false;
        }

        ruleInfoProvider.RuleMessage = RuleMessage;
        return true;
    }

    /// <summary>
    ///     Adds a connection to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="target">The target island.</param>
    internal virtual void AddConnection(IIslandViewModel source, IIslandViewModel target)
    {
        if (EnsureRulesAreBeingApplied() && ExecuteAddConnection(source, target))
        {
            FinalizeConnection(source, target);
        }
    }

    /// <summary>
    ///     Adds multiple connections to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="targets">The target islands.</param>
    internal virtual void AddConnections(IIslandViewModel source, List<IIslandViewModel> targets)
    {
        if (!EnsureRulesAreBeingApplied())
        {
            return;
        }

        foreach (var target in targets)
            if (ExecuteAddConnection(source, target))
            {
                FinalizeConnection(source, target);
            }
    }

    /// <summary>
    ///     Adds multiple connections to an island and stops the rule execution of all rules.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="targets">The target islands.</param>
    internal virtual void AddMultipleConnections(IIslandViewModel source, List<IIslandViewModel> targets)
    {
        if (!EnsureRulesAreBeingApplied())
        {
            return;
        }

        foreach (var target in targets)
        {
            for (var i = 0; i < 2; i++)
            {
                if (!ExecuteAddConnection(source, target))
                {
                    break;
                }
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
    internal virtual void AddMissingConnectionsToOneTarget(IIslandViewModel? source, IIslandViewModel? target,
        int missingConnectionsCount)
    {
        if (!EnsureRulesAreBeingApplied())
        {
            return;
        }

        for (var i = 0; i < missingConnectionsCount; i++)
        {
            if (!ExecuteAddConnection(source, target))
            {
                break;
            }
        }

        FinalizeConnection(source, target);
    }

    /// <summary>
    ///     Finalizes the connection by refreshing the colors of the source and target islands.
    /// </summary>
    internal void FinalizeConnection(IIslandViewModel? source, IIslandViewModel? target)
    {
        source?.RefreshIslandColor();
        target?.RefreshIslandColor();
    }

    internal bool ExecuteAddConnection(IIslandViewModel? source, IIslandViewModel? target)
    {
        if (source == null || target == null || source == target ||
            source.MaxConnectionsReached ||
            target.MaxConnectionsReached ||
            target.AllConnections.Count(x => Analyzer.DoCoordinatesMatch(source.Coordinates, x)) == RuleConstants.MaxBridgesPerConnection ||
            source.AllConnections.Count(x => Analyzer.DoCoordinatesMatch(target.Coordinates, x)) == RuleConstants.MaxBridgesPerConnection)
        {
            return false;
        }

        islandProvider.AddConnection(source, target, HashiPointTypeEnum.Hint);
        return true;
    }

    /// <summary>
    ///     Sets a test connection between the source island and its connectable neighbors. If the source island and its
    ///     connected neighbors are maxed out, it checks if there are isolated groups. If so, it returns a free neighbor.
    /// </summary>
    internal IIslandViewModel? SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(
        IIslandViewModel source,
        List<IIslandViewModel> connectableNeighbors,
        List<IIslandViewModel> allNeighbors)
    {
        foreach (var neighbor in connectableNeighbors)
        {
            // Add a test connection
            islandProvider.AddConnection(source, neighbor, HashiPointTypeEnum.Test);

            // Check if the source and its connected neighbors are maxed out
            var connectedNeighbors = Analyzer.GetConnectedNeighbors(source, allNeighbors, null);
            if (source.MaxConnectionsReached && connectedNeighbors.All(x => x.MaxConnectionsReached))
                // Check if there are isolated groups
            {
                if (islandProvider.CountIsolatedIslandGroups() > 0)
                {
                    // Find a free neighbor
                    var freeNeighbor = Analyzer.GetConnectableNeighbors(allNeighbors).FirstOrDefault();
                    islandProvider.RemoveAllBridges(HashiPointTypeEnum.Test);
                    return freeNeighbor;
                }
            }

            // Remove the test connection
            islandProvider.RemoveAllBridges(HashiPointTypeEnum.Test);
        }

        return null;
    }
}
