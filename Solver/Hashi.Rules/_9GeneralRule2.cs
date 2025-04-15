using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;

namespace Hashi.Rules;

/// <summary>
/// If setting a connection to any neighbor would result in a maxed-out island and maxed-out connected neighbors,
/// check if the whole group is isolated. If it is, set the connection to the remaining neighbor.
/// </summary>
public class _9GeneralRule2(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider)
    : BaseRule(ruleInfoProvider, islandProvider)
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule2)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> connectableNeighbors = null!;
        IIslandViewModel? validNeighbor = null;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections > 0) // Only consider islands that are not maxed out and visible
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors()) // Get all visible neighbors
            .Let(() => connectableNeighbors, () => GetConnectableNeighbors(allNeighbors)) // Get connectable neighbors
            .Having(() =>
                connectableNeighbors.Count == 2 && // Exactly two connectable neighbors
                connectableNeighbors.Any(x => x.RemainingConnections == 1) && // At least one neighbor has one remaining connection
                GetMaxedOutConnectedNeighbors(island, allNeighbors, null).Count == 1) // Exactly one maxed-out connected neighbor
            .Let(() => validNeighbor, () => SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(island, connectableNeighbors, allNeighbors)) // Check for isolated group and a valid neighbor
            .Having(() => validNeighbor != null); // Ensure a valid neighbor exists

        Then()
            .Do(ctx => AddConnection(island, validNeighbor!)); // Add the connection to the valid neighbor
    }
}