using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public class _4ConnectionsRule1 : BaseRule
    {
        protected override string RuleMessage => TranslationSource.Instance[nameof(_4ConnectionsRule1)]!;

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
                .Do(ctx => AddMultipleConnections(island, allNeighbors, connectionManager));
        }
    }
}
