using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public class GeneralRule1 : BaseRule
    {
        protected override string RuleMessage => "Islands with only one active neighbor can set their remaining connections to that neighbor.";

        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            IIslandViewModel neighbor = default!;
            List<IIslandViewModel> allValidNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allValidNeighborsCount = 0;
            var missingConnectionsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allValidNeighbors, () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnectionsReached == false).ToList())
                .Let(() => neighbor, () => allValidNeighbors.First())
                .Let(() => allValidNeighborsCount, () => allValidNeighbors.Count)
                .Let(() => missingConnectionsCount, () => island.MaxConnections - island.AllConnections.Count)
                .Having(() => allValidNeighborsCount == 1);

            Then()
                .Do(ctx => AddMissingConnectionsToOneTarget(island, neighbor, missingConnectionsCount, connectionManager));
        }
    }
}
