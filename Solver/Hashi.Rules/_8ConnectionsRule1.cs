using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of eight connections has four neighbors then set all connections.
/// </summary>
public class _8ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_8ConnectionsRule1)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        IIslandProvider islandProvider = null!;

        When()
            .Query(() => islandProvider, q => q.Match<IIslandProvider>())
            .Match(() => island, x => x.MaxConnections == 8 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 4);

        Then()
            .Do(ctx => AddMultipleConnections(island, allNeighbors, islandProvider));
    }
}