using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _1ConnectionRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_1ConnectionRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = default!;
        List<IIslandViewModel> validNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections == 1)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => validNeighbors, () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnections != 1).ToList())
            .Having(() => validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddConnection(island, validNeighbors.First(), connectionManager));
    }
}