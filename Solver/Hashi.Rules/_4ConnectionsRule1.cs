using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
/// Islands with a maximum of four bridges can set all connections if only two neighbors are available.
/// </summary>
public class _4ConnectionsRule1(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_4ConnectionsRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections == 4)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Let(() => validNeighbors, () => allNeighbors.Where(x => !x.MaxConnectionsReached).ToList())
            .Having(() => allNeighbors.Count == 2 && validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddMultipleConnections(island, validNeighbors));
    }
}