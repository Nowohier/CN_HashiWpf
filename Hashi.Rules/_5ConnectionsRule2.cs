using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _5ConnectionsRule2 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_5ConnectionsRule2)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> neighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 5 && !x.MaxConnectionsReached)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => neighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => neighbors.Count == 3)
            .Let(() => restrictedNeighbors, () => GetIslandsConnectedAndMaxConnectionsReached(island, neighbors, 1))
            .Having(() => restrictedNeighbors.Count == 1);

        Then()
            .Do(ctx => AddMultipleConnections(island, neighbors.Except(restrictedNeighbors).ToList(), connectionManager));
    }
}
