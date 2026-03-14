using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Providers;
using NRules.Fluent;

namespace Hashi.Generator.Simulation;

/// <inheritdoc />
internal class SimulationFactory : ISimulationFactory
{
    private readonly IIslandProviderCore core;
    private readonly IIslandViewModelHelper helper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SimulationFactory" /> class.
    /// </summary>
    /// <param name="core">The shared island provider core logic.</param>
    /// <param name="helper">The island view model helper.</param>
    public SimulationFactory(IIslandProviderCore core, IIslandViewModelHelper helper)
    {
        this.core = core ?? throw new ArgumentNullException(nameof(core));
        this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
    }

    /// <inheritdoc />
    public (IIslandProvider IslandProvider, IRuleInfoProvider RuleInfoProvider,
        IRuleActivator RuleActivator) CreateSimulation(int[][] field)
    {
        var islandProvider = new SimulationIslandProvider(core, helper);
        islandProvider.InitializeFromField(field);

        var ruleInfoProvider = new SimulationRuleInfoProvider();
        var ruleActivator = new SimulationRuleActivator(ruleInfoProvider, islandProvider);

        return (islandProvider, ruleInfoProvider, ruleActivator);
    }
}
