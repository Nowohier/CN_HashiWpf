using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island with a maximum of five connections has three neighbors then each needs one connection.
/// </summary>
public class _5ConnectionsRule1(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance.GetRequired(nameof(_5ConnectionsRule1));

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 5 && !x.MaxConnectionsReached)
            .Let(() => allNeighbors, () => Analyzer.GetAllVisibleNeighbors(island))
            .Let(() => validNeighbors, () => Analyzer.GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => allNeighbors.Count == 3 && validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}