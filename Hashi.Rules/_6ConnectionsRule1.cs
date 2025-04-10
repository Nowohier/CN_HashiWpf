using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _6ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_6ConnectionsRule1)]!;

    public override void Define()
    {
        IIslandViewModel island = default!;
        List<IIslandViewModel> allNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;

        When()
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Match(() => island, x => x.MaxConnections == 6 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 3);

        Then()
            .Do(ctx => AddMultipleConnections(island, allNeighbors, connectionManager));
    }
}