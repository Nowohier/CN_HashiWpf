using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules.OneConnection
{
    /// <summary>
    ///   Defines the rule for connecting islands with one connection.
    ///   Is fired when
    ///     MaxConnection = 1
    ///     MaxConnectionsReached == false
    ///     AllValidNeighbors Count == 1
    ///   Is not fired when
    ///     connectionManager.AreRulesBeingApplied == false
    /// </summary>
    public class OneConnectionRule1 : BaseRule
    {
        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allValidNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var validNeighborsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 1)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allValidNeighbors, () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnections != 1).ToList())
                .Let(() => validNeighborsCount, () => allValidNeighbors.Count)
                .Having(() => validNeighborsCount == 1);

            Then()
                .Do(ctx => AddConnection(island, allValidNeighbors.First(), connectionManager));
        }
    }
}
