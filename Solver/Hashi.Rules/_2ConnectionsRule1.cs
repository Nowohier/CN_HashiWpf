using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _2ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_2ConnectionsRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        List<IIslandViewModel> invalidNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => x.AllConnections.Count == 0 && x.MaxConnections == 2)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => invalidNeighbors,
                () => allNeighbors.Where(x => !x.MaxConnectionsReached && x.MaxConnections != 2).ToList())
            .Let(() => validNeighbors,
                () => allNeighbors.Where(x => x.MaxConnections == 2 && !x.MaxConnectionsReached).ToList())
            .Having(() => invalidNeighbors.Count == 0 && validNeighbors.Count == 2);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}