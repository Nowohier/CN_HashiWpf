using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _9GeneralRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel? island = null!;
        List<IIslandViewModel?> validNeighbors = null!;
        IConnectionManagerViewModel? connectionManager = null!;

        When()
            .Match(() => island, x => x.MaxConnectionsReached == false)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => validNeighbors, () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnectionsReached == false).ToList())
            .Having(() => validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddMissingConnectionsToOneTarget(island, validNeighbors.First(), island.MaxConnections - island.AllConnections.Count, connectionManager));
    }
}