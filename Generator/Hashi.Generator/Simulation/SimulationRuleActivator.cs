using Hashi.Gui.Interfaces.Providers;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace Hashi.Generator.Simulation;

/// <summary>
///     A rule activator that provides the simulation-specific implementations
///     of <see cref="IRuleInfoProvider" /> and <see cref="IIslandProvider" /> to NRules
///     when instantiating rule types during repository loading.
/// </summary>
public class SimulationRuleActivator(
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
