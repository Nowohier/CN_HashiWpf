using Autofac;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Providers;

namespace Hashi.Gui.Providers;

/// <inheritdoc />
public class AutoFacProvidersModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SettingsProvider>().As<ISettingsProvider>().SingleInstance();
        builder.RegisterType<TimerProvider>().As<ITimerProvider>().SingleInstance();
        builder.RegisterType<IslandProvider>().As<IIslandProvider>().SingleInstance();
        builder.RegisterType<HintProvider>().As<IHintProvider>().SingleInstance();
        builder.RegisterType<RuleInfoProvider>().As<IRuleInfoProvider>().SingleInstance();
        builder.RegisterType<TestSolutionProvider>().As<ITestSolutionProvider>().SingleInstance();
        builder.RegisterType<PathProvider>().As<IPathProvider>().SingleInstance();

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