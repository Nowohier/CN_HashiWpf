using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _6ConnectionsRule2 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_6ConnectionsRule2)]!;

    public override void Define()
    {
        //ToDo: Check this rule!!!
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Match(() => island, x => x.MaxConnections == 6 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => !allNeighbors.All(x => x.AllConnections.Any(y => y.Equals(island.Coordinates))))
            .Let(() => restrictedNeighbors, () => allNeighbors.Where(x => x.MaxConnectionsReached && x.AllConnections.Count(y => y.Equals(island.Coordinates)) == 1).ToList())
            .Having(() => allNeighbors.Count == 4 && restrictedNeighbors.Count == 1)
            .Let(() => validNeighbors, () => allNeighbors.Except(restrictedNeighbors).Where(x => !x.MaxConnectionsReached && !x.AllConnections.Any(y => y.Equals(island.Coordinates))).ToList());

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}