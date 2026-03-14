using Hashi.Generator.Extensions;
using Hashi.Gui.Core.Extensions;
using Hashi.Gui.Helpers.Extensions;
using Hashi.Gui.Managers.Extensions;
using Hashi.Gui.Messages.Extensions;
using Hashi.Gui.Models.Extensions;
using Hashi.Gui.Providers.Extensions;
using Hashi.Gui.ViewModels.Extensions;
using Hashi.Gui.Views.Extensions;
using Hashi.Gui.Wrappers.Extensions;
using Hashi.LinearSolver.Extensions;
using Hashi.Logging.Extensions;
using Hashi.Rules.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Extensions;

/// <summary>
///     Extension methods for registering all Hashi services.
/// </summary>
public static class MainServiceExtensions
{
    /// <summary>
    ///     Adds all Hashi services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHashiServices(this IServiceCollection services)
    {
        services.AddGuiCoreServices();
        services.AddGeneratorServices();
        services.AddViewModelServices();
        services.AddViewServices();
        services.AddMessageServices();
        services.AddModelServices();
        services.AddWrapperServices();
        services.AddLinearSolverServices();
        services.AddHelperServices();
        services.AddProviderServices();
        services.AddRuleServices();
        services.AddManagerServices();
        services.AddLoggingServices();

        return services;
    }
}
