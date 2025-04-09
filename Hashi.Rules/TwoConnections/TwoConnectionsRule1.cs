using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules.TwoConnections
{
    public class TwoConnectionsRule1 : BaseRule
    {
        protected override string RuleMessage => "If an island with a maximum of two bridges has two neighbor islands with also a maximum of two bridges, then a connection to each neighbors must be set.";

        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allNeighbors = default!;
            List<IIslandViewModel> allValidNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;
            var validNeighborsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 2)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
                .Let(() => allValidNeighbors, () => allNeighbors.Where(x => x.MaxConnections == 2).ToList())
                .Let(() => allNeighborsCount, () => allNeighbors.Count)
                .Let(() => validNeighborsCount, () => allValidNeighbors.Count)
                .Having(() => allNeighborsCount == 2 && validNeighborsCount == 2);

            Then()
                .Do(ctx => AddConnections(island, allValidNeighbors, connectionManager));
        }
    }
}
