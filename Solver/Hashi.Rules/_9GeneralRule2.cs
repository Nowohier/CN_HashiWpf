using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
/// If setting a connection to any neighbor would result in a maximum connection reached for the island and a max connection count for all connected neighbors, check if the whole group is isolated. If it is, set the connection to the remaining neighbor.
/// </summary>
public class _9GeneralRule2(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule2)]!;

    /// <inheritdoc />
    public override void Define()
    {
        // ToDo: This rule is not yet implemented. It is a placeholder for future development.
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> connectableNeighbors = null!;
        IIslandViewModel? validNeighbor = null;

        When()
            .Match(() => island, x => x.MaxConnectionsReached == false)
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => connectableNeighbors, () => GetConnectableNeighbors(allNeighbors))
            .Having(() => connectableNeighbors.Count == 2 && connectableNeighbors.Any(x => x.RemainingConnections == 1) && GetMaxedOutConnectedNeighbors(island, allNeighbors, null).Count == 1)
            .Let(() => validNeighbor, () => SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(island, allNeighbors))
            .Having(() => validNeighbor != null);

        Then()
            .Do(ctx => AddConnection(island, validNeighbor!));
    }
}