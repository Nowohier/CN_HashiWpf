using Microsoft.Extensions.DependencyInjection;
using NRules.Fluent;
using NRules.RuleModel;

namespace Hashi.Rules.Extensions;

/// <summary>
///     Extension methods for registering NRules services.
/// </summary>
public static class RuleServiceExtensions
{
    /// <summary>
    ///     Adds rule services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRuleServices(this IServiceCollection services)
    {
        services.AddSingleton<IRuleRepository>(sp =>
        {
            var activator = new ServiceProviderRuleActivator(sp);
            var repo = new RuleRepository(activator);
            repo.Load(x => x.From(typeof(_1ConnectionRule1).Assembly));
            return repo;
        });

        return services;
    }
}
