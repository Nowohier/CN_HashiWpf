using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _5ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_5ConnectionsRule1)]!;

    public override void Define()
    {
        IIslandViewModel island = default!;
        List<IIslandViewModel> allNeighbors = default!;
        List<IIslandViewModel> validNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;

        When()
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Match(() => island, x => x.MaxConnections == 5 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => allNeighbors.Where(x => !x.AllConnections.Contains(island.Coordinates)).ToList())
            .Having(() => allNeighbors.Count == 3 && !allNeighbors.All(x => x.AllConnections.Contains(island.Coordinates)));

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}