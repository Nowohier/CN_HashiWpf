using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island with a maximum of three bridges has three neighbors, and one is a 1-connection and another a
///     2-connection island then a connection must be set to the remaining neighbor.
/// </summary>
public class _3ConnectionsRule2(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance.GetRequired(nameof(_3ConnectionsRule2));

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 3 && x.AllConnections.Count <= 2)
            .Let(() => allNeighbors, () => Analyzer.GetAllVisibleNeighbors(island))
            .Having(() => allNeighbors.Count == 3 && allNeighbors.Count(x => x.MaxConnections == 1) == 1 &&
                          allNeighbors.Count(x => x.MaxConnections == 2) == 1)
            .Let(() => validNeighbors,
                () => Analyzer.GetConnectableNeighborsWithoutConnection(island, allNeighbors).Where(x => x.MaxConnections > 2)
                    .ToList())
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}