using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island's remaining connections exactly equal the total available capacity of all connectable
///     neighbors (where each neighbor contributes min(2, its remaining connections)), and at least one
///     neighbor has only one remaining connection, then every connectable neighbor must receive at least
///     one connection. This extends the maximum saturation rule to account for neighbors with limited capacity.
/// </summary>
public class _9GeneralRule5(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule5)]!;

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
                          connectableNeighbors.Sum(x => Math.Min(2, x.RemainingConnections)))
            .Let(() => validNeighbors, () => GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}
