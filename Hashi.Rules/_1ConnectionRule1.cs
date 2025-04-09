using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public class _1ConnectionRule1 : BaseRule
    {
        protected override string RuleMessage => "Islands with a maximum of one bridge can set their connection if there is only one neighbor island accepting bridges. Neighbor islands with a maximum of one bridge cannot be connected to as both islands would be isolated then.";

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
