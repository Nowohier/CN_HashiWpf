using Hashi.Gui.Interfaces.Providers;
using Microsoft.Extensions.DependencyInjection;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace Hashi.Rules;

/// <summary>
///     A rule activator that resolves <see cref="IRuleInfoProvider" /> and <see cref="IIslandProvider" />
///     from the DI container when instantiating rule types during repository loading.
/// </summary>
/// <param name="serviceProvider">The service provider to resolve dependencies from.</param>
public class ServiceProviderRuleActivator(IServiceProvider serviceProvider) : IRuleActivator
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

        var ruleInfoProvider = serviceProvider.GetRequiredService<IRuleInfoProvider>();
        var islandProvider = serviceProvider.GetRequiredService<IIslandProvider>();
        var rule = (Rule)constructor.Invoke([ruleInfoProvider, islandProvider]);
        yield return rule;
    }
}
