using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of two bridges has two neighbor islands with also a maximum of two bridges, then a connection to each neighbor must be set.
/// </summary>
public class _2ConnectionsRule1(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_2ConnectionsRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        List<IIslandViewModel> invalidNeighbors = null!;

        When()
            .Match(() => island, x => x.AllConnections.Count == 0 && x.MaxConnections == 2)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Let(() => invalidNeighbors,
                () => allNeighbors.Where(x => !x.MaxConnectionsReached && x.MaxConnections != 2).ToList())
            .Let(() => validNeighbors,
                () => allNeighbors.Where(x => x.MaxConnections == 2 && !x.MaxConnectionsReached).ToList())
            .Having(() => invalidNeighbors.Count == 0 && validNeighbors.Count == 2);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}