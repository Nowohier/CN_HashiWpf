using System.Reflection;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Simulation;
using Hashi.Logging.Interfaces;
using Hashi.Rules;
using NRules;
using NRules.Fluent;

namespace Hashi.Generator;

/// <inheritdoc cref="IRuleSolvabilityValidator" />
public class RuleSolvabilityValidator : IRuleSolvabilityValidator
{
    private readonly ILogger logger;
    private readonly ISimulationFactory simulationFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RuleSolvabilityValidator" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="simulationFactory">The simulation factory for creating rule-solving simulation contexts.</param>
    public RuleSolvabilityValidator(
        ILoggerFactory loggerFactory,
        ISimulationFactory simulationFactory)
    {
        logger = loggerFactory.CreateLogger<RuleSolvabilityValidator>();
        this.simulationFactory = simulationFactory ?? throw new ArgumentNullException(nameof(simulationFactory));
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
    /// <summary>
    ///     The assembly containing all rule implementations.
    /// </summary>
    private static readonly Assembly RulesAssembly = typeof(BaseRule).Assembly;

    internal bool SimulateRuleSolving(int[][] field)
    {
        var (islandProvider, ruleInfoProvider, ruleActivator) = simulationFactory.CreateSimulation(field);

        var repository = new RuleRepository(ruleActivator);
        repository.Load(x => x.From(RulesAssembly));

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

            if (islandProvider.IslandsFlat.All(island => island.MaxConnections == 0 || island.MaxConnectionsReached))
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

}
