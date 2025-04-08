using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules.OneConnection
{
    /// <summary>
    ///   Defines the rule for connecting islands with one connection.
    ///   Is fired when
    ///     MaxConnection = 1
    ///     MaxConnectionsReached == false
    ///     AllActiveNeighbors.Count == 1
    ///   Is not fired when
    ///     connectionManager.AreRulesBeingApplied == false
    /// </summary>
    public class OneConnectionRule1 : BaseRule
    {
        /// <inheritdoc />
        public override void Define()
        {
            IIslandViewModel island = default!;
            List<IIslandViewModel> allActiveNeighbors = default!;
            IConnectionManagerViewModel connectionManager = default!;
            var activeNeighborsCount = 0;

            When()
                .Match(() => island, x => x.MaxConnectionsReached == false && x.MaxConnections == 1)
                .Query(() => connectionManager, q => q.Match<IConnectionManagerViewModel>())
                .Let(() => allActiveNeighbors, () => island.GetAllVisibleNeighbors().Where(x => x.MaxConnections != 1).ToList())
                .Let(() => activeNeighborsCount, () => allActiveNeighbors.Count)
                .Having(() => activeNeighborsCount == 1);

            Then()
                .Do(ctx => AddConnection(island, allActiveNeighbors.First(), connectionManager));
        }
    }
}
