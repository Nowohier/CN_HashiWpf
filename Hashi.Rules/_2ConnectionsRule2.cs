using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public class _2ConnectionsRule2 : BaseRule
    {
        protected override string RuleMessage => "If an island with a maximum of two bridges has a connection to another island with max one connections, then the other connection can only go to a neighbor with max connections > 1.";

        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allNeighbors = default!;
            List<IIslandViewModel> allValidNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;
            var validNeighborsCount = 0;
            var isSingleConnectionSetToMaxOneNeighbor = false;

            When()
                .Match(() => island, x => x.MaxConnections == 2 && x.AllConnections.Count == 1)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
                .Let(() => allValidNeighbors, () => allNeighbors.Where(x => x.MaxConnections > 1).ToList())
                .Let(() => allNeighborsCount, () => allNeighbors.Count)
                .Let(() => validNeighborsCount, () => allValidNeighbors.Count)
                .Let(() => isSingleConnectionSetToMaxOneNeighbor, () => allNeighbors.Count(x => x.AllConnections.Contains(island.Coordinates) && x.MaxConnections == 1) == 1)
                .Having(() => allNeighborsCount > 1 && validNeighborsCount == 1 && isSingleConnectionSetToMaxOneNeighbor);

            Then()
                .Do(ctx => AddConnection(island, allValidNeighbors.First(), connectionManager));
        }
    }
}
