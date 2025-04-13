using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _4ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_4ConnectionsRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections == 4)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => allNeighbors.Where(x => !x.MaxConnectionsReached).ToList())
            .Having(() => allNeighbors.Count == 2 && validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddMultipleConnections(island, validNeighbors, connectionManager));
    }
}