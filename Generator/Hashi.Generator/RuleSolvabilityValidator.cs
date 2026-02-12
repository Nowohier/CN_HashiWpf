using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Simulation;
using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Core.Providers;
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

    /// <summary>
    ///     Initializes a new instance of the <see cref="RuleSolvabilityValidator" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public RuleSolvabilityValidator(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<RuleSolvabilityValidator>();
    }

    /// <inheritdoc />
    public Task<bool> IsFullySolvableByRules(int[][] field, IList<IBridgeCoordinates> bridgeCoordinates)
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
        var helper = new IslandViewModelHelper();
        var islandProvider = new SimulationIslandProvider(new IslandProviderCore(helper), helper);
        islandProvider.InitializeFromField(field);

        var ruleInfoProvider = new SimulationRuleInfoProvider();

        var ruleActivator = new SimulationRuleActivator(ruleInfoProvider, islandProvider);
        var repository = new RuleRepository(ruleActivator);
        repository.Load(x => x.From(typeof(_1ConnectionRule1).Assembly));

        var factory = repository.Compile();

        var session = factory.CreateSession();
        session.Events.RhsExpressionEvaluatedEvent += (_, _) => ruleInfoProvider.AreRulesBeingApplied = false;
        session.InsertAll(islandProvider.IslandsFlat.ToList());

        const int maxIterations = 500;
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
