using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Translation;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public class _7ConnectionsRule1 : BaseRule
    {
        protected override string RuleMessage => TranslationSource.Instance[nameof(_7ConnectionsRule1)]!;

        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allNeighbors = default!;
            List<IIslandViewModel> validNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var allNeighborsCount = 0;
            var allNeighborsHaveConnectionToSource = false;

            When()
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Match(() => island, x => x.MaxConnections == 7 && !x.MaxConnectionsReached)
                .Let(() => allNeighbors, () => island.GetAllVisibleNeighbors())
                .Let(() => validNeighbors, () => allNeighbors.Where(x => !x.AllConnections.Contains(island.Coordinates)).ToList())
                .Let(() => allNeighborsCount, () => allNeighbors.Count)
                .Let(() => allNeighborsHaveConnectionToSource, () => allNeighbors.All(x => x.AllConnections.Contains(island.Coordinates)))
                .Having(() => allNeighborsCount == 4 && !allNeighborsHaveConnectionToSource);

            Then()
                .Do(ctx => AddConnections(island, validNeighbors, connectionManager));
        }
    }
}
