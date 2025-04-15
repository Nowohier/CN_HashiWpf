using Autofac;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels.Settings;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc />
public class AutoFacViewModelsModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register your view models here
        builder.RegisterType<IslandViewModel>().As<IIslandViewModel>().InstancePerDependency();
        builder.RegisterType<SettingsViewModel>().As<ISettingsViewModel>().InstancePerDependency();
        builder.RegisterType<LanguageViewModel>().As<ILanguageViewModel>().InstancePerDependency();
        builder.RegisterType<HighScorePerDifficultyViewModel>().As<IHighScorePerDifficultyViewModel>()
            .InstancePerDependency();
        builder.RegisterType<MainViewModel>().As<IMainViewModel>().InstancePerDependency();

        builder.Register<Func<int, int, int, IIslandViewModel>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return (x, y, maxConnections) => c.Resolve<IIslandViewModel>(
                new NamedParameter("x", x),
                new NamedParameter("y", y),
                new NamedParameter("maxConnections", maxConnections),
                new NamedParameter("islandProvider", c.Resolve<IIslandProvider>()));
        });
    }
}