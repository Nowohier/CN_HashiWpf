using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules.SixConnections
{
    public class SixConnectionRule1 : BaseRule
    {
        protected override string RuleMessage => "If an island with a maximum of six connections has three neighbors then set all connections.";

        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;

            When()
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Match(() => island, x => x.MaxConnections == 6 && !x.MaxConnectionsReached)
                .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
                .Let(() => allNeighborsCount, () => allNeighbors.Count)
                .Having(() => allNeighborsCount == 3);

            Then()
                .Do(ctx => AddMultipleConnections(island, allNeighbors, connectionManager));
        }
    }
}
