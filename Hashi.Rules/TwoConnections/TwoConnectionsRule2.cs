using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules.TwoConnections
{
    public class TwoConnectionsRule2 : BaseRule
    {
        protected override string RuleMessage => "Islands with a maximum of two bridges can be connected to another island if there is only one neighbor still accepting connections.";

        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            IIslandViewModel neighbor = default!;
            List<IIslandViewModel> allValidNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;
            var missingConnectionsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 2)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allValidNeighbors, () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnectionsReached == false).ToList())
                .Let(() => neighbor, () => allValidNeighbors.First())
                .Let(() => allNeighborsCount, () => allValidNeighbors.Count)
                .Let(() => missingConnectionsCount, () => neighbor.MaxConnections - neighbor.AllConnections.Count)
                .Having(() => allNeighborsCount == 1);

            Then()
                .Do(ctx => AddConnectionsToOneTarget(island, allValidNeighbors.First(), missingConnectionsCount, connectionManager));
        }
    }
}
