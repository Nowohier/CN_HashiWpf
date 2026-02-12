using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island's remaining connections exactly equal 2 × the number of connectable neighbors,
///     then every connectable neighbor must receive exactly two connections. This is the maximum
///     saturation case: the only possible distribution is two bridges per neighbor.
/// </summary>
public class _9GeneralRule4(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule4)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> connectableNeighbors = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections > 0)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Let(() => connectableNeighbors, () => GetConnectableNeighbors(allNeighbors))
            .Having(() => connectableNeighbors.Count >= 2 &&
                          island.RemainingConnections == 2 * connectableNeighbors.Count);

        Then()
            .Do(ctx => AddMultipleConnections(island, connectableNeighbors));
    }
}
