using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public class _2ConnectionsRule1 : BaseRule
    {
        protected override string RuleMessage => TranslationSource.Instance[nameof(_2ConnectionsRule1)]!;

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
