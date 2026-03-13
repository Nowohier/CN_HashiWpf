using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     Islands with a maximum of one bridge can set their connection if there is only one neighbor island accepting
///     bridges.
///     Neighbor islands with a maximum of one bridge cannot be connected to as both islands would be isolated then.
/// </summary>
public class _1ConnectionRule1(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance.GetRequired(nameof(_1ConnectionRule1));

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections == 1)
            .Let(() => validNeighbors,
                () => Analyzer.GetAllVisibleNeighbors(island).Where(x => x.MaxConnections != 1 && !x.MaxConnectionsReached)
                    .ToList())
            .Having(() => validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddConnection(island, validNeighbors.First()));
    }
}