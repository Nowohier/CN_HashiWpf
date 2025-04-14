using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of seven connections has four neighbors then each needs one connection.
/// </summary>
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
            .Having(() => allNeighbors.Count == 4 && !AreAllNeighborsConnected(island, allNeighbors))
            .Let(() => validNeighbors, () => GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}