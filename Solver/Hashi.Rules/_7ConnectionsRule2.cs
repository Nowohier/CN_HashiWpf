using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island with a maximum of seven bridges has four neighbors and one maxed-out neighbor
///     accounts for only one connection, the remaining six bridges must go to three neighbors
///     (max 2 each). Since 3 × 2 = 6, each remaining neighbor must receive exactly two connections.
/// </summary>
public class _7ConnectionsRule2(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_7ConnectionsRule2)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 7 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Having(() => allNeighbors.Count == 4)
            .Let(() => restrictedNeighbors, () => GetMaxedOutConnectedNeighbors(island, allNeighbors, 1))
            .Having(() => restrictedNeighbors.Count == 1)
            .Let(() => validNeighbors,
                () => GetConnectableNeighbors(allNeighbors).Except(restrictedNeighbors).ToList())
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddMultipleConnections(island, validNeighbors));
    }
}
