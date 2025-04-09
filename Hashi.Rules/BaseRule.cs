using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public abstract class BaseRule : Rule
    {
        protected abstract string RuleMessage { get; }

        /// <summary>
        /// Adds a connection to an island and stops the rule execution of all rules.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <param name="connectionManager">The connection manager.</param>
        protected virtual void AddConnection(IIslandViewModel source, IIslandViewModel target,
            IConnectionManagerViewModel connectionManager)
        {
            if (!AreRulesBeingApplied(connectionManager)) return;
            connectionManager.AddConnection(source, target, true);
            source.RefreshIslandColor();
            target.RefreshIslandColor();
        }

        /// <summary>
        /// Adds multiple connections to an island and stops the rule execution of all rules.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="targets">The target islands.</param>
        /// <param name="connectionManager">The connection manager.</param>
        protected virtual void AddConnections(IIslandViewModel source, List<IIslandViewModel> targets,
            IConnectionManagerViewModel connectionManager)
        {
            if (!AreRulesBeingApplied(connectionManager)) return;

            foreach (var target in targets)
            {
                connectionManager.AddConnection(source, target, true);
                target.RefreshIslandColor();
            }

            source.RefreshIslandColor();
        }

        /// <summary>
        /// Adds multiple connections to an island and stops the rule execution of all rules.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="targets">The target islands.</param>
        /// <param name="connectionManager">The connection manager.</param>
        protected virtual void AddMultipleConnectionsToEachTarget(IIslandViewModel source, List<IIslandViewModel> targets,
            IConnectionManagerViewModel connectionManager)
        {
            if (!AreRulesBeingApplied(connectionManager)) return;

            foreach (var target in targets)
            {
                if (source.MaxConnectionsReached || target.MaxConnectionsReached) continue;

                connectionManager.AddConnection(source, target);
                target.RefreshIslandColor();

                if (source.MaxConnectionsReached || target.MaxConnectionsReached) continue;

                connectionManager.AddConnection(source, target, true);
                target.RefreshIslandColor();
            }

            source.RefreshIslandColor();
        }

        /// <summary>
        /// Adds multiple connections to an island and stops the rule execution of all rules.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <param name="missingConnectionsCount">Amount of connections missing.</param>
        /// <param name="connectionManager">The connection manager.</param>
        protected virtual void AddConnectionsToOneTarget(IIslandViewModel source, IIslandViewModel target, int missingConnectionsCount,
            IConnectionManagerViewModel connectionManager)
        {
            if (!AreRulesBeingApplied(connectionManager)) return;

            for (var i = 0; i < missingConnectionsCount; i++)
            {
                if (source.MaxConnectionsReached || target.MaxConnectionsReached) break;
                connectionManager.AddConnection(source, target, true);
            }

            target.RefreshIslandColor();
            source.RefreshIslandColor();
        }

        private bool AreRulesBeingApplied(IConnectionManagerViewModel connectionManager)
        {
            if (connectionManager.AreRulesBeingApplied == false)
            {
                return false;
            }
            connectionManager.AreRulesBeingApplied = false;
            connectionManager.RuleMessage = RuleMessage;
            return true;
        }
    }
}