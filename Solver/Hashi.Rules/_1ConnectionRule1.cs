using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// Islands with a maximum of one bridge can set their connection if there is only one neighbor island accepting bridges.
/// Neighbor islands with a maximum of one bridge cannot be connected to as both islands would be isolated then.
/// </summary>
public class _1ConnectionRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_1ConnectionRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IConnectionManagerViewModel connectionManager = null!;

        When()
            .Match(() => island, x => !x.MaxConnectionsReached && x.MaxConnections == 1)
            .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
            .Let(() => validNeighbors,
                () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnections != 1 && !x.MaxConnectionsReached)
                    .ToList())
            .Having(() => validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddConnection(island, validNeighbors.First(), connectionManager));
    }
}