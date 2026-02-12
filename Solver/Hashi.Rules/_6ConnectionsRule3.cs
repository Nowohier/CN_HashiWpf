using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island with a maximum of six bridges has four neighbors and one neighbor has a maximum of one bridge,
///     then the remaining three neighbors must each receive two connections. Six bridges minus at most one for the
///     restricted neighbor leaves five for three neighbors (max 2 each), so each must get at least one connection.
///     Since the restricted neighbor takes at most one, five bridges remain, meaning each of the three gets at
///     least one (ceil(5/3)=2 for two of them).
/// </summary>
public class _6ConnectionsRule3(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_6ConnectionsRule3)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 6 && x.AllConnections.Count == 0)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Having(() => allNeighbors.Count == 4)
            .Let(() => restrictedNeighbors,
                () => allNeighbors.Where(x => x.MaxConnections == 1 && !x.MaxConnectionsReached).ToList())
            .Having(() => restrictedNeighbors.Count == 1)
            .Let(() => validNeighbors,
                () => allNeighbors.Where(x => x.MaxConnections > 1 && !x.MaxConnectionsReached).ToList())
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}
