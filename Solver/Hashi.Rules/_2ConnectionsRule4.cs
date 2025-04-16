using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of two bridges has a neighbor island with maximum bridges greater than one and all remaining neighbors are islands with a maximum of one bridge, a bridge must be drawn to the larger island.
/// </summary>
public class _2ConnectionsRule4(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_2ConnectionsRule4)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 2 && x.AllConnections.Count == 0)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Let(() => validNeighbors,
                () => allNeighbors.Where(x => x.MaxConnections > 2 && !x.MaxConnectionsReached).ToList())
            .Having(() => allNeighbors.Select(x => x.MaxConnections).Distinct().Count(x => x > 1) == 1 &&
                          validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddConnection(island, validNeighbors.First()));
    }
}