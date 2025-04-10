using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _3ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_3ConnectionsRule1)]!;

    public override void Define()
    {
        IIslandViewModel island = default!;
        List<IIslandViewModel> allNeighbors = default!;
        List<IIslandViewModel> allValidNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;
        var allNeighborsCount = 0;

        When()
            .Match(() => island, x => x.MaxConnections == 3 && x.AllConnections.Count < 2)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => allValidNeighbors,
                () => allNeighbors.Where(x =>
                    !x.MaxConnectionsReached && !x.AllConnections.Any(y => y.Equals(island.Coordinates))).ToList())
            .Let(() => allNeighborsCount, () => allNeighbors.Count)
            .Having(() => allNeighborsCount == 2);

        Then()
            .Do(ctx => AddConnections(island, allValidNeighbors, connectionManager));
    }
}