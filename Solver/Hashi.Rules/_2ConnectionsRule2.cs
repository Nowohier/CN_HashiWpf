using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of two bridges has a connection to another island with max one connection, then the other connection can only go to a neighbor with max connections > 1.
/// </summary>
public class _2ConnectionsRule2(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_2ConnectionsRule2)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        var isSingleConnectionSetToMaxOneNeighbor = false;

        When()
            .Match(() => island, x => x.MaxConnections == 2 && x.AllConnections.Count == 1)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => allNeighbors.Where(x => x.MaxConnections > 1).ToList())
            .Let(() => isSingleConnectionSetToMaxOneNeighbor,
                () => allNeighbors.Count(x => x.AllConnections.Contains(island.Coordinates) && x.MaxConnections == 1) ==
                      1)
            .Having(() => allNeighbors.Count > 1 && validNeighbors.Count == 1 && isSingleConnectionSetToMaxOneNeighbor);

        Then()
            .Do(ctx => AddConnection(island, validNeighbors.First()));
    }
}