using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
/// Islands with only one active neighbor can set their remaining connections to that neighbor.
/// </summary>
public class _9GeneralRule1 : BaseRule
{
    protected override string RuleMessage => TranslationSource.Instance[nameof(_9GeneralRule1)]!;

    /// <inheritdoc />
    public override void Define()
    {
        IIslandViewModel island = null!;
        List<IIslandViewModel> validNeighbors = null!;
        IIslandProvider islandProvider = null!;

        When()
            .Match(() => island, x => x.MaxConnectionsReached == false)
            .Query(() => islandProvider, q => q.Match<IIslandProvider>())
            .Let(() => validNeighbors,
                () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnectionsReached == false).ToList())
            .Having(() => validNeighbors.Count == 1);

        Then()
            .Do(ctx => AddMissingConnectionsToOneTarget(island, validNeighbors.First(),
                island.MaxConnections - island.AllConnections.Count, islandProvider));
    }
}