using Autofac;
using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Core.Providers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Providers;

namespace Hashi.Gui.Core;

/// <summary>
///     Registers GUI core services (helpers and providers) with the Autofac container.
/// </summary>
public class AutoFacGuiCoreModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<IslandViewModelHelper>().As<IIslandViewModelHelper>().SingleInstance()
            .IfNotRegistered(typeof(IIslandViewModelHelper));
        builder.RegisterType<IslandProviderCore>().As<IIslandProviderCore>().SingleInstance()
            .IfNotRegistered(typeof(IIslandProviderCore));
    }
}
