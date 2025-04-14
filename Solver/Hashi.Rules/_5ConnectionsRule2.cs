using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of five  bridges has three neighbors and one connection to the source island only allows a maximum of one bridge, then the remaining connections for the source island can be set.
/// </summary>
public class _5ConnectionsRule2 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_5ConnectionsRule2)]!;

    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> neighbors = null!;
        List<IIslandViewModel> restrictedNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 5 && !x.MaxConnectionsReached)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => neighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => restrictedNeighbors, () => GetMaxedOutConnectedNeighbors(island, neighbors, 1))
            .Having(() => restrictedNeighbors.Count == 1 && neighbors.Count - restrictedNeighbors.Count == 3);

        Then()
            .Do(ctx => AddMultipleConnections(island, neighbors.Except(restrictedNeighbors).ToList(),
                connectionManager));
    }
}