using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island's remaining connections equal the total available capacity of all connectable neighbors
///     minus one (where each neighbor contributes min(2, its remaining connections)), then every connectable
///     neighbor with at least two remaining connections must receive at least one connection.
///     Proof: if any such neighbor (capacity 2) receives zero, the remaining capacity drops below the demand.
///     Neighbors with only one remaining connection may or may not receive a bridge.
/// </summary>
public class _9GeneralRule6(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule6)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> connectableNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections > 0)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Let(() => connectableNeighbors, () => GetConnectableNeighbors(allNeighbors))
            .Having(() => connectableNeighbors.Count >= 2 &&
                          connectableNeighbors.Any(x => x.RemainingConnections == 1) &&
                          island.RemainingConnections ==
                          connectableNeighbors.Sum(x => Math.Min(2, x.RemainingConnections)) - 1)
            .Let(() => validNeighbors,
                () => GetConnectableNeighborsWithoutConnection(island, allNeighbors)
                    .Where(x => x.RemainingConnections >= 2).ToList())
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}
