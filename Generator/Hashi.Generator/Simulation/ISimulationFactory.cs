using Hashi.Gui.Interfaces.Providers;
using NRules.Fluent;

namespace Hashi.Generator.Simulation;

/// <summary>
///     Factory for creating lightweight simulation components used during rule-based solvability validation.
/// </summary>
public interface ISimulationFactory
{
    /// <summary>
    ///     Creates a simulation context initialized from the given puzzle field.
    /// </summary>
    /// <param name="field">The puzzle field where each cell value represents the bridge count (0 = empty).</param>
    /// <returns>A tuple containing the simulation island provider, rule info provider, and rule activator.</returns>
    (IIslandProvider IslandProvider, IRuleInfoProvider RuleInfoProvider,
        IRuleActivator RuleActivator) CreateSimulation(int[][] field);
}
