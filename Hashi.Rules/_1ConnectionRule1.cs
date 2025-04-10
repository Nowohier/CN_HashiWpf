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
        List<IIslandViewModel> allValidNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;
        var validNeighborsCount = 0;

        When()
            .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 1)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allValidNeighbors,
                () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnections != 1).ToList())
            .Let(() => validNeighborsCount, () => allValidNeighbors.Count)
            .Having(() => validNeighborsCount == 1);

        Then()
            .Do(ctx => AddConnection(island, allValidNeighbors.First(), connectionManager));
    }
}