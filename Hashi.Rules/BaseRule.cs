using Hashi.Gui.Interfaces.ViewModels;
using NRules.Fluent.Dsl;

namespace Hashi.Rules
{
    public abstract class BaseRule : Rule
    {
        /// <summary>
        /// Adds a connection to an island.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <param name="connectionManager">The connection manager.</param>
        protected virtual void AddConnection(IIslandViewModel source, IIslandViewModel target, IConnectionManagerViewModel connectionManager)
        {
            if (connectionManager.AreRulesBeingApplied == false)
            {
                return;
            }

            connectionManager.AreRulesBeingApplied = false;
            connectionManager.AddConnection(source, target);
        }
    }
}
