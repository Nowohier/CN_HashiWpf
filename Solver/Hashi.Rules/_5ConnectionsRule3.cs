using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// If a 5-connection island has four neighbors and two full neighbors have a total of no more than three connections to the island, a connection must be drawn to each of the remaining neighbors if no connection has already been established.
/// </summary>
public class _5ConnectionsRule3 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_5ConnectionsRule3)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IIslandProvider islandProvider = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 5 && !x.MaxConnectionsReached)
            .Query(() => islandProvider, q => q.Match<IIslandProvider>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 4)
            .Let(() => restrictedNeighbors, () => GetMaxedOutConnectedNeighbors(island, allNeighbors, null))
            .Having(() =>
                restrictedNeighbors.Count == 2 && CountConnectionsToNeighbors(island, restrictedNeighbors) <= 3)
            .Let(() => validNeighbors, () => GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, islandProvider));
    }
}