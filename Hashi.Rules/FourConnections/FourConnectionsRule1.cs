using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules.FourConnections
{
    public class FourConnectionsRule1 : BaseRule
    {
        protected override string RuleMessage => "Islands with a maximum of four bridges can set all connections if only two neighbors are available.";

        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 4)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
                .Let(() => allNeighborsCount, () => allNeighbors.Count)
                .Having(() => allNeighborsCount == 2);

            Then()
                .Do(ctx => AddMultipleConnectionsToEachTarget(island, allNeighbors, connectionManager));
        }
    }
}
