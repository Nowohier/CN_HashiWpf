using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules.TwoConnections
{
    public class TwoConnectionsRule2 : BaseRule
    {
        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            IIslandViewModel neighbor = default!;
            List<IIslandViewModel> allNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;
            var missingConnectionsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 2)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
                .Let(() => neighbor, () => allNeighbors.First())
                .Let(() => allNeighborsCount, () => allNeighbors.Count)
                .Let(() => missingConnectionsCount, () => neighbor.MaxConnections - neighbor.AllConnections.Count)
                .Having(() => allNeighborsCount == 1);

            Then()
                .Do(ctx => AddConnectionsToOneTarget(island, allNeighbors.First(), missingConnectionsCount, connectionManager));
        }
    }
}
