using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     Islands with only one active neighbor can set their remaining connections to that neighbor.
/// </summary>
public class _9GeneralRule1(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance.GetRequired(nameof(_9GeneralRule1));

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached)
            .Let(() => validNeighbors,
                () => Analyzer.GetAllVisibleNeighbors(island).Where(x => !x.MaxConnectionsReached).ToList())
            .Having(() => validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddMissingConnectionsToOneTarget(island, validNeighbors.First(),
                island.MaxConnections - island.AllConnections.Count));
    }
}