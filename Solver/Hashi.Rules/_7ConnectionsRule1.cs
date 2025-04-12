using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using Hashi.Rules.Extensions;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

public class _7ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_7ConnectionsRule1)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Match(() => island, x => x.MaxConnections == 7 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 4 && !island.AreAllNeighborsConnected(allNeighbors))
            .Let(() => validNeighbors, () => island.GetConnectableNeighborsWithoutConnection(allNeighbors));

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}