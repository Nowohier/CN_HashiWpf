using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _7ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_7ConnectionsRule1)]!;

    public override void Define()
    {
        IIslandViewModel island = default!;
        List<IIslandViewModel> allNeighbors = default!;
        List<IIslandViewModel> validNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;

        When()
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Match(() => island, x => x.MaxConnections == 7 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => allNeighbors.Where(x => !x.AllConnections.Contains(island.Coordinates)).ToList())
            .Having(() => allNeighbors.Count == 4 && !allNeighbors.All(x => x.AllConnections.Contains(island.Coordinates)));

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}