using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class GeneralRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(GeneralRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = default!;
        List<IIslandViewModel> validNeighbors = default!;
        IConnectionManagerViewModel connectionManager = default!;

        When()
            .Match(() => island, x => x.MaxConnectionsReached == false)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => validNeighbors, () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnectionsReached == false).ToList())
            .Having(() => validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddMissingConnectionsToOneTarget(island, validNeighbors.First(), island.MaxConnections - island.AllConnections.Count, connectionManager));
    }
}