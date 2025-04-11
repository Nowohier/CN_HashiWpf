using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using Hashi.Rules.Extensions;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _4ConnectionsRule2 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_4ConnectionsRule2)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 4 && x.AreRemainingConnectionsWithinRange(2, 3))
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 3)
            .Let(() => restrictedNeighbors, () => island.GetMaxedOutConnectedNeighbors(allNeighbors, null))
            .Having(() => restrictedNeighbors.Count == 1 && island.CountConnectionsToNeighbors(restrictedNeighbors) == 1)
            .Let(() => validNeighbors, () => island.GetConnectableNeighborsWithoutConnection(allNeighbors))
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}