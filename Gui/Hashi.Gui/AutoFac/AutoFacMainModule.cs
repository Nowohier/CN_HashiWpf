using Autofac;
using Hashi.Generator;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Helpers;
using Hashi.Gui.Managers;
using Hashi.Gui.Messages;
using Hashi.Gui.Models;
using Hashi.Gui.Providers;
using Hashi.Gui.ViewModels;
using Hashi.Gui.Views;
using Hashi.Gui.Wrappers;
using Hashi.LinearSolver;
using Hashi.Logging;
using Hashi.Logging.Interfaces;
using Hashi.Rules;

namespace Hashi.Gui.AutoFac;

/// <inheritdoc />
public class AutoFacMainModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register your modules here
        builder.RegisterModule<AutoFacGeneratorModule>();

        builder.RegisterModule<AutoFacViewModelsModule>();
        builder.RegisterModule<AutoFacViewsModule>();
        builder.RegisterModule<AutoFacMessagesModule>();
        builder.RegisterModule<AutoFacModelsModule>();
        builder.RegisterModule<AutoFacWrapperModule>();
        builder.RegisterModule<AutoFacLinearSolverModule>();
        builder.RegisterModule<AutoFacHelpersModule>();
        builder.RegisterModule<AutoFacProvidersModule>();
        builder.RegisterModule<AutoFacRulesModule>();
        builder.RegisterModule<AutoFacManagersModule>();
        builder.RegisterModule<AutoFacLoggingModule>();

        builder.Register<Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (hashiField, bridgeCoordinates, name) => c.Resolve<ISolutionProvider>(
                new NamedParameter("hashiField", hashiField),
                new NamedParameter("bridgeCoordinates", bridgeCoordinates),
                new NamedParameter("name", name));
        });
    }
}