using Autofac;
using Hashi.Generator;
using Hashi.Gui.Helpers;
using Hashi.Gui.Messages;
using Hashi.Gui.Models;
using Hashi.Gui.ViewModels;
using Hashi.Gui.Views;
using Hashi.Gui.Wrappers;
using Hashi.LinearSolver;

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
    }
}