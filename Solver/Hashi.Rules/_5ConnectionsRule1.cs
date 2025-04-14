using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of five connections has three neighbors then each needs one connection.
/// </summary>
public class _5ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_5ConnectionsRule1)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IIslandProvider islandProvider = null!;

        When()
            .Query(() => islandProvider, q => q.Match<IIslandProvider>())
            .Match(() => island, x => x.MaxConnections == 5 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => allNeighbors.Count == 3 && validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, islandProvider));
    }
}