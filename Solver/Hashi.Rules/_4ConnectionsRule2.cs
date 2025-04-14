using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
/// Islands with a maximum of four bridges and three neighbors can set one connection to each neighbor if one of the neighbors already has a connection to the island and has max connections.
/// </summary>
public class _4ConnectionsRule2(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_4ConnectionsRule2)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 4)
            .Having(() => AreRemainingConnectionsWithinRange(island, 2, 4))
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Having(() => allNeighbors.Count == 3)
            .Let(() => restrictedNeighbors, () => GetMaxedOutConnectedNeighbors(island, allNeighbors, null))
            .Having(() =>
                restrictedNeighbors.Count == 1 && CountConnectionsToNeighbors(island, restrictedNeighbors) == 1)
            .Let(() => validNeighbors, () => GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}