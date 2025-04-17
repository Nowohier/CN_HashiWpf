using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island with a maximum of two connections has two neighbor islands and one of them is an island with a maximum
///     of two connections, a connection must be established to the other island.
/// </summary>
public class _2ConnectionsRule3(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_2ConnectionsRule3)]!;

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
            .Having(() =>
                allNeighbors.Count == 2 &&
                allNeighbors.Count(x => x.MaxConnections <= 2 && !x.MaxConnectionsReached) == 1 &&
                validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddConnection(island, validNeighbors.First()));
    }
}