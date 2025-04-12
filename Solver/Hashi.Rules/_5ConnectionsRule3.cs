using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using Hashi.Rules.Extensions;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _5ConnectionsRule3 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_5ConnectionsRule3)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 5 && !x.MaxConnectionsReached)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 4)
            .Let(() => restrictedNeighbors, () => island.GetMaxedOutConnectedNeighbors(allNeighbors, null))
            .Having(() => restrictedNeighbors.Count == 2 && island.CountConnectionsToNeighbors(restrictedNeighbors) <= 3)
            .Let(() => validNeighbors, () => island.GetConnectableNeighborsWithoutConnection(allNeighbors))
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}
