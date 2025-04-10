using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _4ConnectionsRule2 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_4ConnectionsRule2)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = default!;
        List<IIslandViewModel> allNeighbors = default!;
        List<IIslandViewModel> validNeighbors = default!;
        List<IIslandViewModel> filteredNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;

        When()
            .Match(() => island,
                x => !x.MaxConnectionsReached && x.MaxConnections == 4 &&
                     (x.MaxConnections - x.AllConnections.Count >= 2 || x.MaxConnections - x.AllConnections.Count <= 3))
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => allNeighbors.Where(x => !x.MaxConnectionsReached).ToList())
            .Let(() => filteredNeighbors,
                () => validNeighbors.Where(x => !x.AllConnections.Contains(island.Coordinates)).ToList())
            .Having(() => allNeighbors.Count == 3 &&
                          validNeighbors.Count == 2 &&
                          filteredNeighbors.Count > 0 &&
                          allNeighbors.Count(x =>
                              x.MaxConnectionsReached && x.AllConnections.Contains(island.Coordinates)) == 1);

        Then()
            .Do(ctx => AddConnections(island, filteredNeighbors, connectionManager));
    }
}