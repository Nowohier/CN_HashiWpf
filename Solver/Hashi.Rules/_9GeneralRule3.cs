using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
///     If an island's remaining connections equal (2 × connectable neighbors) − 1, then every connectable
///     neighbor must receive at least one connection. This is the general pigeonhole principle: since removing
///     any single neighbor would leave insufficient capacity, every neighbor must participate.
/// </summary>
public class _9GeneralRule3(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule3)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> connectableNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections > 0)
            .Let(() => allNeighbors, () => GetAllVisibleNeighbors(island))
            .Let(() => connectableNeighbors, () => GetConnectableNeighbors(allNeighbors))
            .Having(() => connectableNeighbors.Count >= 2 &&
                          island.RemainingConnections == 2 * connectableNeighbors.Count - 1)
            .Let(() => validNeighbors, () => GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors));
    }
}
