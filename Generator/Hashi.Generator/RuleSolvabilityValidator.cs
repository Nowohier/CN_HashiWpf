using Hashi.Generator.Interfaces;
using Hashi.Generator.Simulation;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Logging.Interfaces;
using Hashi.Rules;
using NRules;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace Hashi.Generator;

/// <inheritdoc cref="IRuleSolvabilityValidator" />
public class RuleSolvabilityValidator : IRuleSolvabilityValidator
{
    private readonly ILogger logger;
    private readonly IIslandViewModelHelper helper;
    private readonly IIslandProviderCore core;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RuleSolvabilityValidator" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="helper">The island view model helper.</param>
    /// <param name="core">The island provider core.</param>
    public RuleSolvabilityValidator(
        ILoggerFactory loggerFactory,
        IIslandViewModelHelper helper,
        IIslandProviderCore core)
    {
        logger = loggerFactory.CreateLogger<RuleSolvabilityValidator>();
        this.helper = helper;
        this.core = core;
    }

    /// <inheritdoc />
    public Task<bool> IsFullySolvableByRules(int[][] field)
    {
        return Task.Run(() => SimulateRuleSolving(field));
    }

    /// <summary>
    ///     Simulates rule-based puzzle solving by creating a lightweight in-memory representation
    ///     of the puzzle and iteratively firing NRules until the puzzle is solved or no more rules fire.
    /// </summary>
    /// <param name="field">The puzzle field.</param>
    /// <returns><c>true</c> if all bridges can be placed via rules alone.</returns>
    internal bool SimulateRuleSolving(int[][] field)
    {
        var islandProvider = new SimulationIslandProvider(core, helper);
        islandProvider.InitializeFromField(field);

        var ruleInfoProvider = new SimulationRuleInfoProvider();

        var ruleActivator = new SimulationRuleActivator(ruleInfoProvider, islandProvider);
        var repository = new RuleRepository(ruleActivator);
        repository.Load(x => x.From(typeof(_1ConnectionRule1).Assembly));

        var factory = repository.Compile();

        var session = factory.CreateSession();
        session.Events.RhsExpressionEvaluatedEvent += (_, _) => ruleInfoProvider.AreRulesBeingApplied = false;
        session.InsertAll(islandProvider.IslandsFlat.ToList());

        var maxIterations = GeneratorConstants.MaxRuleSimulationIterations;
        var totalRulesFired = 0;

        for (var iteration = 0; iteration < maxIterations; iteration++)
        {
            ruleInfoProvider.AreRulesBeingApplied = true;
            var rulesFired = session.Fire();
            ruleInfoProvider.AreRulesBeingApplied = false;

            if (rulesFired == 0)
            {
                break;
            }

            totalRulesFired += rulesFired;

            if (islandProvider.AreAllConnectionsSet)
            {
                logger.Debug(
                    $"Rule-based simulation solved puzzle in {iteration + 1} iterations ({totalRulesFired} rules fired)");
                return true;
            }

            // Update all island facts for the next iteration
            session.UpdateAll(islandProvider.IslandsFlat.ToList());
        }

        var remainingIslands = islandProvider.IslandsFlat
            .Count(island => island.MaxConnections > 0 && !island.MaxConnectionsReached);
        logger.Debug(
            $"Rule-based simulation could not solve puzzle: {remainingIslands} islands remain unsatisfied after {totalRulesFired} rules fired");

        return false;
    }

    /// <summary>
    ///     A rule activator that provides the simulation-specific implementations
    ///     of <see cref="IRuleInfoProvider" /> and <see cref="IIslandProvider" /> to NRules
    ///     when instantiating rule types during repository loading.
    /// </summary>
    private class SimulationRuleActivator(
        IRuleInfoProvider ruleInfoProvider,
        IIslandProvider islandProvider) : IRuleActivator
    {
        /// <inheritdoc />
        public IEnumerable<Rule> Activate(Type type)
        {
            var constructor = type.GetConstructors()
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 2
                           && parameters[0].ParameterType == typeof(IRuleInfoProvider)
                           && parameters[1].ParameterType == typeof(IIslandProvider);
                });

            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"Rule type {type.FullName} does not have a constructor accepting (IRuleInfoProvider, IIslandProvider).");
            }

            var rule = (Rule)constructor.Invoke([ruleInfoProvider, islandProvider]);
            yield return rule;
        }
    }
}
