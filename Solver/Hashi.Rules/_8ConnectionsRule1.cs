using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island with a maximum of eight connections has four neighbors then set all connections.
/// </summary>
public class _8ConnectionsRule1(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance.GetRequired(nameof(_8ConnectionsRule1));

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 8 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => Analyzer.GetAllVisibleNeighbors(island))
            .Having(() => allNeighbors.Count == 4);

        Then()
            .Do(ctx => AddMultipleConnections(island, allNeighbors));
    }
}