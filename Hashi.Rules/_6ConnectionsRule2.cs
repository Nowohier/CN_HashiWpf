using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using Hashi.Rules.Extensions;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

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
            .Let(() => restrictedNeighbors, () => island.GetMaxedOutConnectedNeighbors(allNeighbors, 1))
            .Having(() => restrictedNeighbors.Count == 1)
            .Let(() => validNeighbors, () => island.GetConnectableNeighborsWithoutConnection(allNeighbors).Except(restrictedNeighbors).ToList());

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}