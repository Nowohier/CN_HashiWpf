using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// If an island with a maximum of three bridges has only two neighbors, then a connection must be set to each neighbor.
/// </summary>
public class _3ConnectionsRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_3ConnectionsRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> allNeighbors = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => x.MaxConnections == 3 && x.AllConnections.Count < 2)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
            .Let(() => validNeighbors, () => GetConnectableNeighborsWithoutConnection(island, allNeighbors))
            .Having(() => allNeighbors.Count == 2 && validNeighbors.Count > 0);

        Then()
            .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
    }
}