using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public class _4ConnectionsRule2 : BaseRule
    {
        protected override string RuleMessage => "Islands with a maximum of four bridges and three neighbors can set one connection to each neighbor if one of the neighbors already has a connection to the island and has mac connections.";

        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allNeighbors = default!;
            List<IIslandViewModel> validNeighbors = default!;
            List<IIslandViewModel> filteredNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;
            var validNeighborsCount = 0;
            var filteredNeighborsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 4)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
                .Let(() => validNeighbors, () => allNeighbors.Where(x => !x.MaxConnectionsReached).ToList())
                .Let(() => filteredNeighbors, () => validNeighbors.Where(x => !x.AllConnections.Contains(island.Coordinates)).ToList())
                .Let(() => allNeighborsCount, () => allNeighbors.Count)
                .Let(() => validNeighborsCount, () => validNeighbors.Count)
                .Let(() => filteredNeighborsCount, () => filteredNeighbors.Count)
                .Having(() => allNeighborsCount == 3 && validNeighborsCount == 2 && filteredNeighborsCount > 0);

            Then()
                .Do(ctx => AddConnections(island, filteredNeighbors, connectionManager));
        }
    }
}
