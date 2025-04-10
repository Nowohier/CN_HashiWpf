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
        IIslandViewModel island = default!;
        List<IIslandViewModel> allNeighbors = default!;
        List<IIslandViewModel> validNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections == 2)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => allNeighbors.Where(x => x.MaxConnections == 2).ToList())
            .Having(() => allNeighbors.Count == 2 && validNeighbors.Count() == 2);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}