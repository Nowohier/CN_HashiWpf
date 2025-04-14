using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// If a 6-connection island has four neighbors and already has a 1-connection to one of these islands, which no longer allows further connections, then every other island must receive at least one connection.
/// </summary>
public class _6ConnectionsRule2 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_6ConnectionsRule2)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Match(() => island, x => x.MaxConnections == 6 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 4)
            .Let(() => restrictedNeighbors, () => GetMaxedOutConnectedNeighbors(island, allNeighbors, 1))
            .Having(() => restrictedNeighbors.Count == 1)
            .Let(() => validNeighbors,
                () => GetConnectableNeighborsWithoutConnection(island, allNeighbors).Except(restrictedNeighbors)
                    .ToList())
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}