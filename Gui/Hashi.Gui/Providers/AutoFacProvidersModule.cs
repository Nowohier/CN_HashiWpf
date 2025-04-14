using Autofac;
using Hashi.Gui.Interfaces.Providers;

namespace Hashi.Gui.Providers;

/// <inheritdoc />
public class AutoFacProvidersModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HashiSettingsProvider>().As<IHashiSettingsProvider>().SingleInstance();
        builder.RegisterType<TimerProvider>().As<ITimerProvider>().SingleInstance();
        builder.RegisterType<IslandProvider>().As<IIslandProvider>().SingleInstance();
        builder.RegisterType<HintProvider>().As<IHintProvider>().SingleInstance();
        builder.RegisterType<RuleInfoProvider>().As<IRuleInfoProvider>().SingleInstance();
    }
}